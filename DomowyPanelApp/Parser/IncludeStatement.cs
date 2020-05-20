/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-02-25
 * Godzina: 22:26
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace HomeSimCockpit.Parser
{
    /// <summary>
    /// Description of IncludeStatement.
    /// </summary>
    class IncludeStatement
    {
        private IncludeStatement()
        {
        }
        
        public string FilePath
        {
            get;
            internal set;
        }
        
        public string ScriptName
        {
            get;
            private set;
        }
        
        public static IncludeStatement Parsuj(ITokenizer tokenizer)
        {
            IncludeStatement result = new IncludeStatement();
            HCPSToken token = tokenizer.Next();
            if (token == HCPSToken.BlockBegin)
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
                        throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.IncorrectIncludeDefinition));
                    }
                    switch (sa.Left.ToLowerInvariant())
                    {
                        case "file":
                            result.FilePath = sa.Right;
                            break;

                        case "script":
                            result.ScriptName = sa.Right;
                            break;

                        default:
                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InvalidIncludePropertyDefinition), sa.Left, tokenizer.GetPosition(tokenizer.Position)));
                    }
                }
                // sprawdzenie czy file i script są różne od null i ""
                if (string.IsNullOrEmpty(result.FilePath) || string.IsNullOrEmpty(result.ScriptName))
                {
                    throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.MissingFileOrScriptPropertyInIncludeDefinition));
                }
                return result;
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotReadIncludeDefinition), tokenizer.GetPosition(tokenizer.Position)));
        }
    }
}
