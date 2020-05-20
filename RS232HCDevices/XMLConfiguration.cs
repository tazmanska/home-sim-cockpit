using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using HomeSimCockpitX.LCD;
using RS232HCDevices.Servos;
using RS232HCDevices.Steppers;

namespace RS232HCDevices
{
	class XMLConfiguration
	{
		private class LCDSet : ILCDsCollection
		{
			public LCDSet(List<RS232LCD> lcds)
			{
				_lcds = lcds;
			}

			private List<RS232LCD> _lcds = null;

			#region ILCDsCollection Members

			public LCD GetLCD(string id)
			{
				LCD lcd = _lcds.Find(delegate(RS232LCD o)
				                     {
				                     	return o.ID == id;
				                     });
				if (lcd != null)
				{
					return lcd;
				}
				throw new ApplicationException("Nie znalezion wyświetlacza o identyfikatorze '" + id + "'.");
			}

			#endregion
		}
		
		private static string __configPath = null;

		public static string ConfigurationFilePath
		{
			get
			{
				if (__configPath == null)
				{
					__configPath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
				}
				return __configPath;
			}
		}
		
		private static XMLConfiguration __instance = null;
		
		public static XMLConfiguration Reload()
		{
			__instance = null;
			return Load();
		}

		public static XMLConfiguration Load()
		{
			if (__instance != null)
			{
				return __instance;
			}
			
			if (!File.Exists(ConfigurationFilePath))
			{
				throw new FileNotFoundException(ConfigurationFilePath);
			}
			XMLConfiguration c = new XMLConfiguration();
			XmlDocument xml = new XmlDocument();
			xml.Load(ConfigurationFilePath);
			
			// wczytanie interfejsów
			List<RS232Configuration> interfaces = new List<RS232Configuration>();
			XmlNodeList nodes = xml.SelectNodes("/configuration/interfaces/interface");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					RS232Configuration interf = RS232Configuration.Load(node);
					interfaces.Add(interf);
				}
			}
			c.Interfaces = interfaces.ToArray();
			
			// wczytanie urządzeń z LCD
			List<LCDDevice> lcdDevices = new List<LCDDevice>();
			nodes = xml.SelectNodes("/configuration/lcdDevices/lcdDevice");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LCDDevice lcdDevice = LCDDevice.Load(node, interfaces);
					if (lcdDevice != null)
					{
						lcdDevices.Add(lcdDevice);
					}
				}
			}
			c.LCDDevices = lcdDevices.ToArray();
			
			// wczytanie LCD
			List<RS232LCD> lcds = new List<RS232LCD>();
			nodes = xml.SelectNodes("/configuration/lcds/lcd");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					RS232LCD lcd = RS232LCD.Load(lcdDevices, node);
					if (lcd != null)
					{
						lcds.Add(lcd);
					}
				}
			}
			c.LCDs = lcds.ToArray();
			
			// wczytanie obszarów LCD
			List<LCDArea> areas = new List<LCDArea>();
			nodes = xml.SelectNodes("/configuration/lcdAreas/area");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LCDArea area = new LCDArea(node, new LCDSet(lcds));
					areas.Add(area);
				}
			}
			c.Areas = areas.ToArray();
			
			// wczytanie urządzeń z LED
			List<LEDDevice> ledDevices = new List<LEDDevice>();
			nodes = xml.SelectNodes("/configuration/ledDevices/ledDevice");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LEDDevice ledDevice = LEDDevice.Load(node, interfaces);
					if (ledDevice != null)
					{
						ledDevices.Add(ledDevice);
					}
				}
			}
			c.LEDDevices = ledDevices.ToArray();
			
			// wczytanie LED
			List<LED> leds = new List<LED>();
			nodes = xml.SelectNodes("/configuration/leds/led");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LED led = LED.Load(node, ledDevices);
					if (led != null)
					{
						leds.Add(led);
					}
				}
			}
			c.LEDs = leds.ToArray();
			
			// wczytanie obszarów LED
			List<LEDGroup> ledGroups = new List<LEDGroup>();
			nodes = xml.SelectNodes("/configuration/leds/leds");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LEDGroup ledGroup = LEDGroup.Load(node, leds);
					if (ledGroup != null)
					{
						ledGroups.Add(ledGroup);
					}
				}
			}
			c.LEDGroups = ledGroups.ToArray();
			
			// wczytanie urządzeń z 7-LED
			List<LEDDisplayDevice> ledDisplayDevices = new List<LEDDisplayDevice>();
			nodes = xml.SelectNodes("/configuration/ledDisplayDevices/ledDisplayDevice");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LEDDisplayDevice ledDisplayDevice = LEDDisplayDevice.Load(node, interfaces);
					if (ledDisplayDevice != null)
					{
						ledDisplayDevices.Add(ledDisplayDevice);
					}
				}
			}
			c.LEDDisplayDevices = ledDisplayDevices.ToArray();
			
			// wczytanie 7-LED
			List<LEDDisplay> ledDisplays = new List<LEDDisplay>();
			nodes = xml.SelectNodes("/configuration/ledDisplays/ledDisplay");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LEDDisplay ledDisplay = LEDDisplay.Load(node, ledDisplayDevices);
					if (ledDisplay != null)
					{
						ledDisplays.Add(ledDisplay);
					}
				}
			}
			c.LEDDisplays = ledDisplays.ToArray();
			
			// wczytanie obszarów 7-LED
			List<LEDDisplayGroup> ledDisplayGroups = new List<LEDDisplayGroup>();
			nodes = xml.SelectNodes("/configuration/ledDisplays/ledDisplays");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					LEDDisplayGroup ledDisplayGroup = LEDDisplayGroup.Load(node, ledDisplays);
					if (ledDisplayGroup != null)
					{
						ledDisplayGroups.Add(ledDisplayGroup);
					}
				}
			}
			c.LEDDisplayGroups = ledDisplayGroups.ToArray();
			
			// wczytanie słownika dla wyświetlaczy 7-segmentowych
			XmlNode dictionaryNode = xml.SelectSingleNode("/configuration/ledDisplaysDictionary");
			c.LEDDisplaysDictionary = LEDDisplaysDictionary.Load(dictionaryNode);
			
			// wczytanie urządzeń Keys
			List<KeysDevice> keysDevices = new List<KeysDevice>();
			nodes = xml.SelectNodes("/configuration/keysDevices/keysDevice");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					KeysDevice keysDevice = KeysDevice.Load(node, interfaces);
					if (keysDevice != null)
					{
						keysDevices.Add(keysDevice);
					}
				}
			}
			c.KeysDevices = keysDevices.ToArray();
			
			// wczytanie keys
			List<Key> keys = new List<Key>();
			nodes = xml.SelectNodes("/configuration/keys/key");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					Key key = Key.Load(node, keysDevices);
					if (key != null)
					{
						keys.Add(key);
					}
				}
			}
			c.Keys = keys.ToArray();
			
			// wczytanie encoders
			List<Encoder> encoders = new List<Encoder>();
			nodes = xml.SelectNodes("/configuration/encoders/encoder");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					Encoder encoder = Encoder.Load(node, keysDevices);
					if (encoder != null)
					{
						encoders.Add(encoder);
						
						// przypisanie enkoderów do wejść (wykrywanie szybkiego kręcenia)
						foreach (Key key in c.Keys)
						{
							if (key.KeysDevice == encoder.KeysDevice)
							{
								if (key.Index == encoder.LeftIndex || key.Index == encoder.RightIndex)
								{
									key.Encoder = encoder;
								}
							}
						}
					}
				}
			}
			c.Encoders = encoders.ToArray();
			
			// wczytanie stepper motors
			List<StepperDevice> stepperDevices = new List<StepperDevice>();
			nodes = xml.SelectNodes("/configuration/stepperDevices/stepperDevice");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					StepperDevice stepperDevice = StepperDevice.Load(node, interfaces);
					if (stepperDevice != null)
					{
						stepperDevices.Add(stepperDevice);
					}
				}
			}
			c.StepperDevices = stepperDevices.ToArray();
			
			// wczytanie servo devices
			List<ServoDevice> servoDevices = new List<ServoDevice>();
			nodes = xml.SelectNodes("/configuration/servoDevices/servoDevice");
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					ServoDevice servoDevice = ServoDevice.Load(node, interfaces);
					if (servoDevice != null)
					{
						servoDevices.Add(servoDevice);
					}
				}
			}
			c.ServoDevices = servoDevices.ToArray();
			
			
			__instance = c;
			
			return __instance;
		}
		
		public void Save()
		{
			Save(ConfigurationFilePath);
		}

		public void Save(string fileName)
		{
			using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
			{
				xml.Formatting = System.Xml.Formatting.Indented;
				xml.WriteStartDocument(true);
				xml.WriteStartElement("configuration");

				xml.WriteStartElement("interfaces");
				foreach (RS232Configuration interf in Interfaces)
				{
					interf.Save(xml);
				}
				xml.WriteEndElement();

				xml.WriteStartElement("lcdDevices");
				foreach (LCDDevice lcdDevice in LCDDevices)
				{
					lcdDevice.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("lcds");
				foreach (LCD lcd in LCDs)
				{
					lcd.SaveToXml(xml);
				}
				xml.WriteEndElement();

				xml.WriteStartElement("lcdAreas");
				foreach (LCDArea area in Areas)
				{
					area.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("ledDevices");
				foreach (LEDDevice ledDevice in LEDDevices)
				{
					ledDevice.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("leds");
				foreach (LED led in LEDs)
				{
					led.SaveToXml(xml);
				}
				foreach (LEDGroup ledGroup in LEDGroups)
				{
					ledGroup.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("ledDisplayDevices");
				foreach (LEDDisplayDevice ledDisplayDevice in LEDDisplayDevices)
				{
					ledDisplayDevice.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("ledDisplays");
				foreach (LEDDisplay ledDisplay in LEDDisplays)
				{
					ledDisplay.SaveToXml(xml);
				}
				foreach (LEDDisplayGroup ledDisplayGroup in LEDDisplayGroups)
				{
					ledDisplayGroup.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				LEDDisplaysDictionary.Save(xml);
				
				xml.WriteStartElement("keysDevices");
				foreach (KeysDevice keyDevice in KeysDevices)
				{
					keyDevice.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("keys");
				foreach (Key key in Keys)
				{
					key.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("encoders");
				foreach (Encoder encoder in Encoders)
				{
					encoder.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("stepperDevices");
				foreach (StepperDevice stepperDevice in StepperDevices)
				{
					stepperDevice.SaveToXml(xml);
				}
				xml.WriteEndElement();
				
				xml.WriteStartElement("servoDevices");
				foreach (ServoDevice servoDevice in ServoDevices)
				{
					servoDevice.SaveToXml(xml);
				}
				xml.WriteEndElement();

				xml.WriteEndElement();
				xml.WriteEndDocument();
			}
		}

		internal XMLConfiguration()
		{

		}
		
		public RS232Configuration[] Interfaces
		{
			get;
			internal set;
		}
		
		public LCDDevice[] LCDDevices
		{
			get;
			internal set;
		}

		public RS232LCD[] LCDs
		{
			get;
			internal set;
		}

		public LCDArea[] Areas
		{
			get;
			internal set;
		}
		
		public LEDDevice[] LEDDevices
		{
			get;
			internal set;
		}
		
		public LED[] LEDs
		{
			get;
			internal set;
		}
		
		public LEDGroup[] LEDGroups
		{
			get;
			set;
		}
		
		public LEDDisplayDevice[] LEDDisplayDevices
		{
			get;
			set;
		}
		
		public LEDDisplay[] LEDDisplays
		{
			get;
			set;
		}
		
		public LEDDisplayGroup[] LEDDisplayGroups
		{
			get;
			set;
		}
		
		public LEDDisplaysDictionary LEDDisplaysDictionary
		{
			get;
			set;
		}
		
		public KeysDevice[] KeysDevices
		{
			get;
			set;
		}
		
		public Key[] Keys
		{
			get;
			set;
		}
		
		public Encoder[] Encoders
		{
			get;
			set;
		}
		
		public StepperDevice[] StepperDevices
		{
			get;
			set;
		}
		
		public ServoDevice[] ServoDevices
		{
			get;
			set;
		}
		
		public void RemoveDevice(LEDDevice device)
		{
			// usunięcie diod z grup diod
			List<LEDGroup> ledGroups = new List<LEDGroup>();
			for (int i = 0; i < LEDGroups.Length; i++)
			{
				List<LED> leds1 = new List<LED>(LEDGroups[i].LEDs);
				leds1.RemoveAll(delegate(LED o)
				                {
				                	return o.LEDDevice == device;
				                });
				if (leds1.Count > 0)
				{
					LEDGroups[i].LEDs = leds1.ToArray();
					ledGroups.Add(LEDGroups[i]);
				}
			}
			LEDGroups = ledGroups.ToArray();
			
			// usunięcie diod
			List<LED> leds = new List<LED>(LEDs);
			leds.RemoveAll(delegate(LED o)
			               {
			               	return o.LEDDevice == device;
			               });
			LEDs = leds.ToArray();
			
			// usunięcie urządzenia
			List<LEDDevice> ledDevices = new List<LEDDevice>(LEDDevices);
			ledDevices.Remove(device);
			LEDDevices = ledDevices.ToArray();
		}
		
		public void RemoveDevice(LEDDisplayDevice device)
		{
			// usunięcie wyświetlaczy z grup wyświetlaczy
			List<LEDDisplayGroup> ldg = new List<LEDDisplayGroup>();
			for (int i = 0; i < LEDDisplayGroups.Length; i++)
			{
				List<LEDDisplayInGroup> ll = new List<LEDDisplayInGroup>(LEDDisplayGroups[i].LEDDisplaysInGroup);
				ll.RemoveAll(delegate(LEDDisplayInGroup o)
				             {
				             	return o.LEDDisplay.LEDDisplayDevice == device;
				             });
				if (ll.Count > 0)
				{
					LEDDisplayGroups[i].LEDDisplaysInGroup = ll.ToArray();
					ldg.Add(LEDDisplayGroups[i]);
				}
			}
			LEDDisplayGroups = ldg.ToArray();
			
			// usunięcie wyświetlaczy
			List<LEDDisplay> leds = new List<LEDDisplay>(LEDDisplays);
			leds.RemoveAll(delegate(LEDDisplay o)
			               {
			               	return o.LEDDisplayDevice == device;
			               });
			LEDDisplays = leds.ToArray();
			
			// usunięcie urządzenia
			List<LEDDisplayDevice> devs = new List<LEDDisplayDevice>(LEDDisplayDevices);
			devs.Remove(device);
			LEDDisplayDevices = devs.ToArray();
		}
		
		public void RemoveDevice(LCDDevice device)
		{
			// usunięcie znaków z obszarów znakowych
			List<LCDArea> ldg = new List<LCDArea>();
			for (int i = 0; i < Areas.Length; i++)
			{
				List<LCDCharacter> ll = new List<LCDCharacter>(Areas[i].Characters);
				ll.RemoveAll(delegate(LCDCharacter o)
				             {
				             	return ((RS232LCD)o.LCD).LCDDevice == device;
				             });
				if (ll.Count > 0)
				{
					Areas[i].Set(ll.ToArray());
					ldg.Add(Areas[i]);
				}
			}
			Areas = ldg.ToArray();
			
			// usunięcie wyświetlaczy
			List<RS232LCD> leds = new List<RS232LCD>(LCDs);
			leds.RemoveAll(delegate(RS232LCD o)
			               {
			               	return o.LCDDevice == device;
			               });
			LCDs = leds.ToArray();
			
			// usunięcie urządzenia
			List<LCDDevice> devs = new List<LCDDevice>(LCDDevices);
			devs.Remove(device);
			LCDDevices = devs.ToArray();
		}
		
		public void RemoveDevice(StepperDevice device)
		{
			// usunięcie urządzenia
			List<StepperDevice> devs = new List<StepperDevice>(StepperDevices);
			devs.Remove(device);
			StepperDevices = devs.ToArray();
		}
		
		public void RemoveDevice(ServoDevice device)
		{
			// usunięcie urządzenia
			List<ServoDevice> devs = new List<ServoDevice>(ServoDevices);
			devs.Remove(device);
			ServoDevices = devs.ToArray();
		}
		
		public void RemoveDevice(KeysDevice device)
		{
			// usunięcie keys
			List<Key> keys = new List<Key>(Keys);
			keys.RemoveAll(delegate(Key o)
			               {
			               	return o.KeysDevice == device;
			               });
			Keys = keys.ToArray();
			
			// usunięcie enkoderów
			List<Encoder> encoders = new List<Encoder>(Encoders);
			encoders.RemoveAll(delegate(Encoder o)
			                   {
			                   	return o.KeysDevice == device;
			                   });
			Encoders = encoders.ToArray();
			
			// usunięcie urządzenia
			List<KeysDevice> devs = new List<KeysDevice>(KeysDevices);
			devs.Remove(device);
			KeysDevices = devs.ToArray();
		}
		
		/// <summary>
		/// Metoda sprawdza czy na wskazanym intefejsie jest urządzenie o wskazanym id
		/// </summary>
		/// <param name="interf"></param>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		public bool ExistsDevice(RS232Configuration interf, byte deviceId)
		{
			List<Device> devices = new List<Device>();
			devices.AddRange(LEDDevices);
			devices.AddRange(LEDDisplayDevices);
			devices.AddRange(LCDDevices);
			devices.AddRange(KeysDevices);
			devices.AddRange(StepperDevices);
			devices.AddRange(ServoDevices);
			
			if (devices.Find(delegate(Device o)
			                 {
			                 	return o.Interface == interf && o.DeviceId == deviceId;
			                 }) != null)
			{
				return true;
			}
			
			return false;
		}
		
		public IOutputVariable[] GetAddinsVariable()
		{
			List<IOutputVariable> result = new List<IOutputVariable>();
			
			// dodanie zmiennych do regulacji jasności diod
			foreach (LEDDevice ledDevice in LEDDevices)
			{
				BrightnessVariable bv = new BrightnessVariable(ledDevice);
				result.Add(bv);
			}
			
			// dodanie zmiennych do regulacji jasności wyświetlaczy 7-SEG
			foreach (LEDDisplayDevice ledDisplayDevice in LEDDisplayDevices)
			{
				BrightnessVariable bv = new BrightnessVariable(ledDisplayDevice);
				result.Add(bv);
			}
			return result.ToArray();
		}
	}
}