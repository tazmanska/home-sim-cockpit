using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    class Skrypt : IComparable<Skrypt>
    {
        public string Nazwa = null;
        public Zmienna[] Zmienne = null;
        public Stala[] Stale = null;
        public Zdarzenie[] Zdarzenia = null;
        public DefinicjaFunkcji[] Funkcje = null;
        public Akcja Initialize = null;
        public Akcja OutputStarted = null;
        public Akcja OutputStopped = null;
        public Akcja InputStarted = null;
        public Akcja InputStopped = null;
        public Akcja Uninitialize = null;

        public void PrzypiszFunkcjeModulow(ISlownikFunkcjiModulow slownik)
        {
            if (Zdarzenia != null)
            {
                foreach (Zdarzenie z in Zdarzenia)
                {
                    z.PrzypiszReferencje(slownik);
                }
            }
            if (Funkcje != null)
            {
                foreach (DefinicjaFunkcji f in Funkcje)
                {
                    f.PrzypiszReferencje(slownik);
                }
            }
            if (Zmienne != null)
            {
                foreach (Zmienna z in Zmienne)
                {
                    z.PrzypiszReferencje(slownik);
                }
            }
            if (Initialize != null)
            {
                Initialize.PrzypiszReferencje(slownik);
            }
            if (OutputStarted != null)
            {
                OutputStarted.PrzypiszReferencje(slownik);
            }
            if (OutputStopped != null)
            {
                OutputStopped.PrzypiszReferencje(slownik);
            }
            if (InputStarted != null)
            {
                InputStarted.PrzypiszReferencje(slownik);
            }
            if (InputStopped != null)
            {
                InputStopped.PrzypiszReferencje(slownik);
            }
            if (Uninitialize != null)
            {
                Uninitialize.PrzypiszReferencje(slownik);
            }
        }

        public void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (Zdarzenia != null)
            {
                foreach (Zdarzenie z in Zdarzenia)
                {
                    z.PrzypiszReferencje(slownik);
                }
            }
            if (Funkcje != null)
            {
                foreach (DefinicjaFunkcji f in Funkcje)
                {
                    f.PrzypiszReferencje(slownik);
                }
            }
            if (Zmienne != null)
            {
                foreach (Zmienna z in Zmienne)
                {
                    z.PrzypiszReferencje(slownik);
                }
            }
            if (Initialize != null)
            {
                Initialize.PrzypiszReferencje(slownik);
            }
            if (OutputStarted != null)
            {
                OutputStarted.PrzypiszReferencje(slownik);
            }
            if (OutputStopped != null)
            {
                OutputStopped.PrzypiszReferencje(slownik);
            }
            if (InputStarted != null)
            {
                InputStarted.PrzypiszReferencje(slownik);
            }
            if (InputStopped != null)
            {
                InputStopped.PrzypiszReferencje(slownik);
            }
            if (Uninitialize != null)
            {
                Uninitialize.PrzypiszReferencje(slownik);
            }
        }
        
        public int CompareTo(Skrypt other)
        {
            return Nazwa.CompareTo(other.Nazwa);
        }
    }
}
