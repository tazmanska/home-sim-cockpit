/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-05
 * Godzina: 19:51
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using HomeSimCockpitX.LCD;
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LEDDisplayGroup.
    /// </summary>
    class LEDDisplayGroup : IOutputVariable, IComparable<LEDDisplayGroup>
    {
        public LEDDisplayGroup()
        {
        }
        
        public static LEDDisplayGroup Load(XmlNode xml, List<LEDDisplay> ledDisplays)
        {
            LEDDisplayGroup result = new LEDDisplayGroup();
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.Align = (Align)Enum.Parse(typeof(Align), xml.Attributes["align"].Value);
            result.Trim = (Trim)Enum.Parse(typeof(Trim), xml.Attributes["trim"].Value);
            result.Append = (Append)Enum.Parse(typeof(Append), xml.Attributes["append"].Value);
            result.AppendString = xml.Attributes["appendString"].Value;
            if (result.Append != Append.None && result.AppendString.Length == 0)
            {
                throw new Exception("Ustawiono dopełnianie tekstu ale nie ustawiono łańcucha dopełniającego w obszarze wyświetlaczy LED o id = '" + result.ID + "'.");
            }
            XmlNodeList nodes = xml.SelectNodes("ledDisplay");
            List<LEDDisplayInGroup> ledDisplays2 = new List<LEDDisplayInGroup>();
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    string ledDisplayId = node.Attributes["ledDisplay"].Value;
                    byte order = byte.Parse(node.Attributes["order"].Value);
                    LEDDisplay ledDisplay = ledDisplays.Find(delegate(LEDDisplay o)
                                                             {
                                                                 return o.ID == ledDisplayId;
                                                             });
                    if (ledDisplays == null)
                    {
                        return null;
                    }
                    ledDisplays2.Add(new LEDDisplayInGroup(ledDisplay, order));
                }
            }
            ledDisplays2.Sort();
            result.LEDDisplaysInGroup = ledDisplays2.ToArray();
            if (result.LEDDisplaysInGroup.Length == 0)
            {
                return null;
            }
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ledDisplays");
            xmlWriter.WriteAttributeString("id", ID);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("align", Align.ToString());
            xmlWriter.WriteAttributeString("trim", Trim.ToString());
            xmlWriter.WriteAttributeString("append", Append.ToString());
            xmlWriter.WriteAttributeString("appendString", AppendString);
            foreach (LEDDisplayInGroup led in LEDDisplaysInGroup)
            {
                xmlWriter.WriteStartElement("ledDisplay");
                xmlWriter.WriteAttributeString("ledDisplay", led.LEDDisplay.ID);
                xmlWriter.WriteAttributeString("order", led.Order.ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }
        
        public LEDDisplayInGroup[] LEDDisplaysInGroup
        {
            get;
            internal set;
        }
        
        public string ID
        {
            get;
            internal set;
        }
        
        public string Description
        {
            get;
            set;
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
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get
            {
                return HomeSimCockpitSDK.VariableType.String;
            }
        }
        
        public void Initialize()
        {
            foreach (LEDDisplay display in LEDDisplays)
            {
                display.Initialize();
            }
        }
        
        public void Uninitialize()
        {
            
        }
        
        public void SetValue(object value)
        {
            // formatowanie tekstu
            int start = 0;
            byte [] data = Dictionary.CodeString(FormatText(this, (string)value, out start));
            for (int i = 0; i < data.Length; i++)
            {
                LEDDisplaysInGroup[start + i].LEDDisplay.SetValue(data[i]);
            }
        }
        
        public Device[] Devices
        {
            get
            {
                List<Device> result = new List<Device>();
                foreach (LEDDisplayInGroup l in LEDDisplaysInGroup)
                {
                    result.Add(l.LEDDisplay.LEDDisplayDevice);
                }
                return result.ToArray();
            }
        }
        
        public LEDDisplaysDictionary Dictionary
        {
            get;
            set;
        }
        
        private class DotPos
        {
            public int Position = 0;
            public char Dot = '.';
        }
        
        public static string FormatText(LEDDisplaysDictionary dictionary, Align align, Trim trim, Append append, string appendString, int characters, string text, out int start)
        {
            if (text == null || text.Length == 0)
            {
                text = "";
            }
            
            int dotsCount = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (dictionary.IsDotChar(text[i]))
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

        public static string FormatText(LEDDisplayGroup area, string text, out int start)
        {
            return FormatText(area.Dictionary, area.Align, area.Trim, area.Append, area.AppendString, area.LEDDisplaysInGroup.Length, text, out start);
        }
        
        public string LEDDisplaysIDs
        {
            get
            {
                List<string> result = new List<string>();
                for (int i = 0; i < LEDDisplaysInGroup.Length; i++)
                {
                    result.Add(string.Format("{0} ({1})", LEDDisplaysInGroup[i].LEDDisplay.ID, LEDDisplaysInGroup[i].LEDDisplay.Description));
                }                
                return string.Join(", ", result.ToArray());
            }
        }
        
        public LEDDisplay[] LEDDisplays
        {
            get
            {
                List<LEDDisplay> result = new List<LEDDisplay>();
                Array.Sort(LEDDisplaysInGroup);
                for (int i = 0; i < LEDDisplaysInGroup.Length; i++)
                {
                    result.Add(LEDDisplaysInGroup[i].LEDDisplay);
                }                
                return result.ToArray();
            }
            set
            {
                List<LEDDisplayInGroup> displays = new List<LEDDisplayInGroup>();
                for (int i = 0; i < value.Length; i++)
                {
                    displays.Add(new LEDDisplayInGroup(value[i], (byte)i));
                }
                LEDDisplaysInGroup = displays.ToArray();
            }
        }
        
        public int CompareTo(LEDDisplayGroup other)
        {
            return ID.CompareTo(other.ID);
        }
    }
}
