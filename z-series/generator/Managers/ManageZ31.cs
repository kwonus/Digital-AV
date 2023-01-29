namespace DigitalAV.Migration
{
    using FoundationsGenerator;
    using SerializeFromSDK;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ManageZ31 : IInventoryManager
    {
        private BinaryWriter? bomZ31;
        private BinaryWriter? bomZ31_MD5;

        private List<string> bomLines;

        internal ManageZ31()
        {
            this.bomZ31 = AVXManager.OpenBinaryWriter(BOM.Z_31, ".bom");
            this.bomZ31_MD5 = AVXManager.OpenBinaryWriter(BOM.Z_31, ".md5");

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
            this.XWrit128();
            this.XWrit32();

            Console.WriteLine("Calculate and create the BOM.");
            this.SaveInventory();
        }
        private void RecordContent(ORDER id, UInt32 recordCount)
        {
            BOM.Inventory[(byte)id].recordCount = recordCount;
        }
        private void AddInventoryRecord(string fname, string hash, UInt32 len, UInt32 cnt, UInt32 size, byte order)
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
            var entry = BOM.Inventory.ContainsKey(order) ? BOM.Inventory[order] : new FoundationsGenerator.Directory("");
            entry.recordCount = cnt;
            entry.recordLength = len;
            entry.length = size;
            entry.hash = hash;

            entry.offset = (order > 0 && order != BOM.UNDEFINED && order != BOM.IGNORE)
                         ? BOM.Inventory[(byte)(order - 1)].offset + BOM.Inventory[(byte)(order - 1)].length
                         : 0;
        }
        private void StoreInventoryLine(ORDER order, UInt32 recordLength = 0)
        {
            string filepath = BOM.GetZ_Path(order, release:BOM.Z_31);
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

            this.AddInventoryRecord(filename, hashStr, rlen, (UInt32)rcnt, (UInt32)size, (byte)order);
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
            if (this.bomZ31 != null)
            {
                this.bomZ31.Write(bomBytes);
                this.bomZ31.Close();
            }
            var hash = BOM.hasher != null ? BOM.hasher.ComputeHash(bomBytes) : null;
            var md5 = hash != null ? AVXManager.BytesToHex(hash) : "ERROR";
            var md5Bytes = new byte[md5.Length];
            for (int i = 0; i < md5.Length; i++)
            {
                md5Bytes[i] = (byte)(md5[i]);
            }
            if (this.bomZ31_MD5 != null)
            {
                this.bomZ31_MD5.Write(md5Bytes);
                this.bomZ31_MD5.Close();
            }
        }
        private void XBook(ORDER id)
        {
            string file = BOM.GetZ_Path(id, release: BOM.Z_31);

            var fstream = new StreamReader(file);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                byte bookNum = 0;
                do
                {
                    bookNum = breader.ReadByte();        // 1
                    var chapterCnt = breader.ReadByte();        // 1 =  2
                    var chapterIdx = breader.ReadUInt16();      // 2 =  4
                    var verseCnt = breader.ReadUInt16();      // 2 =  6
                    var verseIdx = breader.ReadUInt16();      // 2 =  8
                    var writCnt = breader.ReadUInt32();      // 4 = 12
                    var writIdx = breader.ReadUInt32();      // 4 = 16
                    var bname = breader.ReadBytes(16);     //16 = 32
                    var babbr = breader.ReadBytes(18);     //18 = 50

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
                    for (i = 2; i < 2 + 3 && babbr[i] != 0; i++)
                        abbr2.Append(babbr[i]);
                    for (i = 2 + 3; i < 2 + 3 + 4 && babbr[i] != 0; i++)
                        abbr2.Append(babbr[i]);
                    bool comma = false;
                    for (i = 2 + 3 + 4; i < babbr.Length && babbr[i] != 0; i++)
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
                    BookIndex[bookNum].verse_cnt = verseCnt;
                    BookIndex[bookNum].verse_idx = verseIdx;
                    BookIndex[bookNum].writ_cnt = writCnt;
                    BookIndex[bookNum].writ_idx = writIdx;
                    BookIndex[bookNum].name = name.ToString();

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

                } while (bookNum < 66);

                RecordContent(id, (uint)(1 + bookNum)); // bookNum starts at zero
            }
            StoreInventoryLine(ORDER.Book);
        }
        private void XChapter(ORDER id)
        {
            string file = BOM.GetZ_Path(id, release: BOM.Z_31);
            var info = new FileInfo(file);
            long recordCount = info.Length / 10;

            var fstream = new StreamReader(file);

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int rec = 0; rec < recordCount; rec++)
                {
                    var writIdx = breader.ReadUInt32();
                    var writCnt = breader.ReadUInt16();
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
            RecordContent(id, (uint)recordCount);
            StoreInventoryLine(ORDER.Chapter);
        }
        private void XVerse(ORDER id)
        {
            string file = BOM.GetZ_Path(id);

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
                RecordContent(ORDER.UNDEFINED, recordIdx);
            }
            StoreInventoryLine(ORDER.UNDEFINED);
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
            string file = BOM.GetZ_Path(id, release: BOM.Z_31);
            var length = BOM.GetRecordLength(id, BOM.Z_31);
            var bytes = File.ReadAllBytes(file);
            var cnt = bytes.Length / length;

            RecordContent(id, (UInt32)cnt);
            StoreInventoryLine(ORDER.Written);
        }
        private void XWrit128()
        {
            string file = BOM.GetZ_Path(ORDER.Written).Replace(".dx", "-128.dx");
            StoreInventoryLine(ORDER.Written, 16);
        }
        private void XWrit32()
        {
            string file = BOM.GetZ_Path(ORDER.Written).Replace(".dx", "-32.dx");
            StoreInventoryLine(ORDER.Written, 4);
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
