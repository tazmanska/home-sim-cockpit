/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-04
 * Godzina: 21:48
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using HomeSimCockpitX.LCD;
using System;
using System.Xml;

namespace LCDOnLPT
{
    /// <summary>
    /// Description of LPTLCDArea.
    /// </summary>
    class LPTLCDArea : HomeSimCockpitX.LCD.LCDArea, IOutputVariable, IComparable<LPTLCDArea>
    {
        
        public LPTLCDArea()
            : base()
        {

        }

        public LPTLCDArea(XmlNode xml, ILCDsCollection lcds)
            : base(xml, lcds)
        {

        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.String; }
        }

        public void Set(string id, string description, Align align, Trim trim, Append append, string appendString)
        {
            ID = id;
            Description = description;
            Align = align;
            Trim = trim;
            Append = append;
            AppendString = appendString;
        }
        
        public int CompareTo(LPTLCDArea other)
        {
            return base.CompareTo(other);
        }
        
        public void SetValue(object value)
        {
            WriteText((string)value);
        }
    }
}
