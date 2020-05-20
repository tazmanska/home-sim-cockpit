using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace HomeSimCockpit.Parser
{
    class Parser
    {
        public static Skrypt[] Parsuj2(IProgress progress, object args)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = UI.Language.CurrentCulture;
            return Parsuj((string)args, progress);
        }
        
        public static Skrypt[] Parsuj(string nazwaPliku, IProgress progress)
        {
            if (progress == null)
            {
                progress = new NullProgress();
            }
            string extenstion = Path.GetExtension(nazwaPliku);
            switch (extenstion.ToLower().Trim())
            {
                case ".hcps":
                    return ParsujHCPS(nazwaPliku, progress, new List<string>(), new Dictionary<string, Skrypt[]>(), true);

                default:
                    throw new Exception(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedScriptFileType));
            }
        }
        
        private static void DodajZmienna(HCPSTokenizer tokenizer, List<Zmienna> zmienne, Zmienna zmienna)
        {
            Zmienna z = zmienne.Find(delegate(Zmienna zz)
                                     {
                                         if (zmienna.Kierunek == KierunekZmiennej.None)
                                         {
                                             return zz.Nazwa == zmienna.Nazwa;
                                         }
                                         else
                                         {
                                             return zz.Modul == zmienna.Modul && zz.ID == zmienna.ID;
                                         }
                                     });
            if (z != null && (z.Kierunek != zmienna.Kierunek || z.Typ != zmienna.Typ || (zmienna.Kierunek != KierunekZmiennej.None && zmienna.Modul != z.Modul) || z.Nazwa != zmienna.Nazwa))
            {
                if (tokenizer == null)
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.VariableRedefinition), zmienna.ID, zmienna.Modul, zmienna.Nazwa));
                }
                else
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.VariableRedefinition2), zmienna.ID, zmienna.Modul, zmienna.Nazwa, tokenizer.GetPosition(tokenizer.Position)));
                }
            }
            if (z == null)
            {
                zmienne.Add(zmienna);
            }
            else
            {
                if (z._funkcjaNazwa == null && zmienna._funkcjaNazwa != null)
                {
                    z._funkcjaNazwa = zmienna._funkcjaNazwa;
                }
            }
        }
        
        private static void DodajStala(HCPSTokenizer tokenizer, List<Stala> stale, Stala stala)
        {
            Stala s = stale.Find(delegate(Stala o)
                                 {
                                     return o.Nazwa == stala.Nazwa;
                                 });
            if (s != null && s.Typ != stala.Typ)
            {
                if (tokenizer == null)
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ConstantRedefinition), stala.Nazwa));
                }
                else
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ConstantRedefinition2), stala.Nazwa, tokenizer.GetPosition(tokenizer.Position)));
                }
            }
            if (s == null)
            {
                stale.Add(stala);
            }
        }
        
        private static void DodajZdarzenie(HCPSTokenizer tokenizer, List<Zdarzenie> zdarzenia, Zdarzenie zdarzenie)
        {
            if (zdarzenie is ZmianaZmiennej)
            {
                DodajZdarzenieZmiennej(tokenizer, zdarzenia, (ZmianaZmiennej)zdarzenie);
                return;
            }
            if (zdarzenie is ZmianaZmiennych)
            {
                DodajZdarzenieZmiennych(tokenizer, zdarzenia, (ZmianaZmiennych)zdarzenie);
                return;
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsupportedEventType), zdarzenie));
        }
        
        private static void DodajZdarzenieZmiennej(HCPSTokenizer tokenizer, List<Zdarzenie> zdarzenia, ZmianaZmiennej zmianaZmiennej)
        {
            Zdarzenie zzz = zdarzenia.Find(delegate(Zdarzenie o)
                                           {
                                               if (o is ZmianaZmiennej)
                                               {
                                                   return ((ZmianaZmiennej)o).Zmienna.Nazwa == zmianaZmiennej.Zmienna.Nazwa;
                                               }
                                               return false;
                                           });
            if (zzz != null)
            {
                // połączenie zdarzeń
                ((ZmianaZmiennej)zzz).Akcje = (Akcje)PolaczAkcje(((ZmianaZmiennej)zzz).Akcje, zmianaZmiennej.Akcje);
            }
            else
            {
                zdarzenia.Add(zmianaZmiennej);
            }
        }
        
        private static void DodajZdarzenieZmiennych(HCPSTokenizer tokenizer, List<Zdarzenie> zdarzenia, ZmianaZmiennych zmianaZmiennych)
        {
            Zdarzenie zzz2 = zdarzenia.Find(delegate(Zdarzenie o)
                                            {
                                                if (o is ZmianaZmiennych)
                                                {
                                                    // sprawdzenie czy lista zmiennych jest taka sama
                                                    if (zmianaZmiennych.Zmienne.Length == ((ZmianaZmiennych)o).Zmienne.Length)
                                                    {
                                                        int ident = 0;
                                                        foreach (Zmienna z1 in zmianaZmiennych.Zmienne)
                                                        {
                                                            foreach (Zmienna z2 in ((ZmianaZmiennych)o).Zmienne)
                                                            {
                                                                if (z2.Nazwa == z1.Nazwa)
                                                                {
                                                                    ident++;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        return ident == zmianaZmiennych.Zmienne.Length && zmianaZmiennych.Minimum == ((ZmianaZmiennych)o).Minimum;
                                                    }
                                                    return false;
                                                }
                                                return false;
                                            });
            if (zzz2 != null)
            {
                // połączenie zdarzeń
                ((ZmianaZmiennych)zzz2).Akcje = (Akcje)PolaczAkcje(((ZmianaZmiennych)zzz2).Akcje, zmianaZmiennych.Akcje);
            }
            else
            {
                zdarzenia.Add(zmianaZmiennych);
            }
        }
        
        private static void DodajFunkcje(HCPSTokenizer tokenizer, List<DefinicjaFunkcji> funkcje, DefinicjaFunkcji funkcja)
        {
            DefinicjaFunkcji f = funkcje.Find(delegate(DefinicjaFunkcji o)
                                              {
                                                  return o.Nazwa == funkcja.Nazwa;
                                              });
            if (f != null)
            {
                if (tokenizer == null)
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionRedefinition), f.Nazwa));
                }
                else
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.FunctionRedefinition2), f.Nazwa, tokenizer.GetPosition(tokenizer.Position)));
                }
            }
            funkcje.Add(funkcja);
        }
        
        private static Akcja PolaczAkcje(Akcja akcja1, Akcja akcja2)
        {
            if (akcja1 == null)
            {
                return akcja2;
            }
            else
            {
                if (akcja2 == null)
                {
                    return akcja1;
                }
            }
            return new Akcje()
            {
                akcje = new Akcja[] { akcja1, akcja2 }
            };
        }

        private static Skrypt[] ParsujHCPS(string nazwaPliku, IProgress progress, List<string> included, Dictionary<string, Skrypt[]> scripts, bool bindObjects)
        {
            string fileNameLower = nazwaPliku.ToLowerInvariant();
            if (included.Contains(fileNameLower))
            {
                if (scripts.ContainsKey(fileNameLower))
                {
                    return scripts[fileNameLower];
                }
            }
            else
            {
                included.Add(fileNameLower);
            }
            List<Skrypt> result = new List<Skrypt>();
            string info = string.Format(UI.Language.Instance.GetString(UI.UIStrings.LoadingFile), nazwaPliku);
            progress.Progress(info, "");

            // utworzenie tokenizera
            HCPSTokenizer tokenizer = new HCPSTokenizer(nazwaPliku);
            HCPSToken token = HCPSToken.Unknown;
            while ((token = tokenizer.Next()) != HCPSToken.EOF)
            {
                while (token == HCPSToken.Comment)
                {
                    token = tokenizer.Next();
                }
                if (token == HCPSToken.EOF)
                {
                    break;
                }
                if (token == HCPSToken.Word && tokenizer.Value.ToLowerInvariant() == "script")
                {
                    // chyba mamy skrypt
                    token = tokenizer.Next();
                    if (token == HCPSToken.String)
                    {
                        // nazwa slryptu

                        // sprawdzenie czy skrytp o takiej nazwie nie istnieje
                        Skrypt skrypt = result.Find(delegate(Skrypt o)
                                                    {
                                                        return o.Nazwa == tokenizer.Value;
                                                    });

                        if (skrypt != null)
                        {
                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptAlreadyExists), tokenizer.Value));
                        }
                        skrypt = new Skrypt();
                        skrypt.Nazwa = tokenizer.Value;
                        ZbiorWartosci2 slownik = null;
                        
                        progress.Progress(info, string.Format(UI.Language.Instance.GetString(UI.UIStrings.LoadingScript), skrypt.Nazwa));

                        token = tokenizer.Next();
                        if (token == HCPSToken.BlockBegin)
                        {
                            // rozpoczęcie bloku

                            // odczytywanie po kolei co jest
                            List<Zmienna> zmienne = new List<Zmienna>();
                            List<Stala> stale = new List<Stala>();
                            List<DefinicjaFunkcji> funkcje = new List<DefinicjaFunkcji>();
                            List<Zdarzenie> zdarzenia = new List<Zdarzenie>();
                            List<IncludeStatement> includes = new List<IncludeStatement>();

                            while ((token = tokenizer.Next()) != HCPSToken.BlockEnd)
                            {
                                if (token == HCPSToken.Word)
                                {
                                    if (progress.Cancel)
                                    {
                                        throw new ParsingScriptException(UI.Language.Instance.GetString(UI.UIStrings.LoadingScriptsFileCanceledByUser));
                                    }
                                    
                                    switch (tokenizer.Value.ToLowerInvariant())
                                    {
                                        case "variable":
                                            // odczytanie zmiennej
                                            Zmienna zmienna = Zmienna.Parsuj(tokenizer);
                                            DodajZmienna(tokenizer, zmienne, zmienna);
                                            break;

                                        case "const":
                                            // odczytanie stałej
                                            Stala stala = Stala.Parsuj(tokenizer);
                                            DodajStala(tokenizer, stale, stala);
                                            break;

                                        case "initialize":
                                            // funkcja inicjalizująca
                                            if (skrypt.Initialize != null)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.SpecialEventRedefinition), tokenizer.GetPosition(tokenizer.Position), "initialize"));
                                            }
                                            if (tokenizer.Next() != HCPSToken.BlockBegin)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectSpecialEventDefinition), tokenizer.GetPosition(tokenizer.Position), "initialize"));
                                            }
                                            skrypt.Initialize = Akcje.Parsuj(tokenizer);
                                            break;

                                        case "output_started":
                                            if (skrypt.OutputStarted != null)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.SpecialEventRedefinition), tokenizer.GetPosition(tokenizer.Position), "output_started"));
                                            }
                                            if (tokenizer.Next() != HCPSToken.BlockBegin)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectSpecialEventDefinition), tokenizer.GetPosition(tokenizer.Position), "output_started"));
                                            }
                                            skrypt.OutputStarted = Akcje.Parsuj(tokenizer);
                                            break;

                                        case "output_stopped":
                                            if (skrypt.OutputStopped != null)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.SpecialEventRedefinition), tokenizer.GetPosition(tokenizer.Position), "output_stopped"));
                                            }
                                            if (tokenizer.Next() != HCPSToken.BlockBegin)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectSpecialEventDefinition), tokenizer.GetPosition(tokenizer.Position), "output_stopped"));
                                            }
                                            skrypt.OutputStopped = Akcje.Parsuj(tokenizer);
                                            break;

                                        case "input_started":
                                            if (skrypt.InputStarted != null)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.SpecialEventRedefinition), tokenizer.GetPosition(tokenizer.Position), "input_started"));
                                            }
                                            if (tokenizer.Next() != HCPSToken.BlockBegin)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectSpecialEventDefinition), tokenizer.GetPosition(tokenizer.Position), "input_started"));
                                            }
                                            skrypt.InputStarted = Akcje.Parsuj(tokenizer);
                                            break;

                                        case "input_stopped":
                                            if (skrypt.InputStopped != null)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.SpecialEventRedefinition), tokenizer.GetPosition(tokenizer.Position), "input_stopped"));
                                            }
                                            if (tokenizer.Next() != HCPSToken.BlockBegin)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectSpecialEventDefinition), tokenizer.GetPosition(tokenizer.Position), "input_stopped"));
                                            }
                                            skrypt.InputStopped = Akcje.Parsuj(tokenizer);
                                            break;

                                        case "uninitialize":
                                            if (skrypt.Uninitialize != null)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.SpecialEventRedefinition), tokenizer.GetPosition(tokenizer.Position), "uninitialize"));
                                            }
                                            if (tokenizer.Next() != HCPSToken.BlockBegin)
                                            {
                                                throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectSpecialEventDefinition), tokenizer.GetPosition(tokenizer.Position), "uninitialize"));
                                            }
                                            skrypt.Uninitialize = Akcje.Parsuj(tokenizer);
                                            break;

                                        case "variable_changed":
                                            // zdarzenie zmiany wartości zmiennej
                                            ZmianaZmiennej zmianaZmiennej = ZmianaZmiennej.Parsuj(tokenizer);
                                            DodajZdarzenieZmiennej(tokenizer, zdarzenia, zmianaZmiennej);
                                            break;

                                        case "variables_changed":
                                            // zdarzenie zmiany wielu zmiennych
                                            ZmianaZmiennych zmianaZmiennych = ZmianaZmiennych.Parsuj(tokenizer);
                                            DodajZdarzenieZmiennych(tokenizer, zdarzenia, zmianaZmiennych);
                                            break;

                                        case "function":
                                            // funkcja
                                            DefinicjaFunkcji funkcja = DefinicjaFunkcji.Parsuj(tokenizer);
                                            DodajFunkcje(tokenizer, funkcje, funkcja);
                                            break;
                                            
                                        case "include":
                                            IncludeStatement include = IncludeStatement.Parsuj(tokenizer);
                                            IncludeStatement includeF = includes.Find(delegate(IncludeStatement o)
                                                                                      {
                                                                                          return o.FilePath.ToLowerInvariant() == include.FilePath.ToLowerInvariant() && o.ScriptName == include.ScriptName;
                                                                                      });
                                            if (includeF != null)
                                            {
                                                throw new Exception();
                                            }
                                            includes.Add(include);
                                            break;

                                        default:
                                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnsuportedDefinitionType), tokenizer.Value, tokenizer.GetPosition(tokenizer.Position)));
                                    }
                                }
                                else
                                {
                                    if (token != HCPSToken.Comment)
                                    {
                                        throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptSyntaxError), tokenizer.GetPosition(tokenizer.Position)));
                                    }
                                }
                            }
                            
                            // TODO obsługa dyrektyw include
                            if (includes.Count > 0)
                            {
                                foreach (IncludeStatement inc in includes)
                                {
                                    string includeFileName = inc.FilePath.ToLowerInvariant();
                                    if (included.Contains(includeFileName))
                                    {
                                        continue;
                                    }
                                    if (includeFileName == fileNameLower)
                                    {
                                        throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotJoinScriptsFromOneFile)));
                                    }
                                    
                                    if (!Path.IsPathRooted(inc.FilePath))
                                    {
                                        string dir = nazwaPliku.Substring(0, nazwaPliku.Length - Path.GetFileName(nazwaPliku).Length);
                                        inc.FilePath = Path.Combine(dir, inc.FilePath);
                                    }
                                    
                                    Skrypt[] skrypty = ParsujHCPS(inc.FilePath, progress, included, scripts, false);
                                    Skrypt skryptInc = Array.Find<Skrypt>(skrypty, delegate(Skrypt o)
                                                                          {
                                                                              return o.Nazwa.ToLowerInvariant() == inc.ScriptName.ToLowerInvariant();
                                                                          });
                                    if (skryptInc != null)
                                    {
                                        skrypt.Initialize = PolaczAkcje(skrypt.Initialize, skryptInc.Initialize);
                                        skrypt.OutputStarted = PolaczAkcje(skrypt.OutputStarted, skryptInc.OutputStarted);
                                        skrypt.OutputStopped = PolaczAkcje(skrypt.OutputStopped, skryptInc.OutputStopped);
                                        skrypt.InputStarted = PolaczAkcje(skrypt.InputStarted, skryptInc.InputStarted);
                                        skrypt.InputStopped = PolaczAkcje(skrypt.InputStopped, skryptInc.InputStopped);
                                        for (int i = 0; i < skryptInc.Zmienne.Length; i++)
                                        {
                                            DodajZmienna(null, zmienne, skryptInc.Zmienne[i]);
                                        }
                                        for (int i = 0; i < skryptInc.Stale.Length; i++)
                                        {
                                            DodajStala(null, stale, skryptInc.Stale[i]);
                                        }
                                        for (int i = 0; i < skryptInc.Funkcje.Length; i++)
                                        {
                                            DodajFunkcje(null, funkcje, skryptInc.Funkcje[i]);
                                        }
                                        for (int i = 0; i < skryptInc.Zdarzenia.Length; i++)
                                        {
                                            DodajZdarzenie(null, zdarzenia, skryptInc.Zdarzenia[i]);
                                        }
                                    }
                                    else
                                    {
                                        throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptNotFoundInFile), inc.FilePath, inc.ScriptName));
                                    }
                                }
                            }
                            
                            slownik = new ZbiorWartosci2(zmienne, stale, funkcje);
                            skrypt.Zmienne = zmienne.ToArray();
                            skrypt.Stale = stale.ToArray();
                            skrypt.Funkcje = funkcje.ToArray();
                            skrypt.Zdarzenia = zdarzenia.ToArray();
                        }
                        else
                        {
                            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptSyntaxError), tokenizer.GetPosition(tokenizer.Position)));
                        }

                        // przepisanie referencji zmiennych, stałych i funkcji
                        if (bindObjects)
                        {
                            skrypt.PrzypiszReferencje(slownik);
                        }
                        result.Add(skrypt);
                    }
                    else
                    {
                        throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptSyntaxError), tokenizer.GetPosition(tokenizer.Position)));
                    }
                }
                else
                {
                    throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnrecognizedString), tokenizer.Value, tokenizer.GetPosition(tokenizer.Position)));
                }
            }
            result.Sort();
            return result.ToArray();
        }
    }
}