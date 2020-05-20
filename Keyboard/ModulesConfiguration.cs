using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Keyboard
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
            if (__instance == null)
            {
                __instance = Load(ConfigurationFilePath);
            }
            return __instance;
        }

        public static ModulesConfiguration Reload()
        {
            __instance = null;
            return Load();
        }

        public static ModulesConfiguration Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            ModulesConfiguration c = new ModulesConfiguration();
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);

            XmlNodeList nodes = xml.SelectNodes("configuration/input/keys");
            if (nodes != null && nodes.Count > 0)
            {
                List<KeysInputVariable> keys = new List<KeysInputVariable>();
                foreach (XmlNode node in nodes)
                {
                    keys.Add(KeysInputVariable.Read(node));
                }
                c.Keys = keys.ToArray();
            }
            return c;
        }

        private static ModulesConfiguration __instance = null;

        private ModulesConfiguration()
        {
            Keys = new KeysInputVariable[0];
        }

        public void Save()
        {
            using (XmlTextWriter xml = new XmlTextWriter(ConfigurationFilePath, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");
                xml.WriteStartElement("input");

                if (Keys != null)
                {
                    for (int i = 0; i < Keys.Length; i++)
                    {
                        Keys[i].Write(xml);
                    }
                }

                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        public KeysInputVariable[] Keys
        {
            get;
            set;
        }
    }
}
