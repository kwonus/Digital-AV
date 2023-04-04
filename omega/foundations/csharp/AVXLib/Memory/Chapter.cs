namespace AVXLib.Memory
{
    public struct Chapter
    {
        public ushort writIdx;
        public ushort writCnt;
        public byte bookNum;
        public byte verseCnt;

        public static (ReadOnlyMemory<Chapter> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey("Chapter"))
                return (Memory<Chapter>.Empty, false, "Chapter is missing from directory");

            Artifact artifact = directory["Chapter"];

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (ReadOnlyMemory<Chapter>.Empty, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            var chapter = new Chapter[artifact.recordCount];

            for (int c = 0; c < artifact.recordCount; c++)
            {
                chapter[c].writIdx = reader.ReadUInt16(); //  2 = 2
                chapter[c].writCnt = reader.ReadUInt16(); //  2 = 4
                chapter[c].bookNum = reader.ReadByte();   //  1 = 5
                chapter[c].verseCnt = reader.ReadByte();  //  1 = 6
            }
            return (new ReadOnlyMemory<Chapter>(chapter), true, "");
        }
    }
}
