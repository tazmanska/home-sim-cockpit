using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpit.Parser
{
    enum HCPSToken
    {
        /// <summary>
        /// Nieznany token.
        /// </summary>
        Unknown,

        /// <summary>
        /// Koniec pliku (End Of File).
        /// </summary>
        EOF,

        /// <summary>
        /// Komentarz.
        /// </summary>
        Comment,

        /// <summary>
        /// Słowo.
        /// </summary>
        Word,

        /// <summary>
        /// Liczba.
        /// </summary>
        Number,

        /// <summary>
        /// Tekst.
        /// </summary>
        String,

        /// <summary>
        /// Początek bloku.
        /// </summary>
        BlockBegin,

        /// <summary>
        /// Koniec bloku.
        /// </summary>
        BlockEnd,

        /// <summary>
        /// Zmienna.
        /// </summary>
        Variable,

        /// <summary>
        /// Przecinek.
        /// </summary>
        CommaSeparator,

        /// <summary>
        /// Koniec instrukcji - średnik.
        /// </summary>
        InstructionEnd,

        /// <summary>
        /// Dodawanie "+".
        /// </summary>
        Addition,

        /// <summary>
        /// Odejmowanie "-".
        /// </summary>
        Subtraction,

        /// <summary>
        /// Mnożenie "*".
        /// </summary>
        Multiplication,

        /// <summary>
        /// Dzielenie "/".
        /// </summary>
        Division,

        /// <summary>
        /// Reszta z dzielenia "%".
        /// </summary>
        Modulo,

        /// <summary>
        /// Przypisanie "=".
        /// </summary>
        Assignment,

        /// <summary>
        /// Nawias otwierający "(".
        /// </summary>
        ParenthesisOpen,

        /// <summary>
        /// Nawias zamykający ")".
        /// </summary>
        ParenthesisClose,

        /// <summary>
        /// Równość "=="
        /// </summary>
        Equal,

        /// <summary>
        /// Nierówność "<>".
        /// </summary>
        NotEqual,

        /// <summary>
        /// Większy ">".
        /// </summary>
        Bigger,

        /// <summary>
        /// Mniejszy "<".
        /// </summary>
        Smaller,

        /// <summary>
        /// Większy lub równy ">=".
        /// </summary>
        BiggerOrEqual,

        /// <summary>
        /// Mniejszy lub równy "<=".
        /// </summary>
        SmallerOrEqual,

        /// <summary>
        /// Iloczyn logiczny "&".
        /// </summary>
        And,

        /// <summary>
        /// Suma logiczna "|".
        /// </summary>
        Or,

        /// <summary>
        /// Dwukropek ":".
        /// </summary>
        Colon,
    }
}
