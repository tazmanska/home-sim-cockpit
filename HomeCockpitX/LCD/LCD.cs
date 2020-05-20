using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpitX.LCD
{
    public class LCD : IComparable
    {
        public LCD()
        {
        }

        public LCD(XmlNode xml)
        {
            LoadFromXml(xml);
        }

        public virtual void LoadFromXml(XmlNode xml)
        {
            ID = xml.Attributes["id"].Value;
            Description = xml.Attributes["description"].Value;
            Rows = Convert.ToByte(xml.Attributes["rows"].Value);
            Columns = Convert.ToByte(xml.Attributes["columns"].Value);
        }

        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("lcd");
            WriteToXml(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        protected virtual void WriteToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("id", ID);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("rows", Rows.ToString());
            xmlWriter.WriteAttributeString("columns", Columns.ToString());
        }

        private byte _rows = 0;

        public byte Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        private byte _columns = 0;

        public byte Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        private string _id = "";

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _description = "";

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ID, Description);
        }

        public virtual void On()
        {
        }

        public virtual void Off()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void WriteCharacter(char character, byte row, byte column)
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void Uninitialize()
        {
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is LCD)
            {
                int result = ID.CompareTo(((LCD)obj).ID);
                if (result == 0)
                {
                    result = Description.CompareTo(((LCD)obj).Description);
                }
                return result;
            }
            throw new ArgumentException();
        }

        #endregion
    }
}
