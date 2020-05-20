/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-06
 * Godzina: 10:21
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace HomeSimCockpitSDK
{
	/// <summary>
	/// Typy uruchamiania/zatrzymywania modułów.
	/// </summary>
	public enum StartStopType
	{
		/// <summary>
		/// Uruchomienie/zatrzymanie wyjścia.
		/// </summary>
		Output,
		
		/// <summary>
		/// Uruchomienie/zatrzymanie wejścia.
		/// </summary>
		Input,
		
		/// <summary>
		/// Uruchomienie/zatrzymanie wejścia i wyjścia.
		/// </summary>
		InputOutput,
	}
}
