/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-23
 * Godzina: 10:16
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Xml;

namespace Mouse
{
    /// <summary>
    /// Description of Click.
    /// </summary>
    public class Click : HomeSimCockpitSDK.IVariable, IComparable<Click>
    {
        public static Click Read(XmlNode xml)
        {
            Click result = new Click();
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.Button = (MouseButton)Enum.Parse(typeof(MouseButton), xml.Attributes["button"].Value);
            result.X = int.Parse(xml.Attributes["x"].Value);
            result.Y = int.Parse(xml.Attributes["y"].Value);
            return result;
        }
        
        public void Write(XmlWriter xml)
        {
            xml.WriteStartElement("click");
            xml.WriteAttributeString("id", ID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("button", Button.ToString());
            xml.WriteAttributeString("x", X.ToString());
            xml.WriteAttributeString("y", Y.ToString());
            xml.WriteEndElement();
        }
        
        public Click()
        {
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
        
        public MouseButton Button
        {
            get;
            set;
        }
        
        public int X
        {
            get;
            set;
        }
        
        public int Y
        {
            get;
            set;
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.Bool; }
        }
        
        public MouseOutput Mouse
        {
            get;
            set;
        }
        
        private bool _state = false;
        
        public void Reset()
        {
            _state = false;
        }
        
        public void SetValue(object value)
        {
            bool v = (bool)value;
            if (v != _state)
            {
                _state = v;
                Mouse.Click(this);
            }
        }
        
        public int CompareTo(Click other)
        {
            return ID.CompareTo(other.ID);
        }
    }
}
