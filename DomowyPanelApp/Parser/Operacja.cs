using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Operacja : Wyrazenie, IComparable<Operacja>
    {
        public Wyrazenie OperandLewy = null;
        public Wyrazenie OperandPrawy = null;
        public OperatorDwuargumentowyTyp Operator = OperatorDwuargumentowyTyp.Nieznany;

        public override object Wykonaj()
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0} < {1} > {2}", OperandLewy, Operator, OperandPrawy), "Script");
            switch (Operator)
            {
                case OperatorDwuargumentowyTyp.Dodaj:
                    return Dodaj(OperandLewy.Wykonaj(), OperandPrawy.Wykonaj());

                case OperatorDwuargumentowyTyp.Odejmij:
                    return Odejmij(OperandLewy.Wykonaj(), OperandPrawy.Wykonaj());

                case OperatorDwuargumentowyTyp.Pomnoz:
                    return Pomnoz(OperandLewy.Wykonaj(), OperandPrawy.Wykonaj());

                case OperatorDwuargumentowyTyp.Podziel:
                    return Podziel(OperandLewy.Wykonaj(), OperandPrawy.Wykonaj());

                case OperatorDwuargumentowyTyp.I:
                    return I(OperandLewy.Wykonaj(), OperandPrawy.Wykonaj());

                case OperatorDwuargumentowyTyp.Lub:
                    return Lub(OperandLewy.Wykonaj(), OperandPrawy.Wykonaj());

                case OperatorDwuargumentowyTyp.Modulo:
                    return Modulo(OperandLewy.Wykonaj(), OperandPrawy.Wykonaj());

                default:
                    throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedOperator), Operator));
            }
        }

        public static object Dodaj(object lewy, object prawy)
        {
            if (lewy is double && (prawy is double || prawy is int))
            {
                return (double)lewy + ((prawy is int) ? (double)(int)prawy : (double)prawy);
            }
            if (lewy is int && (prawy is int || prawy is double))
            {
                return (int)lewy + ((prawy is int) ? (int)prawy : (int)(double)prawy);
            }
            if (lewy is string && prawy is string)
            {
                return lewy.ToString() + prawy.ToString();
            }
            throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.CannotAddValues));
        }

        public static object Odejmij(object lewy, object prawy)
        {
            if (lewy is double && (prawy is double || prawy is int))
            {
                return (double)lewy - ((prawy is int) ? (double)(int)prawy : (double)prawy);
            }
            if (lewy is int && (prawy is int || prawy is double))
            {
                return (int)lewy - ((prawy is int) ? (int)prawy : (int)(double)prawy);
            }
            throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.CannotSubtractValues));
        }

        public static object Pomnoz(object lewy, object prawy)
        {
            if (lewy is double && (prawy is double || prawy is int))
            {
                return (double)lewy * ((prawy is int) ? (double)(int)prawy : (double)prawy);
            }
            if (lewy is int && (prawy is int || prawy is double))
            {
                return (int)lewy * ((prawy is int) ? (int)prawy : (int)(double)prawy);
            }
            throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.CannotMultiplyValues));
        }

        public static object Podziel(object lewy, object prawy)
        {
            if (lewy is double && (prawy is double || prawy is int))
            {
                return (double)lewy / ((prawy is int) ? (double)(int)prawy : (double)prawy);
            }
            if (lewy is int && (prawy is int || prawy is double))
            {
                return (int)lewy / ((prawy is int) ? (int)prawy : (int)(double)prawy);
            }
            throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.CannotDivideValues));
        }

        public static object I(object lewy, object prawy)
        {
            if (lewy is bool && prawy is bool)
            {
                return (bool)lewy && (bool)prawy;
            }

            if (lewy is int && prawy is int)
            {
                return (int)lewy & (int)prawy;
            }

            throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.CannotANDValues));
        }

        public static object Lub(object lewy, object prawy)
        {
            if (lewy is bool && prawy is bool)
            {
                return (bool)lewy || (bool)prawy;
            }

            if (lewy is int && prawy is int)
            {
                return (int)lewy | (int)prawy;
            }

            throw new ExecutingScriptException(UI.Language.Instance.GetString(UI.UIStrings.CannotORValues));
        }

        public static object Modulo(object lewy, object prawy)
        {
            if (lewy is int && prawy is int)
            {
                return (int)lewy % (int)prawy;
            }

            throw new ExecutingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotMODValues), lewy, lewy != null ? lewy.GetType().ToString() : "(null)", prawy, prawy != null ? prawy.GetType().ToString() : "(null)"));
        }

        public static OperatorDwuargumentowyTyp TokenNaOperator(HCPSToken token)
        {
            switch (token)
            {
                case HCPSToken.Addition:
                    return OperatorDwuargumentowyTyp.Dodaj;

                case HCPSToken.Subtraction:
                    return OperatorDwuargumentowyTyp.Odejmij;

                case HCPSToken.Multiplication:
                    return OperatorDwuargumentowyTyp.Pomnoz;

                case HCPSToken.Division:
                    return OperatorDwuargumentowyTyp.Podziel;

                case HCPSToken.And:
                    return OperatorDwuargumentowyTyp.I;

                case HCPSToken.Or:
                    return OperatorDwuargumentowyTyp.Lub;

                case HCPSToken.Modulo:
                    return OperatorDwuargumentowyTyp.Modulo;

                default:
                    return OperatorDwuargumentowyTyp.Nieznany;
            }
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (OperandLewy == null || OperandPrawy == null)
            {
                throw new CheckingScriptException();
            }
            if (OperandLewy is ZmiennaNieistniejaca)
            {
                OperandLewy = /*(Zmienna)*/slownik.PobierzWartosc(((ZmiennaNieistniejaca)OperandLewy).Nazwa);
            }
            else
            {
                OperandLewy.PrzypiszReferencje(slownik);
            }
            if (OperandPrawy is ZmiennaNieistniejaca)
            {
                OperandPrawy = /*(Zmienna)*/slownik.PobierzWartosc(((ZmiennaNieistniejaca)OperandPrawy).Nazwa);
            }
            else
            {
                OperandPrawy.PrzypiszReferencje(slownik);
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            OperandLewy.PrzypiszReferencje(slownik);
            OperandPrawy.PrzypiszReferencje(slownik);
        }

        public override string ToString()
        {
            return string.Format("{0}, ( '{1}' i '{2}' )", Operator, OperandLewy is Operacja ? "(operacja)" : OperandLewy.ToString(), OperandPrawy is Operacja ? "(operacja)" : OperandPrawy.ToString());
        }

        public Wyrazenie GetFirstValueOrLastExpressionAndChangeWithExpression(bool left, Wyrazenie wyrazenie)
        {
            Wyrazenie w = left ? OperandLewy : OperandPrawy;
                if (w is Wartosc)
                {
                    if (left)
                    {
                        OperandLewy = wyrazenie;
                    }
                    else
                    {
                        OperandPrawy = wyrazenie;
                    }
                    return w;
                }
                if (w is Operacja)
                {
                    return ((Operacja)w).GetFirstValueOrLastExpressionAndChangeWithExpression(left, wyrazenie);
                }
            throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.ExpressionNotFound));
        }

        public void PrzypiszOstatniNull(Wyrazenie wyrazenie, bool lewy)
        {
            Wyrazenie w = lewy ? OperandLewy : OperandPrawy;
            if (w == null)
            {
                if (lewy)
                {
                    OperandLewy = wyrazenie;
                }
                else
                {
                    OperandPrawy = wyrazenie;
                }
            }
            else
            {
                if (w is Operacja)
                {
                    ((Operacja)w).PrzypiszOstatniNull(wyrazenie, lewy);
                }
            }
        }

        public Wyrazenie PobierzPierwszeNieWyrazenie(bool lewy)
        {
            Wyrazenie w = lewy ? OperandLewy : OperandPrawy;
            if (w is Operacja)
            {
                return ((Operacja)w).PobierzPierwszeNieWyrazenie(lewy);
            }
            return w;
        }

        public static void OdczytajOperacje(Operacja operacja, List<Operacja> lista)
        {
            lista.Add(operacja);
            if (operacja.OperandLewy is Operacja)
            {
                OdczytajOperacje((Operacja)operacja.OperandLewy, lista);
            }
            if (operacja.OperandPrawy is Operacja)
            {
                OdczytajOperacje((Operacja)operacja.OperandPrawy, lista);
            }
        }

        private static bool SaRozneOperatory(List<Operacja> operacje)
        {
            List<OperatorDwuargumentowyTyp> d = new List<OperatorDwuargumentowyTyp>();
            foreach (Operacja o in operacje)
            {
                if (!d.Contains(o.Operator))
                {
                    d.Add(o.Operator);
                }
            }
            return d.Count > 1;
        }

        public static Operacja Uporzadkuj(Operacja operacja)
        {
            List<Operacja> operacje = new List<Operacja>();
            Operacja.OdczytajOperacje(operacja, operacje);
            if (operacje.Count > 1 && SaRozneOperatory(operacje))
            {                
                // ułożenie operacji wg. priorytetów operatorów
                Operacja root = new Operacja()
                {
                    OperandLewy = operacje[0].OperandLewy,
                    OperandPrawy = operacje[0].OperandPrawy is Operacja ? ((Operacja)operacje[0].OperandPrawy).PobierzPierwszeNieWyrazenie(true) : operacje[0].OperandPrawy,
                    Operator = operacje[0].Operator
                };

                for (int i = 1; i < operacje.Count; i++)
                {
                    Operacja op = operacje[i];
                    if (Utils.PriorytetOperatora(root.Operator) >= Utils.PriorytetOperatora(op.Operator))
                    {
                        root.PrzypiszOstatniNull(op.OperandLewy, false);

                        // nowy root, a stary na lewo8
                        Operacja newRoot = new Operacja()
                        {
                            OperandLewy = root,
                            OperandPrawy = op.OperandPrawy is Operacja ? ((Operacja)op.OperandPrawy).PobierzPierwszeNieWyrazenie(true) : op.OperandPrawy,
                            Operator = op.Operator
                        };
                        root = newRoot;
                    }
                    else
                    {
                        Operacja child = new Operacja()
                        {
                            OperandLewy = root.OperandPrawy,
                            OperandPrawy = op.OperandPrawy is Operacja ? ((Operacja)op.OperandPrawy).PobierzPierwszeNieWyrazenie(true) : op.OperandPrawy,
                            Operator = op.Operator
                        };
                        root.OperandPrawy = child;
                    }
                }

                return root;
            }
            return operacja;
        }

        #region IComparable<Operacja> Members

        public int CompareTo(Operacja other)
        {
            if ((Operator == OperatorDwuargumentowyTyp.Dodaj || Operator == OperatorDwuargumentowyTyp.Odejmij) && (other.Operator == OperatorDwuargumentowyTyp.Dodaj || other.Operator == OperatorDwuargumentowyTyp.Odejmij))
            {
                return 0;
            }
            return Operator.CompareTo(other.Operator);
        }

        #endregion
    }
}
