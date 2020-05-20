using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    abstract class Wyrazenie : Akcja
    {
        public new static Wyrazenie Parsuj(ITokenizer tokenizer)
        {
            long position = tokenizer.Position;
            HCPSToken token = tokenizer.Next();            
            while (token == HCPSToken.Comment)
            {
                position = tokenizer.Position;
                token = tokenizer.Next();
            }            
            switch (token)
            {
                case HCPSToken.Word:
                    {
                        switch (tokenizer.Value)
                        {
                            case "if":
                                return Warunek.Parsuj(tokenizer);

                            case "null":
                                {
                                    Stala stala = new Stala()
                                    {
                                        _Wartosc = null
                                    };
                                    token = tokenizer.Next();
                                    switch (token)
                                    {
                                        case HCPSToken.Bigger:
                                        case HCPSToken.BiggerOrEqual:
                                        case HCPSToken.Equal:
                                        case HCPSToken.NotEqual:
                                        case HCPSToken.Smaller:
                                        case HCPSToken.SmallerOrEqual:
                                            {

                                                switch (token)
                                                {
                                                    case HCPSToken.Bigger:
                                                    case HCPSToken.BiggerOrEqual:
                                                    case HCPSToken.Equal:
                                                    case HCPSToken.NotEqual:
                                                    case HCPSToken.Smaller:
                                                    case HCPSToken.SmallerOrEqual:
                                                        Jezeli jezeli = new Jezeli();
                                                        jezeli.WyrazenieLewe = stala;
                                                        jezeli.WarunekLogiczny = new WarunekLogiczny()
                                                        {
                                                            Typ = WarunekLogiczny.TokenNaWarunek(token)
                                                        };
                                                        jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                                                        return jezeli;

                                                    default:
                                                        tokenizer.Back();
                                                        break;
                                                }

                                                tokenizer.Back();
                                                return stala;
                                            }
                                        case HCPSToken.InstructionEnd:
                                        case HCPSToken.ParenthesisClose:
                                        case HCPSToken.CommaSeparator:
                                            {
                                                tokenizer.Back();
                                                break;
                                            }
                                    }
                                    return stala;
                                }

                            case "return":
                                {
                                    Return r = new Return();
                                    r.Wyrazenie = Wyrazenie.Parsuj(tokenizer);
                                    return r;
                                }

                            default:
                                {
                                    string wartosc = tokenizer.Value;
                                    token = tokenizer.Next();
                                    if (token == HCPSToken.Colon)
                                    {
                                        token = tokenizer.Next();
                                        if (token != HCPSToken.Word)
                                        {
                                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InvalidTokenAfterModuleName), token, wartosc));
                                        }
                                        tokenizer.Position = position;
                                        FunkcjaModulu funkcjaModulu = FunkcjaModulu.Parsuj(tokenizer);
                                        token = tokenizer.Next();
                                        switch (token)
                                        {
                                            case HCPSToken.Assignment:
                                                {
                                                    throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.OperationNotAllowed));
                                                }
                                            case HCPSToken.InstructionEnd:
                                            case HCPSToken.ParenthesisClose:
                                            case HCPSToken.CommaSeparator:
                                                {
                                                    tokenizer.Back();
                                                    return funkcjaModulu;
                                                }
                                            case HCPSToken.Addition:
                                            case HCPSToken.Division:
                                            case HCPSToken.Multiplication:
                                            case HCPSToken.Subtraction:
                                            case HCPSToken.And:
                                            case HCPSToken.Or:
                                            case HCPSToken.Modulo:
                                                {
                                                    Wyrazenie w = Wyrazenie.Parsuj(tokenizer);
                                                    if (w is Operacja)
                                                    {
                                                        Wyrazenie w1 = ((Operacja)w).OperandLewy;
                                                        ((Operacja)w).OperandLewy = new Operacja()
                                                        {
                                                            OperandLewy = funkcjaModulu,
                                                            OperandPrawy = w1,
                                                            Operator = Operacja.TokenNaOperator(token)
                                                        };
                                                        return w;
                                                    }
                                                    else
                                                    {
                                                        Operacja operacja = new Operacja()
                                                        {
                                                            OperandLewy = funkcjaModulu,
                                                            OperandPrawy = w,
                                                            Operator = Operacja.TokenNaOperator(token)
                                                        };
                                                        return operacja;
                                                    }
                                                }
                                            case HCPSToken.Bigger:
                                            case HCPSToken.BiggerOrEqual:
                                            case HCPSToken.Equal:
                                            case HCPSToken.NotEqual:
                                            case HCPSToken.Smaller:
                                            case HCPSToken.SmallerOrEqual:
                                                {

                                                    switch (token)
                                                    {
                                                        case HCPSToken.Bigger:
                                                        case HCPSToken.BiggerOrEqual:
                                                        case HCPSToken.Equal:
                                                        case HCPSToken.NotEqual:
                                                        case HCPSToken.Smaller:
                                                        case HCPSToken.SmallerOrEqual:
                                                            Jezeli jezeli = new Jezeli();
                                                            jezeli.WyrazenieLewe = funkcjaModulu;
                                                            jezeli.WarunekLogiczny = new WarunekLogiczny()
                                                            {
                                                                Typ = WarunekLogiczny.TokenNaWarunek(token)
                                                            };
                                                            jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                                                            return jezeli;

                                                        default:
                                                            tokenizer.Back();
                                                            break;
                                                    }

                                                    tokenizer.Back();
                                                    return funkcjaModulu;
                                                }
                                        }

                                        return funkcjaModulu;
                                    }
                                    else
                                    if (token == HCPSToken.ParenthesisOpen)
                                    {
                                        // jakaś funkcja
                                        tokenizer.Position = position;
                                        Funkcja funkcja = Funkcja.Parsuj(tokenizer);

                                        token = tokenizer.Next();
                                        switch (token)
                                        {
                                            case HCPSToken.Assignment:
                                                {
                                                    throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.OperationNotAllowed));
                                                }
                                            case HCPSToken.InstructionEnd:
                                            case HCPSToken.ParenthesisClose:
                                            case HCPSToken.CommaSeparator:
                                                {
                                                    tokenizer.Back();
                                                    return funkcja;
                                                }
                                            case HCPSToken.Addition:
                                            case HCPSToken.Division:
                                            case HCPSToken.Multiplication:
                                            case HCPSToken.Subtraction:
                                            case HCPSToken.And:
                                            case HCPSToken.Or:
                                            case HCPSToken.Modulo:
                                                {
                                                    Wyrazenie w = Wyrazenie.Parsuj(tokenizer);
                                                    if (w is Operacja)
                                                    {
                                                        Wyrazenie w1 = ((Operacja)w).OperandLewy;
                                                        ((Operacja)w).OperandLewy = new Operacja()
                                                        {
                                                            OperandLewy = funkcja,
                                                            OperandPrawy = w1,
                                                            Operator = Operacja.TokenNaOperator(token)
                                                        };
                                                        return w;
                                                    }
                                                    else
                                                    {
                                                        Operacja operacja = new Operacja()
                                                        {
                                                            OperandLewy = funkcja,
                                                            OperandPrawy = w,
                                                            Operator = Operacja.TokenNaOperator(token)
                                                        };
                                                        return operacja;
                                                    }
                                                }
                                            case HCPSToken.Bigger:
                                            case HCPSToken.BiggerOrEqual:
                                            case HCPSToken.Equal:
                                            case HCPSToken.NotEqual:
                                            case HCPSToken.Smaller:
                                            case HCPSToken.SmallerOrEqual:
                                                {

                                                    switch (token)
                                                    {
                                                        case HCPSToken.Bigger:
                                                        case HCPSToken.BiggerOrEqual:
                                                        case HCPSToken.Equal:
                                                        case HCPSToken.NotEqual:
                                                        case HCPSToken.Smaller:
                                                        case HCPSToken.SmallerOrEqual:
                                                            Jezeli jezeli = new Jezeli();
                                                            jezeli.WyrazenieLewe = funkcja;
                                                            jezeli.WarunekLogiczny = new WarunekLogiczny()
                                                            {
                                                                Typ = WarunekLogiczny.TokenNaWarunek(token)
                                                            };
                                                            jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                                                            return jezeli;

                                                        default:
                                                            tokenizer.Back();
                                                            break;
                                                    }

                                                    tokenizer.Back();
                                                    return funkcja;
                                                }
                                        }

                                        return funkcja;
                                    }
                                    else
                                    {
                                        tokenizer.Back();
                                    }

                                    Stala stala = null;

                                    bool vb = false;
                                    
                                    if (Stala.IsBool(wartosc, out vb))
                                    {
                                        stala = new Stala()
                                        {
                                            _Wartosc = vb,
                                            Typ = HomeSimCockpitSDK.VariableType.Bool
                                        };
                                    }

                                    if (stala != null)
                                    {
                                        token = tokenizer.Next();
                                        switch (token)
                                        {
                                            case HCPSToken.Assignment:
                                                {
                                                    throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.OperationNotAllowed));
                                                }
                                            case HCPSToken.InstructionEnd:
                                            case HCPSToken.ParenthesisClose:
                                            case HCPSToken.CommaSeparator:
                                                {
                                                    tokenizer.Back();
                                                    return stala;
                                                }
                                            case HCPSToken.Addition:
                                            case HCPSToken.Division:
                                            case HCPSToken.Multiplication:
                                            case HCPSToken.Subtraction:
                                            case HCPSToken.And:
                                            case HCPSToken.Or:
                                            case HCPSToken.Modulo:
                                                {
                                                    Wyrazenie w = Wyrazenie.Parsuj(tokenizer);
                                                    if (w is Operacja)
                                                    {
                                                        Wyrazenie w1 = ((Operacja)w).OperandLewy;
                                                        ((Operacja)w).OperandLewy = new Operacja()
                                                        {
                                                            OperandLewy = stala,
                                                            OperandPrawy = w1,
                                                            Operator = Operacja.TokenNaOperator(token)
                                                        };
                                                        return w;
                                                    }
                                                    else
                                                    {
                                                        Operacja operacja = new Operacja()
                                                        {
                                                            OperandLewy = stala,
                                                            OperandPrawy = w,
                                                            Operator = Operacja.TokenNaOperator(token)
                                                        };
                                                        return operacja;
                                                    }
                                                }
                                            case HCPSToken.Bigger:
                                            case HCPSToken.BiggerOrEqual:
                                            case HCPSToken.Equal:
                                            case HCPSToken.NotEqual:
                                            case HCPSToken.Smaller:
                                            case HCPSToken.SmallerOrEqual:
                                                {

                                                    switch (token)
                                                    {
                                                        case HCPSToken.Bigger:
                                                        case HCPSToken.BiggerOrEqual:
                                                        case HCPSToken.Equal:
                                                        case HCPSToken.NotEqual:
                                                        case HCPSToken.Smaller:
                                                        case HCPSToken.SmallerOrEqual:
                                                            Jezeli jezeli = new Jezeli();
                                                            jezeli.WyrazenieLewe = stala;
                                                            jezeli.WarunekLogiczny = new WarunekLogiczny()
                                                            {
                                                                Typ = WarunekLogiczny.TokenNaWarunek(token)
                                                            };
                                                            jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                                                            return jezeli;

                                                        default:
                                                            tokenizer.Back();
                                                            break;
                                                    }

                                                    tokenizer.Back();
                                                    return stala;
                                                }
                                        }
                                    }

                                    break;
                                }

                        }
                        break;
                    }

                case HCPSToken.Number:
                    {
                        Stala stala = null;
                        double vd = 0;
                        int vi = 0;

                        // sprawdzenie czy to liczba double                                    
                        if (Stala.IsDouble(tokenizer.Value, out vd))
                        {
                            stala = new Stala()
                            {
                                _Wartosc = vd,
                                Typ = HomeSimCockpitSDK.VariableType.Double
                            };
                        }
                        // sprawdzenie czy to liczba int
                        else if (Stala.IsInt(tokenizer.Value, out vi))
                        {
                            stala = new Stala()
                            {
                                _Wartosc = vi,
                                Typ = HomeSimCockpitSDK.VariableType.Int
                            };
                        }

                        if (stala != null)
                        {
                            token = tokenizer.Next();
                            switch (token)
                            {
                                case HCPSToken.Assignment:
                                    {
                                        throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.OperationNotAllowed));
                                    }
                                case HCPSToken.InstructionEnd:
                                case HCPSToken.ParenthesisClose:
                                case HCPSToken.CommaSeparator:
                                    {
                                        tokenizer.Back();
                                        return stala;
                                    }
                                case HCPSToken.Addition:
                                case HCPSToken.Division:
                                case HCPSToken.Multiplication:
                                case HCPSToken.Subtraction:
                                case HCPSToken.And:
                                case HCPSToken.Or:
                                case HCPSToken.Modulo:
                                    {
                                        Wyrazenie w = Wyrazenie.Parsuj(tokenizer);
                                        if (w is Operacja)
                                        {
                                            Wyrazenie w1 = ((Operacja)w).OperandLewy;
                                            OperatorDwuargumentowyTyp oper = Operacja.TokenNaOperator(token);
                                            Operacja oo = new Operacja()
                                            {
                                                OperandLewy = stala,
                                                OperandPrawy = w,
                                                Operator = oper
                                            };
                                            return oo;
                                        }
                                        else
                                        {
                                            Operacja operacja = new Operacja()
                                            {
                                                OperandLewy = stala,
                                                OperandPrawy = w,
                                                Operator = Operacja.TokenNaOperator(token)
                                            };
                                            return operacja;
                                        }
                                    }
                                case HCPSToken.Bigger:
                                case HCPSToken.BiggerOrEqual:
                                case HCPSToken.Equal:
                                case HCPSToken.NotEqual:
                                case HCPSToken.Smaller:
                                case HCPSToken.SmallerOrEqual:
                                    {

                                        switch (token)
                                        {
                                            case HCPSToken.Bigger:
                                            case HCPSToken.BiggerOrEqual:
                                            case HCPSToken.Equal:
                                            case HCPSToken.NotEqual:
                                            case HCPSToken.Smaller:
                                            case HCPSToken.SmallerOrEqual:
                                                Jezeli jezeli = new Jezeli();
                                                jezeli.WyrazenieLewe = stala;
                                                jezeli.WarunekLogiczny = new WarunekLogiczny()
                                                {
                                                    Typ = WarunekLogiczny.TokenNaWarunek(token)
                                                };
                                                jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                                                return jezeli;

                                            default:
                                                tokenizer.Back();
                                                break;
                                        }

                                        tokenizer.Back();
                                        return stala;
                                    }
                            }
                        }

                        break;
                    }

                case HCPSToken.String:
                    Stala ss = new Stala()
                    {
                        _Wartosc = tokenizer.Value,
                        Typ = HomeSimCockpitSDK.VariableType.String
                    };

                    token = tokenizer.Next();
                    switch (token)
                    {
                        case HCPSToken.InstructionEnd:
                        case HCPSToken.ParenthesisClose:
                        case HCPSToken.CommaSeparator:
                            {
                                tokenizer.Back();
                                return ss;
                            }
                        case HCPSToken.Addition:
                            {
                                Wyrazenie w = Wyrazenie.Parsuj(tokenizer);
                                if (w is Operacja)
                                {
                                    Wyrazenie w1 = ((Operacja)w).OperandLewy;
                                    ((Operacja)w).OperandLewy = new Operacja()
                                    {
                                        OperandLewy = ss,
                                        OperandPrawy = w1,
                                        Operator = Operacja.TokenNaOperator(token)
                                    };
                                    return w;
                                }
                                else
                                {
                                    Operacja operacja = new Operacja()
                                    {
                                        OperandLewy = ss,
                                        OperandPrawy = w,
                                        Operator = Operacja.TokenNaOperator(token)
                                    };
                                    return operacja;
                                }
                            }
                        case HCPSToken.Bigger:
                        case HCPSToken.BiggerOrEqual:
                        case HCPSToken.Equal:
                        case HCPSToken.NotEqual:
                        case HCPSToken.Smaller:
                        case HCPSToken.SmallerOrEqual:
                            {
                                switch (token)
                                {
                                    case HCPSToken.Bigger:
                                    case HCPSToken.BiggerOrEqual:
                                    case HCPSToken.Equal:
                                    case HCPSToken.NotEqual:
                                    case HCPSToken.Smaller:
                                    case HCPSToken.SmallerOrEqual:
                                        Jezeli jezeli = new Jezeli();
                                        jezeli.WyrazenieLewe = ss;
                                        jezeli.WarunekLogiczny = new WarunekLogiczny()
                                        {
                                            Typ = WarunekLogiczny.TokenNaWarunek(token)
                                        };
                                        jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                                        return jezeli;

                                    default:
                                        tokenizer.Back();
                                        break;
                                }

                                tokenizer.Back();
                                return ss;
                            }
                    }

                    break;

                case HCPSToken.Variable:
                    {
                        string zmienna = tokenizer.Value;
                        token = tokenizer.Next();
                        switch (token)
                        {
                            case HCPSToken.Assignment:
                                {
                                    Przypisanie przypisanie = new Przypisanie();
                                    przypisanie.Zmienna = new ZmiennaNieistniejaca()
                                    {
                                        Nazwa = zmienna
                                    };
                                    przypisanie.Wyrazenie = Wyrazenie.Parsuj(tokenizer);

                                    token = tokenizer.Next();
                                    switch (token)
                                    {
                                        case HCPSToken.Bigger:
                                        case HCPSToken.BiggerOrEqual:
                                        case HCPSToken.Equal:
                                        case HCPSToken.NotEqual:
                                        case HCPSToken.Smaller:
                                        case HCPSToken.SmallerOrEqual:
                                            Jezeli jezeli = new Jezeli();
                                            jezeli.WyrazenieLewe = przypisanie.Wyrazenie;
                                            jezeli.WarunekLogiczny = new WarunekLogiczny()
                                                {
                                                    Typ = WarunekLogiczny.TokenNaWarunek(token)
                                                };
                                            jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);
                                            przypisanie.Wyrazenie = jezeli;
                                            break;

                                        default:
                                            tokenizer.Back();
                                            break;
                                    }

                                    przypisanie.Check();
                                    return przypisanie;
                                }
                            case HCPSToken.InstructionEnd:
                            case HCPSToken.ParenthesisClose:
                            case HCPSToken.CommaSeparator:
                                {
                                    tokenizer.Back();
                                    return new ZmiennaNieistniejaca()
                                    {
                                        Nazwa = zmienna
                                    };
                                }
                            case HCPSToken.Addition:
                            case HCPSToken.Division:
                            case HCPSToken.Multiplication:
                            case HCPSToken.Subtraction:
                            case HCPSToken.And:
                            case HCPSToken.Or:
                            case HCPSToken.Modulo:
                                {

                                    Wyrazenie w = Wyrazenie.Parsuj(tokenizer);
                                    if (w is Operacja)
                                    {
                                        Wyrazenie w1 = ((Operacja)w).OperandLewy;
                                        OperatorDwuargumentowyTyp oper = Operacja.TokenNaOperator(token);
                                        Operacja oo = new Operacja()
                                        {
                                            OperandLewy = new ZmiennaNieistniejaca()
                                            {
                                                Nazwa = zmienna
                                            },
                                            OperandPrawy = w,
                                            Operator = oper
                                        };
                                        return oo;
                                    }
                                    else
                                    {
                                        Operacja operacja = new Operacja()
                                        {
                                            OperandLewy = new ZmiennaNieistniejaca()
                                            {
                                                Nazwa = zmienna
                                            },
                                            OperandPrawy = w,
                                            Operator = Operacja.TokenNaOperator(token)
                                        };
                                        return operacja;
                                    }
                                }
                            case HCPSToken.Bigger:
                            case HCPSToken.BiggerOrEqual:
                            case HCPSToken.Equal:
                            case HCPSToken.NotEqual:
                            case HCPSToken.Smaller:
                            case HCPSToken.SmallerOrEqual:
                                {
                                    ZmiennaNieistniejaca zn = new ZmiennaNieistniejaca()
                                    {
                                        Nazwa = zmienna
                                    };
                                    switch (token)
                                    {
                                        case HCPSToken.Bigger:
                                        case HCPSToken.BiggerOrEqual:
                                        case HCPSToken.Equal:
                                        case HCPSToken.NotEqual:
                                        case HCPSToken.Smaller:
                                        case HCPSToken.SmallerOrEqual:
                                            Jezeli jezeli = new Jezeli();
                                            jezeli.WyrazenieLewe = zn;
                                            jezeli.WarunekLogiczny = new WarunekLogiczny()
                                            {
                                                Typ = WarunekLogiczny.TokenNaWarunek(token)
                                            };
                                            jezeli.WyrazeniePrawe = Wyrazenie.Parsuj(tokenizer);                                            
                                            return jezeli;

                                        default:
                                            tokenizer.Back();
                                            break;
                                    }

                                    tokenizer.Back();
                                    return zn;
                                }
                        }

                        break;
                    }

                case HCPSToken.BlockEnd:
                    return null;

                case HCPSToken.ParenthesisClose:
                    return null;

                case HCPSToken.InstructionEnd:
                    return null;

                case HCPSToken.CommaSeparator:
                    return null;

                case HCPSToken.Comment:
                    return Parsuj(tokenizer);
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotReadExpression2), token, tokenizer.Value, tokenizer.GetPosition(tokenizer.Position)));
        }

        public override object Wykonaj()
        {
            throw new NotImplementedException();
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            //throw new NotImplementedException();
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            //throw new NotImplementedException();
        }
    }
}