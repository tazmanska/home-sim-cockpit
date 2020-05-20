using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    class ZmianaZmiennych : Zdarzenie
    {
        public Zmienna[] Zmienne = null;
        private Dictionary<string, bool> _sygnaly = new Dictionary<string, bool>();
        private int _sygnalow = 0;
        public int Minimum = -1;
        public Akcje Akcje = null;
        public string Opis = "";
        public override void Sygnal(Zmienna zmienna)
        {
            if (!_sygnaly[zmienna.Nazwa])
            {
                _sygnaly[zmienna.Nazwa] = true;
                _sygnalow++;
            }
            if ((Minimum == -1 && _sygnalow == Zmienne.Length) || (Minimum > -1 && _sygnalow >= Minimum))
            {
                foreach (Zmienna z in Zmienne)
                {
                    _sygnaly[z.Nazwa] = false;
                }
                _sygnalow = 0;
                try
                {
                    Akcje.Wykonaj();
                }
                catch (EndExecutionException)
                {
                }
            }
        }

        public new static ZmianaZmiennych Parsuj(ITokenizer tokenizer)
        {
            ZmianaZmiennych zdarzenie = new ZmianaZmiennych();
            HCPSToken token = tokenizer.Next();
            if (token == HCPSToken.Number)
            {
                if (!int.TryParse(tokenizer.Value, out zdarzenie.Minimum))
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectNumberForVariablesChangeEvent), tokenizer.Value));
                }
                token = tokenizer.Next();
            }
            List<Zmienna> zmienne = new List<Zmienna>();
            while (token == HCPSToken.Variable)
            {
                Zmienna z = new ZmiennaNieistniejaca()
                {
                    Nazwa = tokenizer.Value
                };
                // sprawdzenie czy ta zmienna nie istnieje już na liście
                for (int i = 0; i < zmienne.Count; i++)
                {
                    if (zmienne[i].Nazwa == z.Nazwa)
                    {
                        throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.RepeatedVariableInVariablesChangeEvent), z.Nazwa));
                    }
                }
                zmienne.Add(z);
                token = tokenizer.Next();
            }
            if (zmienne.Count < 1)
            {
                throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.VariablesChangeEventRequireAtLeastTwoVariables));
            }
            zdarzenie.Zmienne = zmienne.ToArray();
            if (token == HCPSToken.BlockBegin)
            {
                zdarzenie.Akcje = Akcje.Parsuj(tokenizer);
                zdarzenie.Check();
            }
            else
            {
                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectVariablesChangeEventDefinition), tokenizer.GetPosition(tokenizer.Position)));
            }

            return zdarzenie;
        }

        public override void Check()
        {
            base.Check();
            if (Zmienne == null || Zmienne.Length < 2)
            {
                throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NotEnoughVariablesForVariablesChangeEvent), Zmienne.Length));
            }
            if (Minimum > -1)
            {
                if (Minimum == 0 || Minimum > Zmienne.Length)
                {
                    throw new CheckingScriptException(UI.Language.Instance.GetString(UI.UIStrings.IncorrectVariablesChangedMinimumForVariablesChangedEvent));
                }
            }
            _sygnaly = new Dictionary<string, bool>();
            foreach (Zmienna z in Zmienne)
            {
                _sygnaly.Add(z.Nazwa, false);
            }
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (Zmienne != null)
            {
                for (int i = 0; i < Zmienne.Length; i++)
                {
                    if (Zmienne[i] is ZmiennaNieistniejaca)
                    {
                        object v = slownik.PobierzWartosc(Zmienne[i].Nazwa);
                        if (!(v is Zmienna))
                        {
                            throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.EventsAreAllowedOnlyForVariables), ((Wartosc)v).Nazwa));
                        }
                        Zmienne[i] = (Zmienna)v;
                        Zmienne[i].Powiadomienie += new ZmianaWartosciZmiennejPowiadomienie(Sygnal);
                    }
                }
            }            
            if (Akcje != null)
            {
                Akcje.PrzypiszReferencje(slownik);
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            if (Akcje != null)
            {
                Akcje.PrzypiszReferencje(slownik);
            }
        }
    }
}
