/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-04-09
 * Godzina: 08:29
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Xml;

namespace RS232HCDevices.Servos
{
	/// <summary>
	/// Description of Servo.
	/// </summary>
	class Servo
	{
		public Servo()
		{
			Min = 500;
			Max = 4100;
			InitialPosition = 2300;
		}
		
		public static Servo Load(XmlNode xml)
		{
			if (xml == null)
			{
				return null;
			}
			
			Servo result = new Servo();
			result.Id = xml.Attributes["id"].Value;
			result.Description = xml.Attributes["description"].Value;
			result.Index = byte.Parse(xml.Attributes["index"].Value);
			result.Min = int.Parse(xml.Attributes["min"].Value);
			result.Max = int.Parse(xml.Attributes["max"].Value);
			result.InitialPosition = int.Parse(xml.Attributes["initialPosition"].Value);
			return result;
		}
		
		public void SaveToXml(XmlTextWriter xml)
		{
			xml.WriteAttributeString("id", Id);
			xml.WriteAttributeString("description", Description);
			xml.WriteAttributeString("index", Index.ToString());
			xml.WriteAttributeString("min", Min.ToString());
			xml.WriteAttributeString("max", Max.ToString());
			xml.WriteAttributeString("initialPosition", InitialPosition.ToString());
		}
		
		public ServoDevice Device
		{
			get;
			set;
		}
		
		public string Id
		{
			get;
			set;
		}
		
		public string Description
		{
			get;
			set;
		}
		
		public byte Index
		{
			get;
			set;
		}
		
		public int Min
		{
			get;
			set;
		}
		
		public int Max
		{
			get;
			set;
		}
		
		public int InitialPosition
		{
			get;
			set;
		}
		
		private int _min = 0;
		private int _max = 0;
		private int _position = -1;
		private double _factor = 1.0d;
		private double _offset = 0d;
		private bool _scaled = false;
		
		public void Reset()
		{
			_position = -1;
			_min = Min;
			_max = Max;
			_factor = 1.0d;
			_scaled = false;
			
			// ustawienie pozycji startowej
			Position = InitialPosition;
		}
		
		public void SetScale(int min, int max)
		{
//			if (min < Min)
//			{
//				throw new Exception("Minimalna wartość nie może być mniejsza od ustawionej w konfiguracji serwa.");
//			}
//			if (max > Max)
//			{
//				throw new Exception("Maksymalna wartość nie może być mniejsza od ustawionej w konfiguracji serwa.");
//			}
			if (min >= max)
			{
				throw new Exception("Podano błędny zakres.");
			}
			_min = min;
			_max = max;
			_position = -1;
			_offset = 0;
			if (_min < 0)
			{
				_offset = -_min;
			}
			else if (_min > 0)
			{
				_offset = -_min;
			}
			
			int range = 0;
			if (_max < 0)
			{
				range = (-_max) - (-_min);
			}
			else				
			{
				range = _max - _min;
			}
			_factor = Math.Abs((double)(Max - Min) / (double)range);
			_scaled = true;
		}
		
		public void SetEnable(bool enable)
		{
			Device.SetEnable(Index, enable);
		}
		
		public int Position
		{
			get { return _position; }
			set
			{
				int newPosition = value;
				if (_position == newPosition)
				{
					return;
				}
				
				_position = newPosition;
				
				// obliczenie pozycji wg. ustawionej skali
				if (_scaled)
				{
					newPosition = (int)((_position + _offset) * _factor) + Min;
				}
				
				if (newPosition < Min)
				{
					newPosition = Min;
				}
				if (newPosition > Max)
				{
					newPosition = Max;
				}
				
				Device.SetPosition(Index, newPosition);
			}
		}
	}
}
