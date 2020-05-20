using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Stala : Wartosc
    {
        public override object UstawWartosc(object wartosc)
        {
            throw new ApplicationException(UI.Language.Instance.GetString(UI.UIStrings.CannotChangeConstantValue));
        }

        public override object Wykonaj()
        {
            return _Wartosc;
        }

        public new static Stala Parsuj(ITokenizer tokenizer)
        {
            Stala stala = new Stala();
            HCPSToken token = tokenizer.Next();
            if (token == HCPSToken.Variable)
            {
                stala.Nazwa = tokenizer.Value;
                if (tokenizer.Next() == HCPSToken.BlockBegin)
                {
                    while ((token = tokenizer.Next()) != HCPSToken.BlockEnd)
                    {
                        SimpleAssignment sa = tokenizer.ReadSimpleAssignment(token, tokenizer.Value);
                        if (sa == null || sa.Left == null || sa.Right == null)
                        {
                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectConstantDefinition), stala.Nazwa));
                        }
                        switch (sa.Left.ToLowerInvariant())
                        {
                            case "type":
                                stala.Typ = Utils.VariableTypeFromString(sa.Right);
                                switch (stala.Typ)
                                {
                                    case HomeSimCockpitSDK.VariableType.Bool:
                                    case HomeSimCockpitSDK.VariableType.Int:
                                    case HomeSimCockpitSDK.VariableType.Double:
                                    case HomeSimCockpitSDK.VariableType.String:
                                        break;

                                    default:
                                        throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InvalidConstantType), Utils.VariableTypeToString(stala.Typ), stala.Nazwa));
                                }
                                break;

                            case "value":
                                stala._Wartosc = sa.Right;
                                break;

                            default:
                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InvalidConstantDefinitionProperty), sa.Left, stala.Nazwa, tokenizer.GetPosition(tokenizer.Position)));
                        }
                    }
                    stala.Check();
                    return stala;
                }
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotParseConstantDefinition), tokenizer.GetPosition(tokenizer.Position)));
        }

        public static bool IsDouble(string value, out double v)
        {
            v = 0;
            if (!string.IsNullOrEmpty(value))
            {
                //throw new Exception("Do poprawienia i sprawdzenia: NumberStyles.Float, NumberFormatInfo.InvariantInfo");
                //value = value.Replace(".", System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                if (value.EndsWith("d") || value.EndsWith("D") || value.IndexOf(".") > -1)
                {
                    value = value.TrimEnd('d', 'D');
                    return double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out v);
                }                
            }
            return false;
        }

        public static bool IsInt(string value, out int v)
        {
            v = 0;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.StartsWith("0x") || value.StartsWith("0X"))
                {
                    try
                    {
                        v = int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return int.TryParse(value, out v);
            }
            return false;
        }

        public static bool IsBool(string value, out bool v)
        {
            v = value == "true";
            return value == "true" || value == "false";
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {

        }

        public override void Check()
        {
            base.Check();
            if (_Wartosc == null)
            {
                throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.MissingConstantValue), Nazwa));
            }
            else
            {
                if (Typ == HomeSimCockpitSDK.VariableType.Double)
                {
                    double d = 0;
                    if (!Stala.IsDouble((string)_Wartosc, out d))
                    {
                        throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectNumberFormatTypeDoubleForConstantInitialization), (string)_Wartosc, Nazwa));
                    }
                    _Wartosc = d;
                }
                else if (Typ == HomeSimCockpitSDK.VariableType.Int)
                {
                    int i = 0;
                    if (!Stala.IsInt((string)_Wartosc, out i))
                    {
                        throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectNumberFormatTypeIntForConstantInitialization), (string)_Wartosc, Nazwa));
                    }
                    _Wartosc = i;
                }
                else
                {
                    try
                    {
                        _Wartosc = Utils.Parse(_Wartosc, Typ);
                    }
                    catch (Exception ex)
                    {
                        throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InvalidConstantValueType), Nazwa), ex);
                    }
                }
            }
        }
    }
}
