using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Interfejs wykorzystywany do logowania wiadomości (np. o błędach).
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Metoda zapisuje informację do loga.
        /// </summary>
        /// <param name="module">Moduł, który chce zapisać informację.</param>
        /// <param name="text">Informacja do zapisania do loga.</param>
        void Log(IModule module, string text);
    }
}
