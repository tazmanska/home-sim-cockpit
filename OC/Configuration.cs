/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-02
 * Godzina: 18:57
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace OC
{
    class Configuration
    {
        private static Configuration __instance = null;

        public static Configuration Load(string fileName)
        {
            if (__instance == null)
            {
                if (!File.Exists(fileName))
                {
                    __instance = new Configuration();
                }
                else
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(fileName);
                    XmlNode node = xml.SelectSingleNode("/configuration/settings/input");
                    __instance = new Configuration();
                    __instance.Input = ServerConfiguration.Load(node);
                    node = xml.SelectSingleNode("/configuration/settings/output");
                    __instance.Output = ServerConfiguration.Load(node);
                }
            }
            return __instance;
        }

        public ServerConfiguration Output
        {
            get;
            set;
        }

        public ServerConfiguration Input
        {
            get;
            set;
        }

        public void Save(string fileName)
        {
            using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");
                xml.WriteStartElement("settings");
                Output.Save(xml, "output");
                Input.Save(xml, "input");
                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        private Configuration()
        {
            Output = new ServerConfiguration();
            Input = new ServerConfiguration();
        }
    }
}
