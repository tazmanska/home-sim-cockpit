using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs implementowany przez moduł udostępniający funkcje.
    /// </summary>
    public interface IModuleFunctions
    {
        /// <summary>
        /// Zwraca tablicę udostępnionych przez moduł funkcji.
        /// </summary>
        ModuleFunctionInfo[] Functions
        {
            get;
        }
    }
}
