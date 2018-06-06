namespace Sm4shAIEditor
{
    class CustomStringReader
    {
        private static string validWordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
        private static string validEqnChars = "=+-*/";
        private static string validIfChars = "&|";
        private static string spaceCharacters = " \t\r\n";
        private static char CommentChar = '#';

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
            if (!EndString)
            {
                c = CharArray[Position].ToString();
                Position++;
            }
            return c;
        }
        public void SkipWhiteSpace()
        {
            while (!EndString)
            {
                if (spaceCharacters.Contains(CharArray[Position].ToString()))
                    Position++;
                else if (CharArray[Position] == CommentChar)
                {
                    Position++;
                    SkipToEndLine();
                }
                else break;
            }
        }
        private void SkipToEndLine()
        {
            while (!EndString)
            {
                if (CharArray[Position] != '\n')
                    Position++;
                else break;
            }
        }
        public string ReadWord()
        {
            SkipWhiteSpace();
            string s = null;
            for (string c = ReadChar(); c != null && validWordChars.Contains(c); c = ReadChar())
            {
                s += c;
            }
            Position--;
            return s;
        }
        public string ReadEqnSymbols()
        {
            SkipWhiteSpace();
            string s = null;
            for (string c = ReadChar(); c != null && validEqnChars.Contains(c); c = ReadChar())
            {
                s += c;
            }
            Position--;
            return s;
        }
        public string ReadIfSymbols()
        {
            SkipWhiteSpace();
            string s = null;
            for (string c = ReadChar(); c != null && validIfChars.Contains(c); c = ReadChar())
            {
                s += c;
            }
            Position--;
            return s;
        }
        public string ReadUntilAnyOfChars(string charsToEndAt, bool includeLastChar)
        {
            SkipWhiteSpace();
            string s = null;
            if (includeLastChar)
            {
                bool readNext = true;
                while (!EndString && readNext)
                {
                    string c = ReadChar();
                    if (charsToEndAt.Contains(c))
                        readNext = false;
                    s += c;
                }
            }
            else
            {
                for (string c = ReadChar(); c != null && !charsToEndAt.Contains(c); c = ReadChar())
                {
                    s += c;
                }
                Position--;
            }
            return s;
        }
    }
}
