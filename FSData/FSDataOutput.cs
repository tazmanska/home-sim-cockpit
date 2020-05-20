using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FSData
{
    class FSDataOutput : HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleConfiguration, HomeSimCockpitSDK.IModuleFunctions
    {
        private class SpecialOutputVariable : OutputVariable
        {
            public SpecialOutputVariable(bool state) : this(state, false)
            {
            }

            public SpecialOutputVariable(bool state, bool listenForChange)
            {
                _state = state;
                _listenForChange = listenForChange;
            }

            private bool _listenForChange = false;
            private bool _state = false;

            public override void Reset()
            {
                _state = false;
            }

            public bool State
            {
                get { return _state; }
            }

            private bool _lastState = false;

            public override int SetValue(object value, FsuipcSdk.Fsuipc fsuipc)
            {
                if (_listenForChange)
                {
                    _state = (bool)value != _lastState;
                    if (_state)
                    {
                        _lastState = (bool)value;
                    }
                }
                else
                {
                    _state = (bool)value;
                }
                return FsuipcSdk.Fsuipc.FSUIPC_ERR_OK;
            }
        }

        public FSDataOutput()
        {
            _flush = new SpecialOutputVariable(false, true)
            {
                ID = "_flush",
                Description = "Wysyła dane do symulatora (gdy chcemy wysłać dane do symulatora wystarczy wpisać do tej zmiennej wartość przeciwną do aktualne), działa tylko gdy tryb AutoFlush (zmienna _autoFlush) jest wyłączony.",
                Type = HomeSimCockpitSDK.VariableType.Bool
            };

            _autoFlush = new SpecialOutputVariable(true)
            {
                ID = "_autoFlush",
                Description = "Automatyczne wysyłanie danych do symulatora (domyślnie włączone) - moduł wysyła dane do symulatora od razu po zmianie wartości zmiennych. Gdy wyłączymy tryb AutoFlush (przypisanie wartości false) to dane zostaną wysłane po rozkazie Flush (zmienna _flush) lub włączeniu AutoFlush.",
                Type = HomeSimCockpitSDK.VariableType.Bool                
            };
        }

        private SpecialOutputVariable _autoFlush = null;
        private SpecialOutputVariable _flush = null;

        private OutputVariable[] _outputs = new OutputVariable[0];

        #region IModule Members

        public string Name
        {
            get { return "FSDataOutput"; }
        }

        public string Description
        {
            get { return "Moduł do zapisywania danych do symulatora MS Flight Simulator (2002, 2004 i X)."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 2);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        private ModulesConfiguration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            // wczytanie konfiguracji
            _configuration = ModulesConfiguration.Load();

            List<OutputVariable> outs = new List<OutputVariable>();
            outs.AddRange(_configuration.OutputVariables);
            outs.Add(_flush);
            outs.Add(_autoFlush);
            _outputs = outs.ToArray();
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
        }

        private volatile bool _working = false;
        private Thread _processingThread = null;
        Dictionary<string, OutputVariable> _registered = new Dictionary<string, OutputVariable>();

        private class VariableToWrite
        {
            public OutputVariable Variable = null;
            public object Value = null;
        }

        private Queue<VariableToWrite> _writeQueue = new Queue<VariableToWrite>();
        private object _syncQueue = new object();
        private AutoResetEvent _signal = new AutoResetEvent(false);

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
            _writeQueue.Clear();
            _working = true;
            _signal.Reset();
            _flush.Reset();
            _autoFlush.SetValue(true, null);

            // wystartowanie watka (jesli sa jakies zmienne zarejestrowane do zmian)            
            if (_registered.Count > 0)
            {
                foreach (KeyValuePair<string, OutputVariable> kvp in _registered)
                {
                    kvp.Value.Module = this;
                }

                // wystartowanie wątka
                _processingThread = new Thread(new ThreadStart(ProcessingMethod));
                _processingThread.Start();
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            // zatrzymanie watka
            _signal.Set();
            _working = false;
            if (_processingThread != null)
            {
                try
                {
                    _processingThread.Abort();
                }
                catch
                { }
                _processingThread = null;
            }
        }

        private void ProcessingMethod()
        {
            FsuipcSdk.Fsuipc fsuipc = null;
            bool connected = false;
            try
            {
                int ret = 0;
                int interval = _configuration.Settings.Interval;
                int fs = 0;
                switch (_configuration.Settings.FSVersion)
                {
                    case FSVersion.Dowolna:
                        fs = FsuipcSdk.Fsuipc.SIM_ANY;
                        break;

                    case FSVersion.FS2002:
                        fs = FsuipcSdk.Fsuipc.SIM_FS2K2;
                        break;

                    case FSVersion.FS2004:
                        fs = FsuipcSdk.Fsuipc.SIM_FS2K4;
                        break;

                    case FSVersion.FSX:
                        fs = FsuipcSdk.Fsuipc.SIM_FSX;
                        break;

                    default:
                        throw new Exception("Nieobsługiwana wersja symulatora '" + _configuration.Settings.FSVersion.ToString() + "'.");
                }
                _log.Log(this, "Próba połączenia z symulatorem w wersji '" + _configuration.Settings.FSVersion.ToString() + "'.");
                fsuipc = new FsuipcSdk.Fsuipc();
                while (_working)
                {
                    if (fsuipc.FSUIPC_Open(fs, ref ret))
                    {
                        _log.Log(this, "Połączono z symulatorem w wersji '" + _configuration.Settings.FSVersion.ToString() + "'.");
                        connected = true;
                        break;
                    }
                    Thread.Sleep(100);
                }

                List<VariableToWrite> vars = new List<VariableToWrite>();
                _signal.Set();
                while (_working)
                {
                    _signal.WaitOne();
                    if (!_working)
                    {
                        break;
                    }

                    // pobranie tablicy z wartościami do zapisu                    
                    lock (_syncQueue)
                    {
                        while (_writeQueue.Count > 0)
                        {
                            vars.Add(_writeQueue.Dequeue());
                        }
                    }
                    _signal.Reset();
                    if (vars.Count > 0)
                    {
                        int ile = 0;
                        while (vars.Count > 0)
                        {
                            VariableToWrite v = vars[0];
                            if (!(v.Variable is SpecialOutputVariable))
                            {
                                ile++;
                            }
                            ret = v.Variable.SetValue(v.Value, fsuipc);
                            if (ret != FsuipcSdk.Fsuipc.FSUIPC_ERR_OK)
                            {
                                _log.Log(this, string.Format("Błąd ({0}) podczas zapisywania wartości zmiennej o identyfikatorze '{1}'.", ret, v.Variable.ID));
                            }
                            vars.RemoveAt(0);
                        }

                        if (ile > 0 && (_autoFlush.State || _flush.State))
                        {
                            #if DEBUG
                            _log.Log(this, "FSUIPC_Process");
                            #endif
                            if (!fsuipc.FSUIPC_Process(ref ret))
                            {
                                _log.Log(this, string.Format("Błąd ({0}) podczas wysyłania wartości do symulatora.", ret));
                            }

                            _flush.Reset();
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                _log.Log(this, ex.ToString());
            }
            finally
            {
                if (fsuipc != null)
                {
                    try
                    {
                        fsuipc.FSUIPC_Close();
                        if (connected)
                        {
                            _log.Log(this, "Rozłączono z symulatorem '" + _configuration.Settings.FSVersion.ToString() + "'.");
                        }
                    }
                    catch { }
                }
            }
        }

        #endregion

        #region IModuleConfiguration Members

        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            MessageBox.Show(parent, "Konfiguracja części wyjściowej tego modułu korzysta z konfiguracji części wejściowej tego modułu. Wywołaj konfigurację części wejściowej tego modułu aby skonfigurować moduł.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        #endregion

        #region IModuleHelp Members

        public void Help(System.Windows.Forms.IWin32Window parent)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IOutput Members

        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return _outputs; }
        }

        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (OutputVariable v in _outputs)
            {
                if (v.ID == variableID && v.Type == type)
                {
                    if (!_registered.ContainsKey(variableID))
                    {
                        _registered.Add(variableID, v);
                    }
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterChangableVariable(string variableID)
        {
            if (_registered.ContainsKey(variableID))
            {
                _registered.Remove(variableID);
                return;
            }
            throw new Exception(string.Format("Brak zarejestrowanej zmiennej o identyfikatorze '{0}'.", variableID));
        }

        public void SetVariableValue(string variableID, object value)
        {
            lock (_syncQueue)
            {
                _writeQueue.Enqueue(new VariableToWrite()
                {
                    Variable = _registered[variableID],
                    Value = value
                });
            }
            _signal.Set();
        }

        #endregion

        #region IModuleFunctions Members

        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get { return Utils.__outputFunctions; }
        }

        #endregion
    }
}
