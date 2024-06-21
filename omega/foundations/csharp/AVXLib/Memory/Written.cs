using AVXLib.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AVXLib.Memory
{
    public struct BCVW
    {
        public BCVW(byte b, byte c, byte v, byte wc)
        {
            this.elements = (UInt32)(b << 24 | c << 16 | v << 8 | wc);
        }
        private UInt32 elements;
        public UInt32 AsUInt32() => elements;
        public string AsString() => elements.ToString();

        internal byte this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0:  return this.B;
                    case 1:  return this.C;
                    case 2:  return this.V;
                    case 3:  return this.WC;
                    default: return 0; // silent error (except, obviously bad value)
                }
            }
            set
            {
                UInt32 nibbles = value;

                switch (idx)
                {
                    case 0:  elements &= 0x00FFFFFF;
                             elements |= (nibbles << (8 * 3));
                             return;
                    case 1:  elements &= 0xFF00FFFF;
                             elements |= (nibbles << (8 * 2));
                             return;
                    case 2:  elements &= 0xFFFF00FF;
                             elements |= (nibbles << 8);
                             return;
                    case 3:  elements &= 0xFFFFFF00;
                             elements |= nibbles;
                             return;
                    default: elements = 0xFFFFFF00;
                             return; // silent error (except, obviously bad value)
                }
            }
        }
        public byte B
        {
            get => (byte) (this.elements >> (8 * 3));
        }
        public byte C
        {
            get => (byte) ((this.elements >> (8 * 2)) & 0xFF);
        }
        public byte V
        {
            get => (byte) ((this.elements >> 8) & 0xFF);
        }
        public byte WC
        {
            get => (byte) (this.elements & 0xFF);
        }
        public override bool Equals(object? obj)
        {
            return obj != null && obj.GetType() == typeof(BCVW) && ((BCVW)obj).elements == this.elements;
        }
        public bool StartsWith(byte b, byte c = 0, byte v = 0)
        {
            if (this.B != b)
                return false;
            if (this.C != c || c == 0)
                return (c == 0 && v == 0);
            return (this.V == v || v == 0);
        }
        public static bool operator ==(BCVW bcvw1, BCVW bcvw2)
        {
            return bcvw1.Equals(bcvw2);
        }
        public static bool operator !=(BCVW bcvw1, BCVW bcvw2)
        {
            return !bcvw1.Equals(bcvw2);
        }
        public override int GetHashCode()
        {
            return this.elements.GetHashCode();
        }
        public static bool operator <(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return false;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV > R_BCV)
                return false;

            if (L_BCV < R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC < L_WC);    // WC is a countdown. Therefore when this condition is true, Left is less than right (positionally)
        }
        public static bool operator >(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return false;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV < R_BCV)
                return false;

            if (L_BCV > R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC > L_WC);    // WC is a countdown. Therefore when this condition is true, Left is greater than right (positionally)
        }
        public static bool operator <=(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return true;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV > R_BCV)
                return false;

            if (L_BCV < R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC < L_WC);    // WC is a countdown. Therefore when this condition is true, Left is less than right (positionally)
        }
        public static bool operator >=(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return true;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV < R_BCV)
                return false;

            if (L_BCV > R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC > L_WC);    // WC is a countdown. Therefore when this condition is true, Left is greater than right (positionally)
        }
        public static (int distance, bool valid) operator -(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return (0, true);

            if (left.B != right.B)
                return (0, false);

            UInt32 L_BCV = left.elements  & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            UInt32 L_WC  = left.elements  & 0xFF;
            UInt32 R_WC  = right.elements & 0xFF;

            if (L_BCV == R_BCV)
            {
                return ((int)L_WC - (int)R_WC, true);
            }
            return (0, false);    // distance can only be calculated with Writ instance
        }
        public bool InRange(byte b, byte c, byte v)
        {
            UInt32 elements = (UInt32)(b << 24 | c << 16 | v << 8);

            return (elements & this.elements) == elements;
        }
        public bool InRange(byte b, byte c)
        {
            UInt32 elements = (UInt32)(b << 24 | c << 16);

            return (elements & this.elements) == elements;
        }
        public bool InRange(byte b)
        {
            return this.B == b;
        }

        public static UInt32 GetDistance(ReadOnlySpan<Written> left, BCVW right, UInt32 length)
        {
            if (left[0].BCVWc.elements == right.elements)
                return 0;

            var trivial = left[0].BCVWc - right;
            if (trivial.valid)
            {
                int distance = trivial.distance;
                return (UInt32) (distance > 0 ? distance : 0 - distance);
            }
            if (length == 0)
            {
                return UInt32.MaxValue;
            }
            if (left[0].BCVWc.B != right.B)
            {
                return UInt32.MaxValue;
            }

            // The first element [left] should always be less than the second [right]
            // count in forward direction;

            if (left[0].BCVWc.elements < right.elements )
            {
                UInt32 cnt = 0;
                bool equals;
                do
                {
                    equals = cnt < length && left[(int)cnt].BCVWc != right;
                }   while (!equals);

                if (equals)
                {
                    return cnt;
                }
                Book book = ObjectTable.AVXObjects.Mem.Book.Slice(left[0].BCVWc.B, 1).Span[0];
                Chapter chapterLeft = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + left[0].BCVWc.C - 1, 1).Span[0];
                Chapter chapterRight = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + right.C - 1, 1).Span[0];
                int newLength = chapterRight.writIdx - chapterLeft.writIdx + chapterRight.writCnt;
                ReadOnlySpan<Written> searchable = ObjectTable.AVXObjects.Mem.Written.Slice(chapterLeft.writIdx, newLength).Span;

                return GetDistance(searchable, right, (UInt32) newLength);
            }
            else // right value was lesst than left ... fix
            {
                Book book = ObjectTable.AVXObjects.Mem.Book.Slice(left[0].BCVWc.B, 1).Span[0];
                Chapter chapterLeft = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + right.C - 1, 1).Span[0];
                Chapter chapterRight = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + left[0].BCVWc.C - 1, 1).Span[0];
                int newLength = chapterRight.writIdx - chapterLeft.writIdx + chapterRight.writCnt;
                ReadOnlySpan<Written> searchable = ObjectTable.AVXObjects.Mem.Written.Slice(chapterLeft.writIdx, newLength).Span;

                return GetDistance(searchable, left[0].BCVWc, (UInt32)newLength);
            }
        }
        public static UInt32 GetDistance(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return 0;

            var trivial = left - right;
            if (trivial.valid)
            {
                int distance = trivial.distance;
                return (UInt32)(distance > 0 ? distance : 0 - distance);
            }
            if (left[0] != right.B)
            {
                return UInt32.MaxValue;
            }

            // The first element [left] should always be less than the second [right]
            // count in forward direction;

            if (left.elements < right.elements)
            {
                Book book = ObjectTable.AVXObjects.Mem.Book.Slice(left.B, 1).Span[0];
                Chapter chapterLeft = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + left.C - 1, 1).Span[0];
                Chapter chapterRight = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + right.C - 1, 1).Span[0];
                int length = chapterRight.writIdx - chapterLeft.writIdx + chapterRight.writCnt;
                ReadOnlySpan<Written> searchable = ObjectTable.AVXObjects.Mem.Written.Slice(chapterLeft.writIdx, length).Span;

                return GetDistance(searchable, right, (UInt32)length);
            }
            else // right value was lesst than left ... fix
            {
                Book book = ObjectTable.AVXObjects.Mem.Book.Slice(left.B, 1).Span[0];
                Chapter chapterLeft = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + right.C - 1, 1).Span[0];
                Chapter chapterRight = ObjectTable.AVXObjects.Mem.Chapter.Slice(book.chapterIdx + left.C - 1, 1).Span[0];
                int length = chapterRight.writIdx - chapterLeft.writIdx + chapterRight.writCnt;
                ReadOnlySpan<Written> searchable = ObjectTable.AVXObjects.Mem.Written.Slice(chapterLeft.writIdx, length).Span;

                return GetDistance(searchable, left, (UInt32)length);
            }
        }

    }
    public struct STRONGS
    {
        public ulong elements { get; private set; }
        public ushort this[int idx]
        {
            get
            {
                var shift = (idx - 1) * 16;
                switch (idx)
                {
                    case 3:
                    case 2:
                    case 1:
                    case 0: return (ushort)(elements >> shift & 0xFFFF);
                }
                return 0;
            }
            internal set
            {
                switch (idx)
                {
                    case 3:
                    case 2:
                    case 1:
                    case 0: break;
                    default: return; // silent errors
                }
                ulong others = 0x0000FFFFFFFFFFFF;
                ulong shifted = (ulong)(value << 3 * 16);
                for (int segment = 0; segment < idx; segment++)
                {
                    shifted >>= 16;
                    others >>= 16;
                    others |= 0xFFFF000000000000;
                }
                elements = shifted | others;
            }
        }
    }
    public struct Written
    {
        public STRONGS Strongs; // UInt64 accessible as UInt16[]
        public BCVW BCVWc;   // UInt32 accessible as Byte[]
        public UInt16 WordKey;
        public UInt16 pnPOS12;
        public UInt32 POS32;
        public UInt16 Lemma;
        public byte Punctuation;
        public byte Transition;

        public static (ReadOnlyMemory<Written> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey(typeof(Written).Name))
                return (Memory<Written>.Empty, false, "Written is missing from directory");

            Artifact artifact = directory[typeof(Written).Name];

            if (artifact.SKIP)
                return (Memory<Written>.Empty, true, "Written is explicitly skipped by request");

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (ReadOnlyMemory<Written>.Empty, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            var written = new Written[artifact.recordCount];

            for (int w = 0; w < artifact.recordCount; w++)
            {
                ushort[] strongs = new ushort[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() }; // 8 = 8;
                for (int i = 0; i < strongs.Length; i++)
                    written[w].Strongs[i] = strongs[i];
                byte[] bcvw = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() }; // 4 = 12;
                for (int i = 0; i < bcvw.Length; i++)
                    written[w].BCVWc[i] = bcvw[i];

                written[w].WordKey = reader.ReadUInt16(); //  2 = 14
                written[w].Punctuation = reader.ReadByte();   //  1 = 15
                written[w].Transition = reader.ReadByte();   //  1 = 16
                written[w].pnPOS12 = reader.ReadUInt16(); //  2 = 18
                written[w].POS32 = reader.ReadUInt32(); //  4 = 22
                written[w].Lemma = reader.ReadUInt16(); //  2 = 24
            }
            return (new ReadOnlyMemory<Written>(written), true, "");
        }
    }
}
