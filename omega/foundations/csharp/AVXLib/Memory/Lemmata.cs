namespace AVXLib.Memory
{
    public struct Lemmata
    {
        public uint POS32;
        public ushort WordKey;
        public ushort pnPOS12;
        public ReadOnlyMemory<ushort> Lemmas;

        //      private static ReadOnlyMemory<Lemmata> Data = null;

        public static (ReadOnlyMemory<Lemmata> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
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
                lemmata[i].POS32 = reader.ReadUInt32(); //  4 = 4
                lemmata[i].WordKey = reader.ReadUInt16(); //  2 = 6
                lemmata[i].pnPOS12 = reader.ReadUInt16(); //  2 = 8
                var lemmaCount = reader.ReadUInt16();
                var lemmas = new ushort[lemmaCount];
                for (int j = 0; j < lemmaCount; j++)
                    lemmas[j] = reader.ReadUInt16();
                lemmata[i].Lemmas = new ReadOnlyMemory<ushort>(lemmas);
            }
            return (new ReadOnlyMemory<Lemmata>(lemmata), true, "");
        }
    }
}
