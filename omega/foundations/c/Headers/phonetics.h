using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVXLib.Memory
{
    public class Phonetics
    {
        public ushort key;
        public ReadOnlyMemory<char> phonetics;
        private static Dictionary<ushort, ReadOnlyMemory<char>> map = new();

        public static (Dictionary<ushort, ReadOnlyMemory<char>> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey("Phonetic"))
                return (map, false, "Phonetics is missing from directory");
            Artifact artifact = directory["Phonetic"];
            if (artifact.SKIP)
                return (map, true, "Phonetics is explicitly skipped by request");

            Span<byte> buffer = stackalloc byte[1024];

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (map, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            for (int o = 0; o < artifact.recordCount; o++)
            {
                var key = reader.ReadUInt16();
                var val = Deserialization.ReadDelimitedMemory(reader, '\0', buffer);
                if (val.length > 0 && key > 0 && !val.overflow)
                {
                    map[key] = val.text;
                }
            }
            return (map, true, "");
        }
    }

}
