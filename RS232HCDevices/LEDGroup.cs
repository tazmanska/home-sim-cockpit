/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-03
 * Godzina: 20:14
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LEDGroup.
    /// </summary>
    internal class LEDGroup : IOutputVariable, IComparable<LEDGroup>
    {
        public LEDGroup()
        {
        }
        
        public static LEDGroup Load(XmlNode xml, List<LED> leds)
        {
            LEDGroup result = new LEDGroup();
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            XmlNodeList nodes = xml.SelectNodes("led");
            List<LED> leds2 = new List<LED>();
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    string ledId = node.Attributes["led"].Value;
                    LED led = leds.Find(delegate(LED o)
                                        {
                                            return o.ID == ledId;
                                        });
                    if (led == null)
                    {
                        return null;
                    }
                    leds2.Add(led);
                }
            }
            result.LEDs = leds2.ToArray();
            if (result.LEDs.Length == 0)
            {
                return null;
            }
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("leds");
            xmlWriter.WriteAttributeString("id", ID);
            xmlWriter.WriteAttributeString("description", Description);
            foreach (LED led in LEDs)
            {
                xmlWriter.WriteStartElement("led");
                xmlWriter.WriteAttributeString("led", led.ID);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }
        
        public LED[] LEDs
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
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get
            {
                return HomeSimCockpitSDK.VariableType.Bool;
            }
        }
        
        public void Initialize()
        {
        }
        
        public void Uninitialize()
        {
            
        }
        
        public void SetValue(object value)
        {
            for (int i = 0; i < LEDs.Length; i++)
            {
                LEDs[i].SetValue(value);
            }
        }
                
        public Device[] Devices
        {
            get
            {
                List<Device> result = new List<Device>();
                foreach (LED l in LEDs)
                {
                    result.Add(l.LEDDevice);
                }
                return result.ToArray();
            }
        }
        
        public string LEDsIDs
        {
            get
            {
                List<string> ids = new List<string>();
                for (int i = 0; i < LEDs.Length; i++)
                {
                    ids.Add(string.Format("{0} ({1})", LEDs[i].ID, LEDs[i].Description));
                }                
                return string.Join(", ", ids.ToArray());
            }
        }
        
        public int CompareTo(LEDGroup other)
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
