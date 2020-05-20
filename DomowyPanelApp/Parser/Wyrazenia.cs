using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Wyrazenia : Wyrazenie
    {
        public Wyrazenie[] wyrazenia = null;
        public override object Wykonaj()
        {
            object result = null;
            foreach (Wyrazenie w in wyrazenia)
            {
                result = w.Wykonaj();
            }
            return result;
        }
    }
}
