﻿using DigitalAV.FlatBuf;

namespace DigitalAV.Migration
{
    using FlatSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;

    public class ConsoleApp
    {
        private string output;
        private string outputExtent;
        private string baseSDK;

        // AVX Assets:
        AVXObjects AVXObj = new()
        {
            Bible = new List<AVXBook>(),
            Lexicon = new List<AVXLexicon>(),
            ChapterIndex = new List<AVXChapter>(),
            VerseIndex = new List<AVXVerse>(),
            NameIndex = new List<AVXName>(),
            Lemmata = new List<AVXLemma>(),
            Oov = new List<AVXLemmaOOV>()
        };

        private const string Version = "-Z31";
        internal ConsoleApp()
        {
            this.output = @"C:\src\Digital-AV\z-series\FB\content\";
            this.outputExtent = ".data";
            this.baseSDK = @"C:\src\Digital-AV\z-series\";
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

            Console.WriteLine("Calculate and create the BOM.");
            app.Generate();
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
            ; // we don't do this here anymore
        }
        private void SaveFB(string file, byte[] content)
        {
            var fstream = new StreamWriter(file);

            using (var bwriter = new System.IO.BinaryWriter(fstream.BaseStream))
            {
                bwriter.Write(content);
            }
        }
        private void Generate()
        {
            var file     = this.output + "avx-fb" + this.outputExtent;
            var hashFile = this.output + "avx-fb" + ".md5";

            var maxBytesNeeded = AVXObjects.Serializer.GetMaxSize(this.AVXObj);
            byte[] content = new byte[maxBytesNeeded];
            int bytesWritten = AVXObjects.Serializer.Write(content, this.AVXObj);

            // Save file
            this.SaveFB(file, content);

            var hasher = HashAlgorithm.Create(HashAlgorithmName.MD5.ToString());
            try
            {
                TextWriter md5 = new StreamWriter(hashFile);

                var buffer = File.ReadAllBytes(file);

                var hash = hasher.ComputeHash(buffer);
                string hashStr = hash != null ? BytesToHex(hash) : "ERROR";

                md5.WriteLine(hashStr);
                md5.Close();
            }
            catch
            {
                ;
            }
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

                    var book = new AVXBook() { Num = bookNum, ChapterCnt = chapterCnt, ChapterIdx = chapterIdx, VerseCnt = chapterCnt, VerseIdx = chapterIdx, WritCnt = chapterCnt, WritIdx = chapterIdx, Name = name.ToString(), Abbr4 = abbr4.ToString(), Writ = new List<AVXWrit>() };
                    if (abbr2.Length > 0)
                        book.Abbr2 = abbr2.ToString();
                    if (abbr3.Length > 0)
                        book.Abbr3 = abbr3.ToString();
                    if (abbrAlt1.Length > 0 || abbrAlt2.Length > 0)
                    {
                        book.AbbrAlt = new List<string>();
                        if (abbrAlt1.Length > 0)
                            book.AbbrAlt.Add(abbrAlt1.ToString());
                        if (abbrAlt2.Length > 0)
                            book.AbbrAlt.Add(abbrAlt2.ToString());
                    }
                    this.AVXObj.Bible.Add(book);

                }   while (bookNum < 66);
            }
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

                    if (this.AVXObj.VerseIndex != null)
                    {
                        var verse = this.AVXObj.VerseIndex[verseIdx];
                        var book = this.AVXObj.Bible[verse.Book];

                        var chapter = new AVXChapter() { WritIdx = (UInt16) (writIdx-book.WritIdx), WritCnt = writCnt, VerseIdx = verseIdx, VerseCnt = verseCnt };
                        if (this.AVXObj.ChapterIndex != null)
                            this.AVXObj.ChapterIndex.Add(chapter);
                    }
                }
            }
            RecordContent(itype, recordCount);
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

                    var entry = new AVXVerse() { Book = book, Chapter = chapter, Verse = verse, WordCnt = wordCnt };
                    if (this.AVXObj.VerseIndex != null)
                        this.AVXObj.VerseIndex.Add(entry);

                } while (++recordIdx <= 0x797D);
            }
            RecordContent(itype, this.AVXObj.VerseIndex != null ? this.AVXObj.VerseIndex.Count : 0);
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

                    var entry = new AVXLemma() { Pos32 = pos32, WordKey = wordKey, PnPos12 = pnpos, Lemma = lemmata };
                    if (this.AVXObj.Lemmata != null)
                        this.AVXObj.Lemmata.Add(entry);
                }
            }
            RecordContent(itype, cnt);
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
                    var entry = new AVXLemmaOOV() { Wkey = oovKey, Word = oovString };
                    if (this.AVXObj.Oov != null)
                        this.AVXObj.Oov.Add(entry);
                }
            }
            RecordContent(itype, cnt);
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

                    var entry = new AVXName() { WordKey = wkey, Meaning = meaningArray };
                    if (this.AVXObj.NameIndex != null)
                        this.AVXObj.NameIndex.Add(entry);
                }
            }
            RecordContent(itype, cnt);
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

                    if (!string.IsNullOrEmpty(search))
                    {
                        if (!string.IsNullOrEmpty(display))
                        {
                            if (!string.IsNullOrEmpty(modern))
                                this.AVXObj.Lexicon.Add(new AVXLexicon() { Search = search, Display = display, Modern = modern, Entities = entities, Pos = pos });
                            else
                                this.AVXObj.Lexicon.Add(new AVXLexicon() { Search = search, Display = display, Entities = entities, Pos = pos });
                        }
                        else if (!string.IsNullOrEmpty(modern))
                            this.AVXObj.Lexicon.Add(new AVXLexicon() { Search = search, Modern = modern, Entities = entities, Pos = pos });
                        else
                            this.AVXObj.Lexicon.Add(new AVXLexicon() { Search = search, Entities = entities, Pos = pos });
                    }
                }   while (++rec < recordCount);
            }
            RecordContent(itype, recordCount);
        }
        private void XWrit(string itype, string otype)
        {
            string file = DX(itype);

            var fstream = new StreamReader(file);
            var buffer = new char[24];

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                long cnt = 0;
                while (breader.BaseStream.Position < breader.BaseStream.Length - 1)
                {
                    var strongs64 = breader.ReadUInt64();
                    List< UInt16 > strongs = new();
                    int shift = 3 * 16;
                    for (int index = 0; index < 4; shift -= 16, index++)
                    {
                        var num = (UInt16)((strongs64 >> shift) & 0xFFFF);
                        if (num > 0)
                            strongs.Add(num);
                    }
                    var entry = new AVXWrit()
                    {
                        Strongs = strongs64,
                        VerseIdx = breader.ReadUInt16(),
                        Word = breader.ReadUInt16(),
                        Punc = breader.ReadByte(),
                        Trans = breader.ReadByte(),
                        Pnwc = breader.ReadUInt16(),
                        Pos = breader.ReadUInt32(),
                        Lemma = breader.ReadUInt16()
                    };

                    if (this.AVXObj.VerseIndex != null)
                    {
                        var index = this.AVXObj.VerseIndex[entry.VerseIdx];
                        var book = this.AVXObj.Bible[index.Book];
                        book.Writ.Add(entry);
                        cnt++;
                    }
                }
                RecordContent(itype, cnt);
            }
        }
    }
}