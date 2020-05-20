using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Przypisanie : Wyrazenie
    {
        public Zmienna Zmienna = null;
        public Wyrazenie Wyrazenie = null;
        public override object Wykonaj()
        {
            return Zmienna.UstawWartosc(Wyrazenie.Wykonaj());
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (Zmienna is ZmiennaNieistniejaca)
            {
                Zmienna = (Zmienna)slownik.PobierzWartosc(Zmienna.Nazwa);
                Zmienna.SprawdzCzyMoznaUstawiacWartoscWSkrypcie();
            }
            if (Wyrazenie is ZmiennaNieistniejaca)
            {
                Wartosc w = slownik.PobierzWartosc(((ZmiennaNieistniejaca)Wyrazenie).Nazwa);
                if (w is Zmienna)
                {
                    Wyrazenie = (Zmienna)w;
                }
                else
                {
                    if (w is Stala)
                    {
                        Wyrazenie = (Stala)w;
                    }
                    else
                    {
                        Wyrazenie = w;
                    }
                }
            }
            else
            {
                Wyrazenie.PrzypiszReferencje(slownik);
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            Wyrazenie.PrzypiszReferencje(slownik);
        }

        public override void Check()
        {
            base.Check();
            if (Wyrazenie is Operacja)
            {
                Wyrazenie = Operacja.Uporzadkuj((Operacja)Wyrazenie);
            }
        }
    }
}
