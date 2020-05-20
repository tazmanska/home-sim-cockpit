/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-01
 * Godzina: 18:22
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of DeviceType.
	/// </summary>
	class DeviceType : IComparable<DeviceType>
	{
		public const int Type_1 = 1;
		public const int Type_2 = 2;
		
		private static DeviceType _type1 = new DeviceType()
		{
			Type = Type_1,
			Name = "Mała płytka (glare)",
			DigitalInputs = 64,
			DigitalOutputs = 48,
			Displays7LED = 24,
			VID = "04d8",
			PID = "0050"
		};
		
		public static DeviceType Type1
		{
			get { return _type1; }
		}
		
		private static DeviceType _type2 = new DeviceType()
		{
		    Type = Type_2,
		    Name = "Duża płytka (pedestal)",
		    DigitalInputs = 128,
		    DigitalOutputs = 128,
		    Displays7LED = 32,
		    VID = "04d8",
		    PID = "0060"
		};
		
		public static DeviceType Type2
		{
		    get { return _type2; }
		}
		
		public static DeviceType Get(int type)
		{
			switch (type)
			{
				case Type_1:
					return Type1;
	            case Type_2:
					return Type2;
				default:
					throw new Exception("Nieznany typ urządzenia.");
			}
		}
		
		public static DeviceType[] GetTypes()
		{
			return new DeviceType[] { Type1 };
		}
		
		private DeviceType()
		{
		}
		
		public string VID
		{
			get;
			private set;
		}
		
		public string PID
		{
			get;
			private set;
		}
		
		public int Type
		{
			get;
			private set;
		}
		
		public string Name
		{
			get;
			private set;
		}
		
		public int DigitalOutputs
		{
			get;
			private set;
		}
		
		public int DigitalInputs
		{
			get;
			private set;
		}
		
		public int Displays7LED
		{
			get;
			private set;
		}
		
		public string Info
		{
			get { return string.Format("In: {0}; Out: {1}; 7-LED: {2}", DigitalInputs, DigitalOutputs, Displays7LED); }
		}
		
		public int CompareTo(DeviceType other)
		{
			return Name.CompareTo(other.Name);
		}
		
		public override string ToString()
		{
			return Name;
		}
	}
}
