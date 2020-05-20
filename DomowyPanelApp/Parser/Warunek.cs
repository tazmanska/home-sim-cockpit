using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HomeSimCockpit.Parser
{
    class Warunek : Wyrazenie
    {
        public Jezeli Jezeli = null;
        public Akcje Prawda = null;
        public Akcje Falsz = null;

        public override object Wykonaj()
        {
            if (Jezeli.CzyPrawda())
            {
                if (Prawda != null)
                {
                    return Prawda.Wykonaj();
                }
            }
            else
            {
                if (Falsz != null)
                {
                    return Falsz.Wykonaj();
                }
            }
            return null;
        }

        public new static Warunek Parsuj(ITokenizer tokenizer)
        {
            Warunek warunek = new Warunek();
            if (tokenizer.Next() == HCPSToken.ParenthesisOpen)
            {
                warunek.Jezeli = Jezeli.Parsuj(tokenizer);
                if (tokenizer.Next() == HCPSToken.ParenthesisClose && tokenizer.Next() == HCPSToken.BlockBegin)
                {
                    warunek.Prawda = Akcje.Parsuj(tokenizer);
                    if (tokenizer.Next() == HCPSToken.Word && tokenizer.Value == "else")
                    {
                        if (tokenizer.Next() == HCPSToken.BlockBegin)
                        {
                            warunek.Falsz = Akcje.Parsuj(tokenizer);
                            if (tokenizer.LastToken != HCPSToken.BlockEnd)
                            {
                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectElseStatementInIfStatementDefinition), tokenizer.GetPosition(tokenizer.Position)));
                            }
                            if (tokenizer.Next() != HCPSToken.BlockEnd)
                            {
                                tokenizer.Back();
                            }
                        }
                        else
                        {
                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectElseStatementInIfStatementDefinition), tokenizer.GetPosition(tokenizer.Position)));
                        }
                    }
                    else
                    {
                        if (tokenizer.LastToken != HCPSToken.InstructionEnd && tokenizer.LastToken != HCPSToken.BlockEnd)
                        {
                            tokenizer.Back();
                        }
                    }
                    warunek.Check();
                    return warunek;
                }
            }
            throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.IncorrectIfStatementDefinition));
        }

        public override void PrzypiszReferencje(ISlownikSkryptu slownik)
        {
            Jezeli.PrzypiszReferencje(slownik);
            if (Prawda != null)
            {
                Prawda.PrzypiszReferencje(slownik);
            }
            if (Falsz != null)
            {
                Falsz.PrzypiszReferencje(slownik);
            }
        }

        public override void PrzypiszReferencje(ISlownikFunkcjiModulow slownik)
        {
            base.PrzypiszReferencje(slownik);
            Jezeli.PrzypiszReferencje(slownik);
            if (Prawda != null)
            {
                Prawda.PrzypiszReferencje(slownik);
            }
            if (Falsz != null)
            {
                Falsz.PrzypiszReferencje(slownik);
            }
        }
    }
}
