using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace FSData
{
    class ModuleSettings
    {
        public static ModuleSettings Load(XmlNode xml)
        {
            ModuleSettings result = new ModuleSettings();
            result.FSVersion = (FSVersion)Enum.Parse(typeof(FSVersion), xml.SelectSingleNode("fsVersion").InnerText);
            result.Interval = Convert.ToInt32(xml.SelectSingleNode("interval").InnerText);
            if (result.Interval < 0)
            {
                result.Interval = 0;
            }
            XmlNode node = xml.SelectSingleNode("fsDataDirectory");
            if (node != null)
            {
                result.FSDataDirectory = node.InnerText;
                if (Utils.__FSDataDirectory != result.FSDataDirectory)
                {
                    Utils.__gates = null;
                }
                Utils.__FSDataDirectory = result.FSDataDirectory;
            }
            
            return result;
        }

        public void Save(XmlTextWriter xml)
        {
            xml.WriteElementString("fsVersion", FSVersion.ToString());
            xml.WriteElementString("interval", Interval.ToString());
        }

        public int Interval
        {
            get;
            set;
        }

        public FSVersion FSVersion
        {
            get;
            set;
        }
        
        public string FSDataDirectory
        {
            get;
            set;
        }
    }
}