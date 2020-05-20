using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Typy zmiennych.
    /// </summary>
    public enum VariableType
    {
        /// <summary>
        /// Nieznany typ (nie może występować).
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Typ bool'owski, przyjmuje wartości true lub false.
        /// </summary>
        Bool = 1,

        /// <summary>
        /// Tablica wartości Bool.
        /// </summary>
        Bool_Array = 10,

        /// <summary>
        /// Typ całkowitoliczbowy (32 bity).
        /// </summary>
        Int = 2,

        /// <summary>
        /// Tablica wartości Int.
        /// </summary>
        Int_Array = 20,

        /// <summary>
        /// Typ zmiennoprzecinkowy (64 bity).
        /// </summary>
        Double = 3,

        /// <summary>
        /// Tablica wartości Double.
        /// </summary>
        Double_Array = 30,

        /// <summary>
        /// Typ tekstowy (dowolna długość).
        /// </summary>
        String = 4,

        /// <summary>
        /// Tablica wartości String.
        /// </summary>
        String_Array = 40,

        /// <summary>
        /// Tablica wartości różnych typów.
        /// </summary>
        Array = 50,
    }
}
