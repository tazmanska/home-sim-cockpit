/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-24
 * Godzina: 22:31
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Rozszerzony interfejs modułów.
    /// </summary>
    public interface IModule2
    {
        /// <summary>
        /// Metoda inicjalizuje moduł.
        /// </summary>
        /// <param name="scriptHost">Obiekt implementujący interfejs IScripHost (wykonujący skrypt).</param>
        void Init(IScriptHost scriptHost);
        
        /// <summary>
        /// Metoda wskazuje modułowi listę wymaganych funkcji.
        /// </summary>
        /// <param name="functionsNames">Lista nazw wymaganych funkcji, null jeśli żadna funkcja nie jest wymagana.</param>
        void RequireFunctions(string [] functionsNames);
    }
}
