using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static AVX.Numerics.Written;

namespace AVXLib.Framework
{
    public struct BCVW
    {
        public UInt32 elements { get; private set; }

        internal byte this[int idx]
        {
            get
            {
                var shift = (idx - 1) * 8;
                switch (idx)
                {
                    case 3:
                    case 2:
                    case 1:
                    case 0: return (Byte)((this.elements >> shift) & 0xFF);
                }
                return 0;
            }
            set
            {
                switch (idx)
                {
                    case 3:
                    case 2:
                    case 1:
                    case 0:  break;
                    default: return; // silent errors
                }
                UInt64 others = 0x00FFFFFF;
                UInt64 shifted = (UInt32)(value << (3 * 8));
                for (int segment = 0; segment < idx; segment++)
                {
                    shifted >>= 8;
                    others  >>= 8;
                    others |= 0xFF000000;
                }
                this.elements = (UInt32)(shifted | others);
            }
        }
        public byte B
        {
            get => (byte)(this.elements >> 3*8);
        }
        public byte C
        {
            get => (byte)((this.elements >> 2 * 8) & 0xFF);
        }
        public byte V
        {
            get => (byte)((this.elements >> 8) & 0xFF);
        }
        public byte WC
        {
            get => (byte)(this.elements & 0xFF);
        }
    }
    public struct STRONGS
    {
        public UInt64 elements { get; private set; }
        public UInt16 this[int idx]
        {
            get
            {
                var shift = (idx-1) * 16;
                switch(idx)
                {
                    case 3:
                    case 2:
                    case 1:
                    case 0: return (UInt16)((this.elements >> shift) & 0xFFFF);
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
                    case 0:  break;
                    default: return; // silent errors
                }
                UInt64 others = 0x0000FFFFFFFFFFFF;
                UInt64 shifted = (UInt64)(value << (3 * 16));
                for (int segment = 0; segment < idx; segment++)
                {
                    shifted >>= 16;
                    others  >>= 16;
                    others |= 0xFFFF000000000000;
                }
                this.elements = (UInt64)(shifted | others);
            }
        }
    }
    public struct Written
    {
        public STRONGS Strongs; // UInt64 accessible as UInt16[]
        public BCVW    BCVWc;   // UInt32 accessible as Byte[]
        public UInt16  WordKey;
        public UInt16  pnPOS12;
        public UInt32  POS32;
        public UInt32  Lemma;
        public byte    Punctuation;
        public byte    Transition;

        public static (ReadOnlyMemory<Written> result, bool okay, string message) Read(System.IO.BinaryReader reader, Dictionary<string, Artifact> directory)
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
                UInt16[] strongs = new UInt16[] { reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() }; // 8 = 8;
                for (int i = 0; i < strongs.Length; i++)
                    written[w].Strongs[i] = strongs[i];
                byte[] bcvw = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() }; // 4 = 12;
                for (int i = 0; i < bcvw.Length; i++)
                    written[w].BCVWc[i] = bcvw[i];

                written[w].WordKey     = reader.ReadUInt16(); //  2 = 14
                written[w].Punctuation = reader.ReadByte();   //  1 = 15
                written[w].Transition  = reader.ReadByte();   //  1 = 16
                written[w].pnPOS12     = reader.ReadUInt16(); //  2 = 18
                written[w].POS32       = reader.ReadUInt32(); //  4 = 22
                written[w].Lemma       = reader.ReadUInt16(); //  2 = 24
            }
            return (new ReadOnlyMemory<Written>(written), true, "");
        }
    }
}
