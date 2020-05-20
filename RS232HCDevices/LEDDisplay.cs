/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-05
 * Godzina: 19:45
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LEDDisplay.
    /// </summary>
    class LEDDisplay : IOutputVariable, IComparable<LEDDisplay>
    {
        internal LEDDisplay()
        {
            Type = HomeSimCockpitSDK.VariableType.String;
        }
        
        public static LEDDisplay Load(XmlNode xml, List<LEDDisplayDevice> ledDisplayDevices)
        {
            LEDDisplay result = new LEDDisplay();
            string ledDisplayDeviceId = xml.Attributes["ledDisplayDevice"].Value;
            LEDDisplayDevice ledDisplayDevice = ledDisplayDevices.Find(delegate (LEDDisplayDevice o)
                                                                       {
                                                                           return o.Id == ledDisplayDeviceId;
                                                                       });
            if (ledDisplayDevice == null)
            {
                return null;
            }
            
            result.LEDDisplayDevice = ledDisplayDevice;
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.Index = byte.Parse(xml.Attributes["index"].Value);
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ledDisplay");
            xmlWriter.WriteAttributeString("ledDisplayDevice", LEDDisplayDevice.Id);
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
        
        public LEDDisplayDevice LEDDisplayDevice
        {
            get;
            internal set;
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get;
            internal set;
        }
        
        private byte _value = 0;
        
        public void Initialize()
        {
            _value = 0;
        }
        
        public void Uninitialize()
        {
            
        }
        
        public void SetValue(object value)
        {
            switch (Type)
            {
                case HomeSimCockpitSDK.VariableType.String:
                    byte[] data = Dictionary.CodeString((string)value);
                    SetValue(data[0]);
                    break;
                    
                case HomeSimCockpitSDK.VariableType.Int:
                    SetValue((byte)(int)value);
                    break;
            }
        }
        
        public void SetValue(byte value)
        {
            if (_value != value)
            {
                _value = value;
                LEDDisplayDevice.SetDisplayValue(Index, value);
            }
        }
                
        public Device[] Devices
        {
            get { return new Device[] { LEDDisplayDevice }; }
        }
        
        public LEDDisplaysDictionary Dictionary
        {
            get;
            set;
        }
        
        public int CompareTo(LEDDisplay other)
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
