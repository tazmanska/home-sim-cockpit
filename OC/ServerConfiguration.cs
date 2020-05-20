/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-02
 * Godzina: 18:58
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Net;
using System.Xml;

namespace OC
{
    /// <summary>
    /// Description of InputConfiguration.
    /// </summary>
    public class ServerConfiguration
    {
        public ServerConfiguration()
        {
            IP = new IPAddress(new byte[] { 192, 168, 0, 100 });
            Port = 8092;
        }
        
        public static ServerConfiguration Load(XmlNode xml)
        {
            ServerConfiguration result = new ServerConfiguration();
            result.IP = IPAddress.Parse(xml.Attributes["ip"].Value);
            result.Port = int.Parse(xml.Attributes["port"].Value);
            return result;
        }

        public IPAddress IP
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public void Save(XmlTextWriter xml, string elementName)
        {
            xml.WriteStartElement(elementName);
            xml.WriteAttributeString("ip", IP.ToString());
            xml.WriteAttributeString("port", Port.ToString());
            xml.WriteEndElement();
        }
    }
}
