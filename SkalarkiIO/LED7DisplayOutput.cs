/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-11
 * Godzina: 18:03
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Xml;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of LED7DisplayOutput.
    /// </summary>
    class LED7DisplayOutput : OutputVariable, IComparable<LED7DisplayOutput>
    {
        public static LED7DisplayOutput Load(XmlNode xml)
        {
            LED7DisplayOutput result = new LED7DisplayOutput();
            result.DeviceId = xml.Attributes["deviceId"].Value;
            result.Index = int.Parse(xml.Attributes["index"].Value);
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            return result;
        }
        
        public virtual void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("display");
            xml.WriteAttributeString("deviceId", DeviceId);
            xml.WriteAttributeString("index", Index.ToString());
            xml.WriteAttributeString("id", ID);
            xml.WriteAttributeString("description", Description);
            xml.WriteEndElement();
        }
        
        public override string ToString()
        {
            return string.Format("Wyświetlacz 7-LED, Urządzenie: {0}, Indeks: {1}, ID: {2}, Opis: {3}", DeviceId, Index, ID, Description);
        }
        
        public LED7DisplayOutput()
        {
            Type = HomeSimCockpitSDK.VariableType.String;
        }
        
        public int Index
        {
            get;
            set;
        }
        
        public override void Reset()
        {

        }
        
        public override void SetValue(object value)
        {
            byte[] data = LED7DisplaysDictionary.Instance.CodeString((string)value);
            SetValue(data[0]);
        }
        
        public void SetValue(byte data)
        {
            Device.WriteTo7LEDDisplays(new byte[] { (byte)Index }, new byte[] { data }, 1);
        }
        
        public int CompareTo(LED7DisplayOutput other)
        {
            if (DeviceId == null)
            {
                return Index.CompareTo(other.Index);
            }
            int result = DeviceId.CompareTo(other.DeviceId);
            if (result == 0)
            {
                result = Index.CompareTo(other.Index);
            }
            return result;
        }
    }
}