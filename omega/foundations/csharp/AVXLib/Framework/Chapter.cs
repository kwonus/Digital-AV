using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVXLib.Framework
{
    public struct Chapter
    {
        public UInt16 writIdx;
        public UInt16 writCnt;
        public byte   bookNum;
        public byte   verseCnt;

        public static (ReadOnlyMemory<Chapter> result, bool okay, string message) Read(System.IO.BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey("Chapter"))
                return (Memory<Chapter>.Empty, false, "Chapter is missing from directory");

            Artifact artifact =directory["Chapter"];

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
