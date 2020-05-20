using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs implementowany przez moduły udostępniające pomoc (opis itp.) do modułu.
    /// </summary>
    public interface IModuleHelp
    {
        /// <summary>
        /// Metoda wywoływana przez aplikację na rzecz modułu, którego pomoc została wywołana przez użytkownika.
        /// </summary>
        /// <param name="parent">Uchwyt do okna głównego.</param>
        void Help(IWin32Window parent);
    }
}
