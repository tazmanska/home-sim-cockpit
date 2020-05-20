using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    class ZbiorWartosci2 : ISlownikSkryptu
    {

        public ZbiorWartosci2(List<Zmienna> zmienne, List<Stala> stale, List<DefinicjaFunkcji> funkcje)
        {
            _zmienne = zmienne;
            _stale = stale;
            _funkcje = funkcje;
        }

        private List<Zmienna> _zmienne = null;
        private List<Stala> _stale = null;
        private List<DefinicjaFunkcji> _funkcje = null;

        #region ISlownikSkryptu Members

        public Wartosc PobierzWartosc(string nazwa)
        {
            Wartosc result = _zmienne.Find(delegate(Zmienna z)
                                            {
                                                return z.Nazwa == nazwa;
                                            });
            if (result == null)
            {
                result = _stale.Find(delegate(Stala s)
                {
                    return s.Nazwa == nazwa;
                });
            }
            if (result == null)
            {
                throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.VariableOrConstantNotFound), nazwa));
            }
            return result;
        }

        public FunkcjaInformacje PobierzFunkcje(string nazwa)
        {
            DefinicjaFunkcji funkcja = _funkcje.Find(delegate(DefinicjaFunkcji f)
            {
                return f.Nazwa == nazwa;
            });
            if (funkcja == null)
            {
                throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionNotFound), nazwa));
            }
            return new FunkcjaInformacje()
                {
                    Funkcja = new FunkcjaDelegate(funkcja.Wykonaj),
                    Nazwa = funkcja.Nazwa,
                    IloscParametrow = funkcja.argumenty.Length
                };
        }

        #endregion
    }
}
