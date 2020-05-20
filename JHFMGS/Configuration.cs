/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-02-20
 * Godzina: 19:12
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace JHFMGS
{
	/// <summary>
	/// Description of Configuration.
	/// </summary>
   class Configuration
    {
        public static Configuration Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
            	return new Configuration();
            }
            Configuration c = new Configuration();
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            XmlNode node = xml.SelectSingleNode("/configuration/settings/server");
            c.ServerIP = IPAddress.Parse(node.Attributes["ip"].Value);
            c.ServerPort = int.Parse(node.Attributes["port"].Value);
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
                xml.WriteEndElement();
                
                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        private Configuration()
        {
        	ServerIP = IPAddress.Parse("127.0.0.1");
        	ServerPort = 25255;
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
    }
}
