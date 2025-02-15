namespace AVXLib.Memory
{
    public abstract class Deserialization
    {
        private static FileStreamOptions FileReadOptions;
        static Deserialization()
        {
            FileReadOptions = new();
            FileReadOptions.Share = FileShare.Read;
            FileReadOptions.Access = FileAccess.Read;
            FileReadOptions.Mode = FileMode.Open;
        }

        public static Dictionary<string, Artifact> Directory { get; private set; } = new();
        private static BinaryReader CreateReader(string input)
        {
            var fstream = new StreamReader(input, FileReadOptions);
            var reader = new BinaryReader(fstream.BaseStream);

            return reader;
        }
        public class Data
        {
            public Dictionary<string, Artifact> Directory { get; private init; }
            public readonly ReadOnlyMemory<Written> Written;
            public readonly ReadOnlyMemory<Book> Book;
            public readonly ReadOnlyMemory<Chapter> Chapter;
            public readonly ReadOnlyMemory<Lexicon> Lexicon;
            public readonly ReadOnlyMemory<Lemmata> Lemmata;
            public readonly Dictionary<ushort, ReadOnlyMemory<char>> OOVLemmata;
            public readonly Dictionary<ushort, ReadOnlyMemory<ReadOnlyMemory<char>>> Names;
            public readonly Dictionary<ushort, ReadOnlyMemory<char>> Phonetics;

            public readonly bool valid;

            public static UInt64 Version { get; private set; } = 0;

            // Shameless cut&paste from polymorphic method below
            public Data(IAVXObjectSetter objects, BinaryReader data, Type[]? selections = null)
            {
                this.Directory = new();
                this.valid = false;

                    try
                    {
                            for (var entry = new Artifact(data, Directory, selections); !entry.ERROR; entry = new Artifact(data, Directory, selections))
                            {
                                Console.WriteLine(entry.hash + ": " + entry.label);

                                if (Data.Version == 0)
                                    Data.Version = Directory["Directory"].hash_2;

                                if (entry.ERROR)
                                    goto DATA_READ_ERROR;
                                if (entry.DONE)
                                    break;
                            }
                            var written = AVXLib.Memory.Written.Read(data, Directory);
                            if (written.okay)
                            {
                                this.Written = written.result;
                                objects.Written = new AVXLib.Framework.Written(this);
                            }
                            else
                            {
                                Console.WriteLine(written.message);
                                goto DATA_READ_ERROR;
                            }

                            var books = AVXLib.Memory.Book.Read(data, Directory, written.result);
                            if (books.okay)
                            {
                                this.Book = books.result;
                            }
                            else
                            {
                                Console.WriteLine(books.message);
                                goto DATA_READ_ERROR;
                            }

                            var chapters = AVXLib.Memory.Chapter.Read(data, Directory);
                            if (chapters.okay)
                            {
                                this.Chapter = chapters.result;
                            }
                            else
                            {
                                Console.WriteLine(chapters.message);
                                goto DATA_READ_ERROR;
                            }
                            var lexicon = AVXLib.Memory.Lexicon.Read(data, Directory);
                            if (lexicon.okay)
                            {
                                this.Lexicon = lexicon.result;
                                objects.Lexicon = new AVXLib.Framework.Lexicon(this);
                            }
                            else
                            {
                                Console.WriteLine(lexicon.message);
                                goto DATA_READ_ERROR;
                            }

                            var lemmata = AVXLib.Memory.Lemmata.Read(data, Directory);
                            if (lemmata.okay)
                            {
                                this.Lemmata = lemmata.result;
                                objects.Lemmata = new AVXLib.Framework.Lemmata(this);
                            }
                            else
                            {
                                Console.WriteLine(lemmata.message);
                                goto DATA_READ_ERROR;
                            }

                            var oov = AVXLib.Memory.OOV.Read(data, Directory);
                            if (oov.okay)
                            {
                                this.OOVLemmata = oov.result;
                                objects.OOV = new AVXLib.Framework.OOV(this);
                            }
                            else
                            {
                                Console.WriteLine(oov.message);
                                goto DATA_READ_ERROR;
                            }
                            var names = AVXLib.Memory.Names.Read(data, Directory);
                            if (names.okay)
                            {
                                this.Names = names.result;
                            }
                            else
                            {
                                Console.WriteLine(names.message);
                                goto DATA_READ_ERROR;
                            }
                            var phonetics = AVXLib.Memory.Phonetics.Read(data, Directory);
                            if (phonetics.okay)
                            {
                                this.Phonetics = phonetics.result;
                            }
                            else
                            {
                                Console.WriteLine(phonetics.message);
                                goto DATA_READ_ERROR;
                            }
                        valid = true;

                        return;
                    }
                    catch
                    {
                        valid = false;
                    }

                return;
            DATA_READ_ERROR:

                Written = Memory<Written>.Empty;
                Book = Memory<Book>.Empty;
                Chapter = Memory<Chapter>.Empty;
                Names = new();
                Phonetics = new();

                Lexicon = Memory<Lexicon>.Empty;
                Lemmata = Memory<Lemmata>.Empty;
                OOVLemmata = new();
                valid = false;
                return;
            }

            public Data(IAVXObjectSetter objects, string dataPath, Type[]? selections = null)
            {
                this.Directory = new();
                this.valid = false;

                if (File.Exists(dataPath))
                {
                    try
                    {
                        using (var reader = CreateReader(dataPath))
                        {
                            for (var entry = new Artifact(reader, Directory, selections); !entry.ERROR; entry = new Artifact(reader, Directory, selections))
                            {
                                Console.WriteLine(entry.hash + ": " + entry.label);

                                if (entry.ERROR)
                                    goto DATA_READ_ERROR;
                                if (entry.DONE)
                                    break;
                            }
                            var written = AVXLib.Memory.Written.Read(reader, Directory);
                            if (written.okay)
                            {
                                this.Written = written.result;
                                objects.Written = new AVXLib.Framework.Written(this);
                            }
                            else
                            {
                                Console.WriteLine(written.message);
                                goto DATA_READ_ERROR;
                            }

                            var books = AVXLib.Memory.Book.Read(reader, Directory, written.result);
                            if (books.okay)
                            {
                                this.Book = books.result;
                            }
                            else
                            {
                                Console.WriteLine(books.message);
                                goto DATA_READ_ERROR;
                            }

                            var chapters = AVXLib.Memory.Chapter.Read(reader, Directory);
                            if (chapters.okay)
                            {
                                this.Chapter = chapters.result;
                            }
                            else
                            {
                                Console.WriteLine(chapters.message);
                                goto DATA_READ_ERROR;
                            }
                            var lexicon = AVXLib.Memory.Lexicon.Read(reader, Directory);
                            if (lexicon.okay)
                            {
                                this.Lexicon = lexicon.result;
                                objects.Lexicon = new AVXLib.Framework.Lexicon(this);
                            }
                            else
                            {
                                Console.WriteLine(lexicon.message);
                                goto DATA_READ_ERROR;
                            }

                            var lemmata = AVXLib.Memory.Lemmata.Read(reader, Directory);
                            if (lemmata.okay)
                            {
                                this.Lemmata = lemmata.result;
                                objects.Lemmata = new AVXLib.Framework.Lemmata(this);
                            }
                            else
                            {
                                Console.WriteLine(lemmata.message);
                                goto DATA_READ_ERROR;
                            }

                            var oov = AVXLib.Memory.OOV.Read(reader, Directory);
                            if (oov.okay)
                            {
                                this.OOVLemmata = oov.result;
                                objects.OOV = new AVXLib.Framework.OOV(this);
                            }
                            else
                            {
                                Console.WriteLine(oov.message);
                                goto DATA_READ_ERROR;
                            }
                            var names = AVXLib.Memory.Names.Read(reader, Directory);
                            if (names.okay)
                            {
                                this.Names = names.result;
                            }
                            else
                            {
                                Console.WriteLine(names.message);
                                goto DATA_READ_ERROR;
                            }
                            var phonetics = AVXLib.Memory.Phonetics.Read(reader, Directory);
                            if (phonetics.okay)
                            {
                                this.Phonetics = phonetics.result;
                            }
                            else
                            {
                                Console.WriteLine(phonetics.message);
                                goto DATA_READ_ERROR;
                            }
                        }
                        valid = true;

                        return;
                    }
                    catch
                    {
                        valid = false;
                    }
                }
                return;
            DATA_READ_ERROR:

                Written = Memory<Written>.Empty;
                Book = Memory<Book>.Empty;
                Chapter = Memory<Chapter>.Empty;
                Names = new();
                Phonetics = new();

                Lexicon = Memory<Lexicon>.Empty;
                Lemmata = Memory<Lemmata>.Empty;
                OOVLemmata = new();
                valid = false;
                return;
            }
        }
        internal static Memory<char> GetMemoryString(ReadOnlySpan<byte> bytes, int offset, int length)
        {
            int len = 0;
            for (int i = 0; i < length && i + offset < bytes.Length && bytes[i + offset] != 0; i++)
                len++;

            if (len == 0)
                return Memory<char>.Empty;

            var chars = new char[len];
            for (int i = 0; i < len; i++)
                chars[i] = (char)bytes[offset + i];
            return new Memory<char>(chars);
        }
        internal static (Memory<char> text, int length, bool overflow) ReadDelimitedMemory(BinaryReader reader, char delimiter, Span<byte> scratchPad)
        {
            int len = 0;
            for (byte val = reader.ReadByte(); val != delimiter; val = reader.ReadByte())
            {
                if (len + 1 > scratchPad.Length)
                    return (Memory<char>.Empty, 0, true);

                scratchPad[len++] = val;
            }
            if (len > 0)
            {
                var text = new char[len];
                for (int i = 0; i < len; i++)
                    text[i] = (char)scratchPad[i];
                return (new Memory<char>(text), len, false);
            }
            return (Memory<char>.Empty, 0, false);
        }
        internal static (ReadOnlyMemory<ReadOnlyMemory<char>> texts, bool valid) SplitDelimitedMemory(char delimiter, ReadOnlyMemory<char> mem)
        {
            if (mem.IsEmpty)
                return (ReadOnlyMemory<ReadOnlyMemory<char>>.Empty, false);
            int idx = 0;
            int cnt = 1;
            for (idx = 0; idx < mem.Length; idx++)
            {
                if (mem.Span[idx] == delimiter)
                    cnt++;
            }
            ReadOnlyMemory<char>[] items = new ReadOnlyMemory<char>[cnt];
            int delimitIdx = 0;
            for (idx = 0, cnt = 0; idx < mem.Length && cnt < items.Length; idx++)
            {
                if (idx == mem.Length - 1)
                {
                    items[cnt++] = mem.Slice(delimitIdx, idx - delimitIdx);
                }
                else if (mem.Span[idx] == delimiter)
                {
                    items[cnt++] = mem.Slice(delimitIdx, idx - delimitIdx);
                    delimitIdx = idx + 1;
                }
            }
            return (new ReadOnlyMemory<ReadOnlyMemory<char>>(items), true);
        }
    }
}
