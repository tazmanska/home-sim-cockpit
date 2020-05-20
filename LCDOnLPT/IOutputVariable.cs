/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-06
 * Godzina: 22:02
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace LCDOnLPT
{
    /// <summary>
    /// Description of OutputVariable.
    /// </summary>
    interface IOutputVariable: HomeSimCockpitSDK.IVariable
    {
        void SetValue(object value);        
    }
}
