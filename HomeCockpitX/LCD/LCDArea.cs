using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpitX.LCD
{
    public class LCDArea : IComparable
    {
        public static string FormatText(Align align, Trim trim, Append append, string appendString, int characters, string text, out int start)
        {
            if (text == null || text.Length == 0)
            {
                text = "";
            }

            if (append != Append.None)
            {
                while (appendString.Length < characters)
                {
                    appendString += appendString;
                }
            }

            start = 0;

            if (text.Length > characters)
            {
                if (trim == Trim.Right)
                {
                    text = text.Substring(0, characters);
                }
                else
                {
                    text = text.Substring(text.Length - characters, characters);
                }
            }
            else
            {
                if (text.Length < characters)
                {
                    int dif = characters - text.Length;
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
                            start = characters / 2 - text.Length / 2;
                        }
                        else if (align == Align.Right)
                        {
                            start = characters - text.Length;
                        }
                    }
                }
            }
            return text;
        }

        public static string FormatText(LCDArea area, string text, out int start)
        {
            return FormatText(area.Align, area.Trim, area.Append, area.AppendString, area.Characters.Length, text, out start);
        }

        public LCDArea()
        {
        }

        public LCDArea(XmlNode xml, ILCDsCollection lcds)
        {
            LoadFromXml(xml, lcds);
        }

        public virtual void LoadFromXml(XmlNode xml, ILCDsCollection lcds)
        {
            ID = xml.Attributes["id"].Value;
            Description = xml.Attributes["description"].Value;
            Align = (Align)Enum.Parse(typeof(Align), xml.Attributes["align"].Value);
            Trim = (Trim)Enum.Parse(typeof(Trim), xml.Attributes["trim"].Value);
            Append = (Append)Enum.Parse(typeof(Append), xml.Attributes["append"].Value);
            AppendString = xml.Attributes["appendString"].Value;
            if (Append != Append.None && AppendString.Length == 0)
            {
                throw new Exception("Ustawiono dopełnianie tekstu ale nie ustawiono łańcucha dopełniającego w obszarze LCD o id = '" + ID + "'.");
            }
            XmlNodeList nodes = xml.SelectNodes("character");
            if (nodes != null && nodes.Count > 0)
            {
                List<LCDCharacter> characters = new List<LCDCharacter>();
                foreach (XmlNode node in nodes)
                {
                    characters.Add(new LCDCharacter(node, lcds));
                }
                characters.Sort();
                Characters = characters.ToArray();
            }
            if (Characters == null || Characters.Length == 0)
            {
                throw new Exception("Pusty obszar o nazwie '" + ID + "'.");
            }
        }

        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("area");
            WriteToXml(xmlWriter);
            foreach (LCDCharacter character in Characters)
            {
                character.SaveToXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        protected virtual void WriteToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("id", ID);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("align", Align.ToString());
            xmlWriter.WriteAttributeString("trim", Trim.ToString());
            xmlWriter.WriteAttributeString("append", Append.ToString());
            xmlWriter.WriteAttributeString("appendString", AppendString);
        }

        public string ID
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public LCDCharacter[] Characters
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

        public override string ToString()
        {
            return ID;
        }

        public string Value
        {
            get;
            set;
        }

        public int Length
        {
            get { return Characters == null ? 0 : Characters.Length; }
        }

        public virtual void WriteText(string text)
        {
            int start = 0;
            // formatowanie tekstu
            Value = FormatText(this, text, out start);

            for (int i = 0; i < Value.Length; i++)
            {
                Characters[start + i].WriteCharacter(Value[i]);
            }
        }

        public virtual void ArrangeCharacters()
        {
            if (Characters != null)
            {
                Array.Sort(Characters);
                for (int i = 0; i < Characters.Length; i++)
                {
                    Characters[i].SetOrder(i);
                }
            }
        }
        
        public void Set(LCDCharacter[] characters)
        {
            Characters = characters;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is LCDArea)
            {
                int result = ID.CompareTo(((LCDArea)obj).ID);
                if (result == 0)
                {
                    result = Description.CompareTo(((LCDArea)obj).Description);
                }
                return result;
            }
            throw new ArgumentException();
        }
        
        #endregion
    }
}
