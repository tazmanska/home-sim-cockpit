using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FSData
{
	static class Utils
	{
		internal static readonly HomeSimCockpitSDK.ModuleFunctionInfo[] __inputFunctions = new HomeSimCockpitSDK.ModuleFunctionInfo[]
		{
			new HomeSimCockpitSDK.ModuleFunctionInfo("BCDFrequencyToDouble", "Konwertuje częstotliwość zakodowaną w BCD na częstotliwość zapisaną jako wartość double, np. liczba w BCD 0x1225 zostanie przekonwertowana na liczbę typu double 112.25 (jedynka z przodu jest dodawana automatycznie).", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(BCDFrequencyToDouble))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("HexToInt", "Konwertuje liczbę zapisaną w formacie szesnastkowym (hex) na liczbę int, np. 0x1234 przekonwertuje na 1234 a nie na 4660.", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(HexToInt))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("ADFExtendedToDouble", "Konwertuje rozszerzoną część częstotliwości ADF na liczbę typu double, np. liczba 0x0105 zostanie przekonwertowana na liczbę 1000.5.", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(ADFExtendedToDouble))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("GetGatesForAirport", "Zwraca listę miejsc postojowych (Gate, Parking itp.) dla wskazanego lotniska, przykład: GetGatesForAirport(\"EPWA\").", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(GetGatesForAirport))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("GetOffsetValue", "Zwraca wartość ofsetu.", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(GetOffsetValue))
		};

		internal static readonly HomeSimCockpitSDK.ModuleFunctionInfo[] __outputFunctions = new HomeSimCockpitSDK.ModuleFunctionInfo[]
		{
			new HomeSimCockpitSDK.ModuleFunctionInfo("DoubleToBCDFrequency", "Konwertuje częstotliwość z liczby double na zakodowaną w BCD, np. częstotliwość 123.45 zostanie przekonwertowana na liczbę 0x2345 (jedynka z przodu jest domyślna).", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(DoubleToBCDFrequency))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("IntToHex", "Konwertuje liczbę int bezpośrednio na liczbę szesnastkową (hex), np. liczbę 1234 przekonwertuje na liczbę 0x1234 a nie na 0x04D2.", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(IntToHex))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("DoubleToADFExtended", "Konwertuje rozszerzoną częstotliwość ADF zapisanej jako liczbą typu double na format rozszerzony, np. częstotliwość 1234.5 zostanie przekonwertowana na 0x0105.", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(DoubleToADFExtended))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("MovePlaneToGateID", "Ustawia samolot we wskazanym miejscu (identyfikator miejsca), przykład: MovePlaneToGateID( 10 ). Identyfiaktor pochodzi z wyniku działania funkcji GetGatesForAirport(...).", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(MovePlaneToGateID))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("SwapADF", "Zamienia częstotliwość ADF1<>ADF2.", 0, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SwapADF))
				,   new HomeSimCockpitSDK.ModuleFunctionInfo("ChangeADF", "Modyfikuje częstotliwość ADF.", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(ChangeADF))
		};

		private static string ___decimalSeparator = null;// System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

		private static string __decimalSeparator
		{
			get
			{
				if (___decimalSeparator == null)
				{
					___decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
				}
				return ___decimalSeparator;
			}
		}
		
		private static object BCDFrequencyToDouble(object[] arguments)
		{
			return BCDFrequencyToDouble((int)arguments[0]);
		}

		private static double BCDFrequencyToDouble(int value)
		{
			string t = value.ToString("X4");
			t = "1" + t.Substring(0, 2) + "." + t.Substring(2, 2);
			return double.Parse(t, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
		}

		private static object DoubleToBCDFrequency(object[] arguments)
		{
			return DoubleToBCDFrequency((double)arguments[0]);
		}

		private static int DoubleToBCDFrequency(double value)
		{
			int result = 0;
			string s = value.ToString("000.00").Replace(__decimalSeparator, "");
			result = int.Parse(s.Substring(1, 4), System.Globalization.NumberStyles.HexNumber);
			return result;
		}

		private static object HexToInt(object[] arguments)
		{
			return HexToInt((int)arguments[0]);
		}

		private static int HexToInt(int value)
		{
			return int.Parse(value.ToString("X"));
		}

		private static object IntToHex(object[] arguments)
		{
			return IntToHex((int)arguments[0]);
		}

		private static int IntToHex(int value)
		{
			return int.Parse(value.ToString(), System.Globalization.NumberStyles.HexNumber);
		}

		private static object ADFExtendedToDouble(object[] arguments)
		{
			return ADFExtendedToDouble((int)arguments[0]);
		}

		private static double ADFExtendedToDouble(int value)
		{
			string t = value.ToString("X4");
			return double.Parse(t.Substring(0, 2)) * 1000d + double.Parse(t.Substring(3, 1)) / 10d;
		}

		private static object DoubleToADFExtended(object[] arguments)
		{
			return DoubleToADFExtended((double)arguments[0]);
		}

		private static int DoubleToADFExtended(double value)
		{
			string tmp = value.ToString("0000.0");
			return int.Parse(string.Format("0{0}0{1}", tmp[0], tmp[5]), System.Globalization.NumberStyles.HexNumber);
		}
		
		private static object[] GetGatesForAirport(object[] arguments)
		{
			return _GetGatesForAirport((string)arguments[0]);
		}
		
		internal enum GateType
		{
			none = 0,
			Ramp_GA = 1,
			Ramp_GA_Small = 2,
			Ramp_GA_Medium = 3,
			Ramp_GA_Large = 4,
			Ramp_Cargo = 5,
			Ramp_Military_Cargo = 6,
			Ramp_Military_Combat = 7,
			Gate_Small = 8,
			Gate_Medium = 9,
			Gate_Large = 10,
			Dock_GA = 11
		}
		
		/// <summary>
		/// ICAO, GateName*, GateNumber, Latitude, Longitude, Radius*, HeadingTrue, GateType*, AirlineCodeList ...
		/// </summary>
		internal class Gate
		{
			public string ICAO = null;
			public string GateName = null;
			public int GateNumber = 0;
			public double Latitude = 0d;
			public double Longitude = 0d;
			public double Radius = 0d;
			public double HeadingTrue = 0d;
			public GateType GateType = GateType.none;
			public int ID = 0;
			
			public override string ToString()
			{
				return string.Format("{0} {1} {2}", GateName, GateNumber, GateType == GateType.none ? "" : "(" + GateType.ToString().Replace("_", " ") + ")");
			}
		}
		
		internal static List<Gate> __gates = null;
		
		internal static string __FSDataDirectory = null;
		
		private static void LoadGates()
		{
			if (__gates == null)
			{
				__gates = new List<Gate>();
				if (Directory.Exists(__FSDataDirectory))
				{
					string path = Path.Combine(__FSDataDirectory, "g5.csv");
					if (File.Exists(path))
					{
						try
						{
							string [] lines = File.ReadAllLines(path);
							for (int i = 0; i < lines.Length; i++)
							{
								string [] tmp = lines[i].Split(new char[] { ',' });
								if (tmp.Length > 7)
								{
									Gate gate = new Gate();
									gate.ICAO = tmp[0];
									gate.GateName = tmp[1];
									if (gate.GateName == null || gate.GateName == "" || gate.GateName.ToLowerInvariant() == "g")
									{
										gate.GateName = "Gate";
									}
									gate.GateNumber = int.Parse(tmp[2]);
									gate.Latitude = double.Parse(tmp[3], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
									gate.Longitude = double.Parse(tmp[4], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
									gate.Radius = double.Parse(tmp[5], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
									gate.HeadingTrue = double.Parse(tmp[6], NumberStyles.Float, NumberFormatInfo.InvariantInfo);
									gate.GateType = (GateType)int.Parse(tmp[7]);
									__gates.Add(gate);
									gate.ID = __gates.Count;
								}
							}
						}
						catch{}
					}
				}
				__gates.Sort(delegate(Gate left, Gate right)
				             {
				             	int result = left.ICAO.CompareTo(right.ICAO);
				             	if (result == 0)
				             	{
				             		result = left.GateName.CompareTo(right.GateName);
				             		if (result == 0)
				             		{
				             			result = left.GateNumber.CompareTo(right.GateNumber);
				             		}
				             	}
				             	return result;
				             });
			}
		}
		
		private static string[] _GetGatesForAirport(string icao)
		{
			List<string> result = new List<string>();
			LoadGates();
			if (icao == null)
			{
				icao = "";
			}
			icao = icao.ToLowerInvariant().Trim();
			List<Gate> found = __gates.FindAll(delegate(Gate o)
			                                   {
			                                   	return o.ICAO.ToLowerInvariant() == icao;
			                                   });
			for (int i = 0; i < found.Count; i++)
			{
				result.Add(found[i].ToString());
				result.Add(found[i].ID.ToString());
			}
			return result.ToArray();
		}
		
		private static object MovePlaneToGateID(object[] arguments)
		{
			if (arguments == null || arguments.Length == 0)
			{
				throw new Exception("Nie wskazano identyfikatora.");
			}
			int id = Convert.ToInt32(arguments[0]);
			return _MovePlaneToGateID(id);
		}
		
		private static bool _MovePlaneToGateID(int id)
		{
			LoadGates();
			Gate gate = __gates.Find(delegate(Gate o)
			                         {
			                         	return o.ID == id;
			                         });
			if (gate != null)
			{
				// ustawienie samolotu
				
				// połączenie z FSUIPC itp.
				try
				{
					FsuipcSdk.Fsuipc fsuipc = new FsuipcSdk.Fsuipc();
					int result = 0;
					if (fsuipc.FSUIPC_Open(FsuipcSdk.Fsuipc.SIM_ANY, ref result))
					{
						// włączenie SLEW MODE
						int token = 0;
						fsuipc.FSUIPC_Write(0x05dc, (short)1, ref token, ref result);
						fsuipc.FSUIPC_Process(ref result);
						
						// przeniesienie samolotu na wybraną pozycję
						double off = 90d / (10001750d * 65536d * 65536d);// 42957189152768000d;
						long lat = (long)(gate.Latitude / off);
						off = 360d / (65536d * 65536d * 65536d * 65536d);
						long lon = (long)(gate.Longitude / off);
						fsuipc.FSUIPC_Write(0x0560, lat, ref token, ref result);
						fsuipc.FSUIPC_Write(0x0568, lon, ref token, ref result);
						fsuipc.FSUIPC_Write(0x0580, (uint)(gate.HeadingTrue / (360d / (65536d * 65536d))), ref token, ref result);
						fsuipc.FSUIPC_Write(0x0570, long.MinValue, ref token, ref result);
						fsuipc.FSUIPC_Process(ref result);
						
						// wyłączenie SLEW MODE
						fsuipc.FSUIPC_Write(0x05dc, (short)0, ref token, ref result);
						fsuipc.FSUIPC_Process(ref result);
						
						fsuipc.FSUIPC_Close();
						
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		public static object SwapADF(object[] arguments)
		{
			try
			{
				FsuipcSdk.Fsuipc fsuipc = new FsuipcSdk.Fsuipc();
				int result = 0;
				if (fsuipc.FSUIPC_Open(FsuipcSdk.Fsuipc.SIM_ANY, ref result))
				{
					// odczytanie ustawień ADF1
					int adf1btoken = 0;
					fsuipc.FSUIPC_Read(0x034C, 2, ref adf1btoken, ref result);
					int adf1etoken = 0;
					fsuipc.FSUIPC_Read(0x0356, 2, ref adf1etoken, ref result);
					
					// odczytanie ustaiwń ADF2
					int adf2btoken = 0;
					fsuipc.FSUIPC_Read(0x02D4, 2, ref adf2btoken, ref result);
					int adf2etoken = 0;
					fsuipc.FSUIPC_Read(0x02D6, 2, ref adf2etoken, ref result);
					
					fsuipc.FSUIPC_Process(ref result);
					
					// pobranie wartości
					short adf1b = 0;
					fsuipc.FSUIPC_Get(ref adf1btoken, ref adf1b);
					short adf1e = 0;
					fsuipc.FSUIPC_Get(ref adf1etoken, ref adf1e);
					short adf2b = 0;
					fsuipc.FSUIPC_Get(ref adf2btoken, ref adf2b);
					short adf2e = 0;
					fsuipc.FSUIPC_Get(ref adf2etoken, ref adf2e);
					
					// zamiana wartości
					short tmp = 0;
					tmp = adf1b;
					adf1b = adf2b;
					adf2b = tmp;
					tmp = adf1e;
					adf1e = adf2e;
					adf2e = tmp;
					
					// zapisanie
					fsuipc.FSUIPC_Write(0x034C, adf1b, ref adf1btoken, ref result);
					fsuipc.FSUIPC_Write(0x0356, adf1e, ref adf1etoken, ref result);
					fsuipc.FSUIPC_Write(0x02D4, adf2b, ref adf2btoken, ref result);
					fsuipc.FSUIPC_Write(0x02D6, adf2e, ref adf2etoken, ref result);
					
					fsuipc.FSUIPC_Process(ref result);
					
					fsuipc.FSUIPC_Close();
				}
			}
			catch
			{
			}
			return true;
		}
		
		public static object GetOffsetValue(object [] arguments)
		{
			object result = (int)0;
			
			int offset = (int)arguments[0];
			int size = (int)arguments[1];
			int type = 0;
			if (arguments.Length > 2)
			{
				type = (int)arguments[2];
			}
			
			try
			{
				FsuipcSdk.Fsuipc fsuipc = new FsuipcSdk.Fsuipc();
				int dwresult = 0;
				if (fsuipc.FSUIPC_Open(FsuipcSdk.Fsuipc.SIM_ANY, ref dwresult))
				{
					int token = 0;
					if (fsuipc.FSUIPC_Read(offset, size, ref token, ref dwresult))
					{
						if (fsuipc.FSUIPC_Process(ref dwresult))
						{
							switch (type)
							{
									// liczba
								case 0:
									
									switch (size)
									{
										case 1:
											byte bv = 0;
											fsuipc.FSUIPC_Get(ref token, ref bv);
											return (int)bv;
											
										case 2:
											short sv = 0;
											fsuipc.FSUIPC_Get(ref token, ref sv);
											return (int)sv;
									}
									break;
							}
						}
					}
					
					fsuipc.FSUIPC_Close();
				}
			}
			catch
			{
			}
			
			return result;
		}
		
		public static object ChangeADF(object [] arguments)
		{
			int adfOffset = (int)arguments[0] < 1 ? 0x034C : 0x02D4;
			int change = (int)arguments[1];
			try
			{
				FsuipcSdk.Fsuipc fsuipc = new FsuipcSdk.Fsuipc();
				int result = 0;
				if (fsuipc.FSUIPC_Open(FsuipcSdk.Fsuipc.SIM_ANY, ref result))
				{
					// odczytanie ustawień ADF
					int adfbtoken = 0;
					fsuipc.FSUIPC_Read(adfOffset, 2, ref adfbtoken, ref result);
					
					fsuipc.FSUIPC_Process(ref result);
					
					// pobranie wartości
					short adfb = 0;
					fsuipc.FSUIPC_Get(ref adfbtoken, ref adfb);
					
					int iv = HexToInt((int)adfb);
					iv += change;
					if (iv < 100)
					{
						iv += 700;
					}
					if (iv > 799)
					{
						iv -= 700;
					}
					adfb = (short)IntToHex(iv);
					
					// zapisanie
					fsuipc.FSUIPC_Write(adfOffset, adfb, ref adfbtoken, ref result);
					
					fsuipc.FSUIPC_Process(ref result);
					
					fsuipc.FSUIPC_Close();
				}
			}
			catch
			{
			}
			return true;
		}
	}
}