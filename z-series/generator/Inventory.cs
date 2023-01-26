namespace DigitalAV.Migration
{
    using SerializeFromSDK;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
            this.outputExtent = ".data";
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
            app.XVerse("Verse", "Verse-Index");
            app.XBook("Book", "Book-Index");
            app.XChapter("Chapter", "Chapter-Index");
            app.XLemma("Lemma", "Lemmata");
            app.XLemmaOOV("Lemma-OOV", "Lemmata-OOV");
            app.XLexicon("Lexicon", "Lexicon");
            app.XNames("Names", "Names");
            app.XWrit("Writ", "Written");
            app.XWrit128("Writ-128");
            app.XWrit32("Writ-32");

            Console.WriteLine("Calculate and create the BOM.");
            app.SaveInventory();
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

            else switch (Path.GetFileName(itype).ToLower())
            {
                case "av-writ.dx":    return 22;
                case "av-book.ix":    return 50;
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

            return i > 0 ? new string(buffer, 0, i) : string.Empty;
        }
        public static void WriteByteString(BinaryWriter bwriter, string token)
        {
            for (int i = 0; i < token.Length; i++)
            {
                bwriter.Write((byte)token[i]);
            }
            bwriter.Write((byte)0);
        }
        private void RecordContent(string itype, long recordCount = 0)
        {
            if (recordCount > 0)
                this.RecordCounts[itype] = (UInt32) recordCount;
        }
        private void SaveFB(string otype, byte[] content)
        {
            var file = this.output + otype + this.outputExtent;
            var fstream = new StreamWriter(file);

            using (var bwriter = new System.IO.BinaryWriter(fstream.BaseStream))
            {
                bwriter.Write(content);
            }
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

        private void SaveInventory()
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
            string file = IX(itype);

            var fstream = new StreamReader(file);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                byte bookNum = 0;
                do
                {
                    bookNum        = breader.ReadByte();        // 1
                    var chapterCnt = breader.ReadByte();        // 1 =  2
                    var chapterIdx = breader.ReadUInt16();      // 2 =  4
                    var verseCnt   = breader.ReadUInt16();      // 2 =  6
                    var verseIdx   = breader.ReadUInt16();      // 2 =  8
                    var writCnt    = breader.ReadUInt32();      // 4 = 12
                    var writIdx    = breader.ReadUInt32();      // 4 = 16
                    var bname      = breader.ReadBytes(16);     //16 = 32
                    var babbr      = breader.ReadBytes(18);     //18 = 50

                    var name = new StringBuilder();
                    var abbr2 = new StringBuilder();
                    var abbr3 = new StringBuilder();
                    var abbr4 = new StringBuilder();
                    var abbrAlt1 = new StringBuilder();
                    var abbrAlt2 = new StringBuilder();

                    int i;
                    for (i = 0; i < bname.Length && bname[i] != 0; i++)
                        name.Append(bname[i]);
                    for (i = 0; i < 2 && babbr[i] != 0; i++)
                        abbr2.Append(babbr[i]);
                    for (i = 2; i < 2+3 && babbr[i] != 0; i++)
                        abbr2.Append(babbr[i]);
                    for (i = 2+3; i < 2+3+4 && babbr[i] != 0; i++)
                        abbr2.Append(babbr[i]);
                    bool comma = false;
                    for (i = 2+3+4; i < babbr.Length && babbr[i] != 0; i++)
                        if ((char)babbr[i] == ',')
                        {
                            if (comma)
                                break; // we only handle two alt abbreviations here
                            comma = true;
                        }
                        else if (comma)
                            abbrAlt2.Append(babbr[i]);
                        else
                            abbrAlt1.Append(babbr[i]);

                    BookIndex[bookNum].chapter_cnt = chapterCnt;
                    BookIndex[bookNum].chapter_idx = chapterIdx;
                    BookIndex[bookNum].verse_cnt   = verseCnt;
                    BookIndex[bookNum].verse_idx   = verseIdx;
                    BookIndex[bookNum].writ_cnt    = writCnt; 
                    BookIndex[bookNum].writ_idx    = writIdx;
                    BookIndex[bookNum].name        = name.ToString();

                    /*
                    var book = new AVXBook() { Num = bookNum, ChapterCnt = chapterCnt, ChapterIdx = chapterIdx, VerseCnt = chapterCnt, VerseIdx = chapterIdx, WritCnt = chapterCnt, WritIdx = chapterIdx, Name = name.ToString(), Abbr4 = abbr4.ToString(), Writ = new List<AVXWrit>() };
                    if (abbr2.Length > 0)
                        book.Abbr2 = abbr2.ToString();
                    if (abbr3.Length > 0)
                        book.Abbr3 = abbr3.ToString();
                    if (abbrAlt1.Length > 0 || abbrAlt2.Length > 0)
                        book.AbbrAlt = new List<string>();
                    if (abbrAlt1.Length > 0)
                        book.AbbrAlt.Add(abbrAlt1.ToString());
                    if (abbrAlt2.Length > 0)
                        book.AbbrAlt.Add(abbrAlt2.ToString());

                    this.AVXObj.Bible.Add(book);
                    */

                }   while (bookNum < 66);
            }
            StoreInventoryLine(file, otype);
        }
        private void XChapter(string itype, string otype)
        {
            string file = IX(itype);
            var info = new FileInfo(file);
            long recordCount = info.Length / 10;

            var fstream = new StreamReader(file);

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int rec = 0; rec < recordCount; rec++)
                {
                    var writIdx  = breader.ReadUInt32();
                    var writCnt  = breader.ReadUInt16();
                    var verseIdx = breader.ReadUInt16();
                    var verseCnt = breader.ReadUInt16();

                    /*
                    if (this.AVXObj.VerseIndex != null)
                    {
                        var verse = this.AVXObj.VerseIndex[verseIdx];
                        var book = this.AVXObj.Bible[verse.Book];

                        var chapter = new AVXChapter() { WritIdx = (UInt16) (writIdx-book.WritIdx), WritCnt = writCnt, VerseIdx = verseIdx, VerseCnt = verseCnt };
                        this.AVXObj.VerseIndex.Add(chapter);
                    }
                    */
                }
            }
            RecordContent(itype, recordCount);
            StoreInventoryLine(file, otype);
        }
        private void XVerse(string itype, string otype)
        {
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
                }   while (++recordIdx <= 0x797D);
                RecordContent(itype, recordIdx);
            }
            StoreInventoryLine(file, otype);
        }
        private void XLemma(string itype, string otype)
        {
            string file = DXI(itype);

            var fstream = new StreamReader(file);

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (var pos32 = breader.ReadUInt32(); pos32 != 0xFFFFFFFF; pos32 = breader.ReadUInt32())
                {
                    cnt ++;
                    var wordKey = breader.ReadUInt16();
                    var pnpos = breader.ReadUInt16();
                    var lemmaCount = breader.ReadUInt16();
                    var lemmata = new UInt16[lemmaCount];
                    for (int i = 0; i < lemmaCount; i++)
                        lemmata[i] = breader.ReadUInt16();
                }
            }
            RecordContent(itype, cnt);
            StoreInventoryLine(file, otype);
        }
        private void XLemmaOOV(string itype, string otype)
        {
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
                }
            }
            RecordContent(itype, cnt);
            StoreInventoryLine(file, otype);
        }
        private void XNames(string itype, string otype)
        {
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
                }
            }
            RecordContent(itype, cnt);
            StoreInventoryLine(file, otype);
        }

        private void XLexicon(string itype, string otype)
        {
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

                        if (rec == 0)
                        {
                            if (entities == 0xFFFF)
                            {
                                if (p == 0)
                                    recordCount = pos[p];
                            }
                            else
                            {
                                recordCount = 12567;
                            }
                        }
                    }
                    var search = ReadByteString(breader);
                    var display = ReadByteString(breader);
                    var modern = ReadByteString(breader);

                }   while (++rec < recordCount);
            }
            RecordContent(itype, recordCount);
            StoreInventoryLine(file, otype);
        }
        private void XWrit(string itype, string otype)
        {
            string file = this.DX(itype);
            var length = this.GetRecordLength(file);
            var bytes = File.ReadAllBytes(file);
            var cnt = bytes.Length / length;

            RecordContent(itype, cnt);
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
        public static 
        (
            byte   chapter_cnt,
            UInt16 chapter_idx,
            UInt16 verse_cnt,
            UInt16 verse_idx,
            UInt32 writ_idx,
            UInt32 writ_cnt,
            string? name
        )[] BookIndex = new (byte, UInt16, UInt16, UInt16, UInt32, UInt32, string?)[67];
    }
}
