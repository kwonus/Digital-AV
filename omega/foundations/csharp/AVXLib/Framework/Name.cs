using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVXLib.Framework
{
    public class Name
    {
        public UInt16 NameKey;
        public ReadOnlyMemory<ReadOnlyMemory<char>> meanings;

        private static Dictionary<UInt16, ReadOnlyMemory<ReadOnlyMemory<char>>> map = new();

        public static (Name name, bool valid) GetEntry(UInt16 key)
        {
            (Name name, bool valid) result = (new Name(), false);

            if (Name.map.ContainsKey(key))
            {
                result.name.NameKey = key;
                result.name.meanings = Name.map[key];
                result.valid = true;
            }
            return result;
        }
 
        public static (Dictionary<UInt16, ReadOnlyMemory<ReadOnlyMemory<char>>> result, bool okay, string message) Read(System.IO.BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            Name.map.Clear();

            if (!directory.ContainsKey("Names"))
                return (map, false, "Names is missing from directory");
            Artifact artifact = directory["Names"];

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

                    if ((key > 0) && meaningCollection.valid)
                        map[key] = meaningCollection.texts;
                }
            }
            return (map, true, "");
        }
    }
}
