﻿using DigitalAV.Migration;
using FoundationsGenerator;
using System.Text;

namespace SerializeFromSDK
{
    public class RustSrcGen
    {
        public static DateTime now = DateTime.Now;
        public static byte D = (byte)now.Day;
        public static byte M = (byte)now.Month;
        public static byte Y = (byte)(now.Year - 2020);
        public static char day = D < 10 ? D.ToString()[0] : (char)((int)'a' + D - 0xA);

        private string output;
        private string sdk;

        private
        (
            byte book,
            byte chapter,
            byte verse,
            byte word_cnt
        )[] VerseIndex = new (byte, byte, byte, byte)[31102];
        private
        (
            UInt32 writ_idx,
            UInt16 word_cnt,
            UInt16 verse_idx,
            UInt16 verse_cnt,
            byte   book
        )[] ChapterIndex = new (UInt32, UInt16, UInt16, UInt16, byte)[1189];
        private
        (
            byte   chapter_cnt,
            UInt16 chapter_idx,
            UInt16 verse_cnt,
            UInt16 verse_idx,
            UInt32 writ_idx,
            UInt32 writ_cnt,
            string? name
        )[] BookIndex = new (byte, UInt16, UInt16, UInt16, UInt32, UInt32, string?)[67];

        private HashMapper Maps = new();

        public RustSrcGen(string sdk, string src)
        {
            this.sdk = sdk;
            this.output = src;
        }
        public bool Generate()
        {
            Console.WriteLine("Create source initializers for Rust.");

            this.XAny(ORDER.Lemmata);
            this.XAny(ORDER.OOV);
            this.XAny(ORDER.Lexicon);
            this.XAny(ORDER.Names);

            // These need to be done last and in this order (XWrit differs from processing of other files, as it generates sub-modules also)
            this.XAny(ORDER.Book);
            this.XAny(ORDER.Chapter);
            this.XAny(ORDER.Written);

            this.Maps.Print();

            return true;
        }
        private string Pad<T>(T num, int width)
        {
            var val = num != null ? num.ToString() : "";
            var fmt = "{0," + width.ToString() + "}";
            return string.Format(fmt, val);
        }
        private const int BEGIN   = 0;
        private const int END     = 1;
        private const int OP_NOOP =(-1);
        private const int OP_HEAD = 0;
        private const int OP_META = 1;
        private const int OP_INIT = 2;
        private const string HEAD_BEGIN = "// > > > Generated-Code -- Header > > > //";
        private const string META_BEGIN = "// > > > Generated-Code -- Metadata > > > //";
        private const string INIT_BEGIN = "// > > > Generated-Code -- Initialization > > > //";
        private const string HEAD_END   = "// < < < Generated-Code -- Header < < < //";
        private const string META_END   = "// < < < Generated-Code -- Metadata < < < //";
        private const string INIT_END   = "// < < < Generated-Code -- Initialization < < < //";
        private static readonly string[] HEAD = { HEAD_BEGIN, HEAD_END };
        private static readonly string[] META = { META_BEGIN, META_END };
        private static readonly string[] INIT = { INIT_BEGIN, INIT_END };
        private static Dictionary<string, int> OpLookup = new() { { HEAD_BEGIN, OP_HEAD }, { META_BEGIN, OP_META }, { INIT_BEGIN, OP_INIT } };
        private static Dictionary<int, String[]> Operations = new() { { OP_HEAD, HEAD }, { OP_META, META }, { OP_INIT, INIT } };
        private void XAny(ORDER id)
        {
            var bom = BOM.Inventory[(byte)id];
            var vartype = "AVX" + BOM.GetPascal_Type(id);

            var path  = bom.GetRustSource(BOM.Z_31, ".rs");
            var old   = bom.GetRustSource(BOM.Z_31, BOM.Z_31 + ".rs");
            var temp  = bom.GetRustSource(BOM.Z_31, "-temp.rs");
            var lines = File.ReadAllLines(path);

            TextWriter writer = File.CreateText(temp);

            int op = OP_NOOP;
            bool strip = false;
            foreach (string line in lines)
            {
                if (op == OP_NOOP)
                {
                    if (OpLookup.ContainsKey(line))
                    {
                        op = OpLookup[line];
                        strip = true;
                    }
                    else
                    {
                        writer.WriteLine(line);
                    }
                    continue;
                }
                if (strip)
                {
                    if (line != Operations[op][END])
                        continue;
                    strip = false;
                }
                switch (op)
                {
                    case OP_HEAD:
                    {
                        writer.WriteLine(HEAD[BEGIN]);
                        writer.WriteLine("// This file was partially code generated. Some edits to this module will be lost.");
                        writer.WriteLine("// Be sure NOT to add/change code within Generated-Code directives.");
                        writer.WriteLine("// For example, these comments are wrapped in a pair of Generated-Coded directives.");
                        writer.WriteLine(HEAD[END]);
                        op = OP_NOOP;
                        continue;
                    }
                    case OP_META:
                    {
                        var ifile = Path.GetFileName(BOM.GetZ_Path(id, release: BOM.Z_31));
                        writer.WriteLine(META[BEGIN]);
                        writer.WriteLine("static " + vartype + "_Rust_Edition    :u16 = 23108;");
                        writer.WriteLine("static " + vartype + "_SDK_ZEdition    :u16 = 23107;");
                        writer.WriteLine();
                        writer.WriteLine("static " + vartype + "_File: &'static str = \"" + ifile + "\";");
                        writer.WriteLine("static " + vartype + "_RecordLen   :usize = " + Pad(AVXManager.GetRecordLength(ifile), 8) + ";");
                        writer.WriteLine("static " + vartype + "_RecordCnt   :usize = " + Pad(bom.recordCount, 8) + ";");
                        writer.WriteLine("static " + vartype + "_FileLen     :usize = " + Pad(bom.length, 8) + ";");
                        writer.WriteLine(META[END]);
                        op = OP_NOOP;
                        continue;
                    }
                    case OP_INIT:
                    {
                        writer.WriteLine(INIT[BEGIN]);
                        Dictionary<string, (UInt16 count, byte book)> metadata = new();

                        switch ((byte)id)
                        {
                            case BOM.Book:        if (AVXManager.Release_Manager == BOM.Z_31)
                                                      this.XBook_V31(writer, "AVXBook", bom);
                                                  else
                                                      this.XBook_V32(writer, "AVXBook", bom);
                                                  break;
                            case BOM.Chapter:     if (AVXManager.Release_Manager == BOM.Z_31)
                                                      this.XChapter_V31(writer, "AVXChapter", bom);
                                                  else
                                                      this.XChapter_V32(writer, "AVXChapter", bom);
                                                  break;
                            case BOM.Verse:       if (AVXManager.Release_Manager == BOM.Z_31)
                                                      this.XVerse(writer, "AVXVerse",     bom);
                                                  break;
                            case BOM.Lemmata:     this.XLemma(    writer, "AVXLemma",     bom); break;
                            case BOM.OOV:         this.XLemmaOOV( writer, "AVXLemmaOOV",  bom); break;
                            case BOM.Lexicon:     this.XLexicon(  writer, "AVXLexItem",   bom); break; // TBD: Temporary
                            case BOM.Names:       this.XNames(    writer, "AVXName",      bom); break;
                            case BOM.Written:     {
                                                    var fstream = new StreamReader(BOM.GetZ_Path(id, release: AVXManager.Release_Manager));
                                                    using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
                                                    {
                                                        for (byte bk = 1; bk <= 66; bk++)
                                                        {
                                                            var record = AVXManager.Release_Manager == BOM.Z_31
                                                                       ? this.XWrit_V31(breader, bk, "AVXWrit", bom)
                                                                       : this.XWrit_V32(breader, bk, "AVXWrit", bom);

                                                            metadata[record.variable] = (record.count, bk);
                                                        }
                                                    }
                                                }
                                                this.XWrit(writer, "AVXWrit", bom, metadata); break;
                            }
                        writer.WriteLine(INIT[END]);
                        op = OP_NOOP;
                        continue;
                    }
                }
            }
            writer.Close();
            File.Delete(old);
            File.Replace(temp, path, null/*, old */);
        }
        private void XBook_V31(TextWriter writer, string rtype, FoundationsGenerator.Directory bom, bool useZ14 = false, bool genNext = false) // change default values to change behavior
        {
            var fpath = BOM.GetZ_Path(ORDER.Book, release: BOM.Z_31);

            writer.WriteLine("static " + "books" + ": [" + rtype + "; " + 67.ToString() + "] = [");

            var fstream = new StreamReader(!useZ14 ? fpath : fpath.Replace(".ix", "-Z14.ix"));    // we can still baseline against Z14 release, until we are certain that there are no bugs
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (byte bookNum = 0; bookNum <= 66; bookNum++)
                {
                    byte   chapterCnt = 0;
                    UInt16 chapterIdx = 0;
                    UInt16 verseCnt   = 0;
                    UInt16 verseIdx   = 0;
                    UInt32 writCnt    = 0;
                    UInt32 writIdx    = 0;
                    string name       = "";
                    string abbr       = "";

                    if ((bookNum == 0) && useZ14)
                    {
                        writIdx    = (UInt32) ((Y * 0x1000) | (M * 0x100) | D);
                        name       = "Z" + Y.ToString() + M.ToString("X") + day;
                    }
                    else
                    {
                        var test = breader.ReadByte();
                        if (test != bookNum)
                            break;
                        chapterCnt = breader.ReadByte();
                        chapterIdx = breader.ReadUInt16();

                        if (useZ14)
                        {
                            verseCnt   = 0;
                            verseIdx   = 0;
                            writCnt    = 0;
                            writIdx    = 0;
                        }
                        else
                        {
                            verseCnt   = breader.ReadUInt16();
                            verseIdx   = breader.ReadUInt16();
                            writCnt    = breader.ReadUInt32();
                            writIdx    = breader.ReadUInt32();
                        }
                        if ((bookNum > 0) && (verseCnt == 0 || writCnt == 0))
                        {
                            genNext    = true;

                            verseIdx   = this.ChapterIndex[chapterIdx].verse_idx;
                            verseCnt   = 0;
                            writIdx    = this.ChapterIndex[chapterIdx].writ_idx;
                            writCnt    = 0;

                            for (UInt16 chapter = 0; chapter < chapterCnt; chapter++)
                            {
                                verseCnt += ChapterIndex[chapterIdx + chapter].verse_cnt;
                                writCnt += ChapterIndex[chapterIdx + chapter].word_cnt;
                            }
                        }

                        var bname = breader.ReadBytes(16);
                        var babbr = breader.ReadBytes(useZ14 ? 12 : 9+9);

                        var sbname = new StringBuilder();
                        var sbabbr = new StringBuilder();

                        for (int i = 0; i < bname.Length && bname[i] != 0; i++)
                            sbname.Append((char)bname[i]);
                        name = sbname.ToString();

                        for (int i = 0; i < sbabbr.Length && babbr[i] != 0; i++)
                            sbabbr.Append((char)sbabbr[i]);
                        abbr = sbabbr.ToString();    // ussually overridden in Z31 release

                        if (genNext && (bookNum == 0))
                        {
                            abbr = "Revision";
                            name = "";
                        }
                    }

                    writer.Write("\t" + rtype + "{ ");

                    writer.Write("num: "         + Pad(bookNum, 2) + ", ");
                    writer.Write("chapter_cnt: " + Pad(chapterCnt, 3) + ", ");
                    writer.Write("chapter_idx: " + Pad(chapterIdx, 4) + ", ");
                    writer.Write("verse_cnt: "   + Pad(verseCnt, 7) + ", ");
                    writer.Write("verse_idx: "   + Pad(verseIdx, 7) + ", ");
                    writer.Write("writ_cnt: "    + Pad(writCnt, 9) + ", ");
                    writer.Write("writ_idx: "    + Pad(writIdx, 9) + ", ");
                    writer.Write("name: \""      + name + "\", ");

                    if (bookNum > 0)
                    {
                        this.BookIndex[bookNum].chapter_cnt = chapterCnt;
                        this.BookIndex[bookNum].chapter_idx = chapterIdx;
                        this.BookIndex[bookNum].verse_cnt   = verseCnt;
                        this.BookIndex[bookNum].verse_idx   = verseIdx;
                        this.BookIndex[bookNum].writ_cnt    = writCnt;
                        this.BookIndex[bookNum].writ_idx    = writIdx;
                        this.BookIndex[bookNum].name        = name;
                    }
                    else
                    {
                        this.BookIndex[bookNum].chapter_cnt = 0;
                        this.BookIndex[bookNum].chapter_idx = 0;
                        this.BookIndex[bookNum].verse_cnt   = 0;
                        this.BookIndex[bookNum].verse_idx   = 0;
                        this.BookIndex[bookNum].writ_cnt    = 0;
                        this.BookIndex[bookNum].writ_idx    = (UInt32)((Y * 0x1000) | (M * 0x100) | D);
                        this.BookIndex[bookNum].name        = "Z" + Y.ToString() + M.ToString("X") + day;
                    }

                    if (bookNum > 0)
                    {
                        int idx;
                        var abbreviations = RustSrcGen.BK[bookNum];
                        if ((idx = abbreviations.a2.IndexOf('-')) >= 0)
                            abbreviations.a2 = (idx > 0) ? abbreviations.a2.Substring(0, idx) : string.Empty;
                        if ((idx = abbreviations.a3.IndexOf('-')) >= 0)
                            abbreviations.a3 = (idx > 0) ? abbreviations.a3.Substring(0, idx) : string.Empty;
                        if ((idx = abbreviations.a4.IndexOf('-')) >= 0)
                            abbreviations.a4 = (idx > 0) ? abbreviations.a4.Substring(0, idx) : string.Empty;
                        if ((idx = abbreviations.alternates.IndexOf('-')) >= 0)
                            abbreviations.alternates = (idx > 0) ? abbreviations.alternates.Substring(0, idx) : string.Empty;

                        var alts = abbreviations.alternates.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        var alt1 = alts.Length >= 1 ? alts[0] : abbreviations.alternates;
                        var alt2 = alts.Length >= 2 ? alts[1] : string.Empty;

                        writer.Write("abbr2: \"" + abbreviations.a2 + "\", ");
                        writer.Write("abbr3: \"" + abbreviations.a3 + "\", ");
                        writer.Write("abbr4: \"" + abbreviations.a4 + "\", ");
                        writer.Write("abbrAltA: \"" + alt1 + "\", ");
                        writer.Write("abbrAltB: \"" + alt2 + "\", ");
                    }
                    else
                    {
                        writer.Write("abbr2: \"\", ");
                        writer.Write("abbr3: \"\", ");
                        writer.Write("abbr4: \"\", ");
                        writer.Write("abbrAltA: \"\", ");
                        writer.Write("abbrAltB: \"Revision\", ");
                    }
                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");

            if (genNext)
            {
                var opath = AVXManager.SDK_BASELINE + "_NEXT";
                // Update SDK file to Z31-B/11
                var ostream = new StreamWriter(opath + "-" + this.BookIndex[0].name, false, Encoding.ASCII);
                using (var bwriter = new System.IO.BinaryWriter(ostream.BaseStream))
                {
                    for (byte bookNum = 0; bookNum < this.BookIndex.Length; bookNum++)
                    {
                        var bk = this.BookIndex[bookNum];
                        bwriter.Write(bookNum);
                        bwriter.Write(bk.chapter_cnt);
                        bwriter.Write(bk.chapter_idx);
                        bwriter.Write(bk.verse_cnt);
                        bwriter.Write(bk.verse_idx);
                        bwriter.Write(bk.writ_cnt);
                        bwriter.Write(bk.writ_idx);

                        string name = bk.name != null ? bk.name : "";
                        // print 16 bytes for name
                        for (int i = 0; i < 16; i++)
                        {
                            if (i < name.Length)
                                bwriter.Write((byte)name[i]);
                            else
                                bwriter.Write((byte)0);
                        }
                        if (bookNum > 0)
                        {
                            int idx;
                            var abbreviations = RustSrcGen.BK[bookNum];
                            if ((idx = abbreviations.a2.IndexOf('-')) >= 0)
                                abbreviations.a2 = (idx > 0) ? abbreviations.a2.Substring(0, idx) : string.Empty;
                            if ((idx = abbreviations.a3.IndexOf('-')) >= 0)
                                abbreviations.a3 = (idx > 0) ? abbreviations.a3.Substring(0, idx) : string.Empty;
                            if ((idx = abbreviations.a4.IndexOf('-')) >= 0)
                                abbreviations.a4 = (idx > 0) ? abbreviations.a4.Substring(0, idx) : string.Empty;
                            if ((idx = abbreviations.alternates.IndexOf('-')) >= 0)
                                abbreviations.alternates = (idx > 0) ? abbreviations.alternates.Substring(0, idx) : string.Empty;

                            var alts = abbreviations.alternates.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            var altA = alts.Length >= 1 ? alts[0] : abbreviations.alternates;
                            var altB = alts.Length >= 2 ? alts[1] : string.Empty;

                            for (int i = 0; i < 2; i++)
                            {
                                if (i < abbreviations.a2.Length)
                                    bwriter.Write((byte)abbreviations.a2[i]);
                                else
                                    bwriter.Write((byte)0);
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                if (i < abbreviations.a3.Length)
                                    bwriter.Write((byte)abbreviations.a3[i]);
                                else
                                    bwriter.Write((byte)0);
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                if (i < abbreviations.a4.Length)
                                    bwriter.Write((byte)abbreviations.a4[i]);
                                else
                                    bwriter.Write((byte)0);
                            }
                            int a = 0;
                            int b = 0;
                            for (a = 0; a < 9 && a < altA.Length; a++)
                            {
                                bwriter.Write((byte)altA[a]);
                            }
                            if (a > 0)
                            {
                                bwriter.Write((byte)',');
                                a++;
                            }
                            for (b = 0; a + b < 9 && b < altB.Length; b++)
                            {
                                bwriter.Write((byte)altB[b]);
                            }
                            for (a += b; a < 9; a++)
                            {
                                bwriter.Write((byte)0);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 9; i++)
                            {
                                bwriter.Write((byte)0);
                            }
                            string rev = "Revision";
                            for (int i = 0; i < rev.Length; i++)
                            {
                                bwriter.Write((byte)(rev[i]));
                            }
                            for (int i = rev.Length; i < 9;  i++)
                            {
                                bwriter.Write((byte)0);
                            }
                        }
                    }
                }
            }
        }
        private void XBook_V32(TextWriter writer, string rtype, FoundationsGenerator.Directory bom) // change default values to change behavior
        {
            var fpath = BOM.GetZ_Path(ORDER.Book, release: BOM.Z_32);

            writer.WriteLine("static " + "books" + ": [" + rtype + "; " + 67.ToString() + "] = [");

            var fstream = new StreamReader(fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (byte bookNum = 0; bookNum <= 66; bookNum++)
                {
                    byte chapterCnt = 0;
                    UInt16 chapterIdx = 0;
                    UInt16 verseCnt = 0;
                    UInt32 writCnt = 0;
                    UInt32 writIdx = 0;
                    string name = "";
                    string abbr = "";

                    var test = breader.ReadByte();
                    if (test != bookNum)
                        break;
                    chapterCnt = breader.ReadByte();
                    chapterIdx = breader.ReadUInt16();

                    verseCnt = breader.ReadUInt16();
                    writCnt = breader.ReadUInt32();
                    writIdx = breader.ReadUInt32();

                    if ((bookNum > 0) && (verseCnt == 0 || writCnt == 0))
                    {
                        verseCnt = 0;
                        writIdx = this.ChapterIndex[chapterIdx].writ_idx;
                        writCnt = 0;

                        for (UInt16 chapter = 0; chapter < chapterCnt; chapter++)
                        {
                            verseCnt += ChapterIndex[chapterIdx + chapter].verse_cnt;
                            writCnt += ChapterIndex[chapterIdx + chapter].word_cnt;
                        }
                    }

                    var bname = breader.ReadBytes(16);
                    var babbr = breader.ReadBytes(9 + 9);

                    var sbname = new StringBuilder();
                    var sbabbr = new StringBuilder();

                    for (int i = 0; i < bname.Length && bname[i] != 0; i++)
                        sbname.Append((char)bname[i]);
                    name = sbname.ToString();

                    for (int i = 0; i < sbabbr.Length && babbr[i] != 0; i++)
                        sbabbr.Append((char)sbabbr[i]);
                    abbr = sbabbr.ToString();    // ussually overridden in Z31/Z32 release

                    writer.Write("\t" + rtype + "{ ");

                    writer.Write("num: " + Pad(bookNum, 2) + ", ");
                    writer.Write("chapter_cnt: " + Pad(chapterCnt, 3) + ", ");
                    writer.Write("chapter_idx: " + Pad(chapterIdx, 4) + ", ");
                    writer.Write("verse_cnt: " + Pad(verseCnt, 7) + ", ");
                    writer.Write("writ_cnt: " + Pad(writCnt, 9) + ", ");
                    writer.Write("writ_idx: " + Pad(writIdx, 9) + ", ");
                    writer.Write("name: \"" + name + "\", ");

                    if (bookNum > 0)
                    {
                        this.BookIndex[bookNum].chapter_cnt = chapterCnt;
                        this.BookIndex[bookNum].chapter_idx = chapterIdx;
                        this.BookIndex[bookNum].verse_cnt = verseCnt;
                        this.BookIndex[bookNum].verse_idx = 0;          // This is not needed in Z-32/Omega-32
                        this.BookIndex[bookNum].writ_cnt = writCnt;
                        this.BookIndex[bookNum].writ_idx = writIdx;
                        this.BookIndex[bookNum].name = name;
                    }
                    else
                    {
                        this.BookIndex[bookNum].chapter_cnt = 0;
                        this.BookIndex[bookNum].chapter_idx = 0;
                        this.BookIndex[bookNum].verse_cnt = 0;
                        this.BookIndex[bookNum].verse_idx = 0;
                        this.BookIndex[bookNum].writ_cnt = 0;
                        this.BookIndex[bookNum].writ_idx = writIdx;
                        this.BookIndex[bookNum].name = name;
                    }

                    if (bookNum > 0)
                    {
                        int idx;
                        var abbreviations = RustSrcGen.BK[bookNum];
                        if ((idx = abbreviations.a2.IndexOf('-')) >= 0)
                            abbreviations.a2 = (idx > 0) ? abbreviations.a2.Substring(0, idx) : string.Empty;
                        if ((idx = abbreviations.a3.IndexOf('-')) >= 0)
                            abbreviations.a3 = (idx > 0) ? abbreviations.a3.Substring(0, idx) : string.Empty;
                        if ((idx = abbreviations.a4.IndexOf('-')) >= 0)
                            abbreviations.a4 = (idx > 0) ? abbreviations.a4.Substring(0, idx) : string.Empty;
                        if ((idx = abbreviations.alternates.IndexOf('-')) >= 0)
                            abbreviations.alternates = (idx > 0) ? abbreviations.alternates.Substring(0, idx) : string.Empty;

                        var alts = abbreviations.alternates.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        var alt1 = alts.Length >= 1 ? alts[0] : abbreviations.alternates;
                        var alt2 = alts.Length >= 2 ? alts[1] : string.Empty;

                        writer.Write("abbr2: \"" + abbreviations.a2 + "\", ");
                        writer.Write("abbr3: \"" + abbreviations.a3 + "\", ");
                        writer.Write("abbr4: \"" + abbreviations.a4 + "\", ");
                        writer.Write("abbrAltA: \"" + alt1 + "\", ");
                        writer.Write("abbrAltB: \"" + alt2 + "\", ");
                    }
                    else
                    {
                        writer.Write("abbr2: \"\", ");
                        writer.Write("abbr3: \"\", ");
                        writer.Write("abbr4: \"\", ");
                        writer.Write("abbrAltA: \"\", ");
                        writer.Write("abbrAltB: \"Revision\", ");
                    }
                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");
        }
        /*
        private void XChapter_V14(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.rcnt.ToString() + "] = [");

            var fstream = new StreamReader(bom.fpath.Replace(".ix", "-Z14.ix"));    // we still base input on the Z14 release, until we are certain that there are no bugs
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 0; x < bom.rcnt; x++)
                {
                    var writIdx = breader.ReadUInt32();
                    var verseIdx = breader.ReadUInt16();
                    var wordCnt = breader.ReadUInt16();

                    ChapterIndex[x].writ_idx  = writIdx;
                    ChapterIndex[x].word_cnt  = wordCnt;
                    ChapterIndex[x].verse_idx = verseIdx;
                    ChapterIndex[x].verse_cnt = 0;

                    byte b = VerseIndex[verseIdx].book;
                    byte c = VerseIndex[verseIdx].chapter;
                    UInt32 w = 0;
                    for (UInt16 verse = verseIdx; verse < VerseIndex.Length; verse++)
                    {
                        if (c == VerseIndex[verse].chapter && b == VerseIndex[verse].book)
                        {
                            ChapterIndex[x].verse_cnt++;
                            w += VerseIndex[verse].word_cnt;

                            // Sanity-Check
                            if (w > 0xFFFF)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // Sanity-Check
                    if (ChapterIndex[x].word_cnt != w)
                        break;

                    writer.Write("\t" + rtype + "{ ");

                    UInt32 normalizedIdx = writIdx - ConsoleApp.BookIndex[b].writ_idx;
                    writer.Write("writ_idx: " + Pad(normalizedIdx, 5) + ", ");
                    writer.Write("writ_cnt: " + Pad(wordCnt, 4) + ", ");
                    writer.Write("verse_idx: " + Pad(verseIdx, 5) + ", ");
                    writer.Write("verse_cnt: " + Pad(ChapterIndex[x].verse_cnt, 4) + ", ");

                    writer.WriteLine("},");
                }
            }
            writer.WriteLine("];");

            // Update SDK file fo Z31
            var ostream = new StreamWriter(bom.fpath.Replace(".ix", "-Z31.ix"), false, Encoding.ASCII);
            using (var bwriter = new System.IO.BinaryWriter(ostream.BaseStream))
            {
                foreach (var record in this.ChapterIndex)
                {
                    bwriter.Write(record.writ_idx);
                    bwriter.Write(record.word_cnt);
                    bwriter.Write(record.verse_idx);
                    bwriter.Write(record.verse_cnt);
                }
            }
        }
        */
        private void XChapter_V31(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var outname = BOM.GetC_Type(ORDER.Chapter);
            var fpath = BOM.GetZ_Path(ORDER.Chapter, release: BOM.Z_31);

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.recordCount.ToString() + "] = [");

            var fstream = new StreamReader(fpath);    // we still base input on the Z14 release, until we are certain that there are no bugs
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 0; x < bom.recordCount; x++)
                {
                    var writIdx  = breader.ReadUInt32();
                    var writCnt  = breader.ReadUInt16();
                    var verseIdx = breader.ReadUInt16();
                    var verseCnt = breader.ReadUInt16();

                    ChapterIndex[x].writ_idx = writIdx;
                    ChapterIndex[x].word_cnt = writCnt;
                    ChapterIndex[x].verse_idx = verseIdx;
                    ChapterIndex[x].verse_cnt = verseCnt;

                    byte b = VerseIndex[verseIdx].book;
                    byte c = VerseIndex[verseIdx].chapter;
                    UInt32 w = 0;
                    for (UInt16 verse = verseIdx; verse < VerseIndex.Length; verse++)
                    {
                        if (c == VerseIndex[verse].chapter && b == VerseIndex[verse].book)
                        {
                            ChapterIndex[x].verse_cnt++;
                            w += VerseIndex[verse].word_cnt;

                            // Sanity-Check
                            if (w > 0xFFFF)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // Sanity-Check
                    if (ChapterIndex[x].word_cnt != w)
                        break;

                    writer.Write("\t" + rtype + "{ ");

                    UInt32 normalizedIdx = writIdx - BookIndex[b].writ_idx;
                    writer.Write("writ_idx: "  + Pad(normalizedIdx, 5) + ", ");
                    writer.Write("writ_cnt: "  + Pad(writCnt,  4) + ", ");
                    writer.Write("verse_idx: " + Pad(verseIdx, 5) + ", ");
                    writer.Write("verse_cnt: " + Pad(verseCnt, 4) + ", ");

                    writer.WriteLine("},");
                }
            }
            writer.WriteLine("];");
        }
        private void XChapter_V32(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var outname = BOM.GetC_Type(ORDER.Chapter);
            var fpath = BOM.GetZ_Path(ORDER.Chapter, release: BOM.Z_32);

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.recordCount.ToString() + "] = [");

            var fstream = new StreamReader(fpath);    // we still base input on the Z14 release, until we are certain that there are no bugs
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 0; x < bom.recordCount; x++)
                {
                    var writIdx = breader.ReadUInt16();
                    var writCnt = breader.ReadUInt16();
                    var book = breader.ReadByte();
                    var verseCnt = breader.ReadByte();

                    ChapterIndex[x].writ_idx = writIdx;
                    ChapterIndex[x].word_cnt = writCnt;
                    ChapterIndex[x].verse_idx = 0;
                    ChapterIndex[x].book = book;
                    ChapterIndex[x].verse_cnt = verseCnt;

                    writer.Write("\t" + rtype + "{ ");

                    writer.Write("writ_idx: " + Pad(writIdx, 5) + ", ");
                    writer.Write("writ_cnt: " + Pad(writCnt, 4) + ", ");
                    writer.Write("book_num: " + Pad(book, 2) + ", ");
                    writer.Write("verse_cnt: " + Pad(verseCnt, 2) + ", ");

                    writer.WriteLine("},");
                }
            }
            writer.WriteLine("];");
        }
        private void XVerse(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var outname = BOM.GetC_Type(ORDER.UNDEFINED);
            var fpath = BOM.GetZ_Path(ORDER.UNDEFINED);

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.recordCount.ToString() + "] = [");

            var fstream = new StreamReader(fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 0; x < bom.recordCount; x++)
                {
                    var book = breader.ReadByte();
                    var chapter = breader.ReadByte();
                    var verse = breader.ReadByte();
                    var wordCnt = breader.ReadByte();

                    VerseIndex[x].book = book;
                    VerseIndex[x].chapter = chapter;
                    VerseIndex[x].verse = verse;
                    VerseIndex[x].word_cnt = wordCnt;

                    writer.Write("\t" + rtype + "{ ");

                    writer.Write("book: " + Pad(book, 2) + ", ");
                    writer.Write("chapter: " + Pad(chapter, 3) + ", ");
                    writer.Write("verse: " + Pad(verse, 3) + ", ");
                    writer.Write("word_cnt: " + Pad(wordCnt, 3));
                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");
        }
        private void XLemma(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var fpath = BOM.GetZ_Path(ORDER.Lemmata);

//          writer.WriteLine("static lemmata: HashMap < AVXLemmaKey, AVXLemma > = HashMap::from([");

            var outname = "AVXLemmata"; // bom.otype.Replace('-', '_').ToLower();
            writer.WriteLine("static " + outname + ": [(AVXLemmaKey, AVXLemma); " + bom.recordCount.ToString() + "] = [");

            var fstream = new StreamReader(fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    var pos = breader.ReadUInt32();
                    var wordKey = breader.ReadUInt16();
                    var wordClass = breader.ReadUInt16();
                    var lemmaCount = breader.ReadUInt16();

                    if (lemmaCount > 3)
                    {
                        Console.WriteLine("Bad Assumption! (" + lemmaCount.ToString() + ")");
                    }

                    writer.Write("\t( AVXLemmaKey { word_key: 0x"   + wordKey.ToString("X04")   + ", pos32: 0x" + pos.ToString("X08") + " }, "
                                   + "AVXLemma { pn_pos: 0x" + wordClass.ToString("X04") + ", lemmata: [");

                    for (int i = 1; i <= 3; i++) // array is fixed size==3
                    {
                        var lemma = (i <= lemmaCount) ? breader.ReadUInt16() : 0x0000;
                        writer.Write("0x" + lemma.ToString("X04") + ((i < 3) ? ", " : "] "));
                    }
                    writer.WriteLine("} ),");
                }
            }
            writer.WriteLine("];");
        }
        private void XLemmaOOV(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var outname = BOM.GetC_Type(ORDER.OOV);
            var fpath = BOM.GetZ_Path(ORDER.OOV);

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.recordCount.ToString() + "] = [");

            var buffer = new char[24];
            var fstream = new StreamReader(fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    var oovKey = breader.ReadUInt16();

                    int i = 0;
                    byte c = 0;
                    for (c = breader.ReadByte(); c != 0 && i < 24; c = breader.ReadByte())
                        buffer[i++] = (char)c;
                    buffer[i] = '\0';
                    if (c != 0) for (c = breader.ReadByte(); c != 0; c = breader.ReadByte()) // discard ... this should not happen ... check in debugger
                            Console.WriteLine("Bad stuff!!!");

                    var oovString = new string(buffer, 0, i);

                    writer.Write("\t" + rtype + " { ");

                    writer.Write("key: 0x" + oovKey.ToString("X") + ", ");
                    writer.Write("word: \"" + oovString + "\"");

                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");
        }
        private void XNames(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var outname = BOM.GetC_Type(ORDER.Names);
            var fpath = BOM.GetZ_Path(ORDER.Names);

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.recordCount.ToString() + "] = [");

            var buffer = new char[24];
            var fstream = new StreamReader(fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    var wkey = breader.ReadUInt16();
                    string meanings = AVXManager.ReadByteString(breader, maxLen: 4096);
//                  string[] meaningArray = meanings.Split('|', StringSplitOptions.RemoveEmptyEntries);

                    writer.Write("\t" + rtype + " { ");

                    writer.Write("word_key: 0x" + wkey.ToString("X04") + ", ");
                    writer.Write("meanings: \"" + meanings + "\"");

                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");
        }
        /*
        private void XLexicon_V14(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            // Update SDK file fo Z31
            var ostream = new StreamWriter(bom.fpath, false, Encoding.ASCII);
            var bwriter = new System.IO.BinaryWriter(ostream.BaseStream);

            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + (bom.rcnt + 1).ToString() + "] = [");
            writer.WriteLine("\t" + rtype + " { entities: 0xFFFF, search: \"\",  display: \"\", modern: \"\", pos: [ 12568, 31, 9 ] },");
            bwriter.Write((UInt16)0xFFFF);
            bwriter.Write((UInt16)2);
            bwriter.Write((UInt32)(12567));
            bwriter.Write((UInt32)((Y * 0x1000) | (M * 0x100) | D));
            bwriter.Write((byte) 0);
            bwriter.Write((byte) 0);
            bwriter.Write((byte) 0);

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath.Replace(".dxi", "-Z14.dxi"));    // we still base input on the Z14 release, until we are certain that there are no bugs
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    var entities = breader.ReadUInt16();
                    bwriter.Write(entities);
                    var posCnt = breader.ReadUInt16();
                    if (posCnt == 0x3117 && x >= 0x3117 + 1)
                        break;
                    bwriter.Write(posCnt);

                    UInt32[] pos = new UInt32[posCnt];
                    for (int p = 0; p < posCnt; p++)
                    {
                        var val = breader.ReadUInt32();
                        bwriter.Write(val);
                        pos[p] = val;
                    }
                    var search  = ConsoleApp.ReadByteString(breader);
                    var display = ConsoleApp.ReadByteString(breader);
                    var modern  = ConsoleApp.ReadByteString(breader);
                    ConsoleApp.WriteByteString(bwriter, search);
                    ConsoleApp.WriteByteString(bwriter, display);
                    ConsoleApp.WriteByteString(bwriter, modern);

                    writer.Write("\t" + rtype + " { ");

                    writer.Write("entities: 0x" + entities.ToString("X04") + ", search: ");
                    if (!string.IsNullOrEmpty(search))
                        writer.Write("\"" + search + "\", ");
                    else
                        writer.Write("\"\", ");

                    writer.Write("display: ");
                    if (!string.IsNullOrEmpty(display))
                        writer.Write("\"" + display + "\", ");
                    else
                        writer.Write("\"\", ");

                    writer.Write("modern: ");
                    if (!string.IsNullOrEmpty(modern))
                        writer.Write("\"" + modern + "\", ");
                    else
                        writer.Write("\"\", ");

                    writer.Write("pos: [ ");
                    int cnt = 0;
                    foreach (var p in pos)
                    {
                        writer.Write("0x" + p.ToString("X08") + ", ");
                        cnt++;
                    }
                    if (cnt == 0)
                    {
                        writer.Write("0x" + 0.ToString("X08") + ", ");
                    }
                    writer.Write("] ");

                    writer.WriteLine(" },");
                }
                writer.WriteLine("];");
            }
            bwriter.Close();
        }
        */
        private void XLexicon(TextWriter writer, string rtype, FoundationsGenerator.Directory bom)
        {
            var outname = BOM.GetC_Type(ORDER.Lexicon);
            var fpath = BOM.GetZ_Path(ORDER.Lexicon);

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + (bom.recordCount + 1).ToString() + "] = [");

            var buffer = new char[24];
            var fstream = new StreamReader(fpath);    // we still base input on the Z14 release, until we are certain that there are no bugs
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 0; x <= bom.recordCount; x++)
                {
                    var entities = breader.ReadUInt16();
                    var posCnt = breader.ReadUInt16();

                    if (posCnt > 6)
                    {
                        Console.WriteLine("Bad Assumption");
                    }
                    UInt32[] poses = new UInt32[posCnt];
                    for (int p = 0; p < posCnt; p++)
                    {
                        var val = breader.ReadUInt32();
                        poses[p] = val;
                    }
                    var search = AVXManager.ReadByteString(breader);
                    var display = AVXManager.ReadByteString(breader);
                    var modern = AVXManager.ReadByteString(breader);

                    writer.Write("\t" + rtype + " { ");

                    writer.Write("entities: 0x" + entities.ToString("X04") + ", search: ");
                    if (!string.IsNullOrEmpty(search))
                        writer.Write("\"" + search + "\", ");
                    else
                        writer.Write("\"\", ");

                    writer.Write("display: ");
                    if (!string.IsNullOrEmpty(display))
                        writer.Write("\"" + display + "\", ");
                    else
                        writer.Write("\"\", ");

                    writer.Write("modern: ");
                    if (!string.IsNullOrEmpty(modern))
                        writer.Write("\"" + modern + "\", ");
                    else
                        writer.Write("\"\", ");

                    writer.Write("pos: [");

                    int i = 0;
                    foreach (var pos in poses)
                    {
                        writer.Write("0x" + pos.ToString("X08") + (++i < 6 ? ", " : "]"));
                    }
                    for (++i; i <= 6; i++)
                    {
                        writer.Write("0x00000000" + (i < 6 ? ", " : "]"));
                    }
                    writer.WriteLine(" },");
                }
                writer.WriteLine("];");
            }
        }
        private void XWrit(TextWriter writer, string rtype, FoundationsGenerator.Directory bom, Dictionary<string, (UInt16 count, byte book)> metadata)
        {
            foreach (var variable in metadata.Keys)
            {
                writer.WriteLine("pub mod " + variable + ";");
            }
            writer.WriteLine();

            foreach (var variable in metadata.Keys)
            {
                writer.WriteLine("pub use " + variable + ";");
            }
            writer.WriteLine();

            writer.WriteLine("static  AVXWrittenAll: [ AVXWritItem ; " + metadata.Count.ToString() + "] = [");

            foreach (var variable in metadata.Keys)
            {
                writer.WriteLine("\tAVXWritItem { book:" + Pad(metadata[variable].book,  2) + ", written: "  + variable + " },");
            }
            writer.WriteLine("];");
        }
        private (string variable, UInt16 count, byte bookNum) XWrit_V31(BinaryReader breader, byte book, string rtype, FoundationsGenerator.Directory bom)
        {
            (string variable, UInt16 count, byte book) result;

            var dirname = BOM.GetC_Type(ORDER.Written);
            var outname = dirname + "_" + book.ToString("D02");
            var vartype = "AVX" + bom.label.Replace("-", "");
            var wordCnt = this.BookIndex[book].writ_cnt;
            result = (outname, (UInt16)wordCnt, book);

            var path = Path.Combine(this.output, dirname, outname + ".rs");

            TextWriter writer = File.CreateText(path);

            writer.WriteLine("// This file is entirely code generated. All edits to this module will be lost,");
            writer.WriteLine("// when code is regenerated");

            writer.WriteLine();
            writer.WriteLine("pub static " + vartype + "_Rust_Edition    :u16 = 23108;");
            writer.WriteLine("pub static " + vartype + "_SDK_ZEdition    :u16 = 23107;");
            writer.WriteLine();
            writer.WriteLine("use crate::avx::written::AVXWrit;");
            writer.WriteLine();

            writer.WriteLine("pub static " + outname + ": [" + rtype + "; " + this.BookIndex[book].writ_cnt.ToString() + "] = [");

            for (int x = 0; x < wordCnt; x++)
            {
                var Strongs = breader.ReadUInt64();
                var VerseIdx = breader.ReadUInt16();
                var Word = breader.ReadUInt16();
                var Punc = breader.ReadByte();
                var Trans = breader.ReadByte();
                var Pnwc = breader.ReadUInt16();
                var Pos = breader.ReadUInt32();
                var Lemma = breader.ReadUInt16();

                this.Maps.Add(Pos, Pnwc);

                writer.Write("\t" + rtype + " { ");

                writer.Write("strongs: [ ");
                int shift = 3 * 16;
                int index = 0;
                for (/**/; index < 4; shift -= 16, index++)
                {
                    var strongs = (UInt16)((Strongs >> shift) & 0xFFFF);
                    writer.Write(Pad(strongs, 4) + ",");
                }
                writer.Write(" ], ");

                writer.Write("verse_idx: " + Pad(VerseIdx, 5) + ", ");
                writer.Write("word: 0x" + Word.ToString("X04") + ", ");
                writer.Write("punc: 0x" + Punc.ToString("X02") + ", ");
                writer.Write("trans: 0x" + Trans.ToString("X02") + ", ");
                writer.Write("pnwc: 0x" + Pnwc.ToString("X04") + ", ");
                writer.Write("pos: 0x" + Pos.ToString("X08") + ", ");
                writer.Write("lemma: 0x" + Lemma.ToString("X04"));

                writer.WriteLine(" },");
            }
            writer.WriteLine("];");
            writer.Close();
            return result;
        }
        private (string variable, UInt16 count, byte bookNum) XWrit_V32(BinaryReader breader, byte book, string rtype, FoundationsGenerator.Directory bom)
        {
            (string variable, UInt16 count, byte book) result;

            var dirname = BOM.GetC_Type(ORDER.Written);
            var outname = dirname + "_" + book.ToString("D02");
            var vartype = "AVX" + bom.label.Replace("-", "");
            var wordCnt = this.BookIndex[book].writ_cnt;
            result = (outname, (UInt16)wordCnt, book);

            var path = Path.Combine(this.output, dirname, outname + ".rs");

            TextWriter writer = File.CreateText(path);

            writer.WriteLine("// This file is entirely code generated. All edits to this module will be lost,");
            writer.WriteLine("// when code is regenerated");

            writer.WriteLine();
            writer.WriteLine("pub static " + vartype + "_Rust_Edition    :u16 = 23108;");
            writer.WriteLine("pub static " + vartype + "_SDK_ZEdition    :u16 = 23107;");
            writer.WriteLine();
            writer.WriteLine("use crate::avx::written::AVXWrit;");
            writer.WriteLine();

            writer.WriteLine("pub static " + outname + ": [" + rtype + "; " + this.BookIndex[book].writ_cnt.ToString() + "] = [");

            var bcvw = new string[] { "b", "c", "v", "wc" };
            for (int x = 0; x < wordCnt; x++)
            {
                var Strongs = breader.ReadUInt64();
                var BCVW = breader.ReadBytes(4);
                var Word = breader.ReadUInt16();
                var Punc = breader.ReadByte();
                var Trans = breader.ReadByte();
                var Pnwc = breader.ReadUInt16();
                var Pos = breader.ReadUInt32();
                var Lemma = breader.ReadUInt16();

                this.Maps.Add(Pos, Pnwc);

                writer.Write("\t" + rtype + " { ");

                writer.Write("strongs: [ ");
                int shift = 3 * 16;
                int index = 0;
                for (/**/; index < 4; shift -= 16, index++)
                {
                    var strongs = (UInt16)((Strongs >> shift) & 0xFFFF);
                    writer.Write(Pad(strongs, 4) + ",");
                }
                writer.Write(" ], ");

                for (int i = 0; i < 4; i++)
                {
                    writer.Write(bcvw[i] + ": " + Pad(BCVW[i], 2) + ", ");
                }
                writer.Write("word: 0x" + Word.ToString("X04") + ", ");
                writer.Write("punc: 0x" + Punc.ToString("X02") + ", ");
                writer.Write("trans: 0x" + Trans.ToString("X02") + ", ");
                writer.Write("pnwc: 0x" + Pnwc.ToString("X04") + ", ");
                writer.Write("pos: 0x" + Pos.ToString("X08") + ", ");
                writer.Write("lemma: 0x" + Lemma.ToString("X04"));

                writer.WriteLine(" },");
            }
            writer.WriteLine("];");
            writer.Close();
            return result;
        }
        public static (string name, string a2, string a3, string a4, string alternates)[] BK = new[] {
            ///book/////////////////  a2    a3     common  alternates //////////////////////////////
         ( "-", "-", "-", "-", "-" ),
         ( "Genesis",           "Ge", "Gen", "Gen-", "Gn-------" ),
         ( "Exodus",            "Ex", "Exo", "Exo-", "Exod-----" ),
         ( "Leviticus",         "Le", "Lev", "Lev-", "Lv-------" ),
         ( "Numbers",           "Nu", "Num", "Numb", "Nb-------" ),
         ( "Deuteronomy",       "Dt", "D't", "Deut", "De-------" ),
         ( "Joshua",            "Js", "Jsh", "Josh", "Jos------" ),
         ( "Judges",            "Jg", "Jdg", "Judg", "Jdgs-----" ),
         ( "Ruth",              "Ru", "Rth", "Ruth", "Rut------" ),
         ( "1 Samuel",          "1S", "1Sm", "1Sam", "1Sa------" ),
         ( "2 Samuel",          "2S", "2Sm", "2Sam", "1Sa------" ),
         ( "1 Kings",           "1K", "1Ki", "1Kgs", "1Kg,1Kin-" ),
         ( "2 Kings",           "2K", "2Ki", "2Kgs", "2Kg,2Kin-" ),
         ( "1 Chronicles",      "--", "1Ch", "1Chr", "1Chron---" ),
         ( "2 Chronicles",      "--", "2Ch", "2Chr", "2Chron---" ),
         ( "Ezra",              "--", "Ezr", "Ezra", "---------" ),
         ( "Nehemiah",          "Ne", "Neh", "Neh-", "---------" ),
         ( "Esther",            "Es", "Est", "Est-", "Esth-----" ),
         ( "Job",               "Jb", "Job", "Job-", "---------" ),
         ( "Psalms",            "Ps", "Psa", "Pslm", "Psm,Pss--" ),
         ( "Proverbs",          "Pr", "Pro", "Prov", "Prv------" ),
         ( "Ecclesiastes",      "Ec", "Ecc", "Eccl", "Eccle,Qoh" ),
         ( "Song of Solomon",   "So", "SoS", "Song", "SS,Cant--" ),
         ( "Isaiah",            "Is", "Isa", "Isa-", "---------" ),
         ( "Jeremiah",          "Je", "Jer", "Jer-", "Jeremy,Jr" ),
         ( "Lamentations",      "La", "Lam", "Lam-", "---------" ),
         ( "Ezekiel",           "--", "Eze", "Ezek", "Ezk------" ),
         ( "Daniel",            "Da", "Dan", "Dan-", "Dn-------" ),
         ( "Hosea",             "Ho", "Hos", "Hos-", "---------" ),
         ( "Joel",              "Jl", "Jol", "Joel", "Joe------" ),
         ( "Amos",              "Am", "Amo", "Amos", "---------" ),
         ( "Obadiah",           "Ob", "Obd", "Obad", "---------" ),
         ( "Jonah",             "--", "Jnh", "Jona", "---------" ),
         ( "Micah",             "Mc", "Mic", "Mica", "Mi-------" ),
         ( "Nahum",             "Na", "Nah", "Nah-", "---------" ),
         ( "Habakkuk",          "Hb", "Hab", "Hab-", "---------" ),
         ( "Zephaniah",         "Zp", "Zep", "Zeph", "Zph------" ),
         ( "Haggai",            "Hg", "Hag", "Hag-", "---------" ),
         ( "Zechariah",         "Zc", "Zec", "Zech", "Zch------" ),
         ( "Malachi",           "Ml", "Mal", "Mal-", "---------" ),
         ( "Matthew",           "Mt", "Mat", "Matt", "---------" ),
         ( "Mark",              "Mk", "Mrk", "Mark", "Mk,Mr----" ),
         ( "Luke",              "Lk", "Luk", "Luke", "Lu-------" ),
         ( "John",              "Jn", "Jhn", "John", "Joh------" ),
         ( "Acts",              "Ac", "Act", "Acts", "Ats------" ),
         ( "Romans",            "Ro", "Rom", "Rom-", "Rm-------" ),
         ( "1 Corinthians",     "--", "1Co", "1Cor", "---------" ),
         ( "2 Corinthians",     "--", "2Co", "2Cor", "---------" ),
         ( "Galatians",         "Ga", "Gal", "Gal-", "---------" ),
         ( "Ephesians",         "Ep", "Eph", "Eph-", "---------" ),
         ( "Philippians",       "Pp", "Php", "Phil", "Philip---" ),
         ( "Colossians",        "Co", "Col", "Col-", "---------" ),
         ( "1 Thessalonians",   "--", "1Th", "1Th-", "1Thess---" ),
         ( "2 Thessalonians",   "--", "2Th", "2Th-", "2Thess---" ),
         ( "1 Timothy",         "--", "1Ti", "1Tim", "---------" ),
         ( "2 Timothy",         "--", "2Ti", "2Tim", "---------" ),
         ( "Titus",             "Ti", "Ti-", "Ti--", "---------" ),
         ( "Philemon",          "Pm", "Phm", "Phm-", "Philem---" ),
         ( "Hebrews",           "--", "Heb", "Heb-", "Hbr,Hbrs-" ),
         ( "James",             "Jm", "Jam", "Jam-", "---------" ),
         ( "1 Peter",           "1P", "1Pe", "1Pet", "1Pt------" ),
         ( "2 Peter",           "2P", "2Pe", "2Pet", "2Pt------" ),
         ( "1 John",            "1J", "1Jn", "1Jn-", "1Jn,1Jhn-" ),
         ( "2 John",            "2J", "2Jn", "2Jn-", "1Jn,1Jhn-" ),
         ( "3 John",            "3J", "3Jn", "3Jn-", "1Jn,1Jhn-" ),
         ( "Jude",              "Jd", "Jd-", "Jude", "---------" ),
         ( "Revelation",        "Re", "Rev", "Rev-", "---------" ) };
    }
}
