/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-17
 * Godzina: 22:08
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of Encoder.
    /// </summary>
    class Encoder : IComparable<Encoder>
    {
        public Encoder()
        {
            DetectFast = false;
            FastTime = 17000;
            Type = EncoderType.Standard;
        }
        
        public static Encoder Load(XmlNode xml, List<KeysDevice> keysDevices)
        {
            Encoder result = new Encoder();
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
            result.Description = xml.Attributes["description"].Value;
            result.Index = byte.Parse(xml.Attributes["index"].Value);
            if (xml.Attributes["fast"] != null)
            {
                result.DetectFast = bool.Parse(xml.Attributes["fast"].Value);
            }
            if (xml.Attributes["fastTime"] != null)
            {
                result.FastTime = long.Parse(xml.Attributes["fastTime"].Value);
            }
            if (xml.Attributes["type"] != null)
            {
                result.Type = (EncoderType)Enum.Parse(typeof(EncoderType), xml.Attributes["type"].Value);
            }
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("encoder");
            xmlWriter.WriteAttributeString("keysDevice", KeysDevice.Id);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("index", Index.ToString());
            xmlWriter.WriteAttributeString("fast", DetectFast.ToString());
            xmlWriter.WriteAttributeString("fastTime", FastTime.ToString());
            xmlWriter.WriteAttributeString("type", Type.ToString());
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
       
        public byte ConfigData
        {
            //get { return (byte)((((int)Type) << 6) | Index); }
            get { return (byte)Type; }
        }
        
        public bool DetectFast
        {
            get;
            internal set;
        }
                        
        public string Description
        {
            get;
            set;
        }
        
        public EncoderType Type
        {
            get;
            internal set;
        }
        
        public long FastTime
        {
            get;
            internal set;
        }
        
        public byte LeftIndex
        {
            get { return (byte)(Index * 2); }
        }
        
        public byte RightIndex
        {
            get { return (byte)((Index * 2) + 1); }
        }
        
        public string Description2
        {
            get
            {
                if (KeysDevice != null)
                {
                    return string.Format("Wejście {0} i {1} na urządzeniu '{2}'. Typ: {4}.{3}", LeftIndex.ToString("000"), RightIndex.ToString("000"), KeysDevice.Description, DetectFast ? " Wykrywanie szybkości." : "", GetEncoderTypeDescription(Type));
                }
                return "";
            }
        }
        
        public int CompareTo(Encoder other)
        {
            return Description.CompareTo(other.Description);
        }
        
        public static string GetEncoderTypeDescription(EncoderType type)
        {
            switch (type)
            {
                case EncoderType.Standard:
                    return "pełny kod Gray'a";
                    
                case EncoderType.Half_1_2:
                    return "1/2 kodu Gray'a";
                    
                case EncoderType.Quarter_1_4:
                    return "1/4 kodu Gray'a";
                    
                default:
                    return "Nieznany typ enkodera.";
            }
        }
    }
}
