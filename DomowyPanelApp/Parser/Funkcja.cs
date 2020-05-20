using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Funkcja : Wyrazenie
    {
        public string Nazwa = "";
        public Argument[] argumenty = null;
        private FunkcjaDelegate _funkcja = null;

        public override object Wykonaj()
        {
            if (_funkcja != null)
            {
                return _funkcja(argumenty);
            }
            throw new NotImplementedException();
        }

        public new static Funkcja Parsuj(ITokenizer tokenizer)
        {
            Funkcja funkcja = new Funkcja();
            if (tokenizer.Next() == HCPSToken.Word)
            {
                funkcja.Nazwa = tokenizer.Value;
                
                List<Argument> argumenty = new List<Argument>();
                // odczytanie nazw argumentów
                if (tokenizer.Next() == HCPSToken.ParenthesisOpen)
                {
                    do
                    {
                        Argument a = Argument.Parsuj(tokenizer);
                        if (a != null)
                        {
                            argumenty.Add(a);
                        }
                    } while (tokenizer.LastToken != HCPSToken.ParenthesisClose);                    
                    funkcja.argumenty = argumenty.ToArray();
                    funkcja.Check();
                    return funkcja;
                }
                else
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectFunctionInvocation), tokenizer.GetPosition(tokenizer.Position)));
                }
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectFunctionInvocation), tokenizer.GetPosition(tokenizer.Position)));
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            // sprawdzenie czy jest taka funkcja wbudowana
            FunkcjaInformacje fi = FunkcjeWbudowane.PobierzFunkcje(Nazwa);
            if (fi != null)
            {
                if (!fi.DobraIloscParametrow(argumenty.Length))
                {
                    throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectArgumentNumber), Nazwa, fi.IloscParametrow.ToString()));
                }
                _funkcja = fi.Funkcja;
            }

            // nie ma więc poszukanie w zdefiniowanych
            if (_funkcja == null)
            {
                fi = slownik.PobierzFunkcje(Nazwa);
                if (fi != null)
                {
                    if (!fi.DobraIloscParametrow(argumenty.Length))
                    {
                        throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectArgumentNumber), Nazwa, fi.IloscParametrow.ToString()));
                    }
                    _funkcja = fi.Funkcja;
                }
                else
                {
                    throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionNotFound), Nazwa));
                }
            }

            if (argumenty != null)
            {
                foreach (Argument a in argumenty)
                {
                    a.PrzypiszReferencje(slownik);
                }
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            if (argumenty != null)
            {
                foreach (Argument a in argumenty)
                {
                    a.PrzypiszReferencje(slownik);
                }
            }
        }

        public override void Check()
        {
            base.Check();
            if (argumenty != null)
            {
                foreach (Argument a in argumenty)
                {
                    if (a.argument is Operacja)
                    {
                        a.argument = Operacja.Uporzadkuj((Operacja)a.argument);
                    }
                }
            }
        }
    }
}
