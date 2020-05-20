/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-26
 * Godzina: 13:03
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace fsCockpit
{
    /// <summary>
    /// Description of XmlConfiguration.
    /// </summary>
    class XmlConfiguration
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
        
        private static XmlConfiguration __instance = null;
        
        public static XmlConfiguration Reload()
        {
            __instance = null;
            return Load();
        }

        public static XmlConfiguration Load()
        {
            if (__instance != null)
            {
                return __instance;
            }
            
            if (!File.Exists(ConfigurationFilePath))
            {
                throw new FileNotFoundException(ConfigurationFilePath);
            }
            XmlConfiguration c = new XmlConfiguration();
            XmlDocument xml = new XmlDocument();
            xml.Load(ConfigurationFilePath);
            
            // wczytanie interfejsów
            List<FCU> fcus = new List<FCU>();
            XmlNodeList nodes = xml.SelectNodes("/configuration/devices/fcu");
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    fcus.Add(new FCU(Module, node));
                }
            }
            c.FCUs = fcus.ToArray();
            
            List<COMNAV> comnavs = new List<COMNAV>();
            nodes = xml.SelectNodes("/configuration/devices/comNav");
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    comnavs.Add(new COMNAV(Module, node));
                }
            }
            c.COMNAVs = comnavs.ToArray();
            
            List<EFIS> efis = new List<EFIS>();
            nodes = xml.SelectNodes("/configuration/devices/efis");
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    efis.Add(new EFIS(Module, node));
                }
            }
            c.EFISs = efis.ToArray();

            __instance = c;
            
            return __instance;
        }
        
        public static HomeSimCockpitSDK.IInput Module
        {
            get;
            set;
        }
        
        public void Save()
        {
            Save(ConfigurationFilePath);
        }

        public void Save(string fileName)
        {
            using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");

                xml.WriteStartElement("devices");
                
                if (FCUs != null)
                {
                    for (int i = 0; i < FCUs.Length; i++)
                    {
                        FCUs[i].Save(xml);
                    }
                }
                
                if (COMNAVs != null)
                {
                    for (int i = 0; i < COMNAVs.Length; i++)
                    {
                        COMNAVs[i].Save(xml);
                    }
                }
                
                if (EFISs != null)
                {
                    for (int i = 0; i < EFISs.Length; i++)
                    {
                        EFISs[i].Save(xml);
                    }
                }
                
                xml.WriteEndElement();

                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        internal XmlConfiguration()
        {

        }
        
        public FCU[] FCUs
        {
            get;
            set;
        }
        
        public COMNAV[] COMNAVs
        {
            get;
            set;
        }
        
        public EFIS[] EFISs
        {
            get;
            set;
        }
    }
}
