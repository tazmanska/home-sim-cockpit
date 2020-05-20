using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Akcje : Akcja
    {
        public Akcja[] akcje = null;
        public override object Wykonaj()
        {
            object result = null;
            foreach (Akcja a in akcje)
            {
                result = a.Wykonaj();
                if (result is ReturnWFunkcji)
                {
                    return result;
                }
            }
            return result;
        }

        public new static Akcje Parsuj(ITokenizer tokenizer)
        {
            List<Wyrazenie> wyrazenia = new List<Wyrazenie>();
            do
            {
                Wyrazenie wyrazenie = Wyrazenie.Parsuj(tokenizer);
                if (wyrazenie != null)
                {
                    wyrazenia.Add(wyrazenie);
                }
            }
            while (tokenizer.LastToken != HCPSToken.BlockEnd);
            Akcje akcje = new Akcje();
            akcje.akcje = wyrazenia.ToArray();
            akcje.Check();
            return akcje;
            throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotReadExpression), tokenizer.GetPosition(tokenizer.Position)));
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            if (akcje != null)
            {
                foreach (Akcja a in akcje)
                {
                    a.PrzypiszReferencje(slownik);
                }
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            if (akcje != null)
            {
                foreach (Akcja a in akcje)
                {
                    a.PrzypiszReferencje(slownik);
                }
            }
        }
    }
}
