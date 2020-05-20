using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Xml;

namespace RS232HCDevices
{
	class RS232Configuration : IDevice, IComparable<RS232Configuration>
	{
		public static RS232Configuration Load(XmlNode node)
		{
			RS232Configuration result = new RS232Configuration();
			result.Id = node.Attributes["id"].Value;
			result._portName = node.Attributes["portName"].Value;
			result._baudRate = Convert.ToInt32(node.Attributes["baudRate"].Value);
			result._dataBits = Convert.ToInt32(node.Attributes["dataBits"].Value);
			result._handShake = (Handshake)Enum.Parse(typeof(Handshake), node.Attributes["handShake"].Value);
			result._parity = (Parity)Enum.Parse(typeof(Parity), node.Attributes["parity"].Value);
			result._stopBits = (StopBits)Enum.Parse(typeof(StopBits), node.Attributes["stopBits"].Value);
			return result;
		}

		public void Save(XmlTextWriter xml)
		{
			xml.WriteStartElement("interface");
			xml.WriteAttributeString("id", Id);
			xml.WriteAttributeString("portName", PortName);
			xml.WriteAttributeString("baudRate", BaudRate.ToString());
			xml.WriteAttributeString("dataBits", DataBits.ToString());
			xml.WriteAttributeString("handShake", HandShake.ToString());
			xml.WriteAttributeString("parity", Parity.ToString());
			xml.WriteAttributeString("stopBits", StopBits.ToString());
			xml.WriteEndElement();
		}

		internal RS232Configuration()
		{ }
		
		public string Id
		{
			get;
			internal set;
		}

		private string _portName = "";

		public string PortName
		{
			get { return _portName; }
			internal set { _portName = value; }
		}

		private int _baudRate = 9600;

		public int BaudRate
		{
			get { return _baudRate; }
			internal set { _baudRate = value; }
		}

		private int _dataBits = 8;

		public int DataBits
		{
			get { return _dataBits; }
			internal set { _dataBits = value; }
		}

		private Handshake _handShake = Handshake.None;

		public Handshake HandShake
		{
			get { return _handShake; }
			internal set { _handShake = value; }
		}

		private Parity _parity = Parity.None;

		public Parity Parity
		{
			get { return _parity; }
			internal set { _parity = value; }
		}

		private StopBits _stopBits = StopBits.Two;

		public StopBits StopBits
		{
			get { return _stopBits; }
			internal set { _stopBits = value; }
		}
		
		private SerialPort _rs232 = null;
		
		public void Open()
		{
			if (_rs232 == null || !_rs232.IsOpen)
			{
				Close(null);
				_rs232 = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
				_rs232.Handshake = HandShake;
				_rs232.WriteTimeout = 10000;
				_rs232.Open();
				_rs232.DataReceived += new SerialDataReceivedEventHandler(_rs232_DataReceived);
				_rs232.ErrorReceived += new SerialErrorReceivedEventHandler(_rs232_ErrorReceived);
			}
		}

		void _rs232_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		public IRSReceiver Receiver
		{
			get;
			set;
		}

		void _rs232_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			if (Receiver != null)
			{
				#if DEBUG
				List<string> bytes = new List<string>();
				#endif
				while (_rs232.BytesToRead > 0)
				{
					byte data = (byte)_rs232.ReadByte();
					Receiver.ReceivedByte(this, data);
					#if DEBUG
					bytes.Add("0x" + data.ToString("X2"));
					#endif
				}
				#if DEBUG
				string s = string.Join(" ", bytes.ToArray());
				string t = string.Format("Odebrano dane z '{0}': {1}", Id, s);
				if (Log != null)
				{
					Log.Log(Module, t);
				}
				else
				{
					Debug.WriteLine(t);
				}
				#endif
			}
		}
		
		public void Close(XMLConfiguration conf)
		{
			if (_rs232 != null && _rs232.IsOpen)
			{
				if (_rs232.IsOpen)
				{
					// uinicjalizacja wszystkich urządzeń na tym interfejsie
					if (conf != null)
					{
						foreach (LEDDevice dev in conf.LEDDevices)
						{
							if (dev.Interface == this)
							{
								dev.Uninitialize();
							}
						}
						
						foreach (LEDDisplayDevice dev in conf.LEDDisplayDevices)
						{
							if (dev.Interface == this)
							{
								dev.Uninitialize();
							}
						}
						
						foreach (LCDDevice dev in conf.LCDDevices)
						{
							if (dev.Interface == this)
							{
								dev.Uninitialize();
							}
						}
						
						foreach (KeysDevice dev in conf.KeysDevices)
						{
							if (dev.Interface == this)
							{
								dev.Uninitialize();
							}
						}
					}

					
					
					_rs232.Close();
				}
				_rs232.Dispose();
				_rs232 = null;
			}
		}
		
		public void Write(byte[] data)
		{
			if (_rs232 != null && _rs232.IsOpen)
			{
				//                                for (int i = 0; i < data.Length; i++)
				//                                {
				//                                    _rs232.Write(data, i, 1);
				//                                    System.Threading.Thread.Sleep(5);
				//                                }
				lock ("com")
				{
					_rs232.Write(data, 0, data.Length);
				}
				//System.Threading.Thread.Sleep(1);
				//_rs232.BaseStream.Flush();
				#if DEBUG
				string s = "";
				for (int i = 0; i < data.Length; i++)
				{
					s += "0x" + data[i].ToString("X2") + " ";
				}
				string t = string.Format("Wysłano dane do '{0}': {1}", Id, s);
				if (Log != null)
				{
					Log.Log(Module, t);
				}
				else
				{
					Debug.WriteLine(t);
				}
				#endif
			}
		}
		
		public void StartIdentifyDevices()
		{
			Write(new byte[] { 255, 1, 200 });
		}
		
		public void StopIdentifyDevices()
		{
			Write(new byte[] { 255, 1, 201 });
		}
		
		public void DevicesReports()
		{
			Write(new byte[] { 255, 1, 202 });
		}
		
		public void SetDeviceSessionId(byte deviceId, byte deviceSessionId)
		{
			Write(new byte[] { deviceId, 2, 203, deviceSessionId });
		}
		
		public void SetDevicesQuiet()
		{
			Write(new byte[] { 255, 1, 204 });
		}
		
		public void ClearReceiveBufor()
		{
			if (_rs232 != null && _rs232.IsOpen)
			{
				_rs232.DiscardInBuffer();
			}
		}
		
		public HomeSimCockpitSDK.ILog Log
		{
			get;
			set;
		}
		
		public HomeSimCockpitSDK.IModule Module
		{
			get;
			set;
		}
		
		public int CompareTo(RS232Configuration other)
		{
			return Id.CompareTo(other.Id);
		}
	}
}
