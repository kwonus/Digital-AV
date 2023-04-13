using AVXLib.Framework;

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
        public ushort WordKey;
        public ushort pnPOS12;
        public uint POS32;
        public uint Lemma;
        public byte Punctuation;
        public byte Transition;

        public static (ReadOnlyMemory<Written> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey("Written"))
                return (Memory<Written>.Empty, false, "Written is missing from directory");

            Artifact artifact = directory["Written"];

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
