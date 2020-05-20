using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FSData
{
    public class FSDataInput : HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration, HomeSimCockpitSDK.IModuleFunctions
    {
        private InputVariable[] _inputs = new InputVariable[0];

        #region IInput Members

        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _inputs; }
        }

        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (InputVariable v in _inputs)
            {
                if (v.ID == variableID && v.Type == type)
                {
                    v.VariableChanged += listenerMethod;
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            foreach (InputVariable v in _inputs)
            {
                if (v.ID == variableID)
                {
                    v.VariableChanged -= listenerMethod;
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}'.", variableID));
        }

        #endregion

        #region IModule Members

        public string Name
        {
            get { return "FSDataInput"; }
        }

        public string Description
        {
            get { return "Moduł do odczytywania danych z symulatora MS Flight Simulator (2002, 2004 i X)."; }
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

            _inputs = _configuration.InputVariables;
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
        }

        private InputVariable[] _spiesVariables = null;
        private volatile bool _working = false;
        private Thread _processingThread = null;

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
            _working = true;

            // wystartowanie watka (jesli sa jakies zmienne do sledzenia)
            List<InputVariable> vs = new List<InputVariable>();
            if (_inputs != null)
            {
                for (int i = 0; i < _inputs.Length; i++)
                {
                    if (_inputs[i].IsSubscribed)
                    {
                        _inputs[i].Module = this;
                        _inputs[i].Reset();
                        vs.Add(_inputs[i]);
                    }
                }
            }

            _spiesVariables = vs.ToArray();
            if (_spiesVariables.Length > 0)
            {
                // wystartowanie wątka
                
                _processingThread = new Thread(new ThreadStart(ProcessingMethod));
                _processingThread.Start();
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            // zatrzymanie watka
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

                byte v1 = 0;
                short v2 = 0;
                int v4 = 0;
                long v8 = 0;
                object v = null;
                bool ok = false;

                while (_working)
                {
                    //lock (FsuipcSdk.Fsuipc.__FSUIPC_LOCKER)
                    //FsuipcSdk.Fsuipc.__FSUIPC_EVENT.WaitOne();
                    //try
                    {
                        for (int i = 0; i < _spiesVariables.Length; i++)
                        {
                            if (!fsuipc.FSUIPC_Read(_spiesVariables[i].Offset, _spiesVariables[i].Size, ref _spiesVariables[i].Token, ref ret))
                            {
                                _log.Log(this, "Błąd (" + ret.ToString() + " podczas żądania odczytu wartości zmiennej o identyfikatorze '" + _spiesVariables[i].ID + "'.");
                            }
                        }

                        if (!fsuipc.FSUIPC_Process(ref ret))
                        {
                            _log.Log(this, "Błąd (" + ret.ToString() + ") podczas żądania odczytania żądanych wartości.");
                        }
                        else
                        {
                            for (int i = 0; i < _spiesVariables.Length; i++)
                            {

                                switch (_spiesVariables[i].FSType)
                                {
                                    case FSDataType.Byte:
                                        ok = fsuipc.FSUIPC_Get(ref _spiesVariables[i].Token, ref v1);
                                        v = v1;
                                        break;

                                    case FSDataType.Short:
                                        ok = fsuipc.FSUIPC_Get(ref _spiesVariables[i].Token, ref v2);
                                        v = v2;
                                        break;

                                    case FSDataType.Int:
                                        ok = fsuipc.FSUIPC_Get(ref _spiesVariables[i].Token, ref v4);
                                        v = v4;
                                        break;

                                    case FSDataType.Long:
                                        ok = fsuipc.FSUIPC_Get(ref _spiesVariables[i].Token, ref v8);
                                        v = v8;
                                        break;

                                    case FSDataType.ByteArray:
                                        byte[] buf = new byte[_spiesVariables[i].Size];
                                        ok = fsuipc.FSUIPC_Get(ref _spiesVariables[i].Token, buf.Length, ref buf);
                                        v = buf;
                                        break;
                                }

                                if (ok)
                                {
                                    _spiesVariables[i].SetValue(v);
                                }
                                else
                                {
                                    _log.Log(this, "Błąd podczas pobierania wartości zmiennej o identyfikatorze '" + _spiesVariables[i].ID + "', Offset = 0x" + _spiesVariables[i].Offset.ToString("X4") + ".");
                                }
                            }
                        }

                    }
                    //finally
                    //{
                    //    FsuipcSdk.Fsuipc.__FSUIPC_EVENT.Set();
                    //}
                    if (interval > 0)
                    {
                        Thread.Sleep(interval);
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
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ConfigurationDialog d = new ConfigurationDialog();
            d.FSVersion = _configuration.Settings.FSVersion;
            d.Interval = _configuration.Settings.Interval;
            if (d.ShowDialog(parent) == DialogResult.OK)
            {
                _configuration.Settings.FSVersion = d.FSVersion;
                _configuration.Settings.Interval = d.Interval;
                _configuration.Save();
            }

            return false;
        }

        #endregion

        #region IModuleHelp Members

        public void Help(System.Windows.Forms.IWin32Window parent)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IModuleFunctions Members

        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get { return Utils.__inputFunctions; }
        }

        #endregion
    }
}
