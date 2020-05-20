using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs implementowany przez moduły, które udostępniają okno dialogowe do konfiguracji modułu.
    /// </summary>
    public interface IModuleConfiguration
    {
        /// <summary>
        /// Metoda wywoływana przez aplikację na rzecz modułu, którego okno do konfiguracji chce wyświetlić.
        /// </summary>
        /// <param name="parent">Uchwyt do okna głównego, aby okno konfiguracji podczepiło się do niego.</param>
        /// <returns>Informacja czy konfiguracja zmieniła się i aplikacj powinna odświeżyć widok.</returns>
        bool Configuration(IWin32Window parent);
    }
}
