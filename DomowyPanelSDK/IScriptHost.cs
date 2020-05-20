/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-24
 * Godzina: 22:30
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs obiektu wykonującego skrypt.
    /// </summary>
    public interface IScriptHost
    {
        /// <summary>
        /// Metoda wykonuje wskazaną funkcję.
        /// </summary>
        /// <param name="invoker">Moduł wywołujący funkcję.</param>
        /// <param name="functionName">Nazwa funkcji do wykonania.</param>
        /// <param name="arguments">Tablica argumentów dla funkcji.</param>
        /// <returns>Wynik działania funkcji.</returns>
        object ExecuteFunction(IModule invoker, string functionName, object [] arguments);
        
        /// <summary>
        /// Metoda pobiera wartości wskazanych zmiennych.
        /// </summary>
        /// <param name="invoker">Moduł wywołujący funkcję.</param>
        /// <param name="variables">Tablica zmiennych, który wartości mają być zwrócone.</param>
        /// <param name="values">Tablica wartości wskazanych zmiennych.</param>
        /// <returns>Informacja o powodzeniu działania funkcji.</returns>
        bool GetVariables(IModule invoker, string [] variables, out object [] values);
        
        /// <summary>
        /// Metoda ustawia wartości wskazanych zmiennych.
        /// </summary>
        /// <param name="invoker">Moduł wywołujący funkcję.</param>
        /// <param name="variables">Tablica zmiennych, który wartości mają być ustawione.</param>
        /// <param name="values">Tablica wartości zmiennych do ustawienia.</param>
        /// <returns>Informacja o powodzeniu działania funkcji.</returns>
        bool SetVariables(IModule invoker, string [] variables, object [] values);
    }
}
