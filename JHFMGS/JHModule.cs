/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-02-18
 * Godzina: 14:16
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using HomeSimCockpitSDK;
using JHFMGS.Client;

namespace JHFMGS
{
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public class JHModule : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleFunctions, HomeSimCockpitSDK.IModuleConfiguration
	{
		
		
		public string Name {
			get {
				return "JHFMGS";
			}
		}
		
		public string Description {
			get {
				return "Moduł do obsługi FMGS by JH";
			}
		}
		
		public string Author {
			get {
				return "codeking";
			}
		}
		
		public string Contact {
			get {
				return "codeking@o2.pl";
			}
		}
		
		public Version Version {
			get {
				return new Version(1, 0, 0, 1);
			}
		}
		
		private HomeSimCockpitSDK.ILog _log = null;
		
		private Variable _testVariable = null;
		
		public void Load(HomeSimCockpitSDK.ILog log)
		{
            _log = log;
            
            // wczytanie konfiguracji
            _configuration = global::JHFMGS.Configuration.Load(ConfigurationFilePath);
            
            List<Variable> variables = new List<Variable>();
            string [] leds = Enum.GetNames(typeof(Client.JH_LEDs));
            for (int i = 0; i < leds.Length; i++)
            {
            	Variable v = new Variable(this, leds[i], VariableType.Bool, "", (int)Enum.Parse(typeof(Client.JH_LEDs), leds[i]));
            	variables.Add(v);
            }
            
            string [] datas = Enum.GetNames(typeof(Client.JH_Data));
            for (int i = 0; i < datas.Length; i++)
            {
            	Client.JH_Data data = (Client.JH_Data)Enum.Parse(typeof(Client.JH_Data), datas[i]);
            	
            	switch (data)
            	{
            		case JH_Data.DATA_CPT_MCDU_DIMMER:
            		case JH_Data.DATA_CPT_ND_DIMMER:
            			continue;
            	}
            	
            	Variable v = new Variable(this, datas[i], VariableType.Int, "", (int)data);
            	variables.Add(v);
            }
            
            variables.Add(_testVariable);
            
            foreach (Variable v in variables)
            {
            	_vars.Add(v.VID, v);
            }
            
            _input = variables.ToArray();
		}
		
		private Client.Client _client = new JHFMGS.Client.Client();
		private Dictionary<int, Variable> _vars = new Dictionary<int, Variable>();
		
		public JHModule()
		{
			_client.ClientError += new EventHandler<ClientErrorEventArgs>(_client_ClientError);
			_client.Connected += new EventHandler<ConnectedEventArgs>(_client_Connected);
			_client.Disconnected += new EventHandler<DisconnectedEventArgs>(_client_Disconnected);
			_client.LEDStateChange += new EventHandler<LEDStateChangeEventArgs>(_client_LEDStateChange);
			_client.LEDTest += new EventHandler<LEDTestEventArgs>(_client_LEDTest);
			_client.Rescan += new EventHandler<RescanEventArgs>(_client_Rescan);
			
			_testVariable = new JHModule.Variable(this, "LED_TEST", VariableType.Bool, "OVHD Test LED", -666);
		}

		void _client_Rescan(object sender, RescanEventArgs e)
		{
			
		}

		void _client_LEDTest(object sender, LEDTestEventArgs e)
		{
			_testVariable.SetValueIfChanged(e.Test);
		}

		void _client_LEDStateChange(object sender, LEDStateChangeEventArgs e)
		{
			int v = (int)e.LED;
			Variable var = null;
			if (_vars.TryGetValue(v, out var))
			{
				if (v >= 0)
				{
					// led
					var.SetValueIfChanged((bool)(e.State != 0));
				}
				else
				{
					// data
					var.SetValueIfChanged(e.State);
				}
			}
		}

		void _client_Disconnected(object sender, DisconnectedEventArgs e)
		{
			_log.Log(this, "Rozłączono z serwerem JH.");
		}

		void _client_Connected(object sender, ConnectedEventArgs e)
		{
			_log.Log(this, "Połączono z serwerem JH.");
		}

		void _client_ClientError(object sender, ClientErrorEventArgs e)
		{
			_log.Log(this, "Błąd: " + e.Error.Message);
		}
		
		public void Unload()
		{
		}
	
		public void Start(HomeSimCockpitSDK.StartStopType startStopType)
		{
			if (_registeredVariable.Count > 0)
			{	
				foreach (Variable v in _registeredVariable)
				{
					v.First = true;
				}				
				
				_client.ServerAddress = _configuration.ServerIP.ToString();
				_client.ServerPort = _configuration.ServerPort;
				
				_working = true;
				
				_client.Connect();
			}
		}
		
		public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
		{
			_working = false;
			_client.Disconnect();
		}
		
		public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _input; }
        }
		        
        internal class Variable : IVariable
        {
            public Variable(IInput module, string id, VariableType type, string description, int vId)
            {
                _module = module;
                _ID = id;
                _Type = type;
                _description = description;
                VID = vId;

                switch (type)
                {
                    case VariableType.Bool:
                        _value = false;
                        break;

                    case VariableType.Int:
                        _value = 0;
                        break;
                }
            }
            
            public int VID
            {
            	get;
            	private set;
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
            
            public volatile bool First = true;
            
            public bool Registered
            {
            	get { return VariableChanged != null; }
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
                        if ((bool)_value != (bool)value || First)
                        {
                        	First = false;
                            SetValue((bool)value);
                        }
                        break;
                    case VariableType.Int:
                        if ((int)_value != (int)value || First)
                        {
                        	First = false;
                            SetValue((int)value);
                        }
                        break;
                    default:
                        throw new Exception("Nieokreślony typ wartości.");
                }
            }

            protected void FireEvent()
            {
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
        
        private volatile bool _working = false;
        
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
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                _configuration.ServerIP = d.ServerIP;
                _configuration.ServerPort = d.ServerPort;
                _configuration.Save(ConfigurationFilePath);
            }
            return false;
        }		
		
		public ModuleFunctionInfo[] Functions {
			get {
        		return new ModuleFunctionInfo[] { new ModuleFunctionInfo("SendSwitch", "Wysyła sygnał przełącznika", 2, new ModuleExportedFunctionDelegate(SendSwitch)) }; 
			}
		}
	
        private object SendSwitch(object [] arguments)
        {
        	return _client.SendSwitch((JH_Switches)(int)arguments[0], (JH_SwitchControl)(int)arguments[1]);
        }
	}
}