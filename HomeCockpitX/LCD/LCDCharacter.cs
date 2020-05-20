using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpitX.LCD
{
    
    public class LCDCharacter : IComparable
    {
        public LCDCharacter()
        {
        }

        public LCDCharacter(LCD lcd, byte row, byte column, int order)
        {
            LCD = lcd;
            Row = row;
            Column = column;
            Order = order;
        }

        public LCDCharacter(XmlNode xml, ILCDsCollection lcds)
        {
            LoadFromXml(xml, lcds);
        }

        public virtual void LoadFromXml(XmlNode xml, ILCDsCollection lcds)
        {
            LCD = lcds.GetLCD(xml.Attributes["lcd"].Value);
            Row = Convert.ToByte(xml.Attributes["row"].Value);
            Column = Convert.ToByte(xml.Attributes["column"].Value);
            Order = Convert.ToInt32(xml.Attributes["order"].Value);
        }

        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("character");
            WriteToXml(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        protected virtual void WriteToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("lcd", LCD.ID);
            xmlWriter.WriteAttributeString("row", Row.ToString());
            xmlWriter.WriteAttributeString("column", Column.ToString());
            xmlWriter.WriteAttributeString("order", Order.ToString());
        }

        private LCD _lcd = null;

        public LCD LCD
        {
            get { return _lcd; }
            protected set { _lcd = value; }
        }

        private byte _row = 0;

        public byte Row
        {
            get { return _row; }
            protected set { _row = value; }
        }

        private byte _column = 0;

        public byte Column
        {
            get { return _column; }
            protected set { _column = value; }
        }

        private int _order = 0;

        public int Order
        {
            get { return _order; }
            protected set { _order = value; }
        }
        
        public virtual void WriteCharacter(char character)
        {
            LCD.WriteCharacter(character, Row, Column);
        }

        public override string ToString()
        {
            return string.Format("LCD: {0}, Row: {1}, Column: {2}, Order: {3}", LCD, Row, Column, Order);
        }

        internal void SetOrder(int order)
        {
            Order = order;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is LCDCharacter)
            {
                return Order.CompareTo(((LCDCharacter)obj).Order);
            }
            throw new ArgumentException();
        }

        #endregion
    }
}
