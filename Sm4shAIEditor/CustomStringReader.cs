namespace Sm4shAIEditor
{
    class CustomStringReader
    {
        private static string validWordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        private static string validEqnChars = "=+-*/";
        private static string validIfChars = "&|";
        private static string spaceCharacters = " \t";
        private static string newline = "\n";

        public int Position { get; set; }
        private char[] charArray { get; set; }
        public char[] CharArray
        {
            get
            {
                return charArray;
            }
            set
            {
                charArray = value;
                Position = 0;
            }
        }
        public bool EndString
        {
            get
            {
                if (Position >= CharArray.Length)
                    return true;
                else
                    return false;
            }
        }

        public CustomStringReader(string text)
        {
            charArray = text.ToCharArray();
            //when setting a new charArray, position is automatically set to 0
        }

        public string ReadChar()
        {
            string c = null;
            while (!EndString)
            {
                string c2 = CharArray[Position].ToString();
                if (spaceCharacters.Contains(c2))
                {
                    Position++;
                }
                else
                {
                    c = c2;
                    Position++;
                    break;
                }
            }
            return c;
        }
        public string PeekChar()
        {
            string c = null;
            if (!EndString)
                c = CharArray[Position].ToString();
            return c;
        }
        public string ReadWord()
        {
            string s = null;
            for (string c = ReadChar(); !EndString && validWordChars.Contains(c); c = ReadChar())
            {
                s += c;
            }
            Position--;
            return s;
        }
        public string ReadEqnSymbols()
        {
            string s = null;
            for (string c = ReadChar(); !EndString && validEqnChars.Contains(c); c = ReadChar())
            {
                s += c;
            }
            return s;
        }
        public string ReadIfSymbols()
        {
            string s = null;
            for (string c = ReadChar(); !EndString && validIfChars.Contains(c); c = ReadChar())
            {
                s += c;
            }
            return s;
        }
        public string ReadUntilAnyOfChars(string charsToEndAt, bool includeLastChar)
        {
            string s = null;
            if (includeLastChar)
            {
                bool readNext = true;
                while (!EndString && readNext)
                {
                    string c = ReadChar();
                    if (!charsToEndAt.Contains(c))
                        readNext = false;
                    s += c;
                }
            }
            else
            {
                for (string c = ReadChar(); !EndString && !charsToEndAt.Contains(c); c = ReadChar())
                {
                    s += c;
                }
                Position--;
            }
            return s;
        }
        public string ReadLine()
        {
            string s = null;
            for (string c = ReadChar(); !EndString && c != newline; c = ReadChar())
            {
                s += c;
            }
            return s;
        }
    }
}
