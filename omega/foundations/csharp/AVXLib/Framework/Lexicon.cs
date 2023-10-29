namespace AVXLib.Framework
{
    using AVXLib.Memory;
    using static AVXLib.Framework.Numerics;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;

    public class Lexicon
    {
        private Deserialization.Data data;
        private ReadOnlyMemory<AVXLib.Memory.Lexicon> Lex { get => this.data.Lexicon; }

        public Lexicon(Deserialization.Data data)
        {
            this.data = data;
        }
        public static bool ProcessReversals(ushort key, string search, string display, string modern)
        {
            var same = display == modern;

            ReverseLex[Keyify(search)] = key;

            var modkey = Keyify(modern);
            if (ReverseLexModern.ContainsKey(modkey))
            {
                if (!ReverseLexModern[modkey].Contains(key))
                    ReverseLexModern[modkey].Add(key);
            }
            else
            {
                var keys = new HashSet<ushort>() { key };
                ReverseLexModern[modkey] = keys;
            }
            return same;
        }

        private static Dictionary<string, HashSet<UInt16>> ReverseLexModern = new();
        private static Dictionary<string, UInt16> ReverseLex = new();
//      private static char[] subtract = new char[] { '-', ' ', '\'', '(', ')', '.', ':', ';', '!', '?', ',' };
        private static HashSet<char> subtract = new() { '-', ' ' };

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
                var replacement = (idx == 0) ? new StringBuilder(len - 1) : new StringBuilder(result.Substring(0, idx), len - 1);

                for (++idx; idx < len; idx++)
                    if (!subtract.Contains(result[idx]))
                        replacement.Append(result[idx]);
                return replacement.ToString();
            }
            return result;
        }
        public (AVXLib.Memory.Lexicon entry, bool valid) GetRecord(UInt16 id)
        {
            int key = id & WordKeyBits.WordKey;

            bool valid = ((key > 0) && (key < this.Lex.Length));
            if (!valid)
                key = 0;

            return (this.Lex.Slice(key, 1).Span[0], valid); 
        }
        public UInt16 RecordCount { get => (UInt16) this.Lex.Length; }

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
        public HashSet<UInt16>? GetReverseLexModern(string text)
        {
            var lookup = Keyify(text);
            if (ReverseLexModern.ContainsKey(lookup))
            {
                var key = ReverseLexModern[lookup];
                return key;
            }
            return null;
        }
        // TO DO: fix cardinality issues or reverse lex lookups
        public UInt16[] GetReverseLexExtensive(string text, bool useAV = true, bool useAVX = true, byte phonicsThreshold = 0, byte textThreshold = 100)
        {
            HashSet<UInt16>? lex = null;

            var keyified = Lexicon.Keyify(text);

            if (useAVX)
            {
                lex = this.GetReverseLexModern(keyified);
            }
            var kjv = this.GetReverseLex(keyified);
            if (kjv != 0)
            {
                if (lex == null)
                {
                    lex = new HashSet<UInt16>();
                    lex.Add(kjv);
                }
                else if (!lex.Contains(kjv))
                {
                    lex.Add(kjv);
                }
            }
            return lex != null ? lex.ToArray() : new UInt16[0];
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