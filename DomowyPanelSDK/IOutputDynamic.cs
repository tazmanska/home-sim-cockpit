using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Zmienne wyjściowe modułu implementującego są "dynamiczne",
    /// i nie będzie sprawdzane czy w module istnieje dana zmienna
    /// oraz zgodność typu zadeklarowanego w skrypcie i module.
    /// </summary>
    public interface IOutputDynamic
    {
        /// <summary>
        /// Metoda sprawdza czy dany moduł dynamiczny może przetworzyć taką zmienną.
        /// </summary>
        /// <param name="variableID">ID zmiennej.</param>
        /// <param name="variableType">Typ zmiennej.</param>
        /// <returns>Czy moduł może przetworzyć taką zmienną ?</returns>
        bool CanUseVariable(string variableID, VariableType variableType);
    }
}
