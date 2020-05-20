/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-04
 * Godzina: 19:56
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using HomeSimCockpitX.LCD;

namespace LCDOnLPT
{
    /// <summary>
    /// Description of ModuleConfiguration.
    /// </summary>
    class ModuleConfiguration
    {
        private class LCDSet : ILCDsCollection
        {
            public LCDSet(LCD[] lcds)
            {
                _lcds = lcds;
            }

            private LCD[] _lcds = null;

            #region ILCDsCollection Members

            public LCD GetLCD(string id)
            {
                for (int i = 0; i < _lcds.Length; i++)
                {
                    if (_lcds[i].ID == id)
                    {
                        return _lcds[i];
                    }
                }
                throw new ApplicationException("Nie znalezion wyświetlacza o identyfikatorze '" + id + "'.");
            }

            #endregion
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
            c.LPTAddress = int.Parse(xml.SelectSingleNode("/configuration/lpt").Attributes["address"].Value, System.Globalization.NumberStyles.HexNumber);
            c.LCD1 = new LPTLCD(xml.SelectSingleNode("/configuration/lcds/lcd1/lcd"));
            c.LCD2 = new LPTLCD(xml.SelectSingleNode("/configuration/lcds/lcd2/lcd"));
            XmlNodeList nodes = xml.SelectNodes("/configuration/areas/area");
            if (nodes != null && nodes.Count > 0)
            {
                List<LPTLCDArea> areas = new List<LPTLCDArea>();
                LCDSet s = new LCDSet(new LCD[] { c.LCD1, c.LCD2 });
                foreach (XmlNode area in nodes)
                {
                    LPTLCDArea a = new LPTLCDArea(area, s);
                    areas.Add(a);
                }
                c.Areas = areas.ToArray();
            }
            return c;
        }

        public static void Save(string fileName, ModuleConfiguration configuration)
        {
            using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");

                xml.WriteStartElement("lpt");
                xml.WriteAttributeString("address", configuration.LPTAddress.ToString("X"));
                xml.WriteEndElement();

                
                xml.WriteStartElement("lcds");
                xml.WriteStartElement("lcd1");
                configuration.LCD1.SaveToXml(xml);
                xml.WriteEndElement();
                xml.WriteStartElement("lcd2");
                configuration.LCD2.SaveToXml(xml);
                xml.WriteEndElement();
                xml.WriteEndElement();

                xml.WriteStartElement("areas");
                if (configuration.Areas != null)
                {
                    foreach (LCDArea area in configuration.Areas)
                    {
                        area.SaveToXml(xml);
                    }
                }
                xml.WriteEndElement();


                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        internal ModuleConfiguration()
        {
            LPTAddress = 0x0378;
            Areas = new LPTLCDArea[0];
        }

        public LPTLCD LCD1
        {
            get;
            set;
        }
        
        public LPTLCD LCD2
        {
            get;
            set;
        }

        public LPTLCDArea[] Areas
        {
            get;
            internal set;
        }

        public int LPTAddress
        {
            get;
            internal set;
        }
        
    }
}
