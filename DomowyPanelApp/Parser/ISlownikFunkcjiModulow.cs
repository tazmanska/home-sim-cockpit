using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    interface ISlownikFunkcjiModulow
    {
        HomeSimCockpitSDK.ModuleFunctionInfo PobierzFunkcje(string modul, string funkcja);
    }
}
