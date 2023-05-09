namespace AVXLib.Framework
{
    using System.Text;
    using AVXLib.Memory;
    using static System.Net.Mime.MediaTypeNames;
    using static AVXLib.Framework.Numerics;

    public class Written
    {
        private Deserialization.Data Data;
#if INCLUDE_DEPRECATED_BEHAVIOR
        private ReadOnlyMemory<AVXLib.Memory.Book> Book { get => this.Data.Book; }
        private ReadOnlyMemory<AVXLib.Memory.Written> Writ { get => this.Data.Written; }
#endif
//      private static char[] subtract = new char[] { '-', ' ', '\'', '(', ')', '.', ':', ';', '!', '?', ',' };
        private static HashSet<char> subtract = new() { '-', ' ' };

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

            int idx = -1;
            foreach (var c in subtract)
            {
                int tmp = result.IndexOf(c);
                if (tmp < 0)
                    continue;
                if (tmp < idx || idx < 0)
                    idx = tmp;
            }
            if (idx >= 0)
            {
                int len = result.Length;
                var replacement = (idx == 0) ? new StringBuilder(len-1) : new StringBuilder(result.Substring(0, idx), len-1);

                for (++idx; idx < len; idx++)
                    if (!subtract.Contains(result[idx]))
                        replacement.Append(result[idx]);
                return replacement.ToString();
            }
            return result;
        }
#if INCLUDE_DEPRECATED_BEHAVIOR
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
#endif
    }
}
