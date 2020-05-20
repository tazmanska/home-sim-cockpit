/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-11-29
 * Godzina: 22:22
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of DeviceType1.
    /// </summary>
    class DeviceType1 : Device
    {
        public new static DeviceType1 Load(XmlNode xml)
        {
            DeviceType1 result = new DeviceType1(xml.Attributes["id"].Value, xml.Attributes["description"].Value);
            result.DeviceId = 0x00;
            result.Parent = null;
            result.Extension = false;
            result.Index = 0;
            return result;
        }
        
        public DeviceType1(string id, string description) : base(id, description)
        {
            Type = DeviceType.Type1;
        }
    }
}