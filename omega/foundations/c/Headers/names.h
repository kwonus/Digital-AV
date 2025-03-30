#include <avx.h>

namespace avx
{
    struct Names
    {
        u16 name_key;
        char* meanings;
    };
}
/*
        private static Dictionary<ushort, ReadOnlyMemory<ReadOnlyMemory<char>>> map = new();

        public static (Names name, bool valid) GetEntry(ushort key)
        {
            (Names name, bool valid) result = (new Names(), false);

            if (map.ContainsKey(key))
            {
                result.name.NameKey = key;
                result.name.meanings = map[key];
                result.valid = true;
            }
            return result;
        }

        public static (Dictionary<ushort, ReadOnlyMemory<ReadOnlyMemory<char>>> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            map.Clear();

            if (!directory.ContainsKey(typeof(Names).Name))
                return (map, false, "Names is missing from directory");
            Artifact artifact = directory[typeof(Names).Name];
            if (artifact.SKIP)
                return (map, true, "Names is explicitly skipped by request");

            Span<byte> buffer = stackalloc byte[512];

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (map, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            for (int o = 0; o < artifact.recordCount; o++)
            {
                var key = reader.ReadUInt16();
                var meanings = Deserialization.ReadDelimitedMemory(reader, '\0', buffer);

                if (key > 0 && meanings.length > 0 && meanings.overflow != true)
                {
                    var meaningCollection = Deserialization.SplitDelimitedMemory('|', meanings.text);

                    if (key > 0 && meaningCollection.valid)
                        map[key] = meaningCollection.texts;
                }
            }
            return (map, true, "");
        }
    }
}
*/
