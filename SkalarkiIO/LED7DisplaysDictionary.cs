/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-14
 * Godzina: 22:00
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of LED7DisplaysDictionary.
	/// </summary>
	class LED7DisplaysDictionary
	{
		private LED7DisplaysDictionary()
		{
		}
		
		private static LED7DisplaysDictionary __instance = null;
		
		public static LED7DisplaysDictionary Instance
		{
			get { return __instance; }
		}
		
		private  byte _dot = 128;
		
		private Dictionary<char, byte> _charDictionary = new Dictionary<char, byte>();
		
		/// <summary>
		/// Domyślna wartość dla znaków niewystępujących w słowniku.
		/// </summary>
		private byte _default = 0;
		
		/// <summary>
		/// Lista znaków zamienianych na kropkę.
		/// </summary>
		private List<char> _charsForDots = new List<char>();
		
		public static LED7DisplaysDictionary Load(XmlNode xml)
		{
			__instance = new LED7DisplaysDictionary();
			__instance._default = (byte)(int.Parse(xml.Attributes["default"].Value) & 0xff);
			__instance._dot = (byte)(int.Parse(xml.Attributes["dot"].Value) & 0xff);
			XmlNodeList nodes = xml.SelectNodes("dotChars/char");
			foreach (XmlNode node in nodes)
			{
				char c = node.Attributes["chr"].Value[0];
				if (!__instance._charsForDots.Contains(c))
				{
					__instance._charsForDots.Add(c);
				}
			}
			nodes = xml.SelectNodes("char");
			foreach (XmlNode node in nodes)
			{
				byte v = (byte)(int.Parse(node.Attributes["value"].Value) & 0xff);
				char c = node.Attributes["chr"].Value[0];
				if (!__instance._charDictionary.ContainsKey(c))
				{
					__instance._charDictionary.Add(c, v);
				}
			}
			return __instance;
		}
		
		public void Save(XmlWriter xml)
		{
		    xml.WriteStartElement("displays7LEDDictionary");
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
