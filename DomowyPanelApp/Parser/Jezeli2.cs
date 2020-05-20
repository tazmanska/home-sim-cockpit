using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Jezeli : Wyrazenie
    {
        public Wyrazenie WyrazenieLewe = null;
        public Wyrazenie WyrazeniePrawe = null;
        public WarunekLogiczny WarunekLogiczny = null;
        public virtual bool CzyPrawda()
        {
            if (WarunekLogiczny.Typ == WarunekLogicznyTyp.Prawdziwy)
            {
                return (bool)WyrazenieLewe.Wykonaj();
            }
            else
            {
                return WarunekLogiczny.CzyPrawda(WyrazenieLewe.Wykonaj(), WyrazeniePrawe.Wykonaj());
            }
        }

        public override object Wykonaj()
        {
            return CzyPrawda();
        }

        private static List<HCPSToken> __tokenyOperatorow = new List<HCPSToken>() { HCPSToken.Bigger, HCPSToken.BiggerOrEqual, HCPSToken.Equal, HCPSToken.NotEqual, HCPSToken.Smaller, HCPSToken.SmallerOrEqual };

        public new static Jezeli Parsuj(ITokenizer tokenizer)
        {
            Jezeli jezeli = new Jezeli();
            jezeli.WyrazenieLewe = Wyrazenie.Parsuj(tokenizer);
            HCPSToken token = tokenizer.Next();
            if (__tokenyOperatorow.Contains(token))
            {
                jezeli.WarunekLogiczny = new WarunekLogiczny()
                {
                    Typ = WarunekLogiczny.TokenNaWarunek(token)
                };
                jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                jezeli.Check();
                return jezeli;
            }
            else
            {
                if (token == HCPSToken.ParenthesisClose)
                {
                    tokenizer.Back();
                    jezeli.WarunekLogiczny = new WarunekLogiczny()
                    {
                        Typ = WarunekLogicznyTyp.Prawdziwy
                    };
                    jezeli.Check();
                    return jezeli;
                }
            }            
            throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.IncorrectIfStatementDefinition));
        }

        public override void Check()
        {
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (WyrazenieLewe is ZmiennaNieistniejaca)
            {
                WyrazenieLewe = slownik.PobierzWartosc(((ZmiennaNieistniejaca)WyrazenieLewe).Nazwa);
            }
            else
            {
                WyrazenieLewe.PrzypiszReferencje(slownik);
            }
            if (WarunekLogiczny.Typ != WarunekLogicznyTyp.Prawdziwy)
            {
                if (WyrazeniePrawe is ZmiennaNieistniejaca)
                {
                    WyrazeniePrawe = slownik.PobierzWartosc(((ZmiennaNieistniejaca)WyrazeniePrawe).Nazwa);
                }
                else
                {
                    WyrazeniePrawe.PrzypiszReferencje(slownik);
                }
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            WyrazenieLewe.PrzypiszReferencje(slownik);
            if (WarunekLogiczny.Typ != WarunekLogicznyTyp.Prawdziwy)
            {
                WyrazeniePrawe.PrzypiszReferencje(slownik);
            }
        }
    }
}
