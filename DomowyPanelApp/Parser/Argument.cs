using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Argument : Wyrazenie, IComparable<Argument>
    {
        public Wyrazenie argument = null;

        public override object Wykonaj()
        {
            return argument.Wykonaj();
        }

        public new static Argument Parsuj(ITokenizer tokenizer)
        {
            Argument argument = new Argument();
            argument.argument = Wyrazenie.Parsuj(tokenizer);
            if (argument.argument == null)
            {
                return null;
            }
            HCPSToken token = tokenizer.Next();
            if (token != HCPSToken.CommaSeparator && token != HCPSToken.ParenthesisClose)
            {
                tokenizer.Back();
            }
            argument.Check();
            return argument;
        }

        public int index = 0;

        #region IComparable<Argument> Members

        public int CompareTo(Argument other)
        {
            return index.CompareTo(other.index);
        }

        #endregion

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (argument is ZmiennaNieistniejaca)
            {
                argument = slownik.PobierzWartosc(((ZmiennaNieistniejaca)argument).Nazwa);
            }
            else
            {
                argument.PrzypiszReferencje(slownik);
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            argument.PrzypiszReferencje(slownik);
        }

        public override string ToString()
        {
            if (argument != null)
            {
                return argument.ToString();
            }
            return base.ToString();
        }
        
        public static Argument[] ObjectArrayToArguments(object [] arguments)
        {
            Argument [] result = new Argument[arguments == null ? 0 : arguments.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new Argument();
                result[i].argument = new Stala()
                {
                    _Wartosc = arguments[i]
                };
            }
            return result;
        }
    }
}
