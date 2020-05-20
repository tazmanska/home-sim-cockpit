/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-14
 * Godzina: 19:00
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of Display.
	/// </summary>
	class Display : IComparable<Display>
	{
			public LED7DisplayOutput LED7Display
			{
				get;
				set;
			}
			
			public int Index
			{
				get;
				set;
			}
			
			public int CompareTo(Display other)
			{
				return Index.CompareTo(other.Index);
			}
		}
}
