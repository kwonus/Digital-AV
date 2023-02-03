using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AVXLib.Framework
{
    public abstract class Deserialization
    {
        private static FileStreamOptions FileReadOptions;
        static Deserialization()
        {
            Deserialization.FileReadOptions        = new();
            Deserialization.FileReadOptions.Share  = FileShare.Read;
            Deserialization.FileReadOptions.Access = FileAccess.Read;
            Deserialization.FileReadOptions.Mode   = FileMode.Open;
        }

        public static Dictionary<string, Artifact> Directory { get; private set; } = new();
        private static System.IO.BinaryReader CreateReader(string input = @"C:\src\Digital-AV\omega\AVX-Omega.data")
        {
            var fstream = new StreamReader(input, Deserialization.FileReadOptions);
            var reader = new System.IO.BinaryReader(fstream.BaseStream);

            return reader;
        }
        public class Data
        {
            public Dictionary<string, Artifact> Directory { get; private init; }
            public readonly ReadOnlyMemory<Framework.Written>  Written;
            public readonly ReadOnlyMemory<Framework.Book>     Book;
            public readonly ReadOnlyMemory<Framework.Chapter>  Chapter;
            public readonly ReadOnlyMemory<Framework.Lexicon>  Lexicon;
            public readonly ReadOnlyMemory<Framework.Lemmata>  Lemmata;
            public readonly Dictionary<UInt16, ReadOnlyMemory<char>>  OOVLemmata;
            public readonly Dictionary<UInt16, ReadOnlyMemory<ReadOnlyMemory<char>>> Names;
            public readonly bool valid;

            public Data(string dataPath = @"C:\src\Digital-AV\omega\AVX-Omega.data")
            {
                this.Directory = new();
                this.valid = false;

                if (System.IO.File.Exists(dataPath))
                {
                    try
                    {
                        using (var reader = Deserialization.CreateReader(dataPath))
                        {
                            for (var entry = new Artifact(reader, this.Directory); !entry.ERROR; entry = new Artifact(reader, this.Directory))
                            {
                                Console.WriteLine(entry.hash + ": " + entry.label);

                                if (entry.ERROR)
                                    goto DATA_READ_ERROR;
                                if (entry.DONE)
                                    break;
                            }

                            var books = Framework.Book.Read(reader, this.Directory);
                            if (books.okay)
                            {
                                this.Book = books.result;
                            }
                            else
                            {
                                Console.WriteLine(books.message);
                                goto DATA_READ_ERROR;
                            }

                            var chapters = Framework.Chapter.Read(reader, this.Directory);
                            if (chapters.okay)
                            {
                                this.Chapter = chapters.result;
                            }
                            else
                            {
                                Console.WriteLine(chapters.message);
                                goto DATA_READ_ERROR;
                            }

                            var written = Framework.Written.Read(reader, this.Directory);
                            if (written.okay)
                            {
                                this.Written = written.result;
                            }
                            else
                            {
                                Console.WriteLine(written.message);
                                goto DATA_READ_ERROR;
                            }

                            var lexicon = Framework.Lexicon.Read(reader, this.Directory);
                            if (lexicon.okay)
                            {
                                this.Lexicon = lexicon.result;
                            }
                            else
                            {
                                Console.WriteLine(lexicon.message);
                                goto DATA_READ_ERROR;
                            }

                            var lemmata = Framework.Lemmata.Read(reader, this.Directory);
                            if (lemmata.okay)
                            {
                                this.Lemmata = lemmata.result;
                            }
                            else
                            {
                                Console.WriteLine(lemmata.message);
                                goto DATA_READ_ERROR;
                            }

                            var oov = Framework.OOV.Read(reader, this.Directory);
                            if (oov.okay)
                            {
                                this.OOVLemmata = oov.result;
                            }
                            else
                            {
                                Console.WriteLine(oov.message);
                                goto DATA_READ_ERROR;
                            }

                            var names = Framework.Name.Read(reader, this.Directory);
                            if (names.okay)
                            {
                                this.Names = names.result;
                            }
                            else
                            {
                                Console.WriteLine(names.message);
                                goto DATA_READ_ERROR;
                            }
                        }
                        this.valid = true;
                        return;
                    }
                    catch
                    {
                        valid = false;
                    }
                }
DATA_READ_ERROR:
                this.Written = Memory<Framework.Written>.Empty;
                this.Book    = Memory<Framework.Book>   .Empty;
                this.Chapter = Memory<Framework.Chapter>.Empty;
                this.Lexicon = Memory<Framework.Lexicon>.Empty;
                this.Lemmata = Memory<Framework.Lemmata>.Empty;
                this.OOVLemmata = new();
                this.Names = new();
                this.valid = false;
            }
        }

        internal static Memory<char> GetMemoryString(ReadOnlySpan<byte> bytes, int offset, int length)
        {
            int len = 0;
            for (int i = 0; i < length && bytes[i + offset] != 0; i++)
                len++;

            if (len == 0)
                return Memory<char>.Empty;

            var chars = new char[len];
            for (int i = 0; i < len; i++)
                chars[i] = (char)bytes[i];
            return new Memory<char>(chars);
        }
        internal static (Memory<char> text, int length, bool overflow) ReadDelimitedMemory(System.IO.BinaryReader reader, char delimiter, Span<byte> scratchPad)
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
                if (idx == mem.Length-1)
                {
                    items[cnt++] = mem.Slice(delimitIdx, idx-delimitIdx);
                }
                else if (mem.Span[idx] == delimiter)
                {
                    items[cnt++] = mem.Slice(delimitIdx, idx - delimitIdx);
                    delimitIdx = idx+1;
                }
            }
            return (new ReadOnlyMemory<ReadOnlyMemory<char>>(items), true);
        }
    }
}
