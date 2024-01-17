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

                lexicon[i].Search  = Deserialization.ReadDelimitedMemory(reader, '\0', buffer).text;
                lexicon[i].Display = Deserialization.ReadDelimitedMemory(reader, '\0', buffer).text;
                lexicon[i].Modern  = Deserialization.ReadDelimitedMemory(reader, '\0', buffer).text;
            }
            return (new ReadOnlyMemory<Lexicon>(lexicon), true, "");
        }
    }

    public abstract class LEXICON
    {
        public static string ToSearchString(Lexicon lex)
        {
            return lex.Search.ToString();
        }
        public static string ToDisplayString(Lexicon lex)
        {
            return (lex.Display.Length > 0) ? lex.Display.ToString() : lex.Search.ToString(); 
        }
        public static string ToModernString(Lexicon lex)
        {
            return (lex.Modern.Length  > 0) ? lex.Modern.ToString()  : (lex.Display.Length > 0) ? lex.Display.ToString() : lex.Search.ToString();
        }
        public static bool IsModernSameAsDisplay(Lexicon lex)
        {
            return (lex.Modern.Length == 0);
        }
        public static bool IsHyphenated(Lexicon lex)
        {
            return (lex.Display.Length != 0);
        }
    }
}
