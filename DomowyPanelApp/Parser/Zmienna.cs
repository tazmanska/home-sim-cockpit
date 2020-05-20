using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Zmienna : Wartosc, IComparable<Zmienna>
    {
        public object _WartoscPoczatkowa = null;
        public bool Disabled = false;
        public string Modul = "<script>";
        public string ID = "";
        public KierunekZmiennej Kierunek = KierunekZmiennej.None;
        public event ZmianaWartosciZmiennejPowiadomienie Powiadomienie;
        private FunkcjaDelegate _funkcja = null;
        internal string _funkcjaNazwa = null;
        private Argument[] _argumenty = null;

        /// <summary>
        /// Metoda przypisuje nową wartość zmiennej
        /// ale nie generuje zdarzenia.
        /// </summary>
        /// <param name="wartosc"></param>
        /// <returns></returns>
        public object NoSignal(object wartosc)
        {
            SprawdzCzyMoznaUstawiacWartoscWSkrypcie();
            return Ustaw(wartosc, false);
        }

        /// <summary>
        /// Metoda przypisuje zmiennej nową wartość i generuje zdarzenie.
        /// </summary>
        /// <param name="wartosc"></param>
        /// <returns></returns>
        public override object UstawWartosc(object wartosc)
        {
            SprawdzCzyMoznaUstawiacWartoscWSkrypcie();
            return Ustaw(wartosc, true);
        }

        public void SprawdzCzyMoznaUstawiacWartoscWSkrypcie()
        {
            if (Kierunek == KierunekZmiennej.In)
            {
                throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotSetValueToInputVariable), Nazwa));
            }
        }

        public object UstawWartoscOdZdarzenia(object wartosc)
        {
            return Ustaw(wartosc, true);
        }

        private object Ustaw(object wartosc, bool signal)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0} = {1}, signal = {2}", this, wartosc, signal), "Script");

            // zerowanie zmiennej bez powiadamiania o jej zmianie
            if (wartosc == null)
            {
                _Wartosc = _WartoscPoczatkowa;
                return _Wartosc;
            }

            // przetworzenie wartosci
            if (!Utils.CzyPoprawnaWartosc(wartosc, Typ))
            {
                throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectValueTypeForVariable), Nazwa, Utils.VariableTypeToString(Typ), Utils.VariableTypeToString(Utils.SprawdzTyp(wartosc))));
            }

            object v = Utils.ConvertTo(wartosc, Typ);

            // sprawdzanie czy wartość jest zmieniona tylko gdy wymagane jest powiadomienie słuchaczy
            if (signal)
            {
                if (WarunekLogiczny.CzyPrawda(WarunekLogicznyTyp.Rowny, _Wartosc, v))
                {
                    return _Wartosc;
                }
            }

            _Wartosc = v;

            // powiadomienie sluchaczy
            if (signal)
            {
                Signal();
            }

            return wartosc;
        }

        /// <summary>
        /// Metoda generuje zdarzenie zmiany zmiennej,
        /// nie zmieniając wartości zmiennej.
        /// </summary>
        public void Signal()
        {
            if (_funkcja != null)
            {
                _funkcja(_argumenty);
            }

            ZmianaWartosciZmiennejPowiadomienie powiadomienie = Powiadomienie;
            if (powiadomienie != null)
            {
                powiadomienie(this);
            }
        }

        public new static Zmienna Parsuj(ITokenizer tokenizer)
        {
            Zmienna zmienna = new Zmienna();
            HCPSToken token = tokenizer.Next();
            if (token == HCPSToken.Variable)
            {
                zmienna.Nazwa = tokenizer.Value;
                if (tokenizer.Next() == HCPSToken.BlockBegin)
                {
                    while ((token = tokenizer.Next()) != HCPSToken.BlockEnd)
                    {
                        if (token == HCPSToken.Comment)
                        {
                            continue;
                        }
                        SimpleAssignment sa = tokenizer.ReadSimpleAssignment(token, tokenizer.Value);
                        if (sa == null || sa.Left == null || sa.Right == null)
                        {
                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectVariableDefinition), zmienna.Nazwa));
                        }
                        switch (sa.Left.ToLowerInvariant())
                        {
                            case "module":
                                zmienna.Modul = sa.Right;
                                break;

                            case "id":
                                zmienna.ID = sa.Right;
                                break;

                            case "direct":
                                zmienna.Kierunek = Utils.KierunekZmiennejFromString(sa.Right);
                                break;

                            case "type":
                                zmienna.Typ = Utils.VariableTypeFromString(sa.Right);
                                break;

                            case "value":
                                zmienna._Wartosc = zmienna._WartoscPoczatkowa = sa.Right;
                                break;

                            case "function":
                                zmienna._funkcjaNazwa = sa.Right;
                                break;

                            default:
                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InvalidVariableDefinitionProperty), sa.Left, zmienna.Nazwa, tokenizer.GetPosition(tokenizer.Position)));
                        }
                    }
                    zmienna.Check();
                    return zmienna;
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotParseVariableDefinition), zmienna.Nazwa));
                }
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InvalidVariableDefinition), tokenizer.GetPosition(tokenizer.Position)));
        }

        public override object Wykonaj()
        {
            return _Wartosc;
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            base.PrzypiszReferencje(slownik);
            if (_funkcjaNazwa != null)
            {
                // sprawdzenie czy jest taka funkcja wbudowana
                FunkcjaInformacje fi = slownik.PobierzFunkcje(_funkcjaNazwa);
                if (fi != null)
                {
                    if (fi.IloscParametrow != 1)
                    {
                        throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectNumberOfVariablesForVariableFunction), _funkcjaNazwa, Nazwa, fi.IloscParametrow));
                    }
                    _funkcja = fi.Funkcja;
                    _argumenty = new Argument[] { new Argument() { argument = this } };
                }
                else
                {
                    throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionNotFound), _funkcjaNazwa));
                }
            }
        }

        #region IComparable<Zmienna> Members

        public int CompareTo(Zmienna other)
        {
            return Nazwa.CompareTo(other.Nazwa);
        }

        #endregion

        public override void Check()
        {
            base.Check();
            if (Kierunek == KierunekZmiennej.Unknown)
            {
                throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UndefinedVariableDirection), Nazwa));
            }
            switch (Typ)
            {
                case HomeSimCockpitSDK.VariableType.Bool:
                    if (_Wartosc == null)
                    {
                        _Wartosc = _WartoscPoczatkowa = false;
                    }
                    else
                    {
                        _Wartosc = _WartoscPoczatkowa = Utils.Parse(_Wartosc, Typ);
                    }
                    break;

                case HomeSimCockpitSDK.VariableType.Int:
                    if (_Wartosc == null)
                    {
                        _Wartosc = _WartoscPoczatkowa = 0;
                    }
                    else
                    {
                        int i = 0;
                        if (!Stala.IsInt((string)_Wartosc, out i))
                        {
                            throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectNumberFormatTypeIntForVariableInitialization), (string)_Wartosc, Nazwa));
                        }
                        _Wartosc = _WartoscPoczatkowa = i;
                    }
                    break;

                case HomeSimCockpitSDK.VariableType.Double:
                    if (_Wartosc == null)
                    {
                        _Wartosc = _WartoscPoczatkowa = 0d;
                    }
                    else
                    {
                        double d = 0;
                        if (!Stala.IsDouble((string)_Wartosc, out d))
                        {
                            throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectNumberFormatTypeDoubleForVariableInitialization), (string)_Wartosc, Nazwa));
                        }
                        _Wartosc = _WartoscPoczatkowa = d;
                    }
                    break;

                case HomeSimCockpitSDK.VariableType.String:
                    if (_Wartosc == null)
                    {
                        _Wartosc = _WartoscPoczatkowa = "";
                    }
                    else
                    {
                        _Wartosc = _WartoscPoczatkowa = Utils.Parse(_Wartosc, Typ);
                    }
                    break;

                case HomeSimCockpitSDK.VariableType.Bool_Array:
                case HomeSimCockpitSDK.VariableType.Int_Array:
                case HomeSimCockpitSDK.VariableType.Double_Array:
                case HomeSimCockpitSDK.VariableType.String_Array:
                case HomeSimCockpitSDK.VariableType.Array:
                    if (_Wartosc == null)
                    {
                        _Wartosc = _WartoscPoczatkowa = null;
                    }
                    else
                    {
                        throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotInitailizeVariableOfType), Nazwa, Utils.VariableTypeToString(Typ)));
                    }
                    break;
            }
        }
    }
}