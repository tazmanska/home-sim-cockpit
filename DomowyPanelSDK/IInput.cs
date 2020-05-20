using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs modułu wejściowego (dostarczającego danych do skryptu).
    /// </summary>
    public interface IInput : IModule
    {
        /// <summary>
        /// Tablica zmiennych dostępnych w module.
        /// </summary>
        IVariable[] InputVariables
        {
            get;
        }

        /// <summary>
        /// Rejestrowanie oczekiwania zdarzeń na zmianę wartości
        /// zmiennej o wskazanym ID.
        /// </summary>
        /// <param name="listenerMethod">Delegacja na metodę, która
        /// będzie wywoływana gdy zmienna zmieni wartość.</param>
        /// <param name="variableID">Identyfikator zmiennej o której
        /// zmianach wartości chcemy być informowani.</param>
        /// <param name="type"></param>
        void RegisterListenerForVariable(VariableChangeSignalDelegate listenerMethod, string variableID, VariableType type);

        /// <summary>
        /// Odrejestrowanie oczkiwania zdarzeń na zmianę wartości
        /// zmiennej o wskazanym ID.
        /// </summary>
        /// <param name="listenerMethod"></param>
        /// <param name="variableID"></param>
        void UnregisterListenerForVariable(VariableChangeSignalDelegate listenerMethod, string variableID);
    }
}
