using System;

namespace Sm4shAIEditor
{
    class CustomStringReader
    {
        private const string validWordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
        private const string validEqnChars = "=+-*/";
        private const string validIfChars = "&|";
        private const string spaceCharacters = " \t\r\n";
        private static char CommentChar = '#';

        public string name { get; set; } = "Unnamed";
        public int Position { get; set; }
        private string source { get; set; }
        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                Position = 0;
            }
        }
        public bool EndString
        {
            get { return Position >= Source.Length; }
        }

        public CustomStringReader(string text)
        {
            source = text;
        }
        public CustomStringReader(string text, string name)
        {
            source = text;
            this.name = name;
        }

        public string ReadChar()
        {
            string c = null;
            if (!EndString)
                c += Source[Position++];
            return c;
        }
        public void SkipWhiteSpace()
        {
            while (!EndString)
            {
                if (spaceCharacters.Contains(Source[Position].ToString()))
                    Position++;
                else if (Source[Position] == CommentChar)
                {
                    Position++;
                    SkipToEndLine();
                }
                else break;
            }
        }
        public void SkipToEndLine()
        {
            while (!EndString)
            {
                if (Source[Position] != '\n')
                    Position++;
                else break;
            }
        }
        public string ReadWord()
        {
            SkipWhiteSpace();
            string s = null;
            while (true)
            {
                string c = ReadChar();
                if (c == null) break;
                if (!validWordChars.Contains(c))
                {
                    Position--;
                    break;
                }
                s += c;
            }
            return s;
        }
        public string ReadEqnSymbols()
        {
            SkipWhiteSpace();
            string s = null;
            while (true)
            {
                string c = ReadChar();
                if (c == null) break;
                if (!validEqnChars.Contains(c))
                {
                    Position--;
                    break;
                }
                s += c;
            }
            return s;
        }
        public string ReadIfSymbols()
        {
            SkipWhiteSpace();
            string s = null;
            while (true)
            {
                string c = ReadChar();
                if (c == null) break;
                if (!validIfChars.Contains(c))
                {
                    Position--;
                    break;
                }
                s += c;
            }
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
                    s += c;
                Position--;
            }
            return s;
        }

        public string ExceptionMsg(string message)
        {
            return string.Format("[{0}, pos {1}]:\n\t>{2}", name, Position, message);
        }
    }
}
