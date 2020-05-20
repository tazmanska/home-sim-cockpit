using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;

namespace DataViaNetwork
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
                    throw new FileNotFoundException(fileName);
                }
                XmlDocument xml = new XmlDocument();
                xml.Load(fileName);
                XmlNode node = xml.SelectSingleNode("/configuration/settings/sender");
                __instance = new Configuration();
                __instance.Sender = SenderConfiguration.Load(node);
                node = xml.SelectSingleNode("/configuration/settings/receiver");
                __instance.Receiver = ReceiverConfiguration.Load(node);                
            }
            return __instance;
        }

        public SenderConfiguration Sender
        {
            get;
            set;
        }

        public ReceiverConfiguration Receiver
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
                Sender.Save(xml);
                Receiver.Save(xml);
                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }    
        }

        private Configuration()
        {

        }
    }
}
