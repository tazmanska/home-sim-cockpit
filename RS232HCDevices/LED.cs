/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-03
 * Godzina: 20:04
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LED.
    /// </summary>
    internal class LED : IOutputVariable, IComparable<LED>
    {
        internal LED()
        {
        }
        
        public static LED Load(XmlNode xml, List<LEDDevice> ledDevices)
        {
            LED result = new LED();
            string ledDeviceId = xml.Attributes["ledDevice"].Value;
            LEDDevice ledDevice = ledDevices.Find(delegate (LEDDevice o)
                                                        {
                                                            return o.Id == ledDeviceId;
                                                        });
            if (ledDevice == null)
            {
                return null;
            }
            
            result.LEDDevice = ledDevice;
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.Index = byte.Parse(xml.Attributes["index"].Value);
            return result;            
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("led");
            xmlWriter.WriteAttributeString("ledDevice", LEDDevice.Id);
            xmlWriter.WriteAttributeString("id", ID);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("index", Index.ToString());
            xmlWriter.WriteEndElement();
        }
        
        public byte Index
        {
            get;
            internal set;
        }
        
        public string ID
        {
            get;
            internal set;
        }
        
        public string Description
        {
            get;
            internal set;
        }
        
        public LEDDevice LEDDevice
        {
            get;
            internal set;
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get
            {
                return HomeSimCockpitSDK.VariableType.Bool;
            }
        }      
        
        private bool _isOn = false;
        
        public void Initialize()
        {
            _isOn = false;
        }
        
        public void Uninitialize()
        {
            
        }
        
        public void SetValue(object value)
        {
            bool v = (bool)value;
            if (_isOn != v)
            {
                _isOn = v;
                LEDDevice.TurnOnOff(Index, _isOn);
            }
        }
                
        public Device[] Devices
        {
            get { return new Device[] { LEDDevice }; }
        }
        
        public int CompareTo(LED other)
        {
            int result = ID.CompareTo(other.ID);
            if (result == 0)
            {
                result = Description.CompareTo(other.Description);
            }
            return result;
        }
    }
}
