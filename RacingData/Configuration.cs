/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-07-04
 * Godzina: 18:20
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace RacingData
{
    class Configuration
    {
        public static Configuration Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            Configuration c = new Configuration();
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            XmlNode node = xml.SelectSingleNode("/configuration/settings/server");
            c.ServerIP = IPAddress.Parse(node.Attributes["ip"].Value);
            c.ServerPort = int.Parse(node.Attributes["port"].Value);
            c.Password = node.Attributes["password"].Value;
            node = xml.SelectSingleNode("/configuration/settings/client");
            c.ClientPort = int.Parse(node.Attributes["port"].Value);
            return c;
        }

        public void Save(string fileName)
        {
            using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");
                xml.WriteStartElement("settings");
                
                xml.WriteStartElement("server");
                xml.WriteAttributeString("ip", ServerIP.ToString());
                xml.WriteAttributeString("port", ServerPort.ToString());
                xml.WriteAttributeString("password", Password);
                xml.WriteEndElement();
                
                xml.WriteStartElement("client");
                xml.WriteAttributeString("port", ClientPort.ToString());
                xml.WriteEndElement();
                
                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        private Configuration()
        {

        }
        
        public IPAddress ServerIP
        {
            get;
            set;
        }
        
        public int ServerPort
        {
            get;
            set;
        }
        
        public string Password
        {
            get;
            set;
        }
        
        public int ClientPort
        {
            get;
            set;
        }
    }
}
