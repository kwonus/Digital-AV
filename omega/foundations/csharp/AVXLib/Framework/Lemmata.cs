using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVXLib.Framework
{
    public struct Lemmata
    {
        public UInt32 POS32;
        public UInt16 WordKey;
        public UInt16 pnPOS12;
        public ReadOnlyMemory<UInt16> Lemmas;

        public static (ReadOnlyMemory<Lemmata> result, bool okay, string message) Read(System.IO.BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey("Lemmata"))
                return (Memory<Lemmata>.Empty, false, "Lemmata is missing from directory");

            Artifact artifact = directory["Lemmata"];

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (ReadOnlyMemory<Lemmata>.Empty, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            var lemmata = new Lemmata[artifact.recordCount];

            for (int i = 0; i < artifact.recordCount; i++)
            {
                lemmata[i].POS32   = reader.ReadUInt32(); //  4 = 4
                lemmata[i].WordKey = reader.ReadUInt16(); //  2 = 6
                lemmata[i].pnPOS12 = reader.ReadUInt16(); //  2 = 8
                var lemmaCount     = reader.ReadUInt16();
                var lemmas         = new UInt16[lemmaCount];
                for (int j = 0; j < lemmaCount; j++)
                    lemmas[j] = reader.ReadUInt16();
                lemmata[i].Lemmas = new ReadOnlyMemory<UInt16>(lemmas);
            }
            return (new ReadOnlyMemory<Lemmata>(lemmata), true, "");
        }
    }
}
