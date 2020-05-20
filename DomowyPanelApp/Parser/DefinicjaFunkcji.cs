using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    class DefinicjaFunkcji : Wyrazenie
    {
        public class ZmiennaArgument : Zmienna
        {

            public static ZmiennaArgument Copy(Zmienna zmienna)
            {
                return new ZmiennaArgument()
                {
                    Kierunek = zmienna.Kierunek,
                    Nazwa = zmienna.Nazwa,
                    Typ = zmienna.Typ,
                    _Wartosc = zmienna._Wartosc
                };
            }
        }

        public class ZmiennaParametr : Zmienna
        {
            private Wyrazenie _prawdziwaZmienna = null;

            public Wyrazenie PrawdziwaZmienna
            {
                get { return _prawdziwaZmienna; }
                set
                {
                    _prawdziwaZmienna = value;
                    if (_prawdziwaZmienna is Wartosc)
                    {
                        Nazwa = ((Wartosc)_prawdziwaZmienna).Nazwa;
                        Typ = ((Wartosc)_prawdziwaZmienna).Typ;
                    }
                }
            }

            public void SetValue(object value)
            {
                Nazwa = "";
                Typ = HomeSimCockpitSDK.VariableType.Unknown;
                PrawdziwaZmienna = null;
                _value = value;
            }

            public object GetValue()
            {
                return _value;
            }

            private object _value = null;

            public override object Wykonaj()
            {
                if (PrawdziwaZmienna != null)
                {
                    return PrawdziwaZmienna.Wykonaj();
                }
                else
                {
                    return _value;
                }
            }

            public override object UstawWartosc(object wartosc)
            {
                if (PrawdziwaZmienna is Zmienna)
                {
                    return ((Zmienna)PrawdziwaZmienna).UstawWartosc(wartosc);
                }
                throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotSetValueToExpression), PrawdziwaZmienna));
            }

            public override string ToString()
            {
                return Nazwa;
            }
        }

        public string Nazwa = "";
        public ZmiennaParametr[] argumenty = null;
        public Akcje[] akcje = null;

        private Stack<ArgumentBindingInfo[]> _stosWiazaniaArgumentow = new Stack<ArgumentBindingInfo[]>();

        struct ArgumentBindingInfo
        {
            public object Value;
            public Wyrazenie PrawdziwaZmienna;

            public ArgumentBindingInfo(object value, Wyrazenie prawdziwaZmienna)
            {
                Value = value;
                PrawdziwaZmienna = prawdziwaZmienna;
            }

            public override string ToString()
            {
                return string.Format("Value = {0}, PrawdziwaZmienna = {1}", Value, PrawdziwaZmienna);
            }
        }

        public override object Wykonaj()
        {
            System.Diagnostics.Debug.Write(string.Format("{0} (", Nazwa), "Script");
            foreach (ZmiennaParametr z in argumenty)
            {
                System.Diagnostics.Debug.Write(string.Format(" < {0} > ", z), "Script");
            }
            System.Diagnostics.Debug.WriteLine(")", "Script");
            object result = null;
            foreach (Akcje a in akcje)
            {
                result = a.Wykonaj();
                if (result is ReturnWFunkcji)
                {
                    return ((ReturnWFunkcji)result).Value;
                }
            }
            return result;
        }
        
        public object Wykonaj(Argument[] argumenty)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Wiązanie argumentów funkcji '{0}'.", Nazwa), "Script");
            // powiązanie argumentów
            ArgumentBindingInfo[] wiazanieArgumentow = new ArgumentBindingInfo[argumenty.Length];
            _stosWiazaniaArgumentow.Push(wiazanieArgumentow);
            for (int i = 0; i < argumenty.Length; i++)
            {
                wiazanieArgumentow[i].PrawdziwaZmienna = this.argumenty[i].PrawdziwaZmienna;
                wiazanieArgumentow[i].Value = this.argumenty[i].GetValue();
                if (argumenty[i].argument is ZmiennaArgument)
                {
                    this.argumenty[i].PrawdziwaZmienna = ZmiennaArgument.Copy((Zmienna)argumenty[i].argument);
                }
                else
                if (argumenty[i].argument is ZmiennaParametr && ((ZmiennaParametr)argumenty[i].argument).PrawdziwaZmienna != null)
                {
                    this.argumenty[i].PrawdziwaZmienna = ((ZmiennaParametr)argumenty[i].argument).PrawdziwaZmienna;
                }
                else
                {
                    if (argumenty[i].argument is Zmienna && !(argumenty[i].argument is ZmiennaParametr))
                    {
                        this.argumenty[i].PrawdziwaZmienna = argumenty[i].argument;
                    }
                    else
                    {
                        ZmiennaArgument za = new ZmiennaArgument()
                        {
                            Nazwa = this.argumenty[i].Nazwa,
                            _Wartosc = argumenty[i].argument.Wykonaj()                            
                        };
                        za.Typ = Utils.SprawdzTyp(za._Wartosc);
                        this.argumenty[i].PrawdziwaZmienna = za;
                        //this.argumenty[i].SetValue(argumenty[i].argument.Wykonaj());
                    }
                }
                if (this.argumenty[i] == this.argumenty[i].PrawdziwaZmienna)
                {
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CyclicalVariableLinkingFound), argumenty[i], Nazwa));
                }
            }
            object r = Wykonaj();            
            wiazanieArgumentow = _stosWiazaniaArgumentow.Pop();
            for (int i = 0; i < wiazanieArgumentow.Length; i++)
            {
                this.argumenty[i].SetValue(wiazanieArgumentow[i].Value);
                this.argumenty[i].PrawdziwaZmienna = wiazanieArgumentow[i].PrawdziwaZmienna;
            }
            return r;
        }

        public new static DefinicjaFunkcji Parsuj(ITokenizer tokenizer)
        {
            DefinicjaFunkcji funkcja = new DefinicjaFunkcji();
            if (tokenizer.Next() == HCPSToken.Word)
            {
                funkcja.Nazwa = tokenizer.Value;

                // odczytanie nazw argumentów
                if (tokenizer.Next() == HCPSToken.ParenthesisOpen)
                {
                    List<ZmiennaParametr> argumenty = new List<ZmiennaParametr>();
                    HCPSToken token = HCPSToken.Unknown;
                    while ((token = tokenizer.Next()) == HCPSToken.Variable)
                    {
                        argumenty.Add(new ZmiennaParametr()
                        {
                            Nazwa = tokenizer.Value
                        });
                        if (tokenizer.Next() != HCPSToken.CommaSeparator)
                        {
                            tokenizer.Back();
                        }
                    }
                    funkcja.argumenty = argumenty.ToArray();                    
                    if (token != HCPSToken.ParenthesisClose)
                    {
                        throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectFunctionDefinition), tokenizer.GetPosition(tokenizer.Position)));
                    }
                }
                else
                {
                    throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectFunctionDefinition), tokenizer.GetPosition(tokenizer.Position)));
                }

                if (tokenizer.Next() == HCPSToken.BlockBegin)
                {
                    List<Akcje> akcje = new List<Akcje>();
                    do
                    {
                        Akcje a = Akcje.Parsuj(tokenizer);
                        if (a != null)
                        {
                            akcje.Add(a);
                        }
                    }
                    while (tokenizer.LastToken != HCPSToken.BlockEnd);
                    funkcja.akcje = akcje.ToArray();
                    funkcja.Check();
                    return funkcja;
                }
            }
            throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectFunctionDefinition), tokenizer.GetPosition(tokenizer.Position)));
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (FunkcjeWbudowane.PobierzFunkcje(Nazwa) != null)
            {
                throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotOverrideBuiltinFunction), Nazwa));
            }
            if (akcje != null)
            {
                ISlownikSkryptu s = new _ZbiorWartosci(slownik, argumenty);
                foreach (Akcje a in akcje)
                {
                    a.PrzypiszReferencje(s);
                }
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            if (akcje != null)
            {
                foreach (Akcje a in akcje)
                {
                    a.PrzypiszReferencje(slownik);
                }
            }
        }

        private class _ZbiorWartosci : ISlownikSkryptu
        {
            public _ZbiorWartosci(ISlownikSkryptu slownik, Zmienna[] parametry)
            {
                _slownik = slownik;
                _parametry = parametry;
            }

            private ISlownikSkryptu _slownik = null;
            private Zmienna[] _parametry = null;

            #region ISlownikSkryptu Members

            public Wartosc PobierzWartosc(string nazwa)
            {
                foreach (Zmienna z in _parametry)
                {
                    if (z.Nazwa == nazwa)
                    {
                        return z;
                    }
                }
                return _slownik.PobierzWartosc(nazwa);
            }

            public FunkcjaInformacje PobierzFunkcje(string nazwa)
            {
                return _slownik.PobierzFunkcje(nazwa);
            }

            #endregion
        }
    }
}
