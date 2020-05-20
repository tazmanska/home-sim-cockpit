using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    class FunkcjaModulu : Wyrazenie
    {
        public string Modul = "";
        public string Nazwa = "";
        public Argument[] argumenty = null;
        private HomeSimCockpitSDK.ModuleExportedFunctionDelegate _funkcja = null;

        public override object Wykonaj()
        {
            if (_funkcja != null)
            {
                object[] a = new object[argumenty.Length];
                for (int i = 0; i < argumenty.Length; i++)
                {
                    a[i] = argumenty[i].Wykonaj();
                }
                return _funkcja(a);
            }
            throw new NotImplementedException();
        }

        public new static FunkcjaModulu Parsuj(ITokenizer tokenizer)
        {
            FunkcjaModulu funkcja = new FunkcjaModulu();
            if (tokenizer.Next() == HCPSToken.Word)
            {
                funkcja.Modul = tokenizer.Value;
                if (tokenizer.Next() == HCPSToken.Colon && tokenizer.Next() == HCPSToken.Word)
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
                }
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectFunctionInvocation), tokenizer.GetPosition(tokenizer.Position)));
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
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

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);

            HomeSimCockpitSDK.ModuleFunctionInfo info = slownik.PobierzFunkcje(Modul, Nazwa);
            if (info != null)
            {
                if (info.ArgumentsNumber != -1 && info.ArgumentsNumber != argumenty.Length)
                {
                    throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectArgumentNumber2), Nazwa, Modul, info.ArgumentsNumber));
                }
                _funkcja = info.Function;
            }
            else
            {
                throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionNotFoundInModule), Modul, Nazwa));
            }
        }
    }
}
