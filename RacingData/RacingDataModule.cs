/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-07-04
 * Godzina: 18:10
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using SimAdapter.Framework.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using HomeSimCockpitSDK;
using SimAdapter.Framework.Interfaces;

namespace RacingData
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class RacingDataModule : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration, IDisposable
    {
        public RacingDataModule()
        {
            _simAdapter = new _SimAdapter();
            _simAdapter.Show();
            _simAdapter.Hide();
            _simAdapter.simAdapterClientControl1.ConnectionAcknowledged += new EventHandler(_simClient_ConnectionAcknowledged);
            _simAdapter.simAdapterClientControl1.ConnectionDisconnected += new EventHandler(_simClient_ConnectionDisconnected);
            _simAdapter.simAdapterClientControl1.ConnectionRefused += new EventHandler(_simClient_ConnectionRefused);
            _simAdapter.simAdapterClientControl1.CurrentSimChanged += new EventHandler<SimChangedEventArgs>(_simClient_CurrentSimChanged);
            _simAdapter.simAdapterClientControl1.DataIntervalChanged += new EventHandler<DataIntervalChangedEventArgs>(_simClient_DataIntervalChanged);
            _simAdapter.simAdapterClientControl1.ReceivingData += new EventHandler<ReceivingDataEventArgs>(_simClient_ReceivingData);
            _simAdapter.simAdapterClientControl1.ServerNotFound += new EventHandler(_simClient_ServerNotFound);
            _simAdapter.simAdapterClientControl1.WrongPassword += new EventHandler(_simClient_WrongPassword);
        }

        void _simClient_WrongPassword(object sender, EventArgs e)
        {
            _log.Log(this, "Nieprawdiłowe hasło serwera SimAdapter.");
        }

        void _simClient_ServerNotFound(object sender, EventArgs e)
        {
            _log.Log(this, "Nie znaleziono serwera SimAdapter.");
        }

        void _simClient_ReceivingData(object sender, ReceivingDataEventArgs e)
        {
            if (_working)
            {
                if (_firstData)
                {
                    _firstData = false;
                    
                    Type t = e.Data.GetType();
                    foreach (Variable v in _registeredVariable)
                    {
                        PropertyInfo fi = t.GetProperty(v.ID);
                        if (fi != null)
                        {
                            object value = fi.GetValue(e.Data, null);
                            v.SetValue(value);
                        }
                    }
                }
                else
                {
                    Type t = e.Data.GetType();
                    foreach (Variable v in _registeredVariable)
                    {
                        PropertyInfo fi = t.GetProperty(v.ID);
                        if (fi != null)
                        {
                            object value = fi.GetValue(e.Data, null);
                            v.SetValueIfChanged(value);
                        }
                    }
                }
            }
        }

        void _simClient_DataIntervalChanged(object sender, DataIntervalChangedEventArgs e)
        {
            _log.Log(this, "Zmieniono interwał próbkowania danych: " + e.DataInterval.ToString() + "ms.");
        }

        void _simClient_CurrentSimChanged(object sender, SimChangedEventArgs e)
        {
            _log.Log(this, "Nowy aktywny symulator: " + e.SimName + " (" + e.SimVersion + ").");
        }

        void _simClient_ConnectionRefused(object sender, EventArgs e)
        {
            _log.Log(this, "Nie można połączyć z serwerem SimAdapter.");
        }
        
        public string Name
        {
            get { return "RacingModule"; }
        }

        public string Description
        {
            get { return "Moduł odczytuje dane z programu SimAdapter dot. symulatorów wyścigów."; }
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
        
        internal class Variable : IVariable
        {
            public Variable(IInput module, string id, VariableType type, string description)
            {
                _module = module;
                _ID = id;
                _Type = type;
                _description = description;

                switch (type)
                {
                    case VariableType.Bool:
                        _value = false;
                        break;

                    case VariableType.Int:
                        _value = 0;
                        break;

                    case VariableType.Double:
                        _value = 0d;
                        break;
                }
            }

            private string _ID = null;
            private VariableType _Type = VariableType.Unknown;
            private string _description = null;
            private IInput _module = null;

            public string ID
            {
                get { return _ID; }
            }

            public VariableType Type
            {
                get { return _Type; }
            }

            public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

            protected object _value = null;

            public virtual void SetValue(object value)
            {
                switch (Type)
                {
                    case VariableType.Bool:
                        _value = (bool)value;
                        break;
                    case VariableType.Int:
                        _value = (int)value;
                        break;
                    case VariableType.Double:
                        _value = Convert.ToDouble(value);
                        break;
                    default:
                        throw new Exception("Nieokreślony typ wartości.");
                }
                FireEvent();
            }

            public virtual void SetValueIfChanged(object value)
            {
                switch (Type)
                {
                    case VariableType.Bool:
                        if ((bool)_value != (bool)value)
                        {
                            SetValue((bool)value);
                        }
                        break;
                    case VariableType.Int:
                        if ((int)_value != (int)value)
                        {
                            SetValue((int)value);
                        }
                        break;
                    case VariableType.Double:
                        double d1 = Convert.ToDouble(value);
                        if ((double)_value != d1)
                        {
                            SetValue(d1);
                        }
                        break;
                    default:
                        throw new Exception("Nieokreślony typ wartości.");
                }
            }

            protected void FireEvent()
            {
                VariableChangeSignalDelegate variableChanged = VariableChanged;
                if (VariableChanged != null)
                {
                    VariableChanged(_module, ID, _value);
                }
            }

            public string Description
            {
                get { return _description; }
            }

            public override string ToString()
            {
                return ID;
            }

            public object Value
            {
                get { return _value; }
            }
        }

        private Variable[] _input = null;

        private HomeSimCockpitSDK.ILog _log = null;
        
        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;
            
            // wczytanie konfiguracji
            _configuration = global::RacingData.Configuration.Load(ConfigurationFilePath);
            
            List<Variable> variables = new List<Variable>();
            Type t = typeof(ISimData);
            PropertyInfo[] properties = t.GetProperties();//BindingFlags.GetProperty);
            for (int i = 0; i < properties.Length; i++)
            {
                Variable v = null;
                if (properties[i].PropertyType == typeof(bool))
                {
                    v = new Variable(this, properties[i].Name, VariableType.Bool, "");
                }
                
                if (properties[i].PropertyType == typeof(int))
                {
                    v = new Variable(this, properties[i].Name, VariableType.Int, "");
                }
                
                if (properties[i].PropertyType == typeof(double))
                {
                    v = new Variable(this, properties[i].Name, VariableType.Double, "");
                }
                
                if (v != null)
                {
                    variables.Add(v);
                }
            }
            
            _input = variables.ToArray();
        }

        void _simClient_ConnectionDisconnected(object sender, EventArgs e)
        {
            _log.Log(this, "Zamknięto połączenie z serwerem SimAdapter.");
        }

        void _simClient_ConnectionAcknowledged(object sender, EventArgs e)
        {
            _log.Log(this, "Połączono z serwerem SimAdapter.");
        }
        
        public void Unload()
        {
            Stop(StartStopType.Input);
        }
        
        private volatile bool _working = false;
        
        public void Start(HomeSimCockpitSDK.StartStopType startStopType)
        {
            Stop(startStopType);
            _working = true;
            _firstData = true;
            if (_registeredVariable.Count > 0)
            {
                _simAdapter.simAdapterClientControl1.ServerIP = _configuration.ServerIP;
                _simAdapter.simAdapterClientControl1.ServerPort = _configuration.ServerPort;
                _simAdapter.simAdapterClientControl1.Password = _configuration.Password;
                _simAdapter.simAdapterClientControl1.ClientPort = _configuration.ClientPort;
                _simAdapter.simAdapterClientControl1.Connect();
            }
        }
        
        private volatile bool _firstData = false;
        
        public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
        {
            _working = false;
            if (_simAdapter.simAdapterClientControl1.Connected)
            {
                _simAdapter.simAdapterClientControl1.Disconnect();
            }
        }
        
        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _input; }
        }
        
        private List<Variable> _registeredVariable = new List<Variable>();

        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (Variable v in _input)
            {
                if (v.ID == variableID && v.Type == type)
                {
                    v.VariableChanged += listenerMethod;
                    if (!_registeredVariable.Contains(v))
                    {
                        _registeredVariable.Add(v);
                    }
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            foreach (Variable v in _input)
            {
                if (v.ID == variableID)
                {
                    v.VariableChanged -= listenerMethod;
                    return;
                }
            }
            throw new Exception("Nie istnieje zmienna o identyfikatorze '" + variableID + "'.");
        }
        
        private string _configPath = null;

        public string ConfigurationFilePath
        {
            get
            {
                if (_configPath == null)
                {
                    _configPath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
                }
                return _configPath;
            }
        }

        private Configuration _configuration = null;
        
        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ConfigurationDialog d = new ConfigurationDialog();
            d.ServerIP = _configuration.ServerIP;
            d.ServerPort = _configuration.ServerPort;
            d.Password = _configuration.Password;
            d.ClientPort = _configuration.ClientPort;
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                _configuration.ServerIP = d.ServerIP;
                _configuration.ServerPort = d.ServerPort;
                _configuration.Password = d.Password;
                _configuration.ClientPort = d.ClientPort;
                _configuration.Save(ConfigurationFilePath);
            }
            return false;
        }
        
        private _SimAdapter _simAdapter = null;
        
        public void Dispose()
        {
            if (_simAdapter != null)
            {
                _simAdapter.Close();
                _simAdapter.Dispose();
                _simAdapter = null;
            }
        }
    }
}