namespace AVXLib.Memory
{
    public struct Book
    {
        public byte bookNum;
        public byte chapterCnt;
        public UInt16 chapterIdx;
        public uint writCnt;
        public uint writIdx;
        public ReadOnlyMemory<char> name;
        public ReadOnlyMemory<char> abbr2;
        public ReadOnlyMemory<char> abbr3;
        public ReadOnlyMemory<char> abbr4;
        public ReadOnlyMemory<char> abbrAlternates;
        public ReadOnlyMemory<Written> written;

        public static (ReadOnlyMemory<Book> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory, ReadOnlyMemory<Written> written)
        {
            if (!directory.ContainsKey(typeof(Book).Name))
                return (Memory<Book>.Empty, false, "Book is missing from directory");
            Artifact artifact = directory[typeof(Book).Name];

            if (artifact.SKIP)
                return (Memory<Book>.Empty, true, "Book is explicitly skipped by request");

            Span<byte> bname = stackalloc byte[16];
            Span<byte> babbr = stackalloc byte[22];

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (ReadOnlyMemory<Book>.Empty, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            var book = new Book[artifact.recordCount];

            for (int b = 0; b < artifact.recordCount; b++)
            {
                int len = 0;
                book[b].bookNum = reader.ReadByte();      len ++;
                book[b].chapterCnt = reader.ReadByte();   len ++;  
                book[b].chapterIdx = reader.ReadUInt16(); len += 2;
                book[b].writCnt = reader.ReadUInt16();    len += 2;
                book[b].writIdx = reader.ReadUInt32();    len += 4;

                if (reader.Read(bname) != bname.Length || reader.Read(babbr) != babbr.Length)
                {
                    return (ReadOnlyMemory<Book>.Empty, false, "Could not read bytes from input stream");
                }
                len += bname.Length;
                len += babbr.Length;
                
                book[b].name = Deserialization.GetMemoryString(bname, 0, bname.Length);
                book[b].abbr2 = Deserialization.GetMemoryString(babbr, 0, 2);
                book[b].abbr3 = Deserialization.GetMemoryString(babbr, 3, 3);
                book[b].abbr4 = Deserialization.GetMemoryString(babbr, 7, 4);
                book[b].abbrAlternates = Deserialization.GetMemoryString(babbr, 12, 10);
                
                book[b].written = written.Slice((int)book[b].writIdx, (int)book[b].writCnt);
            }
            return (new ReadOnlyMemory<Book>(book), true, "");
        }
    }
}
