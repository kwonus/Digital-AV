namespace DigitalAV.Migration
{
    using FoundationsGenerator;
    using SerializeFromSDK;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ManageZ32 : IInventoryManager
    {
        private BinaryWriter? bomZ32;
        private BinaryWriter? bomZ32_MD5;

        private List<string> bomLines;

        internal ManageZ32()
        {
            this.bomZ32 = AVXManager.OpenBinaryWriter(BOM.Z_32, ".bom");
            this.bomZ32_MD5 = AVXManager.OpenBinaryWriter(BOM.Z_32, ".md5");

            this.bomLines = new();
        }

        public void Manage()
        {
            Console.WriteLine("Read Existing binary content files & and upgrade outdated files");
            this.XVerse(ORDER.UNDEFINED);
            this.XBook(ORDER.Book);
            this.XChapter(ORDER.Chapter);
            this.XLemma(ORDER.Lemmata);
            this.XLemmaOOV(ORDER.OOV);
            this.XLexicon(ORDER.Lexicon);
            this.XNames(ORDER.Names);
            this.XWrit(ORDER.Written);

            Console.WriteLine("Calculate and create the BOM.");
            this.SaveInventory();
        }
        private void RecordContent(ORDER id, UInt32 recordCount)
        {
            BOM.Inventory[(byte)id].recordCount = recordCount;
        }
        private void AddInventoryRecord(string fname, string hash, UInt32 len, UInt32 cnt, UInt32 size, ORDER order)
        {
            if (this.bomLines.Count == 0)
            {
                string header = AVXManager.PadRight("filename", 16) + " " + AVXManager.PadRight("hash", 32) + AVXManager.PadLeft("rlen", 4) + " " + AVXManager.PadLeft("rcnt", 7) + " " + AVXManager.PadLeft("size", 8);
                this.bomLines.Add(header);

                string seperator = AVXManager.PadRight("", 16, '-') + " " + AVXManager.PadRight("", 32, '-') + " " + AVXManager.PadLeft("", 3, '-') + " " + AVXManager.PadLeft("", 7, '-') + " " + AVXManager.PadLeft("", 8, '-');
                this.bomLines.Add(seperator);
            }
            string record = AVXManager.PadRight(fname, 16) + " " + AVXManager.PadRight(hash, 32) + " " + AVXManager.PadLeft(len.ToString(), 3) + " " + AVXManager.PadLeft(cnt.ToString(), 7) + " " + AVXManager.PadLeft(size.ToString(), 8);
            this.bomLines.Add(record);
            var entry = BOM.Inventory.ContainsKey((byte)order) ? BOM.Inventory[(byte)order] : new FoundationsGenerator.Directory("");
            entry.recordCount = cnt;
            entry.recordLength = len;
            entry.length = size;
            entry.hash = hash;

            entry.offset = (order > 0) && BOM.Inventory.ContainsKey((byte)order)
                         ? BOM.Inventory[(byte)(order-1)].offset + BOM.Inventory[(byte)(order-1)].length
                         : 0;
        }
        private void StoreInventoryLine(ORDER order, UInt32 recordLength = 0)
        {
            string filepath = BOM.GetZ_Path(order, release: BOM.Z_32);
            string filename = Path.GetFileName(filepath);

            var bom = BOM.Inventory[(byte)order];
            if (bom.label.StartsWith("Writ") && recordLength != 0)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                filepath = Path.GetDirectoryName(filepath);
#pragma warning restore CS8600
                filename = recordLength >= 10 ? filename.Replace("22", recordLength.ToString()) : filename.Replace("22", "0" + recordLength.ToString());
#pragma warning disable CS8604
                filepath = Path.Combine(filepath, filename);
#pragma warning restore CS8604
            }

            var buffer = File.ReadAllBytes(filepath);
            var size = buffer.Length;
            var rlen = recordLength == 0 ? AVXManager.GetRecordLength(filename) : recordLength;
            var rcnt = bom.recordCount;

            string hashStr = "ERROR(0)";
            if (BOM.hasher != null)
            {
                var hash = BOM.hasher.ComputeHash(buffer);
                hashStr = hash != null ? AVXManager.BytesToHex(hash) : "ERROR(1)";
                bom.hash = hashStr;
            }
            if (bom.recordLength == 0)
                bom.recordCount = rlen;

            this.AddInventoryRecord(filename, hashStr, rlen, (UInt32)rcnt, (UInt32)size, order);
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
            if (this.bomZ32 != null)
            {
                this.bomZ32.Write(bomBytes);
                this.bomZ32.Close();
            }
            var hash = BOM.hasher != null ? BOM.hasher.ComputeHash(bomBytes) : null;
            var md5 = hash != null ? AVXManager.BytesToHex(hash) : "ERROR";
            var md5Bytes = new byte[md5.Length];
            for (int i = 0; i < md5.Length; i++)
            {
                md5Bytes[i] = (byte)(md5[i]);
            }
            if (this.bomZ32_MD5 != null)
            {
                this.bomZ32_MD5.Write(md5Bytes);
                this.bomZ32_MD5.Close();
            }
        }
        private void XBook(ORDER id)
        {
            string ifile = BOM.GetZ_Path(id, release: BOM.Z_31);
            string ofile = BOM.GetZ_Path(id, release: BOM.Z_32);

            // Update SDK file to Z32
            var ostream = new StreamWriter(ofile, false, Encoding.ASCII);
            using (var bwriter = new System.IO.BinaryWriter(ostream.BaseStream))
            {
                var fstream = new StreamReader(ifile);
                using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
                {
                    byte bookNum = 0;
                    do
                    {
                        bookNum = breader.ReadByte();          // 1
                        var chapterCnt = breader.ReadByte();   // 1 =  2
                        var chapterIdx = breader.ReadUInt16(); // 2 =  4
                        var verseCnt = breader.ReadUInt16();   // 2 =  6
                        var verseIdx = breader.ReadUInt16();   // 2 =  8
                        var writCnt = breader.ReadUInt32();    // 4 = 12
                        var writIdx = breader.ReadUInt32();    // 4 = 16
                        var bname = breader.ReadBytes(16);     //16 = 32
                        var babbr = breader.ReadBytes(18);     //18 = 50

                        if (bookNum == 0)
                        {
                            string release = "Z32";
                            for (int i = 0; i < release.Length; i++)
                                bname[i] = (byte)(release[i]);
                            for (int i = release.Length; i < bname.Length; i++)
                                bname[i] = (byte) 0;
                        }

                        var name = new StringBuilder();
                        for (int i = 0; i < bname.Length && bname[i] != 0; i++)
                            name.Append(bname[i]);

                        BookIndex[bookNum].chapter_cnt = chapterCnt;
                        BookIndex[bookNum].chapter_idx = chapterIdx;
                        BookIndex[bookNum].verse_cnt = verseCnt;
                        BookIndex[bookNum].verse_idx = verseIdx;
                        BookIndex[bookNum].writ_cnt = bookNum > 0 ? writCnt : (UInt32) 0;
                        BookIndex[bookNum].writ_idx = bookNum > 0 ? writIdx : (UInt32) 0x3201;
                        BookIndex[bookNum].name = name.ToString();

                        bwriter.Write(bookNum);     // 1
                        bwriter.Write(chapterCnt);  // 1 = 2
                        bwriter.Write(chapterIdx);  // 2 = 4
                        bwriter.Write(verseCnt);    // 2 = 6
                        bwriter.Write(writCnt);     // 4 = 10
                        bwriter.Write(writIdx);     // 4 = 14
                        bwriter.Write(bname);       //16 = 30
                        bwriter.Write(babbr);       //18 = 48

                    }   while (bookNum < 66);

                    RecordContent(id, (uint)(1 + bookNum)); // bookNum starts at zero
                }
            }
            StoreInventoryLine(ORDER.Book, 48);
        }
        private void XChapter(ORDER id)
        {
            string ifile = BOM.GetZ_Path(id, release: BOM.Z_31);
            string ofile = BOM.GetZ_Path(id, release: BOM.Z_32);

            // Update SDK file fo Z32
            var ostream = new StreamWriter(ofile, false, Encoding.ASCII);
            using (var bwriter = new System.IO.BinaryWriter(ostream.BaseStream))
            {
                var info = new FileInfo(ifile);
                long recordCount = info.Length / 10;

                var fstream = new StreamReader(ifile);

                using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
                {
                    for (int rec = 0; rec < recordCount; rec++)
                    {
                        var writIdx = breader.ReadUInt32();
                        var writCnt = breader.ReadUInt16();
                        var verseIdx = breader.ReadUInt16();
                        var verseCnt = breader.ReadUInt16();

                        var verse = verses[verseIdx];

                        bwriter.Write((UInt16) (writIdx - BookIndex[verse.b].writ_idx));
                        bwriter.Write(writCnt);
                        bwriter.Write(verse.b);
                        bwriter.Write((byte)verseCnt);
                    }
                }
                RecordContent(id, (uint)recordCount);
            }
            StoreInventoryLine(ORDER.Chapter, 6);
        }
        private Dictionary<UInt16, (byte b, byte c, byte v, byte wc)> verses = new();
        private void XVerse(ORDER id)
        {
            string file = BOM.GetZ_Path(id, release: BOM.Z_31);

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

                    verses[(UInt16) recordIdx] = (book, chapter, verse, wordCnt);

                }   while (++recordIdx <= 0x797D);
            }
        }
        private void XLemma(ORDER id)
        {
            string file = BOM.GetZ_Path(id);

            var fstream = new StreamReader(file);

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (var pos32 = breader.ReadUInt32(); pos32 != 0xFFFFFFFF; pos32 = breader.ReadUInt32())
                {
                    cnt++;
                    var wordKey = breader.ReadUInt16();
                    var pnpos = breader.ReadUInt16();
                    var lemmaCount = breader.ReadUInt16();
                    var lemmata = new UInt16[lemmaCount];
                    for (int i = 0; i < lemmaCount; i++)
                        lemmata[i] = breader.ReadUInt16();
                }
            }
            RecordContent(id, cnt);
            StoreInventoryLine(ORDER.Lemmata);
        }
        private void XLemmaOOV(ORDER id)
        {
            string file = BOM.GetZ_Path(id);

            var fstream = new StreamReader(file);
            var buffer = new char[24];

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                while (breader.BaseStream.Position < breader.BaseStream.Length)
                {
                    cnt++;
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
            RecordContent(id, cnt);
            StoreInventoryLine(ORDER.OOV);
        }
        private void XNames(ORDER id)
        {
            string file = BOM.GetZ_Path(id);

            var fstream = new StreamReader(file);

            UInt32 cnt = 0;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                while (breader.BaseStream.Position < breader.BaseStream.Length - 1)
                {
                    cnt++;
                    var wkey = breader.ReadUInt16();
                    var meanings = AVXManager.ReadByteString(breader, maxLen: 4096);
                    var meaningArray = meanings.Split('|', StringSplitOptions.RemoveEmptyEntries);
                }
            }
            RecordContent(id, cnt);
            StoreInventoryLine(ORDER.Names);
        }
        private void XLexicon(ORDER id)
        {
            string file = BOM.GetZ_Path(id);

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
                    var search = AVXManager.ReadByteString(breader);
                    var display = AVXManager.ReadByteString(breader);
                    var modern = AVXManager.ReadByteString(breader);

                } while (++rec < recordCount);
            }
            RecordContent(id, recordCount);
            StoreInventoryLine(ORDER.Lexicon);
        }
        private void XWrit(ORDER id)
        {
            string ifile = BOM.GetZ_Path(id, release: BOM.Z_31);
            string ofile = BOM.GetZ_Path(id, release: BOM.Z_32);

            var length = BOM.GetRecordLength(id, BOM.Z_31);
            var bytes = File.ReadAllBytes(ifile);
            var cnt = bytes.Length / length;

            // Update SDK file fo Z32
            (byte b, byte c, byte v, byte wc) verse = (0, 0, 0, 0);
            int lastVerseIdx = -1;
            int wc = 0;
            var ostream = new StreamWriter(ofile, false, Encoding.ASCII);
            using (BinaryWriter bwriter = new BinaryWriter(ostream.BaseStream))
            {
                var fstream = new StreamReader(ifile);
                using (var breader = new BinaryReader(fstream.BaseStream))
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        var Strongs = breader.ReadUInt64();
                        var VerseIdx = breader.ReadUInt16();
                        var Word = breader.ReadUInt16();
                        var Punc = breader.ReadByte();
                        var Trans = breader.ReadByte();
                        var Pnwc = breader.ReadUInt16();
                        var Pos = breader.ReadUInt32();
                        var Lemma = breader.ReadUInt16();

                        int thisVerseIdx = (int) VerseIdx;

                        if (thisVerseIdx != lastVerseIdx)
                        {
                            lastVerseIdx = thisVerseIdx;
                            verse = verses[VerseIdx];
                            wc = 0;
                        }
                        bwriter.Write(Strongs);
                        bwriter.Write(verse.b);
                        bwriter.Write(verse.c);
                        bwriter.Write(verse.v);
                        bwriter.Write((byte)(verse.wc-wc));
                        wc++;
                        bwriter.Write(Word);
                        bwriter.Write(Punc);
                        bwriter.Write(Trans);
                        bwriter.Write(Pnwc);
                        bwriter.Write(Pos);
                        bwriter.Write(Lemma);
                    }
                }
            }
            RecordContent(id, (UInt32)cnt);
            StoreInventoryLine(ORDER.Written, 24);
        }
        internal static
        (
            byte chapter_cnt,
            UInt16 chapter_idx,
            UInt16 verse_cnt,
            UInt16 verse_idx,
            UInt32 writ_idx,
            UInt32 writ_cnt,
            string? name
        )[] BookIndex = new (byte, UInt16, UInt16, UInt16, UInt32, UInt32, string?)[67];
    }
}
