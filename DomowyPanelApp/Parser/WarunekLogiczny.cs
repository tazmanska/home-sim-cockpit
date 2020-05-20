using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class WarunekLogiczny
    {
        public WarunekLogicznyTyp Typ = WarunekLogicznyTyp.Nieznany;

        public bool CzyPrawda(object lewy, object prawy)
        {
            return WarunekLogiczny.CzyPrawda(Typ, lewy, prawy);
        }

        public static bool CzyPrawda(WarunekLogicznyTyp typ, object lewy, object prawy)
        {
            if (lewy == null && prawy != null)
            {
                switch (typ)
                {
                    case WarunekLogicznyTyp.Rowny:
                        return false;

                    case WarunekLogicznyTyp.Rozny:
                        return true;

                    default:
                        throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotCheckStatementForNullValue), typ));
                }
            }
            if (prawy == null)
            {
                switch (typ)
                {
                    case WarunekLogicznyTyp.Rowny:
                        return lewy == null;

                    case WarunekLogicznyTyp.Rozny:
                        return lewy != null;

                    default:
                        throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotCheckStatementForNullValue), typ));
                }
            }

            if (lewy is bool && prawy is bool)
            {
                return CzyPrawda(typ, (bool)lewy, (bool)prawy);
            }

            if (lewy is double && (prawy is double || prawy is int))
            {
                return CzyPrawda(typ, (double)lewy, Convert.ToDouble(prawy));
            }

            if (lewy is int && (prawy is int || prawy is double))
            {
                return CzyPrawda(typ, (int)lewy, Convert.ToInt32(prawy));
            }

            if (lewy is string && prawy is string)
            {
                return CzyPrawda(typ, (string)lewy, (string)prawy);
            }

            if (lewy.GetType() == Utils.__boolArrayType && prawy.GetType() == Utils.__boolArrayType)
            {
                return CzyPrawda<bool>(typ, (bool[])lewy, (bool[])prawy);
            }

            if (lewy.GetType() == Utils.__intArrayType && prawy.GetType() == Utils.__intArrayType)
            {
                return CzyPrawda<int>(typ, (int[])lewy, (int[])prawy);
            }

            if (lewy.GetType() == Utils.__doubleArrayType && prawy.GetType() == Utils.__doubleArrayType)
            {
                return CzyPrawda<double>(typ, (double[])lewy, (double[])prawy);
            }

            if (lewy.GetType() == Utils.__stringArrayType && prawy.GetType() == Utils.__stringArrayType)
            {
                return CzyPrawda<string>(typ, (string[])lewy, (string[])prawy);
            }

            if (lewy.GetType() == Utils.__arrayType && prawy.GetType() == Utils.__arrayType)
            {
                object[] tl = (object[])lewy;
                object[] tp = (object[])prawy;
                return CzyPrawda(typ, tl, tp);
            }

            throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotCheckStatementForNullValue), lewy.GetType()));
        }

        public static bool CzyPrawda(WarunekLogicznyTyp typ, object[] lewy, object[] prawy)
        {
            switch (typ)
            {
                case WarunekLogicznyTyp.Rowny:
                    {
                        if (lewy == prawy)
                        {
                            return true;
                        }
                        if (lewy == null || prawy == null)
                        {
                            return false;
                        }
                        if (lewy.Length != prawy.Length)
                        {
                            return false;
                        }
                        for (int i = 0; i < lewy.Length; i++)
                        {
                            if (CzyPrawda(WarunekLogicznyTyp.Rozny, lewy[i], prawy[i]))
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                case WarunekLogicznyTyp.Rozny:
                    return !CzyPrawda(WarunekLogicznyTyp.Rowny, lewy, prawy);

                default:
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedStatementTypeForArray), typ));
            }
        }

        public static bool CzyPrawda<T>(WarunekLogicznyTyp typ, T[] lewy, T[] prawy)
        {
            switch (typ)
            {
                case WarunekLogicznyTyp.Rowny:
                    {
                        if (lewy == prawy)
                        {
                            return true;
                        }
                        if (lewy == null || prawy == null)
                        {
                            return false;
                        }
                        if (lewy.Length != prawy.Length)
                        {
                            return false;
                        }
                        for (int i = 0; i < lewy.Length; i++)
                        {
                            if (lewy[i] == null)
                            {
                                if (prawy[i] != null)
                                {
                                    return false;
                                }
                            }
                            if (!lewy[i].Equals(prawy[i]))
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                case WarunekLogicznyTyp.Rozny:
                    return !CzyPrawda<T>(WarunekLogicznyTyp.Rowny, lewy, prawy);

                default:
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedStatementTypeForArray), typ));
            }
        }

        public static bool CzyPrawda(WarunekLogicznyTyp typ, bool lewy, bool prawy)
        {
            switch (typ)
            {
                case WarunekLogicznyTyp.Rowny:
                    return lewy == prawy;

                case WarunekLogicznyTyp.Rozny:
                    return lewy != prawy;

                default:
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedStatementTypeForValueType), typ, Utils.VariableTypeToString(HomeSimCockpitSDK.VariableType.Bool)));
            }
        }

        public static bool CzyPrawda(WarunekLogicznyTyp typ, int lewy, int prawy)
        {
            switch (typ)
            {
                case WarunekLogicznyTyp.Rowny:
                    return lewy == prawy;

                case WarunekLogicznyTyp.Rozny:
                    return lewy != prawy;

                case WarunekLogicznyTyp.Mniejszy:
                    return lewy < prawy;

                case WarunekLogicznyTyp.MniejszyLubRowny:
                    return lewy <= prawy;

                case WarunekLogicznyTyp.Wiekszy:
                    return lewy > prawy;

                case WarunekLogicznyTyp.WiekszyLubRowny:
                    return lewy >= prawy;

                default:
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedStatementTypeForValueType), typ, Utils.VariableTypeToString(HomeSimCockpitSDK.VariableType.Int)));
            }
        }

        public static bool CzyPrawda(WarunekLogicznyTyp typ, double lewy, double prawy)
        {
            switch (typ)
            {
                case WarunekLogicznyTyp.Rowny:
                    return lewy == prawy;

                case WarunekLogicznyTyp.Rozny:
                    return lewy != prawy;

                case WarunekLogicznyTyp.Mniejszy:
                    return lewy < prawy;

                case WarunekLogicznyTyp.MniejszyLubRowny:
                    return lewy <= prawy;

                case WarunekLogicznyTyp.Wiekszy:
                    return lewy > prawy;

                case WarunekLogicznyTyp.WiekszyLubRowny:
                    return lewy >= prawy;

                default:
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedStatementTypeForValueType), typ, Utils.VariableTypeToString(HomeSimCockpitSDK.VariableType.Double)));
            }
        }

        public static bool CzyPrawda(WarunekLogicznyTyp typ, string lewy, string prawy)
        {
            switch (typ)
            {
                case WarunekLogicznyTyp.Rowny:
                    return lewy == prawy;

                case WarunekLogicznyTyp.Rozny:
                    return lewy != prawy;

                default:
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedStatementTypeForValueType), typ, Utils.VariableTypeToString(HomeSimCockpitSDK.VariableType.String)));
            }
        }

        public static WarunekLogicznyTyp TokenNaWarunek(HCPSToken token)
        {
            switch (token)
            {
                case HCPSToken.Bigger:
                    return WarunekLogicznyTyp.Wiekszy;

                case HCPSToken.BiggerOrEqual:
                    return WarunekLogicznyTyp.WiekszyLubRowny;

                case HCPSToken.Equal:
                    return WarunekLogicznyTyp.Rowny;

                case HCPSToken.NotEqual:
                    return WarunekLogicznyTyp.Rozny;

                case HCPSToken.Smaller:
                    return WarunekLogicznyTyp.Mniejszy;

                case HCPSToken.SmallerOrEqual:
                    return WarunekLogicznyTyp.MniejszyLubRowny;

                default:
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnrecognizedStatementType), token));
            }
        }
    }
}
