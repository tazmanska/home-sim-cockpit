using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    enum KierunekZmiennej
    {
        /// <summary>
        /// Nieznany kierunek zmiennej.
        /// </summary>
        Unknown,
        
        /// <summary>
        /// Zmienna wejściowa.
        /// </summary>
        In,
        
        /// <summary>
        /// Zmienna wyjściowa.
        /// </summary>
        Out,
        
        /// <summary>
        /// Zmienna wewnętrzna.
        /// </summary>
        None
    }
}
