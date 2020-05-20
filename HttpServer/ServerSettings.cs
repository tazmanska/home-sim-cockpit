/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-20
 * Godzina: 20:50
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Xml;

namespace HttpServer
{
    /// <summary>
    /// Description of ServerSettings.
    /// </summary>
    class ServerSettings
    {
        public static ServerSettings Load(XmlNode xml)
        {
            ServerSettings result = new ServerSettings();
            result.Port = int.Parse(xml.Attributes["port"].Value);            
            return result;
        }
        
        public void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("server");
            xml.WriteAttributeString("port", Port.ToString());
            xml.WriteEndElement();
        }
        
        public ServerSettings()
        {
        }
        
        public int Port
        {
            get;
            set;
        }
    }
}
