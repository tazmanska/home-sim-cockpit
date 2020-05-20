using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    interface ISlownikSkryptu
    {
        Wartosc PobierzWartosc(string nazwa);

        FunkcjaInformacje PobierzFunkcje(string nazwa);

    }
}
