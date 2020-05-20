using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace FSXData
{
    class Configuration
    {
        public static Configuration Load(string fileName, HomeSimCockpitSDK.IInput module)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            Configuration c = new Configuration();
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            XmlNodeList nodes = xml.SelectNodes("/configuration/inputs/input");
            if (nodes != null && nodes.Count > 0)
            {
                List<FSXInput> inputs = new List<FSXInput>();
                foreach (XmlNode node in nodes)
                {
#warning sprawdzenie czy zmienna o danym id już istnieje
                    inputs.Add(FSXInput.Load(node, module));
                }
                inputs.Sort();
                c._inputs = inputs.ToArray();
            }
            return c;
        }

        private Configuration()
        {

        }

        private FSXInput[] _inputs = null;

        public FSXInput[] Inputs
        {
            get { return _inputs; }
        }
    }
}
