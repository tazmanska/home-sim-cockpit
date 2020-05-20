/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-28
 * Godzina: 22:00
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Xml;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of DeviceType2.
    /// </summary>
    class DeviceType2 : Device
    {
        public new static DeviceType2 Load(XmlNode xml)
        {
            DeviceType2 result = new DeviceType2(xml.Attributes["id"].Value, xml.Attributes["description"].Value);
            result.DeviceId = 0x00;
            result.Parent = null;
            result.Extension = false;
            result.Index = 0;
            return result;
        }
        
        public DeviceType2(string id, string description) : base(id, description)
        {
            Type = DeviceType.Type2;
        }
    }
}
