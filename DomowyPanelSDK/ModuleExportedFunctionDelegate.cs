using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Delegacja na metodę funkcji udostępnionej przez moduł.
    /// </summary>
    /// <param name="argumenty">Tablica argumentów przekazanych do funkcji.</param>
    /// <returns>Wynik działania funkcji.</returns>
    public delegate object ModuleExportedFunctionDelegate(object [] argumenty);
}
