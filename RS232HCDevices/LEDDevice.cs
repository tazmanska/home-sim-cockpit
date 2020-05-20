/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-02
 * Godzina: 21:30
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LEDDevice.
    /// </summary>
    class LEDDevice : Device, IComparable<LEDDevice>, IDeviceBrightness
    {
        public static readonly byte LED_ON = 0x01;
        public static readonly byte LED_OFF = 0x02;
        public static readonly byte ALL_ON = 0x03;
        public static readonly byte ALL_OFF = 0x04;
        public static readonly byte LEDS = 0x05;
        public static readonly byte BRIGHTNESS = 0x06;
        
        internal LEDDevice()
        {
            IndexReversed = true;
        }
        
        public static LEDDevice Load(XmlNode xml, List<RS232Configuration> interfaces)
        {
            LEDDevice result = new LEDDevice();
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
            result.LEDsCount = byte.Parse(xml.Attributes["ledsCount"].Value);
            if (xml.Attributes["indexReverse"] != null)
            {
                result.IndexReversed = bool.Parse(xml.Attributes["indexReverse"].Value);
            }
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ledDevice");
            xmlWriter.WriteAttributeString("interface", Interface.Id);
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("device", DeviceId.ToString());
            xmlWriter.WriteAttributeString("ledsCount", LEDsCount.ToString());
            xmlWriter.WriteAttributeString("indexReverse", IndexReversed.ToString());
            xmlWriter.WriteEndElement();
        }
        
        public bool IndexReversed
        {
            get;
            internal set;
        }
        
        public byte LEDsCount
        {
            get;
            internal set;
        }
        
        public override void Initialize()
        {
            // zgaszenie wszystkich diód
            Interface.Write(new byte [] { DeviceId, 1, ALL_OFF });
            
            // ustawienie jasności
            Interface.Write(new byte [] { DeviceId, 2, BRIGHTNESS, 10 });
        }
        
        public override void Uninitialize()
        {
            // zgaszenie wszystkich diód
            Interface.Write(new byte[] { DeviceId, 1, ALL_OFF });
        }
        
        public void TurnOnOff(byte index, bool on)
        {
            if (IndexReversed)
            {
                index = (byte)(LEDsCount - index - 1);
            }
            Interface.Write(new byte[] { DeviceId, 2, on ? LED_ON : LED_OFF, index });
        }
        
        public void SetBrightness(byte level)
        {
            Interface.Write(new byte[] { DeviceId, 2, BRIGHTNESS, level });
        }
        
        public int CompareTo(LEDDevice other)
        {
            return base.CompareTo(other);
        }
    }
}
