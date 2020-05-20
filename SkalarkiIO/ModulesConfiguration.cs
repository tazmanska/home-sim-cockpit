using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SkalarkiIO
{
	class ModulesConfiguration
	{
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

		public static ModulesConfiguration Load()
		{
			if (__instance == null)
			{
				if (!File.Exists(ConfigurationFilePath))
				{
					throw new FileNotFoundException(ConfigurationFilePath);
				}
				ModulesConfiguration c = new ModulesConfiguration();
				XmlDocument xml = new XmlDocument();
				xml.Load(ConfigurationFilePath);

				// zmienna pomocnicza
				Device f = null;
				
				
				// wczytanie urządzeń
				List<Device> devices = new List<Device>();
				XmlNodeList nodes = xml.SelectNodes("configuration/devices/device");
				foreach (XmlNode node in nodes)
				{
					Device d = Device.Load(node);
					f = devices.Find(delegate(Device o)
					                        {
					                        	return o.Id == d.Id;
					                        });
					if (f == null)
					{
						devices.Add(Device.Load(node));
					}
					else
					{
						throw new Exception("Istnieje już urządzenie o identyfikatorze '" + d.Id + "'.");
					}
				}
				
				c.Devices = devices.ToArray();
				
				
				// wczytanie konfiguracji wyjść cyfrowych
				List<DigitalOutput> outputs = new List<DigitalOutput>();
				nodes = xml.SelectNodes("configuration/digitalOutputs/output");
				foreach (XmlNode node in nodes)
				{
					DigitalOutput output = DigitalOutput.Load(node);
					
					// znalezienie urządzenia

						f = devices.Find(delegate(Device o)
						                        {
						                        	return o.Id == output.DeviceId;
						                        });
						if (f == null)
						{
							throw new Exception("Konfiguracja nie zawiera informacji o urządzeniu z identyfikatorem '" + output.DeviceId + "'.");
						}
			
					output.Device = f;
					
					outputs.Add(output);
				}						
				c.DigitalOutputs = outputs.ToArray();
				
				// wczytanie konfiguracji zbiorów wyjść cyfrowych
				nodes = xml.SelectNodes("configuration/digitalOutputs/outputSet");
				foreach (XmlNode node in nodes)
				{
					DigitalOutputSet dos = DigitalOutputSet.Load(c.DigitalOutputs, node);
					outputs.Add(dos);
				}				
				c.DigitalOutputs = outputs.ToArray();
				
				
				// wczytanie konfiguracji wejść cyfrowych
				List<DigitalInput> inputs = new List<DigitalInput>();
				nodes = xml.SelectNodes("configuration/digitalInputs/input");
				foreach (XmlNode node in nodes)
				{
					DigitalInput input = DigitalInput.Load(node);
					
					// znalezienie urządzenia

						f = devices.Find(delegate(Device o)
						                        {
						                        	return o.Id == input.DeviceId;
						                        });
						if (f == null)
						{
							throw new Exception("Konfiguracja nie zawiera informacji o urządzeniu z identyfikatorem '" + input.DeviceId + "'.");
						}
			
					input.Device = f;
					
					inputs.Add(input);
				}		
				
				c.DigitalInputs = inputs.ToArray();
				
				// wczytanie konfiguracji enkoderów
				List<Encoder> encoders = new List<Encoder>();
				nodes = xml.SelectNodes("configuration/encoders/encoder");
				foreach (XmlNode node in nodes)
				{
				    Encoder e = Encoder.Load(c.DigitalInputs, node);
				    encoders.Add(e);
				}
				c.Encoders = encoders.ToArray();
								
				// wczytanie konfiguracji wyświetlaczy 7-led
				List<LED7DisplayOutput> ledOutputs = new List<LED7DisplayOutput>();
				nodes = xml.SelectNodes("configuration/displays7LED/display");
				foreach (XmlNode node in nodes)
				{
					LED7DisplayOutput output = LED7DisplayOutput.Load(node);
					
					// znalezienie urządzenia

						f = devices.Find(delegate(Device o)
						                        {
						                        	return o.Id == output.DeviceId;
						                        });
						if (f == null)
						{
							throw new Exception("Konfiguracja nie zawiera informacji o urządzeniu z identyfikatorem '" + output.DeviceId + "'.");
						}
			
					output.Device = f;
					
					ledOutputs.Add(output);
				}
				
				// sprawdzenie czy jakaś nazwa się nie powtarza
				foreach (DigitalOutput digo in outputs)
				{
					int index = ledOutputs.Count;
					while (index-- > 0)
					{
						if (ledOutputs[index].ID == digo.ID)
						{
							ledOutputs.RemoveAt(index);
						}
					}
				}
				
				c.LED7DisplayOutputs = ledOutputs.ToArray();
				
				// wczytanie konfiguracji zbiorów wyświetlaczy
				nodes = xml.SelectNodes("configuration/displays7LED/displaySet");
				foreach (XmlNode node in nodes)
				{
					LED7DisplayOutputSet dos = LED7DisplayOutputSet.Load(c.LED7DisplayOutputs, node);
					ledOutputs.Add(dos);
				}				
				c.LED7DisplayOutputs = ledOutputs.ToArray();
								
				// wczytanie słownika dla wyświetlaczy 7-led
				LED7DisplaysDictionary.Load(xml.SelectSingleNode("configuration/displays7LEDDictionary"));
				
				CreateOrDeletePeripherals(c);
				
				__instance = c;
			}
			return __instance;
		}
		
		public static void CreateOrDeletePeripherals(ModulesConfiguration configuration)
		{
			if (configuration.Devices != null)
			{
				List<DigitalInput> inputs = new List<DigitalInput>();
				List<DigitalOutput> outputs = new List<DigitalOutput>();
				List<LED7DisplayOutput> displays = new List<LED7DisplayOutput>();
				List<Encoder> encoders = new List<Encoder>();
				foreach (Device d in configuration.Devices)
				{
					// znalezienie wszystkich wejść tego urządzenia
					List<DigitalInput> dis = new List<DigitalInput>();
					if (configuration.DigitalInputs != null)
					{
						foreach (DigitalInput di in configuration.DigitalInputs)
						{
							if (di.DeviceId == d.Id && di.Index >= 0 && di.Index < d.Type.DigitalInputs)
							{
								DigitalInput ddd = dis.Find(delegate (DigitalInput o)
								                            {
								                            	return o.Index == di.Index;
								                            });
								if (ddd == null)
								{
									dis.Add(di);
								}
							}
						}
					}
					// dodanie brakujących
					for (int i = 0; i < d.Type.DigitalInputs; i++)
					{
						if (dis.Find(delegate(DigitalInput o)
						             {
						             	return o.Index == i;
						             }) == null)
						{
							dis.Add(new DigitalInput()
							        {
							        										DeviceId = d.Id,
									Device = d,
									Index = i,
									ID = string.Format("{0}:input_{1}", d.Id, i.ToString("000")),
									Description = "Cyfrowe wejście",
									Repeat = false,
									RepeatAfter = 200,
									RepeatInterval = 200,
							        });
						}
					}
										
					inputs.AddRange(dis);
					
					// sprawdzenie czy istnieją wszystkie wejścia dla enkoderów
					if (configuration.Encoders != null)
					{
					    List<DigitalInput> reserved = new List<DigitalInput>();
					    foreach (Encoder e in configuration.Encoders)
					    {
					        DigitalInput linput = inputs.Find(delegate(DigitalInput o)
					                        {
					                            return o.ID == e.LeftInput.ID;
					                                         });
					        if (linput == null)
					        {
					            throw new Exception("Brak wejścia '" + e.LeftInput.ToString() + "' do obsługi enkodera.");
					        }
					        
					        if (reserved.Contains(linput))
					        {
					            continue;
					        }
					        
					        DigitalInput rinput = inputs.Find(delegate(DigitalInput o)
					                        {
					                            return o.ID == e.RightInput.ID;
					                                         });
					        if (rinput == null)
					        {
					            throw new Exception("Brak wejścia '" + e.RightInput.ToString() + "' do obsługi enkodera.");
					        }
					        
					        if (reserved.Contains(rinput))
					        {
					            continue;
					        }
					        
					        reserved.Add(linput);
					        reserved.Add(rinput);
					        e.LeftInput = linput;
					        e.RightInput = rinput;
					        encoders.Add(e);
					    }
					}
					
					// znalezienie wszystkich wyjść cyfrowych tego urządzenia
					List<DigitalOutput> dos = new List<DigitalOutput>();
					if (configuration.DigitalOutputs != null)
					{
						foreach (DigitalOutput di in configuration.DigitalOutputs)
						{
							if (di.DeviceId == d.Id && di.Index >= 0 && di.Index < d.Type.DigitalOutputs)
							{
								DigitalOutput ddd = dos.Find(delegate (DigitalOutput o)
								                            {
								                            	return o.Index == di.Index;
								                            });
								if (ddd == null)
								{
									dos.Add(di);
								}
							}
						}
					}
					// dodanie brakujących
					for (int i = 0; i < d.Type.DigitalOutputs; i++)
					{
						if (dos.Find(delegate(DigitalOutput o)
						             {
						             	return o.Index == i;
						             }) == null)
						{
							dos.Add(new DigitalOutput()
							        {
							        DeviceId = d.Id,
									Device = d,
									Index = i,
									ID = string.Format("{0}:output_{1}", d.Id, i.ToString("000")),
									Description = "Cyfrowe wyjście",
							        });
						}
					}
					
					// sprawdzenie zbiorów wyjść
					if (configuration.DigitalOutputs != null)
					{
						List<DigitalOutputSet> dosets = new List<DigitalOutputSet>();
						foreach (DigitalOutput di in configuration.DigitalOutputs)
						{
							if (di is DigitalOutputSet)
							{
								DigitalOutputSet dose = (DigitalOutputSet)di;
								if (dose.DigitalOutputs != null && dose.DigitalOutputs.Length > 0)
								{
									List<DigitalOutput> doses = new List<DigitalOutput>(dose.DigitalOutputs);
									int index = doses.Count;
									while (index-- > 0)
									{
										// znalezienie wyjścia o danym id
										if (dos.Find(delegate (DigitalOutput digo)
										             {
										             	return digo.ID == doses[index].ID;
										             }) == null)
										{
											doses.RemoveAt(index);
										}
									}
									
									if (doses.Count > 0)
									{
										dose.DigitalOutputs = doses.ToArray();
										dosets.Add(dose);
									}
								}
							}
						}
						dos.AddRange(dosets.ToArray());
					}
					
					outputs.AddRange(dos);
					
					// znalezienie wszystkich wyświetlaczy 7led
					List<LED7DisplayOutput> led7 = new List<LED7DisplayOutput>();
					if (configuration.LED7DisplayOutputs != null)
					{
						foreach (LED7DisplayOutput di in configuration.LED7DisplayOutputs)
						{
							if (di.DeviceId == d.Id && di.Index >= 0 && di.Index < d.Type.Displays7LED)
							{
								LED7DisplayOutput ddd = led7.Find(delegate (LED7DisplayOutput o)
								                            {
								                            	return o.Index == di.Index;
								                            });
								if (ddd == null)
								{
									di.Device = d;
									led7.Add(di);
								}
							}
						}
					}
					// dodanie brakujących
					for (int i = 0; i < d.Type.Displays7LED; i++)
					{
						if (led7.Find(delegate(LED7DisplayOutput o)
						             {
						             	return o.Index == i;
						             }) == null)
						{
							led7.Add(new LED7DisplayOutput()
							        {
							        DeviceId = d.Id,
									Device = d,
									Index = i,
									ID = string.Format("{0}:7ledDisplay_{1}", d.Id, i.ToString("000")),
									Description = "Wyświetlacz 7-LED",
							        });
						}
					}
					
					// sprawdzenie zbiorów wyjść
					if (configuration.LED7DisplayOutputs != null)
					{
						List<LED7DisplayOutputSet> dosets = new List<LED7DisplayOutputSet>();
						foreach (LED7DisplayOutput di in configuration.LED7DisplayOutputs)
						{
							if (di is LED7DisplayOutputSet)
							{
								LED7DisplayOutputSet dose = (LED7DisplayOutputSet)di;
								if (dose.Displays != null && dose.Displays.Length > 0)
								{
									List<Display> doses = new List<Display>(dose.Displays);
									int index = doses.Count;
									while (index-- > 0)
									{
										// znalezienie wyjścia o danym id
										if (led7.Find(delegate (LED7DisplayOutput digo)
										             {
										             	return digo.ID == doses[index].LED7Display.ID;
										             }) == null)
										{
											doses.RemoveAt(index);
										}
									}
									
									if (doses.Count > 0)
									{
										dose.Displays = doses.ToArray();
										dosets.Add(dose);
									}
								}
							}
						}
						led7.AddRange(dosets.ToArray());
					}
															
					displays.AddRange(led7);
				}
				
				configuration.DigitalInputs = inputs.ToArray();
				configuration.Encoders = encoders.ToArray();
				configuration.DigitalOutputs = outputs.ToArray();
				configuration.LED7DisplayOutputs = displays.ToArray();
			}
		}

		private static ModulesConfiguration __instance = null;

		private ModulesConfiguration()
		{
		}

		public static ModulesConfiguration Reload()
		{
			__instance = null;
			return Load();
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
				
				// zapisanie urządzeń
				xml.WriteStartElement("devices");
				if (Devices != null)
				{
					foreach (Device d in Devices)
					{
						d.Save(xml);
					}
				}
				xml.WriteEndElement();

				// zapisanie wejść cyfrowych
				xml.WriteStartElement("digitalInputs");
				if (DigitalInputs != null)
				{
					foreach (DigitalInput di in DigitalInputs)
					{
						di.Save(xml);
					}
				}
				xml.WriteEndElement();
				
				// zapisanie encoderów
				xml.WriteStartElement("encoders");
				if (Encoders != null)
				{
				    foreach (Encoder e in Encoders)
				    {
				        e.Save(xml);
				    }
				}
				xml.WriteEndElement();
				
				// zapisanie wyjść cyfrowych
				xml.WriteStartElement("digitalOutputs");
				if (DigitalOutputs != null)
				{
					foreach (DigitalOutput di in DigitalOutputs)
					{
						di.Save(xml);
					}
				}
				xml.WriteEndElement();
				
				// zapisanie wyświetlaczy
				xml.WriteStartElement("displays7LED");
				if (LED7DisplayOutputs != null)
				{
					foreach (LED7DisplayOutput d in LED7DisplayOutputs)
					{
						d.Save(xml);
					}
				}
				xml.WriteEndElement();
				
				// zapisanie słownika wyświetlaczy 7led
				LED7DisplaysDictionary.Instance.Save(xml);
				
				xml.WriteEndElement();
				xml.WriteEndDocument();
			}
		}
		
		public Device[] Devices
		{
			get;
			internal set;
		}
		
		public DigitalInput[] DigitalInputs
		{
			get;
			internal set;
		}
		
		public Encoder[] Encoders
		{
		    get;
		    internal set;
		}
		
		public DigitalOutput[] DigitalOutputs
		{
			get;
			internal set;
		}
		
		public LED7DisplayOutput[] LED7DisplayOutputs
		{
			get;
			internal set;
		}
	}
}
