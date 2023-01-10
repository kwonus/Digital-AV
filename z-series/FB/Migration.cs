using AVX.FlatBuf;
using System.Runtime.Intrinsics.X86;

namespace DigitalAV.Migration
{
    using AVX.FlatBuf;
    using FlatSharp;
    using SerializeFromSDK;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    public class ConsoleApp
    {
        private HashAlgorithm? hasher;

        private string rsrc;
        private string csrc;
        private string output;
        private string outputExtent;
        private string baseSDK;
        private BinaryWriter? bom;
        private BinaryWriter? bomMD5;
        private List<string> bomLines;
        private Dictionary<string, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize)> bomDetails;
        private Dictionary<string, UInt32> RecordCounts;

        private const string Version = "-Z31";
        internal ConsoleApp()
        {
            this.csrc = @"C:\src\Digital-AV\z-series\foundations\cpp";
            this.rsrc = @"C:\src\Digital-AV\z-series\foundations\rust\src\avx";
            this.output = @"C:\src\Digital-AV\z-series\FB\content\";
            this.outputExtent = ".bin";
            this.baseSDK = @"C:\src\Digital-AV\z-series\";
            this.hasher = HashAlgorithm.Create(HashAlgorithmName.MD5.ToString());
            try
            {
                string file = this.baseSDK + "AV-Inventory" + Version + ".bom";
                var stream = new FileStream(file, FileMode.Create);
                this.bom = new BinaryWriter(stream, Encoding.ASCII);
            }
            catch
            {
                this.bom = null;
            }
            try
            {
                string file = this.baseSDK + "AV-Inventory" + Version + ".md5";
                var stream = new FileStream(file, FileMode.Create);
                this.bomMD5 = new BinaryWriter(stream, Encoding.ASCII);
            }
            catch
            {
                this.bomMD5 = null;
            }
            this.bomLines = new();
            this.bomDetails = new();
            this.RecordCounts = new();
        }

        public static void Main()
        {
            var app = new ConsoleApp();

            Console.WriteLine("Create FlatBuffers binary content files.");
            app.XBook("Book", "Book-Index");
            app.XChapter("Chapter", "Chapter-Index");
            app.XVerse("Verse", "Verse-Index");
            app.XLemma("Lemma", "Lemmata");
            app.XLemmaOOV("Lemma-OOV", "Lemmata-OOV");
            app.XLexicon("Lexicon", "Lexicon");
            app.XNames("Names", "Names");
            app.XWordClass("WordClass", "Word-Classes");
            app.XWrit("Writ", "Written");
            app.XWrit128("Writ-128");
            app.XWrit32("Writ-32");

            Console.WriteLine("Calculate and create the BOM.");
            app.SaveInventoryAndGenerate();
        }
        private string IX(string itype)
        {
            return this.baseSDK + "AV-" + itype + ".ix";
        }
        private string DX(string itype)
        {
            return this.baseSDK + "AV-" + itype + ".dx";
        }
        private string DXI(string itype)
        {
            return this.baseSDK + "AV-" + itype + ".dxi";
        }
        private UInt32 GetRecordLength(string itype)
        {
            if (!itype.EndsWith('x'))
                return 0; // this is variable lenght // i.e .dxi
            else if (itype.EndsWith("-128.dx"))
                return (UInt32)(128 / 8); // 16 bytes
            else if (itype.EndsWith("-32.dx"))
                return (UInt32) (32 / 8); // 4 bytes

            else switch (itype.ToLower())
            {
                case "av-writ.dx":    return 22;
                case "av-book.ix":    return 48;
                case "av-chapter.ix": return 10;
                case "av-verse.ix":   return  4;
            }
            return 0;
        }
        public static string ReadByteString(BinaryReader breader, UInt16 maxLen = 24)
        {
            var buffer = new char[maxLen];

            int i = 0;
            byte c = 0;
            for (c = breader.ReadByte(); c != 0 && i < maxLen; c = breader.ReadByte())
                buffer[i++] = (char)c;
            buffer[i] = '\0';
            if (c != 0) for (c = breader.ReadByte(); c != 0; c = breader.ReadByte()) // discard ... this should not happen ... check in debugger
                    Console.WriteLine("Bad stuff!!!");

            return new string(buffer, 0, i);
        }
        private void SaveContent(string otype, byte[] content, string itype, UInt32 recordCount = 0)
        {
            var file = this.output + otype + this.outputExtent;
            var fstream = new StreamWriter(file);

            using (var bwriter = new System.IO.BinaryWriter(fstream.BaseStream))
            {
                bwriter.Write(content);
            }
            if (recordCount > 0)
                this.RecordCounts[itype] = recordCount;
        }
        private void AddInventoryRecord(string fname, string fpath, string otype, string hash, UInt32 len, UInt32 cnt, UInt32 size)
        {
            if (this.bomLines.Count == 0)
            {
                string header = PadRight("filename", 16) + " " + PadRight("hash", 32)        + PadLeft("rlen", 4)  + " " + PadLeft("rcnt", 7)  + " " + PadLeft("size", 8);
                this.bomLines.Add(header);

                string seperator = PadRight("", 16, '-') + " " + PadRight("", 32, '-') + " " + PadLeft("", 3, '-') + " " + PadLeft("", 7, '-') + " " + PadLeft("", 8, '-');
                this.bomLines.Add(seperator);
            }
            string record = PadRight(fname, 16) + " " + PadRight(hash, 32) + " " + PadLeft(len.ToString(), 3) + " " + PadLeft(cnt.ToString(), 7) + " " + PadLeft(size.ToString(), 8);
            this.bomLines.Add(record);
            this.bomDetails[fname] = (hash, fpath, otype, len, cnt, size);
        }
        private static string PadLeft(string input, int cnt, char padding = ' ')
        {
            string output = input;
            for (int len = output.Length; len < cnt; len++)
                output = padding + output;
            return output;
        }
        private static string PadRight(string input, int cnt, char padding = ' ')
        {
            string output = input;
            for (int len = output.Length; len < cnt; len++)
                output += padding;
            return output;
        }
        private void StoreInventoryLine(string file, string otype)
        {
            string ifile = Path.GetFileName(file);
            string itype = Path.GetFileNameWithoutExtension(file).Substring(3);

            var buffer = File.ReadAllBytes(file);
            var size = buffer.Length;
            var rlen = this.GetRecordLength(ifile);
            var rcnt = rlen > 0 ? size / rlen : this.RecordCounts.ContainsKey(itype) ? this.RecordCounts[itype] : 0;

            var hash = this.hasher.ComputeHash(buffer);
            string hashStr = hash != null ? BytesToHex(hash) : "ERROR";

            this.AddInventoryRecord(Path.GetFileName(file), file, otype, hashStr, rlen, (UInt32) rcnt, (UInt32) size);
        }
        private static string BytesToHex(byte[] bytes)
        {
            StringBuilder hex = new();

            foreach (byte b in bytes)
            {
                var digits = new byte[] { (byte)(b / 0x10), (byte)(b % 0x10) };

                foreach (var digit in digits)
                {
                    if (digit <= 9)
                    {
                        hex.Append(digit.ToString());
                    }
                    else
                    {
                        char abcdef = (char)((digit - 0xA) + (byte)'A');
                        hex.Append(abcdef.ToString());
                    }
                }
            }
            return hex.ToString();
        }

        private void SaveInventoryAndGenerate()
        {
            int n = 0;
            int len = this.bomLines.Count;
            string output = "";
            foreach (var line in this.bomLines)
            {
                output += (++n < len) ? line + "\n" : line;
            }
            var bomBytes = new byte[output.Length];
            for (int i = 0; i < output.Length; i++)
            {
                bomBytes[i] = (byte)(output[i]);
            }
            if (this.bom != null)
            {
                this.bom.Write(bomBytes);
                this.bom.Close();
            }
            var hash = this.hasher != null ? this.hasher.ComputeHash(bomBytes) : null;
            var md5 = hash != null ? BytesToHex(hash) : "ERROR";
            var md5Bytes = new byte[md5.Length];
            for (int i = 0; i < md5.Length; i++)
            {
                md5Bytes[i] = (byte)(md5[i]);
            }
            if (this.bomMD5 != null)
            {
                this.bomMD5.Write(md5Bytes);
                this.bomMD5.Close();
            }

            var cpp = new CSrcGen(this.baseSDK, this.csrc, this.bomDetails);
            cpp.Generate();

            var rust = new RustSrcGen(this.baseSDK, this.rsrc, this.bomDetails);
            rust.Generate();
        }
        private void XBook(string itype, string otype)
        {
            var index = new AVXBookIndex() { Index = new List<AVXBook>() };
            string file = IX(itype);

            var fstream = new StreamReader(file);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                byte bookNum = 0;
                do
                {
                    bookNum        = breader.ReadByte();
                    var chapterCnt = breader.ReadByte();
                    var chapterIdx = breader.ReadUInt16();
                    var verseCnt   = breader.ReadUInt16();
                    var verseIdx   = breader.ReadUInt16();
                    var writCnt    = breader.ReadUInt32();
                    var writIdx    = breader.ReadUInt32();
                    var bname      = breader.ReadBytes(16);
                    var babbr      = breader.ReadBytes(12);

                    var name = new StringBuilder();
                    var abbr = new StringBuilder();

                    for (int i = 0; i < bname.Length && bname[i] != 0; i++)
                        name.Append(bname[i]);
                    for (int i = 0; i < babbr.Length && babbr[i] != 0; i++)
                        abbr.Append(babbr[i]);

                    var abbreviations = bookNum > 0 ? abbr.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries) : new string[0];

                    var book = new AVXBook() { Num = bookNum, ChapterCnt = chapterCnt, ChapterIdx = chapterIdx, VerseCnt = chapterCnt, VerseIdx = chapterIdx, WritCnt = chapterCnt, WritIdx = chapterIdx, Name = name.ToString(), Abbreviations = abbreviations };

                }   while (bookNum <= 66);
            }
            var maxBytesNeeded = AVXBookIndex.Serializer.GetMaxSize(index);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXBookIndex.Serializer.Write(content, index);

            SaveContent(otype, content, itype);
            StoreInventoryLine(file, otype);
        }
        private void XChapter(string itype, string otype)
        {
            var index = new AVXChapterIndex() { Index = new List<AVXChapter>() };
            string file = IX(itype);
            var info = new FileInfo(file);
            var recordCount = info.Length / 10;

            var fstream = new StreamReader(file);

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int rec = 0; rec < recordCount; rec++)
                {
                    var writIdx  = breader.ReadUInt32();
                    var writCnt  = breader.ReadUInt16();
                    var verseIdx = breader.ReadUInt16();
                    var verseCnt = breader.ReadUInt16();

                    var chapter = new AVXChapter() { WritIdx = writIdx, WritCnt = writCnt, VerseIdx = verseIdx, VerseCnt = verseCnt };
                }
            }
            var maxBytesNeeded = AVXChapterIndex.Serializer.GetMaxSize(index);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXChapterIndex.Serializer.Write(content, index);

            SaveContent(otype, content, itype);
            StoreInventoryLine(file, otype);
        }
        private void XVerse(string itype, string otype)
        {
            var index = new AVXVerseIndex() { Index = new List<AVXVerse>() };
            string file = IX(itype);

            var fstream = new StreamReader(file);

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                UInt32 recordIdx = 0;
                do
                {
                    var book = breader.ReadByte();
                    var chapter = breader.ReadByte();
                    var verse = breader.ReadByte();
                    var wordCnt = breader.ReadByte();

                    var entry = new AVXVerse() { Book = book, Chapter = chapter, Verse = verse, WordCnt = wordCnt };
                    index.Index.Add(entry);

                } while (++recordIdx <= 0x797D);
            }
            var maxBytesNeeded = AVXVerseIndex.Serializer.GetMaxSize(index);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXVerseIndex.Serializer.Write(content, index);

            SaveContent("Verse-Index", content, itype);
            StoreInventoryLine(file, otype);
        }
        private void XLemma(string itype, string otype)
        {
            var records = new AVXLemmata() { Record = new List<AVXLemma>() };
            string file = DXI(itype);

            var fstream = new StreamReader(file);

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (var pos = breader.ReadUInt32(); pos != 0xFFFFFFFF; pos = breader.ReadUInt32())
                {
                    cnt ++;
                    var wordKey = breader.ReadUInt16();
                    var wordClass = breader.ReadUInt16();
                    var lemmaCount = breader.ReadUInt16();
                    var lemmata = new UInt16[lemmaCount];
                    for (int i = 0; i < lemmaCount; i++)
                        lemmata[i] = breader.ReadUInt16();

                    var entry = new AVXLemma() { Pos = pos, WordKey = wordKey, WordClass = wordClass, Lemma = lemmata };
                    records.Record.Add(entry);
                }
            }
            var maxBytesNeeded = AVXLemmata.Serializer.GetMaxSize(records);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXLemmata.Serializer.Write(content, records);

            SaveContent(otype, content, itype, cnt);
            StoreInventoryLine(file, otype);
        }
        private void XLemmaOOV(string itype, string otype)
        {
            var records = new AVXLemmataOOV() { Oov = new List<AVXLemmaOOV>() };

            string file = DXI(itype);

            var fstream = new StreamReader(file);
            var buffer = new char[24];

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                while (breader.BaseStream.Position < breader.BaseStream.Length)
                {
                    cnt ++;
                    var oovKey = breader.ReadUInt16();

                    int i = 0;
                    byte c = 0;
                    for (c = breader.ReadByte(); c != 0 && i < 24; c = breader.ReadByte())
                        buffer[i++] = (char)c;
                    buffer[i] = '\0';
                    if (c != 0) for (c = breader.ReadByte(); c != 0; c = breader.ReadByte()) // discard ... this should not happen ... check in debugger
                        Console.WriteLine("Bad stuff!!!");

                    var oovString = new string(buffer, 0, i);
                    var entry = new AVXLemmaOOV() { Key = oovKey, Word = oovString };
                    records.Oov.Add(entry);
                }
            }
            var maxBytesNeeded = AVXLemmataOOV.Serializer.GetMaxSize(records);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXLemmataOOV.Serializer.Write(content, records);

            SaveContent(otype, content, itype, cnt);
            StoreInventoryLine(file, otype);
        }
        private void XNames(string itype, string otype)
        {
            var records = new AVXNames() { Record = new List<AVXName>() };

            string file = DXI(itype);

            var fstream = new StreamReader(file);

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                while (breader.BaseStream.Position < breader.BaseStream.Length - 1)
                {
                    cnt ++;
                    var wkey = breader.ReadUInt16();
                    var meanings = ReadByteString(breader, maxLen: 4096);
                    var meaningArray = meanings.Split('|', StringSplitOptions.RemoveEmptyEntries);

                    var entry = new AVXName() { WordKey = wkey, Meaning = meaningArray };
                    records.Record.Add(entry);
                }
            }
            var maxBytesNeeded = AVXNames.Serializer.GetMaxSize(records);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXNames.Serializer.Write(content, records);

            SaveContent(otype, content, itype, cnt);
            StoreInventoryLine(file, otype);
        }

        private void XLexicon(string itype, string otype)
        {
            var records = new AVXLexicon() { Lex = new List<AVXLexItem>() };
            string file = DXI(itype);

            var fstream = new StreamReader(file);

            UInt32 recordCount = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                UInt32 rec = 0;
                do
                {
                    var entities = breader.ReadUInt16();
                    int posCnt = breader.ReadUInt16();
   
                    UInt32[] pos = new UInt32[posCnt];
                    for (int p = 0; p < posCnt; p++)
                    {
                        pos[p] = breader.ReadUInt32();

                        if (rec == 0 && p == 0)
                            recordCount = pos[p];
                    }
                    var search = ReadByteString(breader);
                    var display = ReadByteString(breader);
                    var modern = ReadByteString(breader);

                    if (!string.IsNullOrEmpty(search))
                    {
                        if (!string.IsNullOrEmpty(display))
                        {
                            if (!string.IsNullOrEmpty(modern))
                                records.Lex.Add(new AVXLexItem() { Search = search, Display = display, Modern = modern, Entities = entities, Pos = pos });
                            else
                                records.Lex.Add(new AVXLexItem() { Search = search, Display = display, Entities = entities, Pos = pos });
                        }
                        else if (!string.IsNullOrEmpty(modern))
                            records.Lex.Add(new AVXLexItem() { Search = search, Modern = modern, Entities = entities, Pos = pos });
                        else
                            records.Lex.Add(new AVXLexItem() { Search = search, Entities = entities, Pos = pos });
                    }
                }   while (++rec < recordCount);
            }
            var maxBytesNeeded = AVXLexicon.Serializer.GetMaxSize(records);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXLexicon.Serializer.Write(content, records);

            SaveContent(otype, content, itype, recordCount);
            StoreInventoryLine(file, otype);
        }
        private void XWordClass(string itype, string otype)
        {
            var records = new AVXWordClasses() { Instance = new List<AVXWordClass>() };
            string file = DXI(itype);

            var fstream = new StreamReader(file);

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                while (breader.BaseStream.Position < breader.BaseStream.Length - 1)
                {
                    cnt ++;
                    var wclass = breader.ReadUInt16();
                    var posCnt = breader.ReadUInt16();
                    var pos = new UInt32[posCnt];
                    for (int i = 0; i < posCnt; i++)
                        pos[i] = breader.ReadUInt32();

                    var entry = new AVXWordClass() { WordClass = wclass, Pos = pos };
                    records.Instance.Add(entry);
                }
            }
            var maxBytesNeeded = AVXWordClasses.Serializer.GetMaxSize(records);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXWordClasses.Serializer.Write(content, records);

            SaveContent(otype, content, itype, cnt);
            StoreInventoryLine(file, otype);
        }
        private void XWrit(string itype, string otype)
        {
            var items = new AVXWritten() { Writ = new List<AVXWrit>() };

            string file = DX(itype);

            var fstream = new StreamReader(file);
            var buffer = new char[24];

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                while (breader.BaseStream.Position < breader.BaseStream.Length - 1)
                {
                    var entry = new AVXWrit()
                    {
                        Strongs = breader.ReadUInt64(),
                        VerseIdx = breader.ReadUInt16(),
                        Word = breader.ReadUInt16(),
                        Punc = breader.ReadByte(),
                        Trans = breader.ReadByte(),
                        Pnwc = breader.ReadUInt16(),
                        Pos = breader.ReadUInt32(),
                        Lemma = breader.ReadUInt16()
                    };
                    items.Writ.Add(entry);
                }
            }
            var maxBytesNeeded = AVXWritten.Serializer.GetMaxSize(items);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXWritten.Serializer.Write(content, items);

            SaveContent(otype, content, itype);
            StoreInventoryLine(file, otype);
        }
        private void XWrit128(string itype)
        {
            string file = DX(itype);
            StoreInventoryLine(file, "");
        }
        private void XWrit32(string itype)
        {
            string file = DX(itype);
            StoreInventoryLine(file, "");
        }
    }
}
