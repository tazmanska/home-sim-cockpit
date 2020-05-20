using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs zmiennej modułu wejścia/wyjścia.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// Zwraca identyfikator zmiennej w obrębie modułu.
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// Zwraca typ zmiennej.
        /// </summary>
        VariableType Type
        {
            get;
        }

        /// <summary>
        /// Zwraca opis zmiennej.
        /// </summary>
        string Description
        {
            get;
        }
    }
}
