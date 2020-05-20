using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace FSData
{
    internal class VariableComparerForSort<T> : IComparer<T> where T : Variable
    {
        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return x.ID.CompareTo(y.ID);
        }

        #endregion
    }

    abstract class Variable : HomeSimCockpitSDK.IVariable
    {
        public void WriteToXml(XmlTextWriter xml)
        {
            xml.WriteStartElement("variable");
            xml.WriteAttributeString("id", ID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("type", Type.ToString());
            xml.WriteAttributeString("fsOffset", "0x" + Offset.ToString("X8"));
            xml.WriteAttributeString("fsType", FSType.ToString());
            xml.WriteAttributeString("fsSize", Size.ToString());
            xml.WriteAttributeString("change", Change.ToString().Replace(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, "."));
            xml.WriteEndElement();
        }

        public int Offset
        {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }

        public virtual FSDataType FSType
        {
            get;
            set;
        }

        public virtual double Change
        {
            get;
            set;
        }

        public int Token = 0;        

        public abstract void Reset();

        #region IVariable Members

        public string ID
        {
            get;
            set;
        }

        public HomeSimCockpitSDK.VariableType Type
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Variable)
            {
                return Offset == ((Variable)obj).Offset;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
