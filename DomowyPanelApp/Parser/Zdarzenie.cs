using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Zdarzenie : Wyrazenie
    {
        public virtual void Sygnal(Zmienna zmienna) { }

        public override object Wykonaj()
        {
            throw new NotImplementedException();
        }
    }
}
