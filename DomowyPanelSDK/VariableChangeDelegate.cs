using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Delegacja metody informującej o zmianie wartości zmiennej.
    /// </summary>
    /// <param name="inputModule">Moduł wywołujący metodę.</param>
    /// <param name="variableID">Identyfikator zmiennej w obrębie modułu wołającego.</param>
    /// <param name="variableValue">Nowa wartość zmiennej.</param>
    public delegate void VariableChangeSignalDelegate(IInput inputModule, string variableID, object variableValue);
}
