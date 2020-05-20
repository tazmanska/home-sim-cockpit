/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-17
 * Godzina: 21:28
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;
using System.Runtime.InteropServices;

namespace RS232HCDevices
{
	/// <summary>
	/// Description of Key.
	/// </summary>
	internal class Key : IInputVariable, IComparable<Key>
	{
		public Key()
		{
		}
		
		public static Key Load(XmlNode xml, List<KeysDevice> keysDevices)
		{
			Key result = new Key();
			string keysDeviceId = xml.Attributes["keysDevice"].Value;
			KeysDevice keysDevice = keysDevices.Find(delegate (KeysDevice o)
			                                         {
			                                         	return o.Id == keysDeviceId;
			                                         });
			if (keysDevice == null)
			{
				return null;
			}
			
			result.KeysDevice = keysDevice;
			result.ID = xml.Attributes["id"].Value;
			result.Description = xml.Attributes["description"].Value;
			result.Index = byte.Parse(xml.Attributes["index"].Value);
			return result;
		}
		
		public void SaveToXml(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("key");
			xmlWriter.WriteAttributeString("keysDevice", KeysDevice.Id);
			xmlWriter.WriteAttributeString("id", ID);
			xmlWriter.WriteAttributeString("description", Description);
			xmlWriter.WriteAttributeString("index", Index.ToString());
			xmlWriter.WriteEndElement();
		}
		
		public KeysDevice KeysDevice
		{
			get;
			set;
		}
		
		public byte Index
		{
			get;
			internal set;
		}
		
		public string ID
		{
			get;
			set;
		}
		
		public Encoder Encoder
		{
			get;
			set;
		}
		
		public bool HasFastDetection
		{
			get { return Encoder != null && Encoder.DetectFast; }
		}
		
		public HomeSimCockpitSDK.VariableType Type
		{
			get { return HomeSimCockpitSDK.VariableType.Bool; }
		}
		
		public string Description
		{
			get;
			set;
		}
		
		public int CompareTo(Key other)
		{
			int result = ID.CompareTo(other.ID);
			if (result == 0)
			{
				result = Description.CompareTo(other.Description);
			}
			return result;
		}
		
		public bool State
		{
			get;
			private set;
		}
		
		public string IDFast
		{
			get { return string.Format("{0}_fast", ID); }
		}
		
		public void Reset()
		{
			_testIndex = Index;
			if (!KeysDevice.HardwareIndexes)
			{
				if (_testIndex < 8)
				{
					_testIndex += 32;
				}
				else
				{
					_testIndex -= 8;
				}
			}
			State = false;
			_idFast = IDFast;
			_lastTrueState = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
			_fastDetect = HasFastDetection;
			_lastFast = false;
			_fastTimer = 0;
			
			// obliczenie ile to taktów zegara wysokiej częstotliwości
			if (Encoder != null)
			{
				__timeForFastEncoder = (__frequency / 1000000) * Encoder.FastTime;
			}
		}
		
		private string _idFast = "";
		private DateTime _lastTrueState;
		private bool _fastDetect = false;
		private TimeSpan _fastPeriod = new TimeSpan(0, 0, 0, 0, 20);
		private bool _lastFast = false;
		private long _fastTimer = 0;
		private long _timer = 0;
		private int _testIndex = 0;
		
		public bool CheckState(int index, int states)
		{
			if (_testIndex >= index && _testIndex < (index + 8))
			{
				int ind = _testIndex % 8;
				bool newState = (states & (1 << ind)) > 0;
				if (newState != State)
				{
					if (_lastFast)
					{
						OnChangeValueFast(newState);
						_lastFast = false;
					}
					else
					{
						if (newState)
						{
							if (_fastDetect)
							{
								QueryPerformanceCounter(out _timer);
								//(timer - _fastTimer) <= __timeForFastEncoder;
								//System.Diagnostics.Debug.WriteLine(DateTime.Now - _lastTrueState);
								//System.Diagnostics.Debug.WriteLine(_timer - _fastTimer);
								//if ((DateTime.Now - _lastTrueState) < _fastPeriod)
								if ((_timer - _fastTimer) <= __timeForFastEncoder)
								{
									OnChangeValueFast(newState);
									_lastFast = true;
									
									System.Diagnostics.Debug.WriteLine("Fast");
								}
								else
								{
									OnChangeValue(newState);
								}
								_fastTimer = _timer;
							}
							else
							{
								OnChangeValue(newState);
							}
							_lastTrueState = DateTime.Now;
							_fastTimer = _timer;
						}
						else
						{
							OnChangeValue(newState);
						}
					}
				}
				State = newState;
				if (State)
				{
					_lastTrueState = DateTime.Now;
				}
			}
			return State;
		}
		
		public HomeSimCockpitSDK.IInput Module
		{
			get;
			set;
		}

		protected virtual void OnChangeValue(object value)
		{
			HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
			if (variableChanged != null)
			{
				variableChanged(Module, ID, value);
			}
		}

		public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

		public bool IsSubscribed
		{
			get { return VariableChanged != null; }
		}
		
		public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChangedFast;

		public bool IsSubscribedFast
		{
			get { return VariableChangedFast != null; }
		}
		
		protected virtual void OnChangeValueFast(object value)
		{
			HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChangedFast;
			if (variableChanged != null)
			{
				variableChanged(Module, _idFast, value);
			}
		}
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool QueryPerformanceFrequency(out long lpFrequency);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
		
		private static long __frequency = 0;

		private static long __timeForFastEncoder = 0;
		
		static Key()
		{
			QueryPerformanceFrequency(out __frequency);
		}
	}
}
