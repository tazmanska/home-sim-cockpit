using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs modułu wyjściowego (odbierającego dane ze skryptu).
    /// </summary>
    public interface IOutput : IModule
    {
        /// <summary>
        /// Tablica zmiennych dostępnych w module.
        /// </summary>
        IVariable[] OutputVariables
        {
            get;
        }

        /// <summary>
        /// Metoda informuje, że zmienna o wskazanym ID
        /// może być zmienia.
        /// </summary>
        /// <param name="variableID"></param>
        /// <param name="type"></param>
        void RegisterChangableVariable(string variableID, VariableType type);

        /// <summary>
        /// Metoda odwołuje zmienianie zmiennej
        /// o wskazanym ID.
        /// </summary>
        /// <param name="variableID"></param>
        void UnregisterChangableVariable(string variableID);

        /// <summary>
        /// Metoda ustawia wartość zmiennej wyjściowej.
        /// </summary>
        /// <param name="variableID">Identyfikator zmiennej.</param>
        /// <param name="value">Wartość.</param>
        void SetVariableValue(string variableID, object value);
    }
}
