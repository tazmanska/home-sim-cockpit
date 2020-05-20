using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace FalconData
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
            XmlNode node = xml.SelectSingleNode("/configuration/settings/dataFormat");
            c._dataFormat = (F4SharedMem.FalconDataFormats)Enum.Parse(typeof(F4SharedMem.FalconDataFormats), node.InnerText);
            node = xml.SelectSingleNode("/configuration/settings/interval");
            c._interval = int.Parse(node.InnerText);
            if (c._interval < 0)
            {
                throw new Exception("Nieprawidłowa wartość parametru 'interval.");
            }
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
                xml.WriteElementString("dataFormat", DataFormat.ToString());
                xml.WriteElementString("interval", Interval.ToString());
                xml.WriteEndElement();
                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        private Configuration()
        {

        }

        private F4SharedMem.FalconDataFormats _dataFormat = F4SharedMem.FalconDataFormats.AlliedForce;

        public F4SharedMem.FalconDataFormats DataFormat
        {
            get { return _dataFormat; }
            set { _dataFormat = value; }
        }

        private int _interval = 50;

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
    }
}
