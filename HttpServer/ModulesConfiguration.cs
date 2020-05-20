/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-20
 * Godzina: 20:14
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace HttpServer
{
    class ModulesConfiguration
    {
        private static string __configPath = null;

        public static string ConfigurationFilePath
        {
            get
            {
                if (__configPath == null)
                {
                    __configPath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
                }
                return __configPath;
            }
        }

        public static ModulesConfiguration Load()
        {
            ModulesConfiguration c = new ModulesConfiguration();
            if (File.Exists(ConfigurationFilePath))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(ConfigurationFilePath);
                
                c.Server = ServerSettings.Load(xml.SelectSingleNode("/configuration/server"));
                
                XmlNodeList nodes = xml.SelectNodes("/configuration/applications/application");
                List<HttpApplication> applications = new List<HttpApplication>();
                foreach (XmlNode node in nodes)
                {
                    applications.Add(HttpApplication.Load(node));
                }
                c.Applications = applications.ToArray();
            }
            return c;
        }

        private ModulesConfiguration()
        {
        }

        public void Save()
        {
            using (XmlTextWriter xml = new XmlTextWriter(ConfigurationFilePath, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");

                Server.Save(xml);

                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }
        
        public ServerSettings Server
        {
            get;
            private set;
        }
        
        public HttpApplication[] Applications
        {
            get;
            set;
        }
    }
}
