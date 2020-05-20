/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-11
 * Godzina: 19:54
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of LED7DisplayOutputSet.
    /// </summary>
    class LED7DisplayOutputSet : LED7DisplayOutput, IDevices
    {
        private class DotPos
        {
            public int Position = 0;
            public char Dot = '.';
        }
        public static string FormatText(Align align, Trim trim, Append append, string appendString, int characters, string text, out int start)
        {
            if (text == null || text.Length == 0)
            {
                text = "";
            }
                        
            int dotsCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (LED7DisplaysDictionary.Instance.IsDotChar(text[i]))
                {
                    dotsCount++;
                }
            }
            
            int textLength = text.Length - dotsCount;

            if (append != Append.None)
            {
                while (appendString.Length < characters)
                {
                    appendString += appendString;
                }
            }

            start = 0;

            if (textLength > characters)
            {
                if (trim == Trim.Right)
                {
                    text = text.Substring(0, characters);
                }
                else
                {
                    text = text.Substring(textLength - characters, characters);
                }
            }
            else
            {
                if (textLength < characters)
                {
                    int dif = characters - textLength;
                    if (append == Append.Left)
                    {
                        text = appendString.Substring(0, dif) + text;
                    }
                    else if (append == Append.Right)
                    {
                        text = text + appendString.Substring(appendString.Length - dif, dif);
                    }
                    else if (align != Align.Left)
                    {
                        if (align == Align.Center)
                        {
                            start = characters / 2 - textLength / 2;
                        }
                        else if (align == Align.Right)
                        {
                            start = characters - textLength;
                        }
                    }
                }
            }
            return text;
        }

        public static string FormatText(LED7DisplayOutputSet area, string text, out int start)
        {
            return FormatText(area.Align, area.Trim, area.Append, area.AppendString, area.Displays.Length, text, out start);
        }
        
        public static LED7DisplayOutputSet Load(LED7DisplayOutput [] displays, XmlNode xml)
        {
            LED7DisplayOutputSet result = new LED7DisplayOutputSet();
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            List<Display> outputs = new List<Display>();
            XmlNodeList nodes = xml.SelectNodes("display");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    string id = node.Attributes["id"].Value;
                    int index = int.Parse(node.Attributes["index"].Value);
                    LED7DisplayOutput dio = Array.Find<LED7DisplayOutput>(displays, delegate(LED7DisplayOutput o)
                                                                          {
                                                                              return o.ID == id;
                                                                          });
                    if (dio == null)
                    {
                        throw new Exception("Brak zdefiniowanego wyświetlacza '" + id + "'.");
                    }
                    
                    outputs.Add(new Display()
                                {
                                    LED7Display = dio,
                                    Index = index
                                });
                }
            }
            result.Displays = outputs.ToArray();
            
            result.Align = (Align)Enum.Parse(typeof(Align), xml.Attributes["align"].Value);
            result.Trim = (Trim)Enum.Parse(typeof(Trim), xml.Attributes["trim"].Value);
            result.Append = (Append)Enum.Parse(typeof(Append), xml.Attributes["append"].Value);
            result.AppendString = xml.Attributes["appendString"].Value;
            if (result.Append != Append.None && result.AppendString.Length == 0)
            {
                throw new Exception("Ustawiono dopełnianie tekstu ale nie ustawiono łańcucha dopełniającego w grupie wyświetlaczy '" + result.ID + "'.");
            }
            
            return result;
        }
        
        public override void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("displaySet");
            xml.WriteAttributeString("id", ID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("align", Align.ToString());
            xml.WriteAttributeString("trim", Trim.ToString());
            xml.WriteAttributeString("append", Append.ToString());
            xml.WriteAttributeString("appendString", AppendString);
            Array.Sort(Displays);
            for (int i = 0; i < Displays.Length; i++)
            {
                Display d = Displays[i];
                d.Index = i;
                xml.WriteStartElement("display");
                xml.WriteAttributeString("id", d.LED7Display.ID);
                xml.WriteAttributeString("index", d.Index.ToString());
                xml.WriteEndElement();
            }
            
            xml.WriteEndElement();
        }
        
        public override string ToString()
        {
            return string.Format("Grupa wyświetlaczy 7-LED, ID: {0}, Opis: {1}", ID, Description);
        }
        
        public LED7DisplayOutputSet()
        {
            Type = HomeSimCockpitSDK.VariableType.String;
        }
        
        public Align Align
        {
            get;
            set;
        }

        public Trim Trim
        {
            get;
            set;
        }

        public Append Append
        {
            get;
            set;
        }
        
        public string AppendString
        {
            get;
            set;
        }
        
        public Display[] Displays
        {
            get;
            set;
        }
        
        public Device[] MainDevices
        {
            get
            {
                List<Device> result = new List<Device>();
                foreach (Display dio in Displays)
                {
                    if (!result.Contains(dio.LED7Display.Device.MainDevice))
                    {
                        result.Add(dio.LED7Display.Device.MainDevice);
                    }
                }
                return result.ToArray();
            }
        }
        
        public string OutsIDs()
        {
            if (Displays != null && Displays.Length > 0)
            {
                string [] ss = new string[Displays.Length];
                for (int i = 0; i < Displays.Length; i++)
                {
                    ss[i] = Displays[i].LED7Display.ID;
                }
                Array.Sort(ss);
                return string.Join(", ", ss);
            }
            return "";
        }
        
        public override void SetValue(object value)
        {
            // formatowanie tekstu
            int start = 0;
            byte [] data = LED7DisplaysDictionary.Instance.CodeString(FormatText(this, (string)value, out start));
            //byte [] indexes = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                Displays[start + i].LED7Display.SetValue(data[i]);
                //indexes[i] = (byte)Displays[start + i].Index;
            }
            //Device.WriteTo7LEDDisplays(indexes, data, data.Length > Displays.Length ? Displays.Length : data.Length);
        }
        
        public override void Reset()
        {
            foreach (Display dio in Displays)
            {
                dio.LED7Display.Reset();
            }
            // sortowanie wyświetlaczy
            Array.Sort(Displays);
        }
    }
}
