/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-04
 * Godzina: 18:49
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace RSSReader
{
    class ModuleConfiguration
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

        public static ModuleConfiguration Load()
        {
            if (__instance == null)
            {
                __instance = Load(ConfigurationFilePath);
            }
            return __instance;
        }

        public static ModuleConfiguration Reload()
        {
            __instance = null;
            return Load();
        }

        public static ModuleConfiguration Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            ModuleConfiguration c = new ModuleConfiguration();
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);

            XmlNodeList nodes = xml.SelectNodes("configuration/input/rss");
            if (nodes != null && nodes.Count > 0)
            {
                List<RSS> rsss = new List<RSS>();
                foreach (XmlNode node in nodes)
                {
                    rsss.Add(RSS.Read(node));
                }
                c.RSSs = rsss.ToArray();
            }
            return c;
        }

        private static ModuleConfiguration __instance = null;

        private ModuleConfiguration()
        {
            RSSs = new RSS[0];
        }

        public static void Save(string fileName, ModuleConfiguration configuration)
        {
            using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");
                xml.WriteStartElement("input");

                if (configuration.RSSs != null)
                {
                    for (int i = 0; i < configuration.RSSs.Length; i++)
                    {
                        configuration.RSSs[i].Save(xml);
                    }
                }

                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        public RSS[] RSSs
        {
            get;
            set;
        }
    }
}
