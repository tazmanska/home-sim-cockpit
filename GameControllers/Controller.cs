using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameControllers
{
    class Controller
    {
        public string ToString2()
        {
            return string.Format("{0}.{1}.{2}", Name, Index, Id);
        }
        
        public string Name
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public Guid Id
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public InputVariable[] Variables
        {
            get;
            set;
        }

        public UpdateStateType UpdateType
        {
            get;
            set;
        }

        public int ReadingStateInterval
        {
            get;
            set;
        }

        public void WriteToXml(XmlTextWriter xml)
        {
            xml.WriteStartElement("controller");
            xml.WriteAttributeString("name", Name);
            xml.WriteAttributeString("index", Index.ToString());
            xml.WriteAttributeString("id", Id.ToString());
            xml.WriteAttributeString("alias", Alias);
            xml.WriteAttributeString("update", UpdateType.ToString());
            xml.WriteAttributeString("updateInterval", ReadingStateInterval.ToString());
            xml.WriteStartElement("buttons");
            if (Variables != null)
            {
                // posortowanie zmiennych
                Array.Sort(Variables);
                foreach (InputVariable iv in Variables)
                {
                    if (iv.InputType == InputType.Button || iv.InputType == InputType.HATSwitch)
                    {
                        iv.WriteToXml(xml);
                    }
                }
            }
            xml.WriteEndElement();
            xml.WriteStartElement("axes");
            if (Variables != null)
            {
                foreach (InputVariable iv in Variables)
                {
                    if (iv.InputType == InputType.Axis)
                    {
                        iv.WriteToXml(xml);
                    }
                }
            }
            xml.WriteEndElement();
            xml.WriteEndElement();
        }

        public override bool Equals(object obj)
        {
            if (obj is Controller)
            {
                Controller c = (Controller)obj;
                if (c.Id == Id)
                {
                    return true;
                }
                return c.Name == Name && c.Index == Index;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int NumberOfObjects(InputType type)
        {
            int result = 0;
            if (Variables != null)
            {
                for (int i = 0; i < Variables.Length; i++)
                {
                    if (Variables[i].InputType == type)
                    {
                        result++;
                    }
                }
            }
            return result;
        }
    }
}
