using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    class FunkcjaInformacje
    {
        public string Nazwa = null;
        public FunkcjaDelegate Funkcja = null;
        public int IloscParametrow = 0;
        public string Description = "";

        public bool DobraIloscParametrow(int ilosc)
        {
            if (IloscParametrow == -1)
            {
                return true;
            }
            return ilosc == IloscParametrow;
        }
    }
}
