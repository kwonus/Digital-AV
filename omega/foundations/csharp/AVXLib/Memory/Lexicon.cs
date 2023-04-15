using AVXLib.Framework;

namespace AVXLib.Memory
{
    using AVXLib;

    public struct Lexicon
    {
        public ReadOnlyMemory<char> Search;
        public ReadOnlyMemory<char> Display;
        public ReadOnlyMemory<char> Modern;
        public ReadOnlyMemory<uint> POS;
        public ushort Entities;
        public bool ModernSameAsOriginal;
        public static (ReadOnlyMemory<Lexicon> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey(typeof(Lexicon).Name))
                return (Memory<Lexicon>.Empty, false, "Lexicon is missing from directory");

            Artifact artifact = directory[typeof(Lexicon).Name];

            if (artifact.SKIP)
                return (Memory<Lexicon>.Empty, true, "Lexicon is explicitly skipped by request");

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (ReadOnlyMemory<Lexicon>.Empty, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            var lexicon = new Lexicon[artifact.recordCount];
            Span<byte> buffer = stackalloc byte[24];

            for (int i = 0; i < artifact.recordCount; i++)
            {
                lexicon[i].Entities = reader.ReadUInt16(); //  2 = 2
                var cnt = reader.ReadUInt16(); //  2 = 4
                var pos = new uint[cnt];                 //  2 = 6
                for (int j = 0; j < cnt; j++)
                    pos[j] = reader.ReadUInt32();
                lexicon[i].POS = new ReadOnlyMemory<uint>(pos);

                lexicon[i].Search = Deserialization.ReadDelimitedMemory(reader, '\0', buffer).text;
                var display = Deserialization.ReadDelimitedMemory(reader, '\0', buffer).text;
                var modern = Deserialization.ReadDelimitedMemory(reader, '\0', buffer).text;

                lexicon[i].Display = display.Length > 0 ? display : lexicon[i].Search;
                lexicon[i].Modern = modern.Length > 0 ? modern : lexicon[i].Display;
                lexicon[i].ModernSameAsOriginal = Framework.Written.ProcessReversals((ushort)i, lexicon[i].Search.ToString(), lexicon[i].Display.ToString(), lexicon[i].Modern.ToString());

            }
            return (new ReadOnlyMemory<Lexicon>(lexicon), true, "");
        }
    }
}
