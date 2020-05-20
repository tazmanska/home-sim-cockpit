using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace RS232HCDevices
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
            c._rs232Configuration = RS232Configuration.Load(xml.SelectSingleNode("/configuration/rs232"));
            XmlNodeList nodes = xml.SelectNodes("/configuration/lcds/lcd");
            if (nodes != null && nodes.Count > 0)
            {
                List<LCD> lcds = new List<LCD>();
                foreach (XmlNode node in nodes)
                {
                    lcds.Add(LCD.Load(node));
                }
                c._lcds = lcds.ToArray();
            }
            return c;
        }

        private Configuration()
        {

        }

        private LCD[] _lcds = null;

        public LCD[] LCDs
        {
            get { return _lcds; }
        }

        private RS232Configuration _rs232Configuration = null;

        public RS232Configuration RS232Configuration
        {
            get { return _rs232Configuration; }
        }
    }
}
