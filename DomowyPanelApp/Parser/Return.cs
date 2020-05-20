using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    class Return : Wyrazenie
    {
        public Wyrazenie Wyrazenie = null;

        public override object Wykonaj()
        {
            if (Wyrazenie == null)
            {
                return new ReturnWFunkcji()
                {
                    Value = null
                };
            }
            return new ReturnWFunkcji()
            {
                Value = Wyrazenie.Wykonaj()
            };
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (Wyrazenie == null)
            {
                return;
            }

            if (Wyrazenie is ZmiennaNieistniejaca)
            {
                Wyrazenie = (Zmienna)slownik.PobierzWartosc(((ZmiennaNieistniejaca)Wyrazenie).Nazwa);
            }
            else
            {
                Wyrazenie.PrzypiszReferencje(slownik);
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            if (Wyrazenie != null)
            {
                Wyrazenie.PrzypiszReferencje(slownik);
            }
        }
    }
}
