using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using HomeSimCockpitX.LCD;
using RS232HCDevices.Servos;
using RS232HCDevices.Steppers;

namespace RS232HCDevices
{
	public class RS232HCDevices : HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleConfiguration, HomeSimCockpitSDK.IModule2, HomeSimCockpitSDK.IModuleFunctions
	{
		private IOutputVariable[] _outputVariables = null;
		private XMLConfiguration _configuration = null;
		private bool _working = false;

		#region IOutput Members

		public HomeSimCockpitSDK.IVariable[] OutputVariables
		{
			get { return _outputVariables; }
		}

		public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
		{
			foreach (IOutputVariable v in _outputVariables)
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
			if (!_working)
			{
				return;
			}
			_registered[variableID].SetValue(value);
		}
		
		private Dictionary<string, IOutputVariable> _registered = new Dictionary<string, IOutputVariable>();

		#endregion

		#region IModule Members

		public string Name
		{
			get { return "simOUT"; }
		}

		public string Description
		{
			get { return "Obsługa urządzeń wyjściowych podpiętych do portu RS232. Więcej informacji: www.simproject.info"; }
		}

		public string Author
		{
			get { return "codeking"; }
		}

		public string Contact
		{
			get { return "codeking@homesimcockpit.com"; }
		}

		private Version _version = new Version(1, 0, 0, 4);

		public Version Version
		{
			get { return _version; }
		}

		private HomeSimCockpitSDK.ILog _log = null;

		public void Load(HomeSimCockpitSDK.ILog log)
		{
			_log = log;

			LoadConfiguration();
		}
		
		private void LoadConfiguration()
		{
			_configuration = XMLConfiguration.Load();
			
			LoadVariables();
		}

		private void LoadVariables()
		{
			// odczytanie zmiennych
			List<IOutputVariable> variables = new List<IOutputVariable>();

			// dodanie zmiennych do sterowania wyświetlaczami
			foreach (RS232LCD lcd in _configuration.LCDs)
			{
				variables.Add(new LCDOnOffCommandVariable(lcd));
				variables.Add(new LCDClearCommandVariable(lcd));
			}
			
			// dodanie zmiennych do obszarów na LCD
			foreach (LCDArea lcdArea in _configuration.Areas)
			{
				variables.Add(new RS232LCDArea(lcdArea));
			}
			
			// dodanie zmiennych do sterowania diodami
			foreach (LED led in _configuration.LEDs)
			{
				variables.Add(led);
			}
			
			// dodanie zmiennych do obszarów diód
			foreach (LEDGroup ledGroup in _configuration.LEDGroups)
			{
				variables.Add(ledGroup);
			}
			
			// dodanie zmiennych do sterowania wyświetlaczami 7-segmentowymi
			foreach (LEDDisplay ledDisplay in _configuration.LEDDisplays)
			{
				variables.Add(ledDisplay);
				LEDDisplay ledDisplayInt = new LEDDisplay()
				{
					Description = string.Format("{0} Zapis bitowy.", ledDisplay.Description),
					Dictionary = ledDisplay.Dictionary,
					ID = string.Format("{0}_int", ledDisplay.ID),
					Index = ledDisplay.Index,
					LEDDisplayDevice = ledDisplay.LEDDisplayDevice,
					Type = HomeSimCockpitSDK.VariableType.Int
				};
				variables.Add(ledDisplayInt);
			}
			
			// dodanie zmiennych do obszarów wyświetlaczy 7-segmentowych
			foreach (LEDDisplayGroup ledDisplayGroup in _configuration.LEDDisplayGroups)
			{
				variables.Add(ledDisplayGroup);
			}
			
			// dodanie zmiennych do regulacji jasności diod i wyświetlaczy
			IOutputVariable [] addins = _configuration.GetAddinsVariable();
			if (addins != null && addins.Length > 0)
			{
				variables.AddRange(addins);
			}

			_outputVariables = variables.ToArray();
		}

		public void Unload()
		{
			foreach (RS232Configuration inte in _configuration.Interfaces)
			{
				try
				{
					inte.Close(_configuration);
				}
				catch
				{}
			}
		}
		
		private RS232Configuration[] _interfaces = null;
		private Device[] _devices = null;
		private Dictionary<string, StepperMotor> _motors = new Dictionary<string, StepperMotor>();
		private Dictionary<string, Servo> _servos = new Dictionary<string, Servo>();

		public void Start(HomeSimCockpitSDK.StartStopType startType)
		{
			Stop(startType);
			
			// próba otworzenia połączenia z RS232
			_working = true;
			
			if (_registered.Count == 0 && !_requireFunction)
			{
				return;
			}
			
			_motors.Clear();
			_servos.Clear();
			
			// sprawdzenie które interfejsy trzeba uruchomić
			List<RS232Configuration> interfaces = new List<RS232Configuration>();
			List<Device> devices = new List<Device>();
			foreach (KeyValuePair<string, IOutputVariable> kvp in _registered)
			{
				Device[] ints = kvp.Value.Devices;
				foreach (Device inte in ints)
				{
					if (interfaces.Find(delegate(RS232Configuration o)
					                    {
					                    	return o.Id == inte.Interface.Id;
					                    }) == null)
					{
						interfaces.Add(inte.Interface);
						inte.Interface.Log = _log;
						inte.Interface.Module = this;
					}
					if (devices.Find(delegate(Device o)
					                 {
					                 	return o.Id == inte.Id;
					                 }) == null)
					{
						devices.Add(inte);
					}
				}
			}
			
			if (_runSteppers)
			{
				foreach (StepperDevice stepperDevice in _configuration.StepperDevices)
				{
					if (interfaces.Find(delegate(RS232Configuration o)
					                    {
					                    	return o.Id == stepperDevice.Interface.Id;
					                    }) == null)
					{
						interfaces.Add(stepperDevice.Interface);
						stepperDevice.Interface.Log = _log;
						stepperDevice.Interface.Module = this;
					}
					if (devices.Find(delegate(Device o)
					                 {
					                 	return o.Id == stepperDevice.Id;
					                 }) == null)
					{
						devices.Add(stepperDevice);
						if (stepperDevice.Motor1 != null && !string.IsNullOrEmpty(stepperDevice.Motor1.Id))
						{
							if (!_motors.ContainsKey(stepperDevice.Motor1.Id))
							{
								_motors.Add(stepperDevice.Motor1.Id, stepperDevice.Motor1);
							}
						}
						if (stepperDevice.Motor2 != null && !string.IsNullOrEmpty(stepperDevice.Motor2.Id))
						{
							if (!_motors.ContainsKey(stepperDevice.Motor2.Id))
							{
								_motors.Add(stepperDevice.Motor2.Id, stepperDevice.Motor2);
							}
						}
					}
				}
			}
			
			if (_runServos)
			{
				foreach (ServoDevice servoDevice in _configuration.ServoDevices)
				{
					if (interfaces.Find(delegate(RS232Configuration o)
					                    {
					                    	return o.Id == servoDevice.Interface.Id;
					                    }) == null)
					{
						interfaces.Add(servoDevice.Interface);
						servoDevice.Interface.Log = _log;
						servoDevice.Interface.Module = this;
					}
					if (devices.Find(delegate(Device o)
					                 {
					                 	return o.Id == servoDevice.Id;
					                 }) == null)
					{
						devices.Add(servoDevice);
						foreach (Servo servo in servoDevice.Servos)
						{
							if (!string.IsNullOrEmpty(servo.Id) && !_servos.ContainsKey(servo.Id))
							{
								_servos.Add(servo.Id, servo);
							}
						}
					}
				}
			}			
			
			_interfaces = interfaces.ToArray();
			_devices = devices.ToArray();
			
			// otwarcie potrzebnych interfejsów
			for (int i = 0; i < _interfaces.Length; i++)
			{
				_interfaces[i].Open();
			}
			
			// inicjalizacja urządzeń
			for (int j = 0; j < _devices.Length; j++)
			{
				if (_devices[j] is LEDDisplayDevice)
				{
					((LEDDisplayDevice)_devices[j]).Dictionary = _configuration.LEDDisplaysDictionary;
				}
				_devices[j].Initialize();
			}
			
			// inicjalizacja zmiennych
			foreach (KeyValuePair<string, IOutputVariable> kvp in _registered)
			{
				if (kvp.Value is LEDDisplay)
				{
					((LEDDisplay)kvp.Value).Dictionary = _configuration.LEDDisplaysDictionary;
				}
				if (kvp.Value is LEDDisplayGroup)
				{
					((LEDDisplayGroup)kvp.Value).Dictionary = _configuration.LEDDisplaysDictionary;
				}
				kvp.Value.Initialize();
			}
			
			_log.Log(this, "Uruchomiono simOUT");
		}

		public void Stop(HomeSimCockpitSDK.StartStopType stopType)
		{
			if (!_working)
			{
				return;
			}
			_working = false;
			
			// inicjalizacja zmiennych
			foreach (KeyValuePair<string, IOutputVariable> kvp in _registered)
			{
				kvp.Value.Uninitialize();
			}
			
			// uninicjalizacja urządzeń
			if (_devices != null)
			{
				for (int i = 0; i < _devices.Length; i++)
				{
					_devices[i].Uninitialize();
				}
				_devices = null;
			}
			
			// zamknięcie interfejsów
			if (_interfaces != null)
			{
				for (int i = 0; i < _interfaces.Length; i++)
				{
					_interfaces[i].Close(_configuration);
				}
				_interfaces = null;
			}
			
			_log.Log(this, "Zamknięto simOUT");
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
			ConfigurationDialog cd = new ConfigurationDialog(XMLConfiguration.Load(), _log, this);
			if (cd.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
			{
				cd.Configuration.Save();
				_configuration = cd.Configuration;
				LoadVariables();
				return true;
			}
			return false;
		}

		#endregion
		
		private HomeSimCockpitSDK.IScriptHost _scriptHost = null;
		
		public void Init(HomeSimCockpitSDK.IScriptHost scriptHost)
		{
			_scriptHost = scriptHost;
		}
		
		private bool _runSteppers = true;
		private bool _runServos = true;
		private bool _requireFunction = false;
		
		public void RequireFunctions(string[] functionsNames)
		{
			_requireFunction = functionsNames != null && functionsNames.Length > 0;
		}
		
		public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
		{
			get
			{
				return new HomeSimCockpitSDK.ModuleFunctionInfo[]
				{
					new HomeSimCockpitSDK.ModuleFunctionInfo("DefineCharacter", "Funkcja służy do definiowania znaków dla wyświetlacza.", 10, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(DefineCharacter))
						, new HomeSimCockpitSDK.ModuleFunctionInfo("SetPauseStepper", "Funkcja włącza/wyłącza pauze silnika.", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetPauseStepper))
						, new HomeSimCockpitSDK.ModuleFunctionInfo("SetStepperPosition", "Funkcja ustawia pozycję silnika.", -1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetStepperPosition))
						, new HomeSimCockpitSDK.ModuleFunctionInfo("ZeroStepper", "Funkcja ustawia silnik w pozycji zerowej.", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(ZeroStepper))
						
						, new HomeSimCockpitSDK.ModuleFunctionInfo("SetServoEnabled", "Funkcja włącza/wyłącza serwomechanizm.", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetServoEnabled))
						, new HomeSimCockpitSDK.ModuleFunctionInfo("SetServoScale", "Funkcja ustawia skalowanie pozycji serwomechanizmu.", 3, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetServoScale))
						, new HomeSimCockpitSDK.ModuleFunctionInfo("SetServoPosition", "Funkcja ustawia pozycję serwomechanizmu.", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetServoPosition))
						
				};
			}
		}
		
		private object DefineCharacter(object [] arguments)
		{
			string lcd = (string)arguments[0];
			byte characterIndex = (byte)(int)arguments[1];
			byte [] character = new Byte[8];
			for (int i = 0; i < 8; i++)
			{
				character[i] = (byte)(int)arguments[i + 2];
			}
			if (_configuration != null && _configuration.LCDs != null)
			{
				for (int i = 0; i < _configuration.LCDs.Length; i++)
				{
					if (_configuration.LCDs[i].ID == lcd)
					{
						_configuration.LCDs[i].DefineCharacter(characterIndex, character);
					}
				}
			}
			return true;
		}
		
		private object SetPauseStepper(object [] arguments)
		{
			string id = (string)arguments[0];
			bool pause = (bool)arguments[1];
			StepperMotor motor = GetMotor(id);
			motor.SetPause(pause);
			return true;
		}
		
		private object ZeroStepper(object [] arguments)
		{
			string id = (string)arguments[0];
			StepperMotor motor = GetMotor(id);
			motor.Zero();
			return true;
		}
		
		private object StopStepper(object [] arguments)
		{
			string id = (string)arguments[0];
			GetMotor(id).StopMotor();
			return true;
		}
		
		private object SetStepperPosition(object [] arguments)
		{
			string id = (string)arguments[0];
			double position = Convert.ToDouble(arguments[1]);
			bool? right = null;
			if (arguments.Length > 2)
			{
				int r = Convert.ToInt32(arguments[2]);
				if (r < 0)
				{
					right = false;
				} else if (r > 0)
				{
					right = false;
				}
			}
			int? speed = 10;
			if (arguments.Length > 3)
			{
				speed = Convert.ToInt32(arguments[3]);
			}
			StepperMotor motor = GetMotor(id);
			motor.SetPosition(position, null, right, speed);
			return true;
		}
		
		private StepperMotor GetMotor(string id)
		{
			StepperMotor result = null;
			if (!_motors.TryGetValue(id, out result))
			{
				throw new Exception(string.Format("Brak silnika o identyfikatorze '{0}'.", id));
			}
			return result;
		}
		
		private Servo GetServo(string id)
		{
			Servo servo = null;
			if (!_servos.TryGetValue(id, out servo))
			{
				throw new Exception(string.Format("Brak serwomechanizmu o identyfikatorze '{0}'.", id));
			}
			return servo;
		}
		
		private object SetServoEnabled(object [] arguments)
		{
			string id = (string)arguments[0];
			bool enable = (bool)arguments[1];
			GetServo(id).SetEnable(enable);
			return true;
		}
		
		private object SetServoScale(object [] arguments)
		{
			string id = (string)arguments[0];
			int min = (int)arguments[1];
			int max = (int)arguments[2];
			GetServo(id).SetScale(min, max);
			return true;
		}
		
		private object SetServoPosition(object [] arguments)
		{
			string id = (string)arguments[0];
			int position = (int)arguments[1];
			GetServo(id).Position = position;
			return true;
		}
	}
}