/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-04-09
 * Godzina: 08:29
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices.Servos
{
	/// <summary>
	/// Description of ServoDevice.
	/// </summary>
	class ServoDevice : Device, IComparable<ServoDevice>
	{
		public static readonly byte ON = 0x01;
		public static readonly byte OFF = 0x02;
		public static readonly byte SET_POS = 0x03;
		
		public ServoDevice()
		{
			Servos = new Servo[0];
		}
		
        public static ServoDevice Load(XmlNode xml, List<RS232Configuration> interfaces)
        {
        	ServoDevice result = new ServoDevice();
            
            string interfaceId = xml.Attributes["interface"].Value;
            RS232Configuration interf = interfaces.Find(delegate (RS232Configuration o)
                                                        {
                                                            return o.Id == interfaceId;
                                                        });
            if (interf == null)
            {
                return null;
            }
            
            result.Interface = interf;
            result.Id = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.DeviceId = byte.Parse(xml.Attributes["device"].Value);
            
            List<Servo> servos = new List<Servo>();
            foreach (XmlNode node in xml.SelectNodes("servo"))
            {
            	Servo s = Servo.Load(node);
            	if (s != null)
            	{
            		s.Device = result;
            		servos.Add(s);
            	}
            }
            result.Servos = servos.ToArray();
            
            return result;
        }
        
        public virtual void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("servoDevice");
            xmlWriter.WriteAttributeString("interface", Interface.Id);
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("device", DeviceId.ToString());
            foreach (Servo s in Servos)
            {
            	xmlWriter.WriteStartElement("servo");
            	s.SaveToXml(xmlWriter);
            	xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }
		
		public Servo[] Servos
		{
			get;
			set;
		}
		
		public int CompareTo(ServoDevice other)
		{
			return base.CompareTo(other);
		}
		
		private int _enabled = 0;
		
		public override void Uninitialize()
		{	
			Initialize();
		}
		
		public override void Initialize()
		{
			// wyłączenie wszystkich serw
			SetEnable(false);			
			
			// powrót pozycji startowych serw
			for (int i = 0; i <Servos.Length; i++)
			{
				Servos[i].Reset();
			}
			
			// włączenie serw
			SetEnable(true);
			
			// poczekanie aż serwa się ustawią
			System.Threading.Thread.Sleep(200);
			
			// wyłączenie wszystkich serw
			SetEnable(false);
		}
		
		public void SetEnable(bool enable)
		{
			if (enable)
			{
				_enabled = 0x0f;
			}
			else
			{
				_enabled = 0;
			}
			Interface.Write(new byte[] { DeviceId, 1, enable ? ON : OFF } );
		}
		
		public void SetEnable(byte index, bool enable)
		{
			if (enable)
			{
				_enabled |= (1 << index);
			}
			else
			{
				_enabled &= ~(1 << index);
			}
			Interface.Write(new byte[] { DeviceId, 2, ON, (byte)_enabled } );
		}
		
		public void SetPosition(byte index, int position)
		{
			Interface.Write(new byte[] { DeviceId, 4, SET_POS, index, (byte)((position & 0xff00) >> 8), (byte)(position & 0x00ff) });
		}		
	}
}
