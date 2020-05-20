using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SkalarkiIO
{
    internal interface IWorking
    {
        bool Working
        {
            get;
        }
        
        void Log(string text);
        
        void Exception(object source, Exception exception);
    }
    
    public class SkalarkiIO : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration, IWorking
    {
        public SkalarkiIO()
        {
        }

        #region IModule Members

        public string Name
        {
            get { return "SkalarkiIO"; }
        }

        public string Description
        {
            get { return "Moduł do obsługi urządzeń SkalarkiIO."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 1);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        private ModulesConfiguration _configuration = null;
        
        private InputVariable[] _inputs = new InputVariable[0];
        private OutputVariable[] _outputs = new OutputVariable[0];

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            LoadConfiguration();
        }
        
        public void Log(string text)
        {
            _log.Log(this, text);
        }
        
        private void LoadConfiguration()
        {
            // wczytanie konfiguracji
            _configuration = ModulesConfiguration.Load();
            
            List<InputVariable> inputs = new List<InputVariable>();
            inputs.AddRange(_configuration.DigitalInputs);
            foreach (Device d in _configuration.Devices)
            {
                inputs.AddRange(d.DeviceInputVariables);
            }
            _inputs = inputs.ToArray();
            
            List<OutputVariable> outputs = new List<OutputVariable>();
            outputs.AddRange(_configuration.DigitalOutputs);
            outputs.AddRange(_configuration.LED7DisplayOutputs);
            foreach (Device d in _configuration.Devices)
            {
                outputs.AddRange(d.DeviceOutputVariables);
            }
            
            #warning TODO 7led
            // dla każdego pojedynczego wyświetlacza dodać zmienną typu int do bitowego sterowania segmentami
            
            _outputs = outputs.ToArray();
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.InputOutput);
        }

        private volatile bool _working = false;
        private Thread [] _readingThreads = null;
        private Dictionary<string, OutputVariable> _outputVariables = new Dictionary<string, OutputVariable>();
        private List<Device> _usedDevice = new List<Device>();
        private List<EncoderInput> _encoders = new List<EncoderInput>();

        public void Start(HomeSimCockpitSDK.StartStopType startType)
        {
            if (Working)
            {
                return;
            }
            
            Stop(startType);
            _working = true;
            _usedDevice.Clear();
            
            Dictionary<Device, List<InputVariable>> inputsDevice = new Dictionary<Device, List<InputVariable>>();
            
            // sprawdzenie czy są subskrypcje na enkodery
            for (int i = 0; i < _configuration.Encoders.Length; i++)
            {
                Encoder e = _configuration.Encoders[i];
                if (e.LeftInput.IsSubscribed || e.RightInput.IsSubscribed)
                {
                    e.LeftInput.Module = this;
                    e.LeftInput.Reset();
                    e.RightInput.Module = this;
                    e.RightInput.Reset();
                    EncoderInput ei = new EncoderInput(e.LeftInput, e.RightInput);
                    _encoders.Add(ei);
                    // dodanie enkodera jako zmiennej do śledzenia
                    if (ei.UseAsInputVariable)
                    {
                        ei.Module = this;
                        ei.Reset();
                        Device d = ei.Device.Extension ? ei.Device.Parent : ei.Device;
                        if (!inputsDevice.ContainsKey(d))
                        {
                            inputsDevice.Add(d, new List<InputVariable>());
                        }
                        inputsDevice[d].Add(ei);
                    }
                }
            }
            
            for (int i = 0; i < _inputs.Length; i++)
            {
                if (_inputs[i].IsSubscribed)
                {
                    if (_encoders.Find(delegate(EncoderInput o)
                                       {
                                           return o.LeftInput == _inputs[i] || o.RightInput == _inputs[i];
                                       }) != null)
                    {
                        continue;
                    }
                    
                    _inputs[i].Module = this;
                    _inputs[i].Reset();
                    Device d = _inputs[i].Device.Extension ? _inputs[i].Device.Parent : _inputs[i].Device;
                    if (!inputsDevice.ContainsKey(d))
                    {
                        inputsDevice.Add(d, new List<InputVariable>());
                    }
                    inputsDevice[d].Add(_inputs[i]);
                }
            }
            if (inputsDevice.Count > 0)
            {
                _readingThreads = new Thread[inputsDevice.Count];
                int i = 0;
                foreach (KeyValuePair<Device, List<InputVariable>> kvp in inputsDevice)
                {
                    kvp.Key.PrepareForReading();
                    _readingThreads[i] = new Thread(new ParameterizedThreadStart(kvp.Key.ReadingMethod));
                    _readingThreads[i].Name = "Wątek [" + i.ToString() + "] - Czytanie z urządzenia: " + kvp.Key.Id;
                    _readingThreads[i].Start(new object[] { this , kvp.Value.ToArray() });
                    i++;
                }
            }
            
            foreach (KeyValuePair<string, OutputVariable> kvp in _outputVariables)
            {
                kvp.Value.Reset();
                if (kvp.Value is IDevices)
                {
                    Device [] ds = ((IDevices)kvp.Value).MainDevices;
                    for (int i = 0; i < ds.Length; i++)
                    {
                        if (!_usedDevice.Contains(ds[i]))
                        {
                            _usedDevice.Add(ds[i]);
                        }
                    }
                }
                else
                {
                    if (!_usedDevice.Contains(kvp.Value.Device.MainDevice))
                    {
                        _usedDevice.Add(kvp.Value.Device.MainDevice);
                    }
                }
            }
            
            foreach (Device d in _usedDevice)
            {
                if (inputsDevice.ContainsKey(d))
                {
                    d.Open(this, true);
                }
                else
                {
                    d.Open(this, false);
                }
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType stopType)
        {
            _working = false;
            
            // zatrzymanie wszystkich wątków
            if (_readingThreads != null)
            {
                Thread.Sleep(500);
                for (int i = 0; i < _readingThreads.Length; i++)
                {
                    try
                    {
                        _readingThreads[i].Join(100);
                    }
                    catch{}
                    if (_readingThreads[i].IsAlive)
                    {
                        try
                        {
                            _readingThreads[i].Abort();
                        }
                        catch{}
                        _readingThreads[i] = null;
                    }
                }
                _readingThreads = null;
            }
            
            // zamknięcie wszystkich urządzeń
            foreach (Device d in _usedDevice)
            {
                d.Close();
            }
            _usedDevice.Clear();
            
            // przywrócenie konfiguracji enkoderów
            foreach (EncoderInput ei in _encoders)
            {
                ei.Clear();
            }
            _encoders.Clear();
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
                    if (!_outputVariables.ContainsKey(variableID))
                    {
                        _outputVariables.Add(variableID, v);
                    }
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterChangableVariable(string variableID)
        {
            if (_outputVariables.ContainsKey(variableID))
            {
                _outputVariables.Remove(variableID);
                return;
            }
            throw new Exception(string.Format("Brak zarejestrowanej zmiennej o identyfikatorze '{0}'.", variableID));
        }

        public void SetVariableValue(string variableID, object value)
        {
            _outputVariables[variableID].SetValue(value);
        }

        #endregion

        #region IModuleConfiguration Members

        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                System.Windows.Forms.MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return false;
            }
            ConfigurationDialog d = new ConfigurationDialog(this, ModulesConfiguration.Load());
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                d.Configuration.Save();
                ModulesConfiguration.Reload();
                LoadConfiguration();
                return true;
            }
            return false;
        }

        #endregion

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
                    v.RegisterListener(variableID, listenerMethod);
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
                    v.UnregisterListener(variableID, listenerMethod);
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}'.", variableID));
        }

        #endregion
        
        public bool Working {
            get {
                return _working;
            }
        }
        
        public void Exception(object source, Exception exception)
        {
            
        }
    }
}

