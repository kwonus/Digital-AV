using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AVXLib.Framework
{
    public struct Book
    {
        public byte   bookNum;
        public byte   chapterCnt;
        public UInt16 chapterIdx;
        public UInt16 verseCnt;
        public UInt32 writCnt;
        public UInt32 writIdx;
        public ReadOnlyMemory<char> name;
        public ReadOnlyMemory<char> abbr2;
        public ReadOnlyMemory<char> abbr3;
        public ReadOnlyMemory<char> abbr4;
        public ReadOnlyMemory<char> abbrAltA;
        public ReadOnlyMemory<char> abbrAltB;
        public ReadOnlyMemory<Written> written;

        public static (ReadOnlyMemory<Book> result, bool okay, string message) Read(System.IO.BinaryReader reader, Dictionary<string, Artifact> directory, ReadOnlyMemory<Written> written)
        {
            if (!directory.ContainsKey("Book"))
                return (Memory<Book>.Empty, false, "Book is missing from directory");
            Artifact artifact = directory["Book"];

            Span<byte> bname = stackalloc byte[16];
            Span<byte> babbr = stackalloc byte[18];

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (ReadOnlyMemory<Book>.Empty, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            var book = new Book[artifact.recordCount];

            for (int b = 0; b < artifact.recordCount; b++)
            {
                book[b].bookNum    = reader.ReadByte();      //  1 =  1
                book[b].chapterCnt = reader.ReadByte();      //  1 =  2
                book[b].chapterIdx = reader.ReadUInt16();    //  2 =  4
                book[b].verseCnt   = reader.ReadUInt16();    //  2 =  6
                book[b].writCnt    = reader.ReadUInt32();    //  4 = 10
                book[b].writIdx    = reader.ReadUInt32();    //  4 = 14

                if (reader.Read(bname) != bname.Length || reader.Read(babbr) != babbr.Length) // 16 + 18 + 14 = 48
                {
                    return (ReadOnlyMemory<Book>.Empty, false, "Could not read bytes from nput stream");
                }
                book[b].name  = Deserialization.GetMemoryString(bname, 0, bname.Length);
                book[b].abbr2 = Deserialization.GetMemoryString(babbr, 0, 2);
                book[b].abbr3 = Deserialization.GetMemoryString(babbr, 2, 3);
                book[b].abbr4 = Deserialization.GetMemoryString(babbr, 5, 4);
                book[b].written = written.Slice((int)book[b].writIdx, (int)book[b].writCnt);

                if (babbr[9] > 0)
                {
                    var alts = Deserialization.GetMemoryString(babbr, 9, 9).ToString().Split(',');
                    if (alts.Length == 1)
                    {
                        book[b].abbrAltA = alts[0].AsMemory();
                        book[b].abbrAltB = Memory<char>.Empty;
                        continue;
                    }
                    if (alts.Length >= 2)
                    {
                        book[b].abbrAltA = alts[0].AsMemory();
                        book[b].abbrAltB = alts[1].AsMemory();
                        continue;
                    }
                }
                book[b].abbrAltA = Memory<char>.Empty;
                book[b].abbrAltB = Memory<char>.Empty;
            }
            return (new ReadOnlyMemory<Book>(book), true, "");
        }
    }
}
