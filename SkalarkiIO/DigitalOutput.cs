/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-05
 * Godzina: 16:36
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Diagnostics;
using System.Xml;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of DigitalOutput.
	/// </summary>
	class DigitalOutput : OutputVariable, IComparable<DigitalOutput>
	{
		public static DigitalOutput Load(XmlNode xml)
		{
			DigitalOutput result = new DigitalOutput();
			result.DeviceId = xml.Attributes["deviceId"].Value;
			result.Index = int.Parse(xml.Attributes["index"].Value);
			result.ID = xml.Attributes["id"].Value;
			result.Description = xml.Attributes["description"].Value;
			return result;
		}
		
		public virtual void Save(XmlTextWriter xml)
		{
			xml.WriteStartElement("output");
			xml.WriteAttributeString("deviceId", DeviceId);
			xml.WriteAttributeString("index", Index.ToString());
			xml.WriteAttributeString("id", ID);
			xml.WriteAttributeString("description", Description);
			xml.WriteEndElement();
		}
		
		public override string ToString()
		{
			return string.Format("Wyjście cyfrowe, Urządzenie: {0}, Indeks: {1}, ID: {2}, Opis: {3}", DeviceId, Index, ID, Description);
		}
		
		public DigitalOutput()
		{
			Type = HomeSimCockpitSDK.VariableType.Bool;
		}
		
		public int Index
		{
			get;
			set;
		}
		
		public int ChipAddress
		{
			get;
			set;
		}
		
		public int Bit
		{
			get;
			set;
		}
		
		private bool _state = false;
		
		public override void Reset()
		{
			int chip = 0;
			int bit = 0;
			Device.GetDigitalOutputChipAndBit(Index, out chip, out bit);
			ChipAddress = chip;
			Bit = bit;
			_state = false;
		}
		
		public override void SetValue(object value)
		{
			bool s = (bool)value;
			if (_state != s)
			{
			    Debug.WriteLine("Zmienna '" + ID + "' zmiana wartości na '" + s.ToString() + "'.");
				_state = s;
				Device.WriteBit((byte)ChipAddress, (byte)Bit, _state);
			}
		}
		
		public int CompareTo(DigitalOutput other)
		{
			if (DeviceId == null)
			{
				return Index.CompareTo(other.Index);
			}
			int result = DeviceId.CompareTo(other.DeviceId);
			if (result == 0)
			{
				result = Index.CompareTo(other.Index);
			}
			return result;
		}
	}
}
