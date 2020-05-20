using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace HomeSimCockpit.Parser
{
	static class FunkcjeWbudowane
	{
		internal static ScriptLogDelegate __log = null;
		
		internal static Main __main = null;

		private static readonly FunkcjaInformacje[] __funkcjeWbudowane = new FunkcjaInformacje[]
		{
			new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(MakeDouble), Nazwa = "MakeDouble", IloscParametrow = 2, Description = "zwraca liczbę double stworzoną z dwóch liczb int" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Round), Nazwa = "Round", IloscParametrow = 1, Description = "zaokrąglenie liczby" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Abs), Nazwa = "Abs", IloscParametrow = 1, Description = "wartość absolutna liczby" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Truncate), Nazwa = "Truncate", IloscParametrow = 1, Description = "część całkowita liczby" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Ceiling), Nazwa = "Ceiling", IloscParametrow = 1, Description = "sufit liczby" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Floor), Nazwa = "Floor", IloscParametrow = 1, Description = "podłoga liczby" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToBoolArray), Nazwa = "ToBoolArray", IloscParametrow = -1, Description = "tworzenie tablicy typu bool" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToIntArray), Nazwa = "ToIntArray", IloscParametrow = -1, Description = "tworzenie tablicy typu int" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToDoubleArray), Nazwa = "ToDoubleArray", IloscParametrow = -1, Description = "tworzenie tablicy typu double" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToStringArray), Nazwa = "ToStringArray", IloscParametrow = -1, Description = "tworzenie tablicy typu string" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToArray), Nazwa = "ToArray", IloscParametrow = -1, Description = "tworzenie tablicy typu object" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Sqrt), Nazwa = "Sqrt", IloscParametrow = 1, Description = "pierwiastek kwadratowy" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Power), Nazwa = "Power", IloscParametrow = 2, Description = "potęga" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Error), Nazwa = "Error", IloscParametrow = 1, Description = "przerywa działanie skryptu" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(IndexOf), Nazwa = "IndexOf", IloscParametrow = 3, Description = "zwraca pozycję szukanego ciągu" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Substring), Nazwa = "Substring", IloscParametrow = 3, Description = "zwraca wskazany wycinek łańcucha" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetLength), Nazwa = "GetLength", IloscParametrow = 1, Description = "zwraca długość łańcucha tekstowego" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetSize), Nazwa = "GetSize", IloscParametrow = 1, Description = "zwraca rozmiar tablicy" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(SetSize), Nazwa = "SetSize", IloscParametrow = 2, Description = "ustawia rozmiar tablicy" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetValue), Nazwa = "GetValue", IloscParametrow = 2, Description = "zwraca wskazaną wartość w tablicy" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(SetValue), Nazwa = "SetValue", IloscParametrow = 4, Description = "ustawia wskazaną wartość w tablicy" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetName), Nazwa = "GetName", IloscParametrow = 1, Description = "zwraca nazwę zmiennej/stałej" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetID), Nazwa = "GetID", IloscParametrow = 1, Description = "zwraca ID zmiennej modułu wejściowego lub wyjściowego" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetType), Nazwa = "GetType", IloscParametrow = 1, Description = "zwraca typ wartości" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetRandom), Nazwa = "GetRandom", IloscParametrow = 2, Description = "zwraca losową liczbę z podanego zakresu" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Not), Nazwa = "Not", IloscParametrow = 1, Description = "neguje wartość logiczną" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(LogIf), Nazwa = "LogIf", IloscParametrow = 2, Description = "wpisuje do okienka informacje (drugi argument) gdy pierwszy argument ma wartość true" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Log), Nazwa = "Log", IloscParametrow = 1, Description = "wpisuje do okienka log informacje, jeden argument - string" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToBool), Nazwa = "ToBool", IloscParametrow = 1, Description = "konwertue na bool" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToInt), Nazwa = "ToInt", IloscParametrow = 1, Description = "konwertuje na int" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToDouble), Nazwa = "ToDouble", IloscParametrow = 1, Description = "konwertuje na double" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ToString), Nazwa = "ToString", IloscParametrow = 1, Description = "konwertuje na string" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(StringReplace), Nazwa = "StringReplace", IloscParametrow = 3, Description = "zamienia string" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(FormatNumber), Nazwa = "FormatNumber", IloscParametrow = 2, Description = "formatuje liczbę" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(ForceSignal), Nazwa = "ForceSignal", IloscParametrow = 1, Description = "wymusza wygenerowanie zdarzenia bez zmiany wartości zmiennej" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(NoSignal), Nazwa = "NoSignal", IloscParametrow = 2, Description = "ustawia wartość zmiennej bez generowania zdarzenia" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetBitState), Nazwa = "GetBitState", IloscParametrow = 2, Description = "pobiera stan bitu (true lub false)" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(SetBitState), Nazwa = "SetBitState", IloscParametrow = 3, Description = "ustawia wartość bitu" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(SetWithSignal), Nazwa = "SetWithSignal", IloscParametrow = 2, Description = "ustawia wartość i wymusza wywołanie zdarzenia (nawet gdy wartość zmiennej się nie zmieniła)" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(GetDateTime), Nazwa = "GetDateTime", IloscParametrow = 1, Description = "Zwraca sformatowaną datę jako łańcuch tekstowy (string)." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Trim), Nazwa = "Trim", IloscParametrow = -1, Description = "Usuwa niewidzialne (białe) znaki na początku i końcu przekazanego łańcucha tekstu, usuwane znaki można podawać po przecinku jako argumenty funkcji." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(TrimStart), Nazwa = "TrimStart", IloscParametrow = -1, Description = "Usuwa niewidzialne (białe) znaki z początku przekazanego łańcucha tekstu, usuwane znaki można podawać po przecinku jako argumenty funkcji." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(TrimEnd), Nazwa = "TrimEnd", IloscParametrow = -1, Description = "Usuwa niewidzialne (białe) znaki z końca przekazanego łańcucha tekstu, usuwane znaki można podawać po przecinku jako argumenty funkcji." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Sleep), Nazwa = "Sleep", IloscParametrow = 1, Description = "Zatrzymuje wykonywanie skryptu na wskazaną ilość milisekund (kolejka zdarzeń nie jest zamykana)." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Stop), Nazwa = "Stop", IloscParametrow = -1, Description = "Zatrzymuje wykonywanie skryptu, można podać argument będący komunikatem wypisanym w oknie Log." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(IntToChar), Nazwa = "IntToChar", IloscParametrow = 1, Description = "Konwertuje kod znaku ASCII/UNICODE na znak (string)." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(CharToInt), Nazwa = "CharToInt", IloscParametrow = 1, Description = "Konwertuje znak na kod ASCII/UICODE." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(FileWriteLine), Nazwa = "FileWriteLine", IloscParametrow = 2, Description = "Dopisuje wskazany tekst do pliku. Użycie: WriteFileLine(<scieżka do pliku>, <tekst do zapisu>)." }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Sin), Nazwa = "Sin", IloscParametrow = 1, Description = "" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Cos), Nazwa = "Cos", IloscParametrow = 1, Description = "" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(Acos), Nazwa = "Acos", IloscParametrow = 1, Description = "" }
			,   new FunkcjaInformacje() { Funkcja = new FunkcjaDelegate(CaptureWindow), Nazwa = "CaptureWindow", IloscParametrow = 1, Description = "" }
		};

		public static FunkcjaInformacje PobierzFunkcje(string nazwa)
		{
			for (int i = 0; i < __funkcjeWbudowane.Length; i++)
			{
				if (__funkcjeWbudowane[i].Nazwa == nazwa)
				{
					return __funkcjeWbudowane[i];
				}
			}
			return null;
		}

		private static object Not(Argument[] argumenty)
		{
			return !(bool)argumenty[0].Wykonaj();
		}

		private static object Log(Argument[] argumenty)
		{
			string v = argumenty[0].Wykonaj().ToString();
			__log(v);
			return v;
		}

		private static object LogIf(Argument[] argumenty)
		{
			bool b = (bool)argumenty[0].Wykonaj();
			if (b)
			{
				string v = argumenty[1].Wykonaj().ToString();
				__log(v);
				return v;
			}
			return null;
		}

		private static object ToBool(Argument[] argumenty)
		{
			object v = argumenty[0].Wykonaj();
			if (v is bool)
			{
				return (bool)v;
			}
			if (v is int)
			{
				return (int)v != 0;
			}
			if (v is double)
			{
				return (double)v != 0d;
			}
			if (v is string)
			{
				bool r = false;
				if (Stala.IsBool((string)v, out r))
				{
					return r;
				}
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotConvertValueToType), v, v == null ? "(null)" : v.GetType().Name, "bool"));
		}

		private static object ToInt(Argument[] argumenty)
		{
			object v = argumenty[0].Wykonaj();
			if (v is bool)
			{
				return (bool)v ? 1 : 0;
			}
			if (v is int)
			{
				return (int)v;
			}
			if (v is double)
			{
				return (int)(double)v;
			}
			if (v is string)
			{
				double r1 = 0;
				if (Stala.IsDouble((string)v, out r1))
				{
					return (int)r1;
				}
				int r2 = 0;
				if (Stala.IsInt((string)v, out r2))
				{
					return r2;
				}
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotConvertValueToType), v, v == null ? "(null)" : v.GetType().Name, "int"));
		}

		private static object ToDouble(Argument[] argumenty)
		{
			object v = argumenty[0].Wykonaj();
			if (v is bool)
			{
				return (bool)v ? 1d : 0d;
			}
			if (v is int)
			{
				return (double)(int)v;
			}
			if (v is double)
			{
				return (double)v;
			}
			if (v is string)
			{
				double r1 = 0;
				if (Stala.IsDouble((string)v, out r1))
				{
					return r1;
				}
				int r2 = 0;
				if (Stala.IsInt((string)v, out r2))
				{
					return (double)r2;
				}
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotConvertValueToType), v, v == null ? "(null)" : v.GetType().Name, "double"));
		}

		private static string ToString(Argument[] argumenty)
		{
			object v = argumenty[0].Wykonaj();
			if (v is bool)
			{
				return v.ToString().ToLowerInvariant();
			}
			if (v is int || v is double || v is string)
			{
				return v.ToString();
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotConvertValueToType), v, v == null ? "(null)" : v.GetType().Name, "string"));
		}

		private static string StringReplace(Argument[] argumenty)
		{
			return ((string)argumenty[0].Wykonaj()).Replace((string)argumenty[1].Wykonaj(), (string)argumenty[2].Wykonaj());
		}

		private static string FormatNumber(Argument[] argumenty)
		{
			object liczba = argumenty[0].Wykonaj();
			if (liczba is int)
			{
				return ((int)liczba).ToString((string)argumenty[1].Wykonaj());
			}
			if (liczba is double)
			{
				return ((double)liczba).ToString((string)argumenty[1].Wykonaj());
			}
			throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.ValueMustBeIntOrDouble));
		}

		private static object ForceSignal(Argument[] argumenty)
		{
			if (argumenty[0].argument is Zmienna)
			{
				((Zmienna)argumenty[0].argument).Signal();
				return ((Zmienna)argumenty[0].argument).Wykonaj();
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionArgumentMayBeOnlyVariable), "ForceSignal(...)"));
		}

		private static object NoSignal(Argument[] argumenty)
		{
			if (argumenty[0].argument is Zmienna)
			{
				return ((Zmienna)argumenty[0].argument).NoSignal(argumenty[1].Wykonaj());
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionArgumentMayBeOnlyVariable), "NoSignal(...)"));
		}

		private static object SetWithSignal(Argument[] argumenty)
		{
			NoSignal(argumenty);
			return ForceSignal(argumenty);
		}

		private static object GetBitState(Argument[] argumenty)
		{
			int v = (int)argumenty[0].Wykonaj();
			int b = (int)argumenty[1].Wykonaj();
			if (b < 0)
			{
				throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.WrongBitNumberForFunction), "GetBitState(...)"));
			}
			return ((v >> b) & 1) == 1;
		}

		private static object SetBitState(Argument[] argumenty)
		{
			int value = (int)argumenty[0].Wykonaj();
			int bit = (int)argumenty[1].Wykonaj();
			bool state = (bool)argumenty[2].Wykonaj();
			if (bit < 0)
			{
				throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.WrongBitNumberForFunction), "SetBitState(...)"));
			}
			if (state)
			{
				return value | (1 << bit);
			}
			else
			{
				return value & (-1 ^ (1 << bit));
			}
		}

		private static readonly Random __random = new Random();

		private static object GetRandom(Argument[] argumenty)
		{
			int min = (int)argumenty[0].Wykonaj();
			int max = (int)argumenty[1].Wykonaj();
			return __random.Next(min, max + 1);
		}

		private static object GetType(Argument[] argumenty)
		{
			if (argumenty[0].argument is Zmienna)
			{
				return (int)((Zmienna)argumenty[0].argument).Typ;
			}
			return (int)Utils.SprawdzTyp(argumenty[0].Wykonaj());
		}

		private static object GetName(Argument[] argumenty)
		{
			if (argumenty[0].argument is Zmienna || argumenty[0].argument is Stala)
			{
				return ((Wartosc)argumenty[0].argument).Nazwa;
			}
			throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.WrongGetNameFunctionArgument));
		}
		
		private static object GetID(Argument[] argumenty)
		{
			if (argumenty[0].argument is Zmienna && ((Zmienna)argumenty[0].argument).Kierunek != KierunekZmiennej.None)
			{
				return ((Zmienna)argumenty[0].argument).ID;
			}
			throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.WrongGetIDFunctionArgument));
		}

		private static object GetSize(Argument[] argumenty)
		{
			object array = argumenty[0].Wykonaj();
			if (array == null)
			{
				return 0;
			}

			if (array.GetType() == Utils.__boolArrayType)
			{
				return ((bool[])array).Length;
			}

			if (array.GetType() == Utils.__intArrayType)
			{
				return ((int[])array).Length;
			}

			if (array.GetType() == Utils.__doubleArrayType)
			{
				return ((double[])array).Length;
			}

			if (array.GetType() == Utils.__stringArrayType)
			{
				return ((string[])array).Length;
			}

			if (array.GetType() == Utils.__arrayType)
			{
				return ((object[])array).Length;
			}

			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.WrongGetSizeFunctionArgument), array.GetType()));
		}

		private static object SetSize(Argument[] argumenty)
		{
			if (argumenty[0].argument is Zmienna)
			{
				int size = (int)argumenty[1].Wykonaj();
				if (size < 0)
				{
					throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.WrongArraySize), size));
				}
				Zmienna z = (Zmienna)argumenty[0].argument;
				object array = z.Wykonaj();
				switch (z.Typ)
				{
					case HomeSimCockpitSDK.VariableType.Bool_Array:
						bool [] t1 = (bool[])array;
						Array.Resize<bool>(ref t1, size);
						return z.UstawWartosc(t1);

					case HomeSimCockpitSDK.VariableType.Int_Array:
						int[] t2 = (int[])array;
						Array.Resize<int>(ref t2, size);
						return z.UstawWartosc(t2);

					case HomeSimCockpitSDK.VariableType.Double_Array:
						double[] t3 = (double[])array;
						Array.Resize<double>(ref t3, size);
						return z.UstawWartosc(t3);

					case HomeSimCockpitSDK.VariableType.String_Array:
						string[] t4 = (string[])array;
						Array.Resize<string>(ref t4, size);
						return z.UstawWartosc(t4);

					case HomeSimCockpitSDK.VariableType.Array:
						object[] t5 = (object[])array;
						Array.Resize<object>(ref t5, size);
						return z.UstawWartosc(t5);

					default:
						throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.WrongSetSizeFunctionArgumentType), argumenty[0]));
				}
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ArgumentIsNotVariable), argumenty[0], "SetSize"));
		}

		private static object GetValue(Argument[] argumenty)
		{
			object array = argumenty[0].Wykonaj();
			int index = (int)argumenty[1].Wykonaj();
			if (array == null)
			{
				throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ArrayNotInitialized), argumenty[0], "GetValue"));
			}
			try
			{
				if (array.GetType() == Utils.__boolArrayType)
				{
					return ((bool[])array)[index];
				}
				if (array.GetType() == Utils.__intArrayType)
				{
					return ((int[])array)[index];
				}
				if (array.GetType() == Utils.__doubleArrayType)
				{
					return ((double[])array)[index];
				}
				if (array.GetType() == Utils.__stringArrayType)
				{
					return ((string[])array)[index];
				}
				if (array.GetType() == Utils.__arrayType)
				{
					object v = ((object[])array)[index];
					if (v == null)
					{
						throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsetValueInArray), argumenty[0], index));
					}
					if (v is bool)
					{
						return (bool)v;
					}
					if (v is int)
					{
						return (int)v;
					}
					if (v is double)
					{
						return (double)v;
					}
					if (v is string)
					{
						return (string)v;
					}
					if (v.GetType() == Utils.__boolArrayType)
					{
						return (bool[])v;
					}
					if (v.GetType() == Utils.__intArrayType)
					{
						return (int[])v;
					}
					if (v.GetType() == Utils.__doubleArrayType)
					{
						return (double[])v;
					}
					if (v.GetType() == Utils.__stringArrayType)
					{
						return (string[])v;
					}
					if (v.GetType() == Utils.__arrayType)
					{
						return (object[])v;
					}
					throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedValueTypeInArray), v.GetType(), argumenty[0], index));
				}
				throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedArrayType), array.GetType(), argumenty[0]));
			}
			catch (IndexOutOfRangeException)
			{
				throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NoValueInArrayAtIndex), index, argumenty[0]));
			}
		}

		private static object SetValue(Argument[] argumenty)
		{
			if (argumenty[0].argument is Zmienna)
			{
				Zmienna z = (Zmienna)argumenty[0].argument;
				object array = z.Wykonaj();
				object value = argumenty[1].Wykonaj();
				int indexFrom = (int)argumenty[2].Wykonaj();
				//int indexTo = (int)argumenty[3].Wykonaj() + 1;
				int indexTo = indexFrom + (int)argumenty[3].Wykonaj();
				if (array == null)
				{
					throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ArrayNotInitialized), argumenty[0], "SetValue"));
				}
				if (value == null)
				{
					throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsetValueForArray), argumenty[0], indexFrom));
				}
				try
				{
					if (array.GetType() == Utils.__boolArrayType)
					{
						return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<bool>((bool[])array, indexFrom, indexTo, (bool)value));
					}
					if (array.GetType() == Utils.__intArrayType)
					{
						return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<int>((int[])array, indexFrom, indexTo, (int)value));
					}
					if (array.GetType() == Utils.__doubleArrayType)
					{
						return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<double>((double[])array, indexFrom, indexTo, (double)value));
					}
					if (array.GetType() == Utils.__stringArrayType)
					{
						return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<string>((string[])array, indexFrom, indexTo, (string)value));
					}
					if (array.GetType() == Utils.__arrayType)
					{
						object[] t5 = (object[])array;
						if (value is bool)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<bool>(t5, indexFrom, indexTo, (bool)value));
						}
						if (value is int)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<int>(t5, indexFrom, indexTo, (int)value));
						}
						if (value is double)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<double>(t5, indexFrom, indexTo, (double)value));
						}
						if (value is string)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<string>(t5, indexFrom, indexTo, (string)value));
						}
						if (value.GetType() == Utils.__boolArrayType)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<bool>(t5, indexFrom, indexTo, (bool[])value));
						}
						if (value.GetType() == Utils.__intArrayType)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<int>(t5, indexFrom, indexTo, (int[])value));
						}
						if (value.GetType() == Utils.__doubleArrayType)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<double>(t5, indexFrom, indexTo, (double[])value));
						}
						if (value.GetType() == Utils.__stringArrayType)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<string>(t5, indexFrom, indexTo, (string[])value));
						}
						if (value.GetType() == Utils.__arrayType)
						{
							return z.UstawWartosc(SprawdzCzyRoznaUstawZwroc<object>(t5, indexFrom, indexTo, (object[])value));
						}
						throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedValueTypeInArray), value.GetType(), argumenty[0], indexFrom));
					}
					throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedArrayType), array.GetType(), argumenty[0]));
				}
				catch (IndexOutOfRangeException)
				{
					throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ArrayIndexOutOfBound), indexFrom, indexTo, argumenty[0]));
				}
			}
			throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ArgumentIsNotVariable), argumenty[0], "SetValue"));
		}

		private static object SprawdzCzyRoznaUstawZwroc<T>(T[] array, int indexFrom, int indexTo, T value)
		{
			bool change = false;
			for (int i = indexFrom; i < indexTo; i++)
			{
				if (array[i] == null || !array[i].Equals(value))
				{
					change = true;
					break;
				}
			}
			if (change)
			{
				T[] result = new T[array.Length];
				Array.Copy(array, result, result.Length);
				for (int i = indexFrom; i < indexTo; i++)
				{
					result[i] = value;
				}
				return result;
			}
			return array;
		}

		private static object SprawdzCzyRoznaUstawZwroc<T>(object[] array, int indexFrom, int indexTo, T value)
		{
			bool change = false;
			for (int i = indexFrom; i < indexTo; i++)
			{
				if (array[i] == null || !array[i].Equals(value))
				{
					change = true;
					break;
				}
			}
			if (change)
			{
				object[] result = new object[array.Length];
				Array.Copy(array, result, result.Length);
				for (int i = indexFrom; i < indexTo; i++)
				{
					result[i] = value;
				}
				return result;
			}
			return array;
		}

		private static object SprawdzCzyRoznaUstawZwroc<T>(object[] array, int indexFrom, int indexTo, T[] value)
		{
			bool change = false;
			for (int i = indexFrom; i < indexTo; i++)
			{
				if (array[i] == null || !EqualsArray((T[])array[i], value))
				{
					change = true;
					break;
				}
			}
			if (change)
			{
				object[] result = new object[array.Length];
				Array.Copy(array, result, result.Length);
				for (int i = indexFrom; i < indexTo; i++)
				{
					result[i] = value;
				}
				return result;
			}
			return array;
		}

		private static bool EqualsArray<T>(T[] left, T[] right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			if (left.Length != right.Length)
			{
				return false;
			}
			for (int i = 0; i < left.Length; i++)
			{
				if (!left[i].Equals(right[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static object GetLength(Argument[] argumenty)
		{
			string v = (string)argumenty[0].Wykonaj();
			return v.Length;
		}

		private static object Substring(Argument[] argumenty)
		{
			string v = (string)argumenty[0].Wykonaj();
			int start = (int)argumenty[1].Wykonaj();
			int length = (int)argumenty[2].Wykonaj();
			return v.Substring(start, length);
		}

		private static object IndexOf(Argument[] argumenty)
		{
			string v = (string)argumenty[0].Wykonaj();
			string c = (string)argumenty[1].Wykonaj();
			int start = (int)argumenty[2].Wykonaj();
			return v.IndexOf(c, start);
		}

		private static object Error(Argument[] argumenty)
		{
			string t = (string)argumenty[0].Wykonaj();
			throw new UserErrorException(t);
		}

		/// <summary>
		/// Potęga
		/// </summary>
		/// <param name="argumenty"></param>
		/// <returns></returns>
		private static object Power(Argument[] argumenty)
		{
			object v = argumenty[0].Wykonaj();
			int pow = (int)argumenty[1].Wykonaj();
			if (v is int)
			{
				return (int)Math.Pow((double)(int)v, Convert.ToDouble(pow));
			}
			else
			{
				return Math.Pow((double)v, Convert.ToDouble(pow));
			}
		}

		/// <summary>
		/// Pierwiastek
		/// </summary>
		/// <param name="argumenty"></param>
		/// <returns></returns>
		private static object Sqrt(Argument[] argumenty)
		{
			object v = argumenty[0].Wykonaj();
			if (v is int)
			{
				return Math.Sqrt((double)(int)v);
			}
			else
			{
				return Math.Sqrt((double)v);
			}
		}

		/// <summary>
		/// Tworzenie tablicy bool
		/// </summary>
		/// <param name="argumenty"></param>
		/// <returns></returns>
		private static object ToBoolArray(Argument[] argumenty)
		{
			return ToArray<bool>(argumenty);
		}

		/// <summary>
		/// Tworzenie tablicy int
		/// </summary>
		/// <param name="argumenty"></param>
		/// <returns></returns>
		private static object ToIntArray(Argument[] argumenty)
		{
			return ToArray<int>(argumenty);
		}

		/// <summary>
		/// Tworzenie tablicy double
		/// </summary>
		/// <param name="argumenty"></param>
		/// <returns></returns>
		private static object ToDoubleArray(Argument[] argumenty)
		{
			return ToArray<double>(argumenty);
		}

		/// <summary>
		/// Tworzenie tablicy string
		/// </summary>
		/// <param name="argumenty"></param>
		/// <returns></returns>
		private static object ToStringArray(Argument[] argumenty)
		{
			return ToArray<string>(argumenty);
		}

		/// <summary>
		/// Tworzenie tablicy object
		/// </summary>
		/// <param name="argumenty"></param>
		/// <returns></returns>
		private static object ToArray(Argument[] argumenty)
		{
			return ToArray<object>(argumenty);
		}

		private static T[] ToArray<T>(Argument[] argumenty)
		{
			if (argumenty != null && argumenty.Length > 0)
			{
				T[] result = new T[argumenty.Length];
				for (int i = 0; i < argumenty.Length; i++)
				{
					result[i] = (T)argumenty[i].Wykonaj();
				}
				return result;
			}
			return null;
		}

		private static object Floor(Argument[] argumenty)
		{
			return (int)Math.Floor((double)argumenty[0].Wykonaj());
		}

		private static object Ceiling(Argument[] argumenty)
		{
			return (int)Math.Ceiling((double)argumenty[0].Wykonaj());
		}

		private static object Truncate(Argument[] argumenty)
		{
			return (int)Math.Truncate((double)argumenty[0].Wykonaj());
		}

		private static object Abs(Argument[] argumenty)
		{
			object v = argumenty[0].Wykonaj();
			if (v is int)
			{
				return (int)Math.Abs((int)argumenty[0].Wykonaj());
			}
			else
			{
				return (double)Math.Abs((double)argumenty[0].Wykonaj());
			}
		}

		private static object Round(Argument[] argumenty)
		{
			double v = (double)argumenty[0].Wykonaj();
			double m = Math.Ceiling(v);
			if (Math.Abs((m - v)) <= 0.5d)
			{
				return (int)m;
			}
			return (int)v;
		}

		private static object MakeDouble(Argument[] argumenty)
		{
			int high = (int)argumenty[0].Wykonaj();
			int low = (int)argumenty[1].Wykonaj();
			long result = high;
			result <<= 32;
			result |= low;
			double d = BitConverter.Int64BitsToDouble(result);
			return d;
		}

		private static object GetDateTime(Argument[] argumenty)
		{
			if (argumenty == null || argumenty.Length == 0)
			{
				return DateTime.Now.ToString();
			}
			return DateTime.Now.ToString((string)argumenty[0].Wykonaj());
		}
		
		private static object Trim(Argument[] args)
		{
			return _Trim(args, true, true);
		}
		
		private static object TrimStart(Argument[] args)
		{
			return _Trim(args, true, false);
		}
		
		private static object TrimEnd(Argument[] args)
		{
			return _Trim(args, false, true);
		}
		
		private static object _Trim(Argument[] args, bool start, bool end)
		{
			string text = (string)args[0].Wykonaj();
			char[] chars = null;
			if (args.Length > 1)
			{
				chars = new char[args.Length - 1];
				for (int i = 1; i < args.Length; i++)
				{
					chars[i - 1] = ((string)args[i].Wykonaj())[0];
				}
			}
			if (start)
			{
				text = text.TrimStart(chars);
			}
			if (end)
			{
				text = text.TrimEnd(chars);
			}
			return text;
		}
		
		private static object Sleep(Argument[] args)
		{
			System.Threading.Thread.Sleep((int)args[0].Wykonaj());
			return true;
		}
		
		private static object Stop(Argument[] args)
		{
			if (args != null && args.Length > 0)
			{
				__main.Zatrzymaj((string)args[0].Wykonaj());
			}
			else
			{
				__main.Zatrzymaj(null);
			}
			return true;
		}
		
		private static object IntToChar(Argument [] args)
		{
			int value = (int)args[0].Wykonaj();
			return ((char)value).ToString();
		}
		
		private static object CharToInt(Argument [] args)
		{
			string value = (string)args[0].Wykonaj();
			return (int)value[0];
		}
		
		private static object FileWriteLine(Argument [] args)
		{
			string filePath = (string)args[0].Wykonaj();
			string line = (string)args[1].Wykonaj();
			File.AppendAllText(filePath, line + "\r\n", Encoding.UTF8);
			return true;
		}
		
		private static object Sin(Argument [] args)
		{
			return Math.Sin((double)args[0].Wykonaj());
		}
		
		private static object Cos(Argument [] args)
		{
			return Math.Cos((double)args[0].Wykonaj());
		}
		
		private static object Acos(Argument [] args)
		{
			return Math.Acos((double)args[0].Wykonaj());
		}
		
		private static Dictionary<string, string> __imgs = new Dictionary<string, string>();
		
		private static object CaptureWindow(Argument [] args)
		{
			string windowTitle = (string)args[0].Wykonaj();
			IntPtr hwnd = WinAPI.FindWindow(null, windowTitle);
			if (hwnd != IntPtr.Zero)
			{
				WinAPI.RECT rect = new WinAPI.RECT();
				WinAPI.GetWindowRect(hwnd, ref rect);
				Rectangle rectangle = rect;
				Size size = rectangle.Size;
				try
				{
					if (!(size.IsEmpty || size.Height < 0 || size.Width < 0))
					{
						using (Bitmap bmp = new Bitmap(size.Width, size.Height))
						{
							Graphics g = Graphics.FromImage(bmp);
							IntPtr dc = g.GetHdc();

							WinAPI.PrintWindow(hwnd, dc, 0);

							g.ReleaseHdc();
							g.Dispose();
							string f = null;
							if (!__imgs.ContainsKey(windowTitle))
							{
								__imgs.Add(windowTitle, new Random().Next(0, 99999).ToString() + ".png");
							}
							f = __imgs[windowTitle];
							
							string filePath = @"F:\DANE_1\programowanie\WWW\HSC_test\" + f;// Path.GetFullPath("screen.png");
							bmp.Save(filePath, ImageFormat.Png);
							return f;
						}
					}
				}
				catch
				{
					return "";
				}
			}
			return "";
		}
	}
}