/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-05
 * Godzina: 19:31
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LEDDisplayDevice.
    /// </summary>
    class LEDDisplayDevice : Device, IComparable<LEDDisplayDevice>, IDeviceBrightness
    {
        internal LEDDisplayDevice()
        {
        }
        
        public static LEDDisplayDevice Load(XmlNode xml, List<RS232Configuration> interfaces)
        {
            LEDDisplayDevice result = new LEDDisplayDevice();
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
            result.LEDDisplaysCount = byte.Parse(xml.Attributes["displaysCount"].Value);
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ledDisplayDevice");
            xmlWriter.WriteAttributeString("interface", Interface.Id);
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("device", DeviceId.ToString());
            xmlWriter.WriteAttributeString("displaysCount", LEDDisplaysCount.ToString());
            xmlWriter.WriteEndElement();
        }
        
        public byte LEDDisplaysCount
        {
            get;
            internal set;
        }
        
        private byte _brightness = 10;
        
        public override void Initialize()
        {
            // zgaszenie wszystkich wyświetlaczy (diod)
            Interface.Write(new byte [] { DeviceId, 1, LEDDevice.ALL_OFF });
            
            // ustawienie jasności
            _brightness = 0;
            SetBrightness(10);
        }
        
        public override void Uninitialize()
        {
            // zgaszenie wszystkich diód
            Interface.Write(new byte[] { DeviceId, 1, LEDDevice.ALL_OFF });
        }
        
        public void SetBrightness(byte level)
        {
            if (_brightness != level)
            {
                Interface.Write(new byte[] { DeviceId, 2, LEDDevice.BRIGHTNESS, level });
                _brightness = level;
            }
        }
        
        public void SetDisplayValue(byte displayIndex, byte value)
        {
            Interface.Write(new byte[] { DeviceId, 3, LEDDevice.LEDS, displayIndex, value });
        }
        
        public byte[] CodeString(string text)
        {
            return Dictionary.CodeString(text);
        }
        
        public LEDDisplaysDictionary Dictionary
        {
            get;
            set;
        }
        
        public int CompareTo(LEDDisplayDevice other)
        {
            return base.CompareTo(other);
        }
    }
}
