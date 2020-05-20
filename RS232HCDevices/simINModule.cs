/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-11
 * Godzina: 22:41
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RS232HCDevices
{
	/// <summary>
	/// Description of simINModule.
	/// </summary>
	public class simINModule : HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration
	{
		public simINModule()
		{
		}
		
		public HomeSimCockpitSDK.IVariable[] InputVariables
		{
			get
			{
				List<Key> keys = new List<Key>(LoadConfiguration().Keys);
				foreach (Key key in LoadConfiguration().Keys)
				{
					if (key.HasFastDetection)
					{
						keys.Add(new Key()
						         {
						         	ID = key.IDFast,
						         	Description = "Szybkie kręcenie enkoderem."
						         });
					}
				}
				return keys.ToArray();
			}
		}
		
		public string Name
		{
			get { return "simIN"; }
		}

		public string Description
		{
			get { return "Obsługa urządzeń wejściowych podpiętych do portu RS232. Więcej informacji: www.simproject.info"; }
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

		public void Load(HomeSimCockpitSDK.ILog log)
		{
			_log = log;
		}
		
		private XMLConfiguration LoadConfiguration()
		{
			return XMLConfiguration.Load();
		}
		
		public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
		{
			Key key = Array.Find<Key>(LoadConfiguration().Keys, delegate(Key o)
			                          {
			                          	return o.ID == variableID;
			                          });
			if (type == HomeSimCockpitSDK.VariableType.Bool && key != null)
			{
				key.VariableChanged += listenerMethod;
			}
			else
			{
				key = Array.Find<Key>(LoadConfiguration().Keys, delegate(Key o)
				                      {
				                      	return o.HasFastDetection && o.IDFast == variableID;
				                      });
				if (type == HomeSimCockpitSDK.VariableType.Bool && key != null)
				{
					key.VariableChangedFast += listenerMethod;
				}
				else
				{
					throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
				}
			}
		}
		
		public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
		{
			Key key = Array.Find<Key>(LoadConfiguration().Keys, delegate(Key o)
			                          {
			                          	return o.ID == variableID;
			                          });
			if (key != null)
			{
				key.VariableChanged -= listenerMethod;
			}
			else
			{
				key = Array.Find<Key>(LoadConfiguration().Keys, delegate(Key o)
				                      {
				                      	return o.HasFastDetection && o.IDFast == variableID;
				                      });
				if (key != null)
				{
					key.VariableChangedFast -= listenerMethod;
				}
				else
				{
					throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}'.", variableID));
				}
			}
		}
		
		public void Unload()
		{
			Stop(HomeSimCockpitSDK.StartStopType.Input);
		}
		
		private volatile bool _working = false;
		
		private List<simINDevices> _devices = new List<simINDevices>();
		private Dictionary<simINDevice, List<Key>> _deviceKeys = new Dictionary<simINDevice, List<Key>>();
		
		public void Start(HomeSimCockpitSDK.StartStopType startStopType)
		{
			if (_working)
			{
				return;
			}
			
			Stop(startStopType);
			
			// pobranie listy interfejsów RS do nasłuchu
			_deviceKeys.Clear();
			Dictionary<RS232Configuration, List<simINDevice>> interf = new Dictionary<RS232Configuration, List<simINDevice>>();
			foreach (Key key in LoadConfiguration().Keys)
			{
				if (!key.IsSubscribed)
				{
					continue;
				}
				RS232Configuration interfac = key.KeysDevice.Interface;
				if (!interf.ContainsKey(interfac))
				{
					interf.Add(interfac, new List<simINDevice>());
				}
				if (!interf[interfac].Contains(key.KeysDevice))
				{
					interf[interfac].Add(key.KeysDevice);
				}
				
				key.Module = this;
				key.Reset();
				
				if (!_deviceKeys.ContainsKey(key.KeysDevice))
				{
					_deviceKeys.Add(key.KeysDevice, new List<Key>());
				}
				_deviceKeys[key.KeysDevice].Add(key);
			}
			
			if (_deviceKeys.Count == 0)
			{
				return;
			}
			
			_devices.Clear();
			
			foreach (KeyValuePair<RS232Configuration, List<simINDevice>> kvp in interf)
			{
				if (kvp.Value.Count > 0)
				{
					_devices.Add(new simINDevices(LoadConfiguration(), kvp.Key, kvp.Value.ToArray()));
				}
			}
			
			foreach (KeysDevice keysDevice in LoadConfiguration().KeysDevices)
			{
				List<Encoder> encoders = new List<Encoder>();
				if (LoadConfiguration().Encoders != null)
				{
					foreach (Encoder enc in LoadConfiguration().Encoders)
					{
						if (enc.KeysDevice == keysDevice)
						{
							encoders.Add(enc);
						}
					}
				}
				keysDevice.Encoders = encoders.ToArray();
			}
			
			_working = true;
			
			foreach (simINDevices simin in _devices)
			{
				simin.ReceivedReportEvent += new ReceivedReportDelegate(simin_ReceivedReportEvent);
				simin.Start();
			}
		}
		
		void simin_ReceivedReportEvent(simINDevice device, byte reportType, byte dataLength, byte[] data)
		{
			switch (reportType)
			{
				case 1: // GET_KEYS
					if (dataLength == 2)
					{
						int index = data[0] * 8;
						if (_deviceKeys.ContainsKey(device))
						{
							List<Key> keys = _deviceKeys[device];
							for (int i = 0; i < keys.Count; i++)
							{
								keys[i].CheckState(index, data[1]);
							}
						}
						else
						{
							throw new Exception("Nieprawidłowa ilość danych w pakiecie typu GET_KEYS.");
						}
					}
					break;
					
				case 2: // GET_KEYS2
					if (dataLength == 3)
					{
						int index = data[1] * 8;
						if (_deviceKeys.ContainsKey(device))
						{
							List<Key> keys = _deviceKeys[device];
							for (int i = 0; i < keys.Count; i++)
							{
								keys[i].CheckState(index, data[2]);
							}
						}
					}
					else
					{
						throw new Exception("Nieprawidłowa ilość danych w pakiecie typu GET_KEYS2.");
					}
					break;
			}
		}
		
		public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
		{
			if (!_working)
			{
				return;
			}
			
			foreach (simINDevices simin in _devices)
			{
				simin.Stop(true);
			}
			_devices.Clear();
			_working = false;
		}
		
		public bool Configuration(System.Windows.Forms.IWin32Window parent)
		{
			MessageBox.Show(parent, "Konfiguracja modułu simIN zawarta jest w konfiguracji modułu simOUT. Wywołaj konfigurację modułu simOUT aby skonfigurować ten moduł.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return false;
		}
	}
}
