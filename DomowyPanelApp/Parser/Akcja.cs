using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    abstract class Akcja : IComparable<Akcja>
    {
        public int Priorytet = 0;
        public abstract object Wykonaj();

        public static Akcja Parsuj(ITokenizer tokenizer)
        {
            return null;
        }

        #region IComparable<Akcja> Members

        public int CompareTo(Akcja other)
        {
            return Priorytet.CompareTo(other.Priorytet);
        }

        #endregion

        public virtual void Check()
        {
            
        }

        public abstract void PrzypiszReferencje(ISlownikSkryptu slownik);

        public abstract void PrzypiszReferencje(ISlownikFunkcjiModulow slownik);
    }
}
