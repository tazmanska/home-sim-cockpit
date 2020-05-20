using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HomeSimCockpit.Parser
{
    internal class SimpleAssignment
    {
        public string Left
        {
            get;
            set;
        }

        public string Right
        {
            get;
            set;
        }
    }

    internal interface ITokenizer
    {
        HCPSToken Next();

        void Back();

        bool Peek(out HCPSToken token, out string value);

        string Value
        {
            get;
        }

        long Position
        {
            get;
            set;
        }

        HCPSToken LastToken
        {
            get;
        }

        SimpleAssignment ReadSimpleAssignment(HCPSToken token, string value);

        string GetPosition(long position);
    }

    class HCPSTokenizer : ITokenizer
    {
        public HCPSTokenizer(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                _contents = new char[reader.BaseStream.Length];
                reader.Read(_contents, 0, _contents.Length);
                                
            }
            string[] lines = File.ReadAllLines(fileName);
            _linesLenght = new int[lines.Length];
            int count = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                _linesLenght[i] = lines[i].Length;
                count += _linesLenght[i];
            }
            _endLineCharactersCount = (int)Math.Round((decimal)((double)(_contents.Length - count) / (double)_linesLenght.Length));
        }

        private char[] _contents = null;
        private int[] _linesLenght = null;
        private int _endLineCharactersCount = 0;

        private long _position = 0;
        private string _value = null;

        private long _lastPosition = 0;

        public HCPSToken Next()
        {
            _lastPosition = Position;
            do
            {
                LastToken = _Next();
                System.Diagnostics.Debug.WriteLine(string.Format("{0} = {1}", LastToken, Value), "LoadingScript");
            } while (LastToken == HCPSToken.Comment);
            return LastToken;
        }

        public void Back()
        {
            Position = _lastPosition;
        }

#warning implemenetacja operatorów: negacji (!), xorowania (^), odwracania bitów (~)

        private HCPSToken _Next()
        {
            Value = "";

            // sprawdzenie czy nie jest na końcu
            if (EOF)
            {
                return HCPSToken.EOF;
            }

            // przesunięcie pozycja na jakiś znak
            long start = Position;
            // sprawdzenie czym jest znak
            char znak = LastCharacter;

            POBIERZ:

            while (!EOF && char.IsWhiteSpace(znak = NextChar()))
            {
                ;
            }
            if (EOF && ((Position > _contents.Length) || (char.IsWhiteSpace(znak))))
            {
                Position = _contents.Length;
                return HCPSToken.EOF;
            }

            long stop = start;

            if (znak == '/')
            {
                // jeśli kolejny znak to 
                char next = NextChar();
                if (next == '/')
                {
                    // komentarz
                    // odczytanie komentarza do końca linii
                    Value = ReadToEndLine();
                    return HCPSToken.Comment;
                }
                else if (next == '*')
                {
                    // komentarz wielo-wierszowy
                    Value = ReadTo("*/");
                    Value = Value.Remove(Value.Length - 2);
                    return HCPSToken.Comment;
                }
                else
                {
                    // znak dzielenia
                    Value = "/";
                    return HCPSToken.Division;
                }
            }
            else if (znak == '$')
            {
                // identyfikator zmiennej/stałej
                Value = ReadVariable();
                Position--;
                return HCPSToken.Variable;
            }
            else if (znak == '=')
            {
                if (NextChar(false) == '=')
                {
                    Value = "==";
                    return HCPSToken.Equal;
                }
                Position--;
                Value = "=";
                return HCPSToken.Assignment;
            }
            else if (znak == '<')
            {
                if (NextChar(false) == '=')
                {
                    Value = "<=";
                    return HCPSToken.SmallerOrEqual;
                }
                Position--;
                if (NextChar(false) == '>')
                {
                    Value = "<>";
                    return HCPSToken.NotEqual;
                }
                Position--;
                Value = "<";
                return HCPSToken.Smaller;
            }
            else if (znak == '>')
            {
                if (NextChar(false) == '=')
                {
                    Value = ">=";
                    return HCPSToken.BiggerOrEqual;
                }
                Position--;
                Value = ">";
                return HCPSToken.Bigger;
            }
            else if (znak == '+')
            {
                Value = "+";
                return HCPSToken.Addition;
            }
            else if (znak == '-')
            {
                if (IsDigit(NextChar(false)))
                {
                    Position--;
                    Value = "-" + ReadNumber();
                    Position--;
                    return HCPSToken.Number;
                }
                Value = "-";
                return HCPSToken.Subtraction;
            }
            else if (znak == '*')
            {                
                Value = "*";
                return HCPSToken.Multiplication;
            }
            else if (znak == '%')
            {
                Value = "%";
                return HCPSToken.Modulo;
            }
            else if (znak == '&')
            {
                Value = "&";
                return HCPSToken.And;
            }
            else if (znak == '|')
            {
                Value = "|";
                return HCPSToken.Or;
            }
            else if (znak == ';')
            {
                // zakończenie instrukcji
                Value = ";";
                return HCPSToken.InstructionEnd;
            }
            else if (znak == ',')
            {
                // rozdzielenie parametrów funkcji
                Value = ",";
                return HCPSToken.CommaSeparator;
            }
            else if (znak == '{')
            {
                // początek wyrażenia
                Value = "{";
                return HCPSToken.BlockBegin;
            }
            else if (znak == '}')
            {
                // koniec wyrażenia
                Value = "}";
                return HCPSToken.BlockEnd;
            }
            else if (znak == '(')
            {
                Value = "(";
                return HCPSToken.ParenthesisOpen;
            }
            else if (znak == ')')
            {
                Value = ")";
                return HCPSToken.ParenthesisClose;
            }
            else if (znak == '"')
            {
                // string
                Value = ReadString();
                return HCPSToken.String;
            }
            else if (znak == ':')
            {
                // dwukropek
                Value = ":";
                return HCPSToken.Colon;
            }
            else if (IsCharacter(znak))
            {
                // litera więc jakieś słowo kluczowe itp.
                Position--;
                Value = ReadWord();
                Position--;
                return HCPSToken.Word;
            }
            else if (IsDigit(znak))
            {
                // mamy liczbę
                Position--;
                Value = ReadNumber();
                Position--;
                return HCPSToken.Number;
            }
            else if (znak == '\0')
            {
                if (EOF)
                {
                    Position++;
                }
                goto POBIERZ;
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.UnrecognizedCharacter), znak, Position - 1));
        }

        public bool Peek(out HCPSToken token, out string value)
        {
            token = HCPSToken.Unknown;
            value = null;
            return false;
        }

        public bool EOF
        {
            get { return Position >= _contents.Length; }
        }

        private char NextChar()
        {
            return NextChar(false);
        }

        private char _lastCharacter = ' ';

        private char LastCharacter
        {
            get { return _lastCharacter; }
        }

        private char NextChar(bool ommitWhiteSpace)
        {
            if (ommitWhiteSpace)
            {
                while (!EOF && char.IsWhiteSpace(_contents[_position]))
                {
                    _position++;
                }
            }
            if (EOF)
            {
                return '\0';
            }
            _lastCharacter = _contents[_position++];
            return _lastCharacter;
        }

        private string ReadToEndLine()
        {
            long pos = Position;
            char znak;
            do
            {
                znak = NextChar();
            }
            while (!EOF && znak != '\r' && znak != '\n');
            //Position--;
            if (Position > pos)
            {
                return GetString(pos, Position);
            }
            return "";
        }

        private string ReadWord()
        {
            long pos = Position;
            char znak;
            do
            {
                znak = NextChar();
            }
            while (!EOF && (IsCharacter(znak) || IsDigit(znak)));
            //Position--;
            if (Position > pos)
            {
                return GetString(pos, Position);
            }
            return "";
        }

        private string ReadVariable()
        {
            long pos = Position;
            char znak;
            int count = 0;
            do
            {
                znak = NextChar();
                count++;
            }
            while (!EOF && (IsCharacter(znak) || (IsDigit(znak) && count > 1)));
            //Position--;
            if (Position > pos)
            {
                return GetString(pos, Position);
            }
            return "";
        }

        private string ReadNumber()
        {
            long pos = Position;
            char znak;
            do
            {
                znak = NextChar();
            }
            while (!EOF && (IsDigit(znak) || IsHexDigit(znak) || znak == 'x' || znak == 'X' || znak == '.' || znak == 'd' || znak == 'D'));
            //Position--;
            if (Position > pos)
            {
                return GetString(pos, Position);
            }
            return "";
        }

        private string ReadString()
        {
            long pos = Position;
            char znak = '\0';
            char prev = '\0';
            int backSlashCount = 0;
            do
            {
                prev = znak;
                znak = NextChar();
                if (prev == '\\')
                {
                	backSlashCount++;
                }
                else
                {
              		backSlashCount = 0;
                }
            }
            while (!EOF && (znak != '"' || ((backSlashCount % 2) == 1)));// prev == '\\')));
            if (Position > pos)
            {
            	return GetString(pos, Position).Replace("\\\"", "\"").Replace("\\\\", "\\");
            }
            return "";
        }

        private string ReadTo(string text)
        {
            long pos = Position;
            string tmp = "";
            do
            {
                tmp += NextChar();
            }
            while (!tmp.EndsWith(text));
            return tmp;
        }

        public SimpleAssignment ReadSimpleAssignment(HCPSToken token, string value)
        {
            SimpleAssignment result = new SimpleAssignment();
            while (token == HCPSToken.Comment)
            {
                token = Next();
            }
            if (token == HCPSToken.Variable || token == HCPSToken.Word)
            {
                result.Left = Value;
                token = Next();
                while (token == HCPSToken.Comment)
                {
                    token = Next();
                }
                if (token == HCPSToken.Assignment)
                {
                    token = Next();
                    while (token == HCPSToken.Comment)
                    {
                        token = Next();
                    }
                    if (token == HCPSToken.Word || token == HCPSToken.Number || token == HCPSToken.String)
                    {                        
                        result.Right = Value;
                        token = Next();
                        while (token == HCPSToken.Comment)
                        {
                            token = Next();
                        }
                        if (token == HCPSToken.InstructionEnd)
                        {
                            return result;
                        }
                    }
                }
            }
            if (token == HCPSToken.BlockEnd)
            {
                return null;
            }
            throw new ParsingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.CannotReadAssignmentDefinition), GetPosition(Position)));
        }

        private string GetString(long start, long stop)
        {
            char[] text = new char[stop - start - 1];
            Array.Copy(_contents, start, text, 0, text.Length);
            return new string(text);
        }

        private static readonly List<char> _characters = new List<char>() { '_', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'q', 'o', 'p', 'r', 's', 't', 'u', 'w', 'v', 'x', 'y', 'z' };
        private static readonly List<char> _digits = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly List<char> _hexDigits = new List<char>() { 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F' };

        public static bool IsWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }
            for (int i = 0; i < word.Length; i++)
            {
                if (!IsCharacter(word[i]) && !IsDigit(word[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsCharacter(char character)
        {
            return _characters.Contains(char.ToLowerInvariant(character));
        }

        private static bool IsDigit(char character)
        {
            return _digits.Contains(character);
        }

        private static bool IsHexDigit(char character)
        {
            return _hexDigits.Contains(character);
        }

        public long GoTo(long position)
        {
            return _position = position;
        }

        public string Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        public long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public long ValuePosition
        {
            get { return Value != null ? Position - Value.Length : Position; }
        }

        public string GetPosition(long position)
        {
            int line = -1;
            int character = -1;
            if (position >= 0 && position < _contents.Length)
            {
                int count = 0;
                for (int i = 0; i < _linesLenght.Length; i++)
                {
                    count += _linesLenght[i] + _endLineCharactersCount;
                    if (position < count)
                    {
                        line = i + 1;
                        character = (int)((long)count - position);
                        break;
                    }
                }
            }
            return string.Format(UI.Language.Instance.GetString(UI.UIStrings.RowCharacter), line, character);
        }

        public static string TokenToString(HCPSToken token)
        {
            switch (token)
            {
                case HCPSToken.Addition:
                    return "+";

                case HCPSToken.Assignment:
                    return "=";

                case HCPSToken.BlockBegin:
                    return "{";

                case HCPSToken.BlockEnd:
                    return "}";

                case HCPSToken.CommaSeparator:
                    return ",";

                case HCPSToken.Comment:
                    return "//";

                case HCPSToken.Division:
                    return "/";

                case HCPSToken.InstructionEnd:
                    return ";";

                case HCPSToken.Modulo:
                    return "%";

                case HCPSToken.Multiplication:
                    return "*";

                case HCPSToken.ParenthesisClose:
                    return ")";

                case HCPSToken.ParenthesisOpen:
                    return "(";

                case HCPSToken.String:
                    return "\"";

                case HCPSToken.Subtraction:
                    return "-";

                case HCPSToken.Variable:
                    return "$";
            }
            return "";
        }

        public HCPSToken LastToken
        {
            get;
            private set;
        }

        public static string PrepareVariableName(string variableName)
        {
            string result = variableName;
            if (result == null)
            {
                result = "_";
            }

            if (result.Length > 0)
            {
                if (!IsCharacter(result[0]))
                {
                    result = "_" + result;
                }
            }

            for (int i = 1; i < result.Length; i++)
            {
                if (!IsCharacter(result[i]) && !IsDigit(result[i]))
                {
                    result = result.Substring(0, i) + "_" + result.Substring(i + 1);
                }
            }

            return result;
        }
    }
}
