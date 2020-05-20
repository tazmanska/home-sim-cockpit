/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-19
 * Godzina: 15:18
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Xml;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of Encoder.
    /// </summary>
    class Encoder
    {
		public static Encoder Load(DigitalInput [] digitalnputs, XmlNode xml)
		{
			Encoder result = new Encoder();
			string left = xml.Attributes["leftInput"].Value;
			string right = xml.Attributes["rightInput"].Value;
			for (int i = 0; i < digitalnputs.Length && (result.LeftInput == null || result.RightInput == null); i++)
			{
			    if (digitalnputs[i].ID == left)
			    {
			        result.LeftInput = digitalnputs[i];
			        continue;
			    }
			    
			    if (digitalnputs[i].ID == right)
			    {
			        result.RightInput = digitalnputs[i];
			        continue;
			    }
			}
			
			if (result.LeftInput == null || result.RightInput == null)
			{
			    throw new Exception("Nie istnieją wejścia ('" + left + "' i '" + right + "') do obsługi enkodera.");
			}
			
			return result;
		}
		
		public void Save(XmlTextWriter xml)
		{
			xml.WriteStartElement("encoder");
			xml.WriteAttributeString("leftInput", LeftInput.ID);
			xml.WriteAttributeString("rightInput", RightInput.ID);			
			xml.WriteEndElement();
		}
        
        public Encoder()
        {
        }
        
        public DigitalInput LeftInput
        {
            get;
            set;
        }
        
        public DigitalInput RightInput
        {
            get;
            set;
        }
    }
}
