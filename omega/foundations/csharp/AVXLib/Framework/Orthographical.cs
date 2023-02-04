using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static AVX.Numerics.Written;

namespace AVXLib.Framework
{
    public class Orthographical
    {
        Deserialization.Data data;

        public static bool ProcessReversals(UInt16 key, string search, string display, string modern)
        {
            var same = (display == modern);

            Orthographical.ReverseLex[Orthographical.Keyify(search)] = key;
            Orthographical.ReverseLexModern[Orthographical.Keyify(modern)] = key;

            return same;
        }

        private static ReadOnlyMemory<Lexicon> Lex = ReadOnlyMemory<Lexicon>.Empty;
        private static Dictionary<string, UInt16> ReverseLexModern = new();
        private static Dictionary<string, UInt16> ReverseLex = new();
        private static string[] subtract = new string[] { "-", " ", "'", "(", ")", ".", ":", ";", "!", "?", "," };

        public Orthographical(Deserialization.Data data)
        {
            this.data = data;
        }

        private static string Keyify(ReadOnlyMemory<char> input)
        {
            return (input.ToString());
        }
        private static string Keyify(string input)
        {
            string result = input.ToLower();

            foreach (var c in subtract)
            {
                if (result.IndexOf(c[0]) >= 0)
                    result = result.Replace(c, "");
            }
            return result;
        }
        public string GetLexNormalized(UInt16 id)
        {
            UInt16 caps = (UInt16)(id & WordKeyBits.CAPS);
            int key = id & WordKeyBits.WordKey;

            if (key > 0 && key < this.data.Lexicon.Length)
            {
                var lex = this.data.Lexicon.Span[key].Search.ToString();
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
        public string GetLexDisplay(UInt16 id)
        {
            UInt16 caps = (UInt16)(id & WordKeyBits.CAPS);
            int key = id & WordKeyBits.WordKey;

            if (key > 0 && key < this.data.Lexicon.Length)
            {
                var lex = this.data.Lexicon.Span[key].Display.ToString();
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
        public string GetLexModern(UInt16 id)
        {
            UInt16 caps = (UInt16)(id & WordKeyBits.CAPS);
            int key = id & WordKeyBits.WordKey;

            if (key > 0 && key < this.data.Lexicon.Length)
            {
                var lex = this.data.Lexicon.Span[key].Modern.ToString();
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
        public (UInt16 key, Lexicon lex, bool found) GetReverseLexRecord(string text)
        {
            var lookup = Orthographical.Keyify(text);
            if (Orthographical.ReverseLex.ContainsKey(lookup))
            {
                UInt16 key = Orthographical.ReverseLex[lookup];
                return (key, this.data.Lexicon.Span[key], true);
            }
            if (this.data.Lexicon.Length > 0)
            {
                return (0, this.data.Lexicon.Span[0], false);
            }
            return (0, new Lexicon(), false);
        }
        public (UInt16 key, Lexicon lex, bool found) GetReverseLexRecordModern(string text)
        {
            var lookup = Orthographical.Keyify(text);
            if (Orthographical.ReverseLexModern.ContainsKey(lookup))
            {
                UInt16 key = Orthographical.ReverseLexModern[lookup];
                return (key, this.data.Lexicon.Span[key], true);
            }
            if (this.data.Lexicon.Span.Length > 0)
            {
                return (0, this.data.Lexicon.Span[0], false);
            }
            return (0, new Lexicon(), false);
        }
        public (UInt16 key, Lexicon lex, bool found) GetLexRecord(UInt16 id)
        {
            UInt16 key = (UInt16)(id & WordKeyBits.WordKey);

            if (key > 0 && key < this.data.Lexicon.Span.Length)
            {
                return (key, this.data.Lexicon.Span[key], true);
            }
            if (this.data.Lexicon.Span.Length > 0)
            {
                return (0, this.data.Lexicon.Span[0], false);
            }
            return (0, new Lexicon(), false);
        }
        private static string PrePunc(UInt16 previousPunctuation, UInt16 currentPunctuation)
        {
            bool prevParen = ((previousPunctuation & Punctuation.Parenthetical) != (UInt16)0);
            bool thisParen = ((currentPunctuation  & Punctuation.Parenthetical) != (UInt16)0);

            return (thisParen && !prevParen) ? "(" : "";
        }
        private static string PostPunc(UInt16 punctuation, bool s)
        {
            bool eparen  = ((punctuation & Punctuation.CloseParen) == Punctuation.CloseParen);
            bool posses  = ((punctuation & Punctuation.Possessive) == Punctuation.Possessive);
            bool exclaim = ((punctuation & Punctuation.Clause) == Punctuation.Exclamatory);
            bool declare = ((punctuation & Punctuation.Clause) == Punctuation.Declarative);
            bool dash    = ((punctuation & Punctuation.Clause) == Punctuation.Dash);
            bool semi    = ((punctuation & Punctuation.Clause) == Punctuation.Semicolon);
            bool colon   = ((punctuation & Punctuation.Clause) == Punctuation.Colon);
            bool comma   = ((punctuation & Punctuation.Clause) == Punctuation.Comma);
            bool quest   = ((punctuation & Punctuation.Clause) == Punctuation.Interrogative);

            String punc = posses ? !s ? "'s" : "'" : "";
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
        public string GetDisplayWithPunctuation(byte bookNum, UInt32 writIdx)
        {
            if (this.data.Written.Length > 0 && bookNum < this.data.Book.Length)
            {
                var book = this.data.Book.Span[(int)bookNum];

                if (writIdx < book.written.Length)
                {
                    var writ = book.written.Span[(int)writIdx];
                    var text = this.GetLexDisplay(writ.WordKey);

                    var punc = writ.Punctuation;
                    var puncPrev = writIdx >= 1 ? book.written.Span[(int)writIdx].Punctuation : (byte)0x00;
                    var s = text.EndsWith("s", StringComparison.InvariantCultureIgnoreCase);

                    return Orthographical.PrePunc(puncPrev, punc) + text + Orthographical.PostPunc(punc, s);
                }
            }
            return "";
        }
        public string GetModernWithPunctuation(byte bookNum, UInt16 writIdx)
        {
            if (this.data.Written.Length > 0 && bookNum < this.data.Book.Length)
            {
                var book = this.data.Book.Span[(int)bookNum];

                if (writIdx < book.written.Length)
                {
                    var writ = book.written.Span[(int)writIdx];
                    var text = this.GetLexModern(writ.WordKey);

                    var punc = writ.Punctuation;
                    var puncPrev = writIdx >= 1 ? book.written.Span[(int)writIdx].Punctuation : (byte)0x00;
                    var s = text.EndsWith("s", StringComparison.InvariantCultureIgnoreCase);

                    return Orthographical.PrePunc(puncPrev, punc) + text + Orthographical.PostPunc(punc, s);
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
                    var abbr = this.data.Book.Span[b].abbr2.ToString();
                    if (name.Equals(abbr, StringComparison.InvariantCultureIgnoreCase))
                        return (this.data.Book.Span[b], true);
                }
            }
            if (len <= 3)
            {
                for (int b = 1; b <= 66; b++)
                {
                    var abbr = this.data.Book.Span[b].abbr3.ToString();
                    if (name.Equals(abbr, StringComparison.InvariantCultureIgnoreCase))
                        return (this.data.Book.Span[b], true);
                }
            }
            if (len <= 4)
            {
                for (int b = 1; b <= 66; b++)
                {
                    var abbr = this.data.Book.Span[b].abbr4.ToString();
                    if (name.Equals(abbr, StringComparison.InvariantCultureIgnoreCase))
                        return (this.data.Book.Span[b], true);
                }
            }
            for (int b = 1; b <= 66; b++)
            {
                string test = this.data.Book.Span[b].name.ToString();
                if (name.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                    return (this.data.Book.Span[b], true);

                test = this.data.Book.Span[b].abbrAltA.ToString();
                if (name.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                    return (this.data.Book.Span[b], true);

                test = this.data.Book.Span[b].abbrAltB.ToString();
                if (name.Equals(test, StringComparison.InvariantCultureIgnoreCase))
                    return (this.data.Book.Span[b], true);
            }
            return (new Book(), false);
        }
        // For Part-of-Speech:
        public static UInt32 EncodePOS(string input7charsMaxWithHyphen)
        { // input string must be ascii
            var len = input7charsMaxWithHyphen.Length;
            if (len < 1 || len > 7)
                return 0;

            var input = input7charsMaxWithHyphen.Trim().ToLower();
            len = input7charsMaxWithHyphen.Length;
            if (len < 1 || len > 7)
                return 0;

            var encoded = (UInt32)0x0;
            var hyphen = (UInt32)input.IndexOf('-');
            if (hyphen > 0 && hyphen <= 3)
                hyphen <<= 30;
            else if (len > 6)   // 6 characters max if a compliant hyphen is not part of the string
                return 0;
            else
                hyphen = (UInt32)0x0;

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
                        b += (char)(27);
                        break;
                }
                buffer[c++] = b;
            }
            var position = (UInt32)0x02000000;
            for (var i = 0; i < 6 - len; i++)
            {
                position >>= 5;
            }
            for (var i = 0; i < len; i++)
            {
                char letter = (char)(buffer[i] & 0x1F);
                if (letter == 0)
                    break;

                encoded |= (UInt32)letter * position;
                position >>= 5;
            }
            return (UInt32)(encoded | hyphen);
        }
        //  For Part-of-Speech:
        public static string DecodePOS(UInt32 encoding)
        {
            char[] buffer = new char[7]; // 6x 5bit characters + 2bits for hyphen position = 32 bits;

            var hyphen = (UInt32)(encoding & 0xC0000000);
            if (hyphen > 0)
                hyphen >>= 30;

            var index = 0;
            for (var mask = (UInt32)(0x1F << 25); mask >= 0x1F; mask >>= 5)
            {
                var digit = encoding & mask >> (5 * (5 - index));
                if (digit == 0)
                    continue;
                byte b = (byte)digit;
                if (b <= 26)
                    b |= 0x60;
                else
                {
                    b -= (byte)27;
                    b += (byte)'0';
                }
                if (hyphen == index)
                    buffer[index++] = '-';
                buffer[index++] = (char)b;
            }
            var decoded = new StringBuilder(index + 1);
            for (int i = 0; i < index; i++)
                decoded.Append((char)buffer[i]);
            return decoded.ToString();
        }
    }
}
