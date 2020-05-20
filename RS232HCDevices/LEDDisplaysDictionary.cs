/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-05
 * Godzina: 20:36
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LEDDisplaysDictionary.
    /// </summary>
    class LEDDisplaysDictionary
    {
		public LEDDisplaysDictionary()
		{
		}
				
		private byte _dot = 128;
		
		private Dictionary<char, byte> _charDictionary = new Dictionary<char, byte>();
		
		/// <summary>
		/// Domyślna wartość dla znaków niewystępujących w słowniku.
		/// </summary>
		private byte _default = 0;
		
		/// <summary>
		/// Lista znaków zamienianych na kropkę.
		/// </summary>
		private List<char> _charsForDots = new List<char>();
		
		public static LEDDisplaysDictionary Load(XmlNode xml)
		{
			LEDDisplaysDictionary result = new LEDDisplaysDictionary();
			result._default = (byte)(int.Parse(xml.Attributes["default"].Value) & 0xff);
			result._dot = (byte)(int.Parse(xml.Attributes["dot"].Value) & 0xff);
			XmlNodeList nodes = xml.SelectNodes("dotChars/char");
			foreach (XmlNode node in nodes)
			{
				char c = node.Attributes["chr"].Value[0];
				if (!result._charsForDots.Contains(c))
				{
					result._charsForDots.Add(c);
				}
			}
			nodes = xml.SelectNodes("char");
			foreach (XmlNode node in nodes)
			{
				byte v = (byte)(int.Parse(node.Attributes["value"].Value) & 0xff);
				char c = node.Attributes["chr"].Value[0];
				if (!result._charDictionary.ContainsKey(c))
				{
					result._charDictionary.Add(c, v);
				}
			}
			return result;
		}
		
		public void Save(XmlWriter xml)
		{
		    xml.WriteStartElement("ledDisplaysDictionary");
		    xml.WriteAttributeString("default", _default.ToString());
		    xml.WriteAttributeString("dot", _dot.ToString());
		    xml.WriteStartElement("dotChars");
		    for (int i = 0; i < _charsForDots.Count; i++)
		    {
		        xml.WriteStartElement("char");
		        xml.WriteAttributeString("chr", _charsForDots[i].ToString());
		        xml.WriteEndElement();
		    }
		    xml.WriteEndElement();
		    foreach (KeyValuePair<char, byte> kvp in _charDictionary)
		    {
		        xml.WriteStartElement("char");
		        xml.WriteAttributeString("chr", kvp.Key.ToString());
		        xml.WriteAttributeString("value", kvp.Value.ToString());
		        xml.WriteEndElement();
		    }
		    xml.WriteEndElement();
		}
		
		public bool IsDotChar(char c)
		{
		    return _charsForDots.Contains(c);
		}
		
		public byte[] CodeString(string text)
		{			
		    if (text == null || text.Length == 0)
			{
		        return new byte[] { 0 };
			}
			List<byte> result = new List<byte>();			
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (_charsForDots.Contains(c))
				{
					// kropka
					if (result.Count == 0)
					{
						result.Add(_dot);
						continue;
					}
					result[result.Count - 1] |= _dot;					
					continue;
				}
				byte b = 0;
				if (_charDictionary.TryGetValue(c, out b))
				{
					result.Add(b);
					continue;
				}
				result.Add(_default);
			}
			return result.ToArray();
		}
		
		public byte this[char c]
		{
			get
			{
				return 0;
			}
		}
    }
}
