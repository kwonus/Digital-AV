namespace AVXLib.Framework
{
    using System.Text;
    using AVXLib.Memory;
    using static System.Net.Mime.MediaTypeNames;
    using static AVXLib.Framework.Numerics;

    public class Written
    {
        private Deserialization.Data Data;
        private ReadOnlyMemory<AVXLib.Memory.Written> Writ { get => this.Data.Written; }
        private ReadOnlyMemory<AVXLib.Memory.Lexicon> Lex  { get => this.Data.Lexicon; }
        private ReadOnlyMemory<AVXLib.Memory.Book>    Book { get => this.Data.Book;    }

        public static bool ProcessReversals(ushort key, string search, string display, string modern)
        {
            var same = display == modern;

            ReverseLex[Keyify(search)] = key;
            ReverseLexModern[Keyify(modern)] = key;

            return same;
        }

        private static Dictionary<string, UInt16> ReverseLexModern = new();
        private static Dictionary<string, UInt16> ReverseLex = new();
        private static char[] csubtract = new char[] { '-', ' ', '\'', '(', ')', '.', ':', ';', '!', '?', ',' };
        private static string[] subtract = new string[] { "-", " ", "'", "(", ")", ".", ":", ";", "!", "?", "," };


        public Written(Deserialization.Data data)
        {
            this.Data = data;
        }

        private static string Keyify(ReadOnlyMemory<char> input)
        {
            return Keyify(input.ToString());
        }
        public static string Keyify(string input)
        {
            string result = input.ToLower();

            if (result.IndexOfAny(csubtract) >= 0)
            {
                int i = 0;
                foreach (var c in subtract)
                {
                    if (result.IndexOf(c) >= 0)
                        result = result.Replace(subtract[i], "");
                    i++;
                }
            }
            return result;
        }
        public string GetLexNormalized(ushort id)
        {
            ushort caps = (ushort)(id & WordKeyBits.CAPS);
            int key = id & WordKeyBits.WordKey;

            if (key > 0 && key < this.Lex.Length)
            {
                var lex = this.Lex.Span[key].Search.ToString();
                if (lex.Length > 1)
                {
                    if (caps == WordKeyBits.CAPS_FirstLetter)
                        return lex.Substring(0, 1).ToUpper() + lex.Substring(1);
                    else if (caps == WordKeyBits.CAPS_AllLetters)
                        return lex.ToUpper();
                }
                else if (caps != 0)
                {
                    return lex.ToUpper();
                }
                return lex;
            }
            return "";
        }
        public string GetLexDisplay(ushort id)
        {
            ushort caps = (ushort)(id & WordKeyBits.CAPS);
            int key = id & WordKeyBits.WordKey;

            if (key > 0 && key < this.Lex.Length)
            {
                var lex = this.Lex.Span[key].Display.ToString();
                if (lex.Length > 1)
                {
                    if (caps == WordKeyBits.CAPS_FirstLetter)
                        return lex.Substring(0, 1).ToUpper() + lex.Substring(1);
                    else if (caps == WordKeyBits.CAPS_AllLetters)
                        return lex.ToUpper();
                }
                else if (caps != 0)
                {
                    return lex.ToUpper();
                }
                return lex;
            }
            return "";
        }
        public string GetLexModern(ushort id)
        {
            ushort caps = (ushort)(id & WordKeyBits.CAPS);
            int key = id & WordKeyBits.WordKey;

            if (key > 0 && key < this.Lex.Length)
            {
                var lex = this.Lex.Span[key].Modern.ToString();
                if (lex.Length > 1)
                {
                    if (caps == WordKeyBits.CAPS_FirstLetter)
                        return lex.Substring(0, 1).ToUpper() + lex.Substring(1);
                    else if (caps == WordKeyBits.CAPS_AllLetters)
                        return lex.ToUpper();
                }
                else if (caps != 0)
                {
                    return lex.ToUpper();
                }
                return lex;
            }
            return "";
        }
        public UInt16 GetReverseLex(string text)
        {
            var lookup = Keyify(text);
            if (ReverseLex.ContainsKey(lookup))
            {
                UInt16 key = ReverseLex[lookup];
                return key;
            }
            return 0;
        }
        public UInt16 GetReverseLexModern(string text)
        {
            var lookup = Keyify(text);
            if (ReverseLexModern.ContainsKey(lookup))
            {
                UInt16 key = ReverseLexModern[lookup];
                return key;
            }
            return 0;
        }
        public UInt16 GetReverseLexExtensive(string text, bool strict = false)
        {
            var lex = this.GetReverseLex(text);
            if ((lex == 0) && !strict)
            {
                lex = this.GetReverseLexModern(text);
            }
            if (lex == 0)
            { 
                var keyified = AVXLib.Framework.Written.Keyify(text);
                lex = this.GetReverseLex(keyified);
                if ((lex == 0) && !strict)
                {
                    lex = this.GetReverseLexModern(keyified);
                }
            }
            return lex;
        }
        private static string PrePunc(ushort previousPunctuation, ushort currentPunctuation)
        {
            bool prevParen = (previousPunctuation & Punctuation.Parenthetical) != 0;
            bool thisParen = (currentPunctuation & Punctuation.Parenthetical) != 0;

            return thisParen && !prevParen ? "(" : "";
        }
        private static string PostPunc(ushort punctuation, bool s)
        {
            bool eparen = (punctuation & Punctuation.CloseParen) == Punctuation.CloseParen;
            bool posses = (punctuation & Punctuation.Possessive) == Punctuation.Possessive;
            bool exclaim = (punctuation & Punctuation.Clause) == Punctuation.Exclamatory;
            bool declare = (punctuation & Punctuation.Clause) == Punctuation.Declarative;
            bool dash = (punctuation & Punctuation.Clause) == Punctuation.Dash;
            bool semi = (punctuation & Punctuation.Clause) == Punctuation.Semicolon;
            bool colon = (punctuation & Punctuation.Clause) == Punctuation.Colon;
            bool comma = (punctuation & Punctuation.Clause) == Punctuation.Comma;
            bool quest = (punctuation & Punctuation.Clause) == Punctuation.Interrogative;

            string punc = posses ? !s ? "'s" : "'" : "";
            if (eparen)
                punc += ")";
            if (declare)
                punc += ".";
            else if (comma)
                punc += ",";
            else if (semi)
                punc += ";";
            else if (colon)
                punc += ":";
            else if (quest)
                punc += "?";
            else if (exclaim)
                punc += "!";
            else if (dash)
                punc += "--";

            return punc;
        }
        public string GetDisplayWithPunctuation(byte bookNum, uint writIdx)
        {
            if (this.Writ.Length > 0 && bookNum < this.Book.Length)
            {
                var book = this.Book.Span[bookNum];

                if (writIdx < book.written.Length)
                {
                    var writ = book.written.Span[(int)writIdx];
                    var text = GetLexDisplay(writ.WordKey);

                    var punc = writ.Punctuation;
                    var puncPrev = writIdx >= 1 ? book.written.Span[(int)writIdx].Punctuation : (byte)0x00;
                    var s = text.EndsWith("s", StringComparison.InvariantCultureIgnoreCase);

                    return PrePunc(puncPrev, punc) + text + PostPunc(punc, s);
                }
            }
            return "";
        }
        public string GetModernWithPunctuation(byte bookNum, ushort writIdx)
        {
            if (this.Writ.Length > 0 && bookNum < this.Book.Length)
            {
                var book = this.Book.Span[bookNum];

                if (writIdx < book.written.Length)
                {
                    var writ = book.written.Span[writIdx];
                    var text = GetLexModern(writ.WordKey);

                    var punc = writ.Punctuation;
                    var puncPrev = writIdx >= 1 ? book.written.Span[writIdx].Punctuation : (byte)0x00;
                    var s = text.EndsWith("s", StringComparison.InvariantCultureIgnoreCase);

                    return PrePunc(puncPrev, punc) + text + PostPunc(punc, s);
                }
            }
            return "";
        }
        public (Book book, bool found) FindBook(string name)
        {
            int len = name.Length;

            if (len <= 2)
            {
                for (int b = 1; b <= 66; b++)
                {
                    var abbr = this.Book.Span[b].abbr2.ToString();
                    if (name.Equals(abbr, StringComparison.InvariantCultureIgnoreCase))
                        return (this.Book.Span[b], true);
                }
            }
            if (len <= 3)
            {
                for (int b = 1; b <= 66; b++)
                {
                    var abbr = this.Book.Span[b].abbr3.ToString();
                    if (name.Equals(abbr, StringComparison.InvariantCultureIgnoreCase))
                        return (this.Book.Span[b], true);
                }
            }
            if (len <= 4)
            {
                for (int b = 1; b <= 66; b++)
                {
                    var abbr = this.Book.Span[b].abbr4.ToString();
                    if (name.Equals(abbr, StringComparison.InvariantCultureIgnoreCase))
                        return (this.Book.Span[b], true);
                }
            }
            for (int b = 1; b <= 66; b++)
            {
                string test = this.Book.Span[b].name.ToString();
                if (name.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                    return (this.Book.Span[b], true);

                test = this.Book.Span[b].abbrAltA.ToString();
                if (name.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                    return (this.Book.Span[b], true);

                test = this.Book.Span[b].abbrAltB.ToString();
                if (name.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                    return (this.Book.Span[b], true);
            }
            return (new Book(), false);
        }
        // For Part-of-Speech:
        public static uint EncodePOS(string input7charsMaxWithHyphen)
        { // input string must be ascii
            var len = input7charsMaxWithHyphen.Length;
            if (len < 1 || len > 7)
                return 0;

            var input = input7charsMaxWithHyphen.Trim().ToLower();
            len = input7charsMaxWithHyphen.Length;
            if (len < 1 || len > 7)
                return 0;

            var encoded = (uint)0x0;
            var hyphen = (uint)input.IndexOf('-');
            if (hyphen > 0 && hyphen <= 3)
                hyphen <<= 30;
            else if (len > 6)   // 6 characters max if a compliant hyphen is not part of the string
                return 0;
            else
                hyphen = 0x0;

            int c = 0;
            char[] buffer = new char[6]; // 6x 5bit characters
            for (var i = 0; i < len; i++)
            {
                var b = input[i];
                switch (b)
                {
                    case '-':
                        continue;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                        b -= '0';
                        b += (char)27;
                        break;
                }
                buffer[c++] = b;
            }
            var position = (uint)0x02000000;
            for (var i = 0; i < 6 - len; i++)
            {
                position >>= 5;
            }
            for (var i = 0; i < len; i++)
            {
                char letter = (char)(buffer[i] & 0x1F);
                if (letter == 0)
                    break;

                encoded |= letter * position;
                position >>= 5;
            }
            return encoded | hyphen;
        }
        //  For Part-of-Speech:
        public static string DecodePOS(uint encoding)
        {
            char[] buffer = new char[7]; // 6x 5bit characters + 2bits for hyphen position = 32 bits;

            var hyphen = encoding & 0xC0000000;
            if (hyphen > 0)
                hyphen >>= 30;

            var index = 0;
            for (var mask = (uint)(0x1F << 25); mask >= 0x1F; mask >>= 5)
            {
                var digit = encoding & mask >> 5 * (5 - index);
                if (digit == 0)
                    continue;
                byte b = (byte)digit;
                if (b <= 26)
                    b |= 0x60;
                else
                {
                    b -= 27;
                    b += (byte)'0';
                }
                if (hyphen == index)
                    buffer[index++] = '-';
                buffer[index++] = (char)b;
            }
            var decoded = new StringBuilder(index + 1);
            for (int i = 0; i < index; i++)
                decoded.Append(buffer[i]);
            return decoded.ToString();
        }
    }
}
