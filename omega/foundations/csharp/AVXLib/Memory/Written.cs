using AVXLib.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AVXLib.Memory
{
    public struct BCVW
    {
        public UInt32 elements { get; private set; }

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
        public static Int64? operator -(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return 0;

            if (left < right)
                return 0 - (right - left);

            UInt32 L_BCV = left.elements  & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            UInt32 L_WC  = left.elements  & 0xFF;
            UInt32 R_WC  = right.elements & 0xFF;

            if (L_BCV == R_BCV)
            {
                return L_WC - R_WC;
            }
            return null;    // distance can only be calculated with Writ instance
        }
        public static Int64? operator -(ReadOnlySpan<Written> left, BCVW right)
        {
            if (left[0].BCVWc.elements == right.elements)
                return 0;

            if (left[0].BCVWc < right)
                return 0 - (right - left[0].BCVWc); // can return null!!! // only guarenteed non-null return: is to pass written/left that is <= the BCVW right/comparison value

            Int64? trivial = left[0].BCVWc - right;

            if (trivial != null)
            {
                return trivial.Value;
            }

            // The first element [left] is always less than the second [right]
            // count in forward direction;

            Int64 cnt = 0;
            for (int i = 0; left[0].BCVWc != right; i++, cnt++)
            {
                ;
            }
            return cnt;
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
