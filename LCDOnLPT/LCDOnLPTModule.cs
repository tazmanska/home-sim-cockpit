/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-04
 * Godzina: 19:25
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LCDOnLPT
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class LCDOnLPTModule : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleConfiguration, IDevice
    {
        private class Device : IDevice
        {
            public Device(int lptAddress)
            {
                _port = new LPTPort(lptAddress);
            }
            
            private LPTPort _port = null;
            
            public void Write(LCDData data)
            {
                if (data.Command)
                {
                    _port.WriteControl(data.LCD, data.Data, data.Multiplier);
                }
                else
                {
                    _port.WriteData(data.LCD, data.Data, data.Multiplier);
                }
            }
            
            public void WriteControl(LCDSeq lcd, int control, int multiplier)
            {
                _port.WriteControl(lcd, control, multiplier);
            }
        }
        
        public string Name
        {
            get { return "LCDOnLPT"; }
        }

        public string Description
        {
            get { return "Moduł do obsługi wyświetlaczy HD44780 podłączonych do portu LPT."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 0);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        private string _xmlConfigurationFilePath = null;
        
        private ModuleConfiguration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            LoadConfiguration();
        }
        
        private void LoadConfiguration()
        {
            // utworzenie ścieżki do pliku konfiguracyjnego
            _xmlConfigurationFilePath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");

            // wczytanie konfiguracji
            _configuration = ModuleConfiguration.Load(_xmlConfigurationFilePath);
            
            _configuration.LCD1.Configuration = _configuration;
            _configuration.LCD2.Configuration = _configuration;
            List<IOutputVariable> vars = new List<IOutputVariable>();
            vars.AddRange(_configuration.Areas);
            if (_configuration.LCD1.Enabled)
            {
                vars.AddRange(_configuration.LCD1.Variables);
            }
            if (_configuration.LCD2.Enabled)
            {
                vars.AddRange(_configuration.LCD2.Variables);
            }
            
            _outputs = vars.ToArray();
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
        }

        private volatile bool _working = false;

        public void Start(HomeSimCockpitSDK.StartStopType startType)
        {
            _working = true;
            _queue.Clear();
            if (_registered.Count > 0)
            {
                _port = new LPTPort(_configuration.LPTAddress);
                _configuration.LCD1.Device = this;
                if (_configuration.LCD1.Enabled)
                {
                    _configuration.LCD1.Initialize();
                }
                _configuration.LCD2.Device = this;
                if (_configuration.LCD2.Enabled)
                {
                    _configuration.LCD2.Initialize();
                }
                _thread = new Thread(new ThreadStart(ProcessingThread));
                _thread.Start();
            }
        }
        
        private Queue<LCDData> _queue = new Queue<LCDData>();
        private Thread _thread = null;
        private AutoResetEvent _event = new AutoResetEvent(false);
        private object _syncQueue = new object();
        private LPTPort _port = null;
        
        private void ProcessingThread()
        {
            try
            {
                while (_working)
                {
                    _event.WaitOne();
                    LCDData[] data = null;
                    lock (_syncQueue)
                    {
                        data = _queue.ToArray();
                        _queue.Clear();
                    }
                    _event.Reset();
                    if (data != null && data.Length > 0)
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i].Command)
                            {
                                _port.WriteControl(data[i].LCD, data[i].Data, data[i].Multiplier);
                            }
                            else
                            {
                                _port.WriteData(data[i].LCD, data[i].Data, data[i].Multiplier);
                            }
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
        }
        
        public void Stop(HomeSimCockpitSDK.StartStopType stopType)
        {
            _working = false;
            _event.Set();
            Thread.Sleep(10);
            if (_thread != null)
            {
                try
                {
                    _thread.Abort();
                }
                catch
                { }
                _thread = null;
            }
            IDevice d = new Device(_configuration.LPTAddress);
            if (_configuration.LCD1.Enabled)
            {
                _configuration.LCD1.Device = d;
                _configuration.LCD1.Uninitialize();
            }
            if (_configuration.LCD2.Enabled)
            {
                _configuration.LCD2.Device = d;
                _configuration.LCD2.Uninitialize();
            }
        }
        
        private IOutputVariable[] _outputs = null;
        
        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return _outputs; }
        }
        
        private Dictionary<string, IOutputVariable> _registered = new Dictionary<string, IOutputVariable>();
        
        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (IOutputVariable v in _outputs)
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
            _registered[variableID].SetValue(value);
        }
        
        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ConfigDialog cd = new ConfigDialog(ModuleConfiguration.Load(_xmlConfigurationFilePath));
            if (cd.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                ModuleConfiguration.Save(_xmlConfigurationFilePath, cd.Configuration);
                _configuration = cd.Configuration;
                LoadConfiguration();
                return true;
            }
            return false;
        }
        
        public void Write(LCDData data)
        {
            lock (_syncQueue)
            {
                _queue.Enqueue(data);
            }
            _event.Set();
        }
        
        public void WriteControl(LCDSeq lcd, int control, int multiplier)
        {
            _port.WriteControl(lcd, control, multiplier);
        }
        
        #region Timer
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        
        private static long __frequency = 0;
        
        static LCDOnLPTModule()
        {
            QueryPerformanceFrequency(out __frequency);
        }
        
        internal static void Wait(long uSeconds, int multipier)
        {
            if (uSeconds <= 0 || multipier <= 0)
            {
                return;
            }
            long elapsed = 0;
            long scaled = 0;
            long start = 0;
            long current = 0;
            scaled = uSeconds * multipier;            

            QueryPerformanceCounter(out start);
            do
            {
                QueryPerformanceCounter(out current);
                if (current < start)
                {
                    start = 0;
                }
                elapsed = ((current - start) * 1000000) / __frequency;

            } while (elapsed <= scaled);
        }
        
        #endregion
    }
}