using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitSDK;

namespace HomeSimCockpit.Parser
{
    static class Utils
    {
        public static object ConvertTo(object value, VariableType typ)
        {
            switch (typ)
            {
                case VariableType.Bool:
                    return (bool)value;

                case VariableType.Bool_Array:
                    return (bool[])value;

                case VariableType.Int:
                    if (value is double)
                    {
                        value = (int)(double)value;
                    }
                    return (int)value;

                case VariableType.Int_Array:
                    return (int[])value;

                case VariableType.Double:
                    if (value is int)
                    {
                        value = (double)(int)value;
                    }
                    return (double)value;

                case VariableType.Double_Array:
                    return (double[])value;

                case VariableType.String:
                    return value;

                case VariableType.String_Array:
                    return (string[])value;

                case VariableType.Array:
                    return (object[])value;

                default:
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedValueType), typ));
            }
        }

        public static object Parse(object value, VariableType typ)
        {
            if (!(value is string))
            {
                throw new CheckingScriptException(UI.Language.Instance.GetString(UI.UIStrings.ValueCanBeParsedOnlyFromString));
            }
            switch (typ)
            {
                case VariableType.Bool:
                    if ((string)value == "true" || (string)value == "false")
                    {
                        return (string)value == "true";
                    }
                    throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectBoolValue), value, typ));

                case VariableType.String:
                    return (string)value;

                default:
                    throw new CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedTypeParsing), typ));
            }
        }

        public static readonly Type __boolArrayType = typeof(bool[]);
        public static readonly Type __intArrayType = typeof(int[]);
        public static readonly Type __doubleArrayType = typeof(double[]);
        public static readonly Type __stringArrayType = typeof(string[]);
        public static readonly Type __arrayType = typeof(object[]);

        public static bool CzyPoprawnaWartosc(object value, VariableType typ)
        {
            switch (typ)
            {
                case VariableType.Bool:
                    return value is bool;

                case VariableType.Bool_Array:
                    return value.GetType() == __boolArrayType;

                case VariableType.Int:
                    return value is int || value is double;

                case VariableType.Int_Array:
                    return value.GetType() == __intArrayType;

                case VariableType.Double:
                    return value is double || value is int;

                case VariableType.Double_Array:
                    return value.GetType() == __doubleArrayType;

                case VariableType.String:
                    return value is string;

                case VariableType.String_Array:
                    return value.GetType() == __stringArrayType;

                case VariableType.Array:
                    return value.GetType() == __arrayType;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Metoda sprawdza typ wartości.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VariableType SprawdzTyp(object value)
        {
            if (value == null)
            {
                return VariableType.Unknown;
            }

            if (value is bool)
            {
                return VariableType.Bool;
            }

            if (value is int)
            {
                return VariableType.Int;
            }

            if (value is double)
            {
                return VariableType.Double;
            }

            if (value is string)
            {
                return VariableType.String;
            }

            if (value.GetType() == __boolArrayType)
            {
                return VariableType.Bool_Array;
            }

            if (value.GetType() == __intArrayType)
            {
                return VariableType.Int_Array;
            }

            if (value.GetType() == __doubleArrayType)
            {
                return VariableType.Double_Array;
            }
            
            if (value.GetType() == __stringArrayType)
            {
                return VariableType.String_Array;
            }

            if (value.GetType() == __arrayType)
            {
                return VariableType.Array;
            }

            return VariableType.Unknown;
        }

        public static KierunekZmiennej KierunekZmiennejFromString(string text)
        {
            switch (text)
            {
                case "in":
                    return KierunekZmiennej.In;

                case "out":
                    return KierunekZmiennej.Out;

                case "none":
                    return KierunekZmiennej.None;

                default:
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedDirectionType), text));
            }
        }

        public static string KierunekZmiennejToString(KierunekZmiennej kierunek)
        {
            return kierunek.ToString().ToLowerInvariant();
        }

        public static VariableType VariableTypeFromString(string text)
        {
            switch (text)
            {
                case "bool":
                    return VariableType.Bool;

                case "bool_array":
                    return VariableType.Bool_Array;

                case "int":
                    return VariableType.Int;

                case "int_array":
                    return VariableType.Int_Array;

                case "double":
                    return VariableType.Double;

                case "double_array":
                    return VariableType.Double_Array;

                case "string":
                    return VariableType.String;

                case "string_array":
                    return VariableType.String_Array;

                case "array":
                    return VariableType.Array;

                default:
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedValueType), text));
            }
        }

        public static string VariableTypeToString(VariableType type)
        {
            return type.ToString().ToLowerInvariant();
        }

        public static int PriorytetOperatora(OperatorDwuargumentowyTyp o)
        {
            if (o == OperatorDwuargumentowyTyp.Odejmij)
            {
                return (int)OperatorDwuargumentowyTyp.Dodaj;
            }
            if (o == OperatorDwuargumentowyTyp.Podziel)
            {
                return (int)OperatorDwuargumentowyTyp.Pomnoz;
            }
            return (int)o;
        }
    }
}
