#include <avx.h>

namespace avx
{
    struct oov_lemmata
    {
        u16 oov_key;
        char* oov_text;
    };
}
/*
        public static (Dictionary<ushort, ReadOnlyMemory<char>> result, bool okay, string message) Read(BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            if (!directory.ContainsKey("OOV-Lemmata"))
                return (map, false, "OOV-Lemmata is missing from directory");
            Artifact artifact = directory["OOV-Lemmata"];
            if (artifact.SKIP)
                return (map, true, "OOV-Lemmata is explicitly skipped by request");

            Span<byte> buffer = stackalloc byte[24];

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
*/
