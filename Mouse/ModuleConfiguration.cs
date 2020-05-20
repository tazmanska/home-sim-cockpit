using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Mouse
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

            XmlNodeList nodes = xml.SelectNodes("configuration/output/clicks/click");
            if (nodes != null && nodes.Count > 0)
            {
                List<Click> clicks = new List<Click>();
                foreach (XmlNode node in nodes)
                {
                    clicks.Add(Click.Read(node));
                }
                c.Clicks = clicks.ToArray();
            }
            return c;
        }

        private static ModuleConfiguration __instance = null;

        private ModuleConfiguration()
        {
            Clicks = new Click[0];
        }

        public static void Save(string fileName, ModuleConfiguration configuration)
        {
            using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");
                xml.WriteStartElement("output");
                xml.WriteStartElement("clicks");

                if (configuration.Clicks != null)
                {
                    for (int i = 0; i < configuration.Clicks.Length; i++)
                    {
                        configuration.Clicks[i].Write(xml);
                    }
                }

                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        public Click[] Clicks
        {
            get;
            set;
        }
    }
}
