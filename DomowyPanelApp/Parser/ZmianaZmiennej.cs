using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class ZmianaZmiennej : Zdarzenie
    {
        public Zmienna Zmienna = null;
        public Akcje Akcje = null;
        public string Opis = "";
        public override void Sygnal(Zmienna zmienna)
        {
            try
            {
                Akcje.Wykonaj();
            }
            catch (EndExecutionException)
            {
            }
        }

        public new static ZmianaZmiennej Parsuj(ITokenizer tokenizer)
        {
            ZmianaZmiennej zdarzenie = new ZmianaZmiennej();
            if (tokenizer.Next() == HCPSToken.Variable)
            {
                zdarzenie.Zmienna = new ZmiennaNieistniejaca()
                {
                    Nazwa = tokenizer.Value
                };
                if (tokenizer.Next() == HCPSToken.BlockBegin)
                {
                    zdarzenie.Akcje = Akcje.Parsuj(tokenizer);
                    zdarzenie.Check();
                }
                else
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectEventDefinition), tokenizer.GetPosition(tokenizer.Position)));
                }
            }
            else
            {
                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectEventDefinition), tokenizer.GetPosition(tokenizer.Position)));
            }
            return zdarzenie;
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (Zmienna is ZmiennaNieistniejaca)
            {
                object v = slownik.PobierzWartosc(Zmienna.Nazwa);
                if (!(v is Zmienna))
                {
                    throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.EventsAreAllowedOnlyForVariables), Zmienna.Nazwa));
                }
                Zmienna = (Zmienna)v;
                Zmienna.Powiadomienie += new ZmianaWartosciZmiennejPowiadomienie(Sygnal);
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
