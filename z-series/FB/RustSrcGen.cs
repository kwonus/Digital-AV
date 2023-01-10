using DigitalAV.Migration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace SerializeFromSDK
{
    public class RustSrcGen
    {
        private string output;
        private string sdk;
        private Dictionary<string, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize)> inventory;

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
            UInt16 verse_cnt
        )[] ChapterIndex = new (UInt32, UInt16, UInt16, UInt16)[1189];
        private
        (
            byte   chapter_cnt,
            UInt16 chapter_idx,
            UInt16 verse_cnt,
            UInt16 verse_idx,
            UInt32 writ_idx,
            UInt32 writ_cnt,
            string? name,
            string?[] abbreviations
        )[] BookIndex = new (byte, UInt16, UInt16, UInt16, UInt32, UInt32, string?, string?[])[67];

        public RustSrcGen(string sdk, string src, Dictionary<string, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize)> inventory)
        {
            this.inventory = inventory;
            this.sdk = sdk;
            this.output = src;
        }
        public bool Generate()
        {
            Console.WriteLine("Create source initializers for Rust.");

            foreach (var item in inventory.Keys)
            {
                var record = this.inventory[item];
                var select = Path.GetFileNameWithoutExtension(item).Substring(3);

                if (select == "Verse")
                {
                    this.XAny(select, record);
                    break;
                }
            }
            string saveBookItem = "";
            string saveWritItem = "";
            foreach (var item in inventory.Keys)
            {
                var record = this.inventory[item];
                var select = Path.GetFileNameWithoutExtension(item).Substring(3);

                switch (select)
                {
                    case "Book":        saveBookItem = item;       break;
                    case "Chapter":     this.XAny(select, record); break;
                    //case "Verse":     this.XAny(select, record); break;
                    case "Lemma":       this.XAny(select, record); break;
                    case "Lemma-OOV":   this.XAny(select, record); break;
                    case "Lexicon":     this.XAny(select, record); break;
                    case "Names":       this.XAny(select, record); break;
                    case "WordClass":   this.XAny(select, record); break;
                    case "Writ":        saveWritItem = item;       break;
                }
            }
            // These need to be done last and in this order (XWrit differs from processing of other files)
            this.XAny("Book", this.inventory[saveBookItem]);
            var writBom = this.inventory[saveWritItem];
            var fstream = new StreamReader(writBom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            { 
                for (byte i = 1; i <= 66; i++)
                    this.XWrit(breader, i, "AVXWrit", writBom);
            }
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
        private void XAny(string select, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();
            var vartype = "AVX" + bom.otype[0].ToString().ToUpper() + bom.otype.Substring(1).Replace("-", "");

            var fname = Path.GetFileName(bom.fpath);
            var path = Path.Combine(this.output, outname + ".rs");
            var old  = Path.Combine(this.output, outname + "-Z31.rs");
            var temp = Path.Combine(this.output, outname + "-temp.rs");
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
                        writer.WriteLine("// For example, these comments are wrapped in a pair Generated-Coded directives.");
                        writer.WriteLine(HEAD[END]);
                        op = OP_NOOP;
                        continue;
                    }
                    case OP_META:
                    {
                        writer.WriteLine(META[BEGIN]);
                        writer.WriteLine("static " + vartype + "_Rust_Edition    :u16 = 23108;");
                        writer.WriteLine("static " + vartype + "_SDK_ZEdition    :u16 = 23107;");
                        writer.WriteLine();
                        writer.WriteLine("static " + vartype + "_File: &'static str = \"" + fname + "\";");
                        writer.WriteLine("static " + vartype + "_RecordLen   :usize = " + Pad(bom.rlen, 8) + ";");
                        writer.WriteLine("static " + vartype + "_RecordCnt   :usize = " + Pad(bom.rcnt, 8) + ";");
                        writer.WriteLine("static " + vartype + "_FileLen     :usize = " + Pad(bom.fsize, 8) + ";");
                        writer.WriteLine(META[END]);
                        op = OP_NOOP;
                        continue;
                    }
                    case OP_INIT:
                    {
                        writer.WriteLine(INIT[BEGIN]);
                        switch (select)
                        {
                            case "Book":        this.XBook(     writer, "AVXBook",      bom); break;
                            case "Chapter":     this.XChapter(  writer, "AVXChapter",   bom); break;
                            case "Verse":       this.XVerse(    writer, "AVXVerse",     bom); break;
                            case "Lemma":       this.XLemma(    writer, "AVXLemma",     bom); break;
                            case "Lemma-OOV":   this.XLemmaOOV( writer, "AVXLemmaOOV",  bom); break;
                            case "Lexicon":     this.XLexicon(  writer, "AVXLexItem",   bom); break;
                            case "Names":       this.XNames(    writer, "AVXName",      bom); break;
                            case "WordClass":   this.XWordClass(writer, "AVXWordClass", bom); break;
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
        private void XBook(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + "books" + ": [" + rtype + "; " + (bom.rcnt+1).ToString() + "] = [");
            writer.WriteLine("\t" + rtype + "{ num:  0, chapter_cnt:   0, chapter_idx:    0, verse_cnt:       0, verse_idx:       0, writ_cnt:        31, writ_idx:        31, name: \"Z31.9\", abbreviations: [  \"\", \"\", \"\" ] },");
            this.BookIndex[0].chapter_cnt = 0;
            this.BookIndex[0].chapter_idx = 0;
            this.BookIndex[0].verse_idx = 0;
            this.BookIndex[0].verse_cnt = 0;
            this.BookIndex[0].writ_cnt = 31;
            this.BookIndex[0].writ_idx = 0;
            this.BookIndex[0].name = "Z31.9";
            this.BookIndex[0].abbreviations = new string[0];

            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                byte bookNum = 0;
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    bookNum = breader.ReadByte();
                    var chapterCnt = breader.ReadByte();
                    var chapterIdx = breader.ReadUInt16();
                    var bname = breader.ReadBytes(16);
                    var babbr = breader.ReadBytes(12);

                    this.BookIndex[bookNum].chapter_cnt = chapterCnt;
                    this.BookIndex[bookNum].chapter_idx = chapterIdx;
                    this.BookIndex[bookNum].verse_idx = this.ChapterIndex[chapterIdx].verse_idx;
                    this.BookIndex[bookNum].verse_cnt = 0;
                    this.BookIndex[bookNum].writ_idx = this.ChapterIndex[chapterIdx].writ_idx;

                    for (UInt16 chapter = 0; chapter < chapterCnt; chapter++)
                    {
                        this.BookIndex[bookNum].verse_cnt += ChapterIndex[chapterIdx + chapter].verse_cnt;
                        this.BookIndex[bookNum].writ_cnt  += ChapterIndex[chapterIdx + chapter].word_cnt;
                    }

                    var name = new StringBuilder();
                    var abbr = new StringBuilder();

                    for (int i = 0; i < bname.Length && bname[i] != 0; i++)
                        name.Append((char)bname[i]);
                    for (int i = 0; i < babbr.Length && babbr[i] != 0; i++)
                        abbr.Append((char)babbr[i]);

                    this.BookIndex[bookNum].abbreviations = abbr.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    this.BookIndex[bookNum].name = name.ToString();

                    writer.Write("\t" + rtype + "{ ");

                    var bk = this.BookIndex[bookNum];
                    writer.Write("num: " + Pad(bookNum, 2) + ", ");
                    writer.Write("chapter_cnt: " + Pad(chapterCnt, 3) + ", ");
                    writer.Write("chapter_idx: " + Pad(chapterIdx, 4) + ", ");
                    writer.Write("verse_cnt: " + Pad(bk.verse_cnt, 7) + ", ");
                    writer.Write("verse_idx: " + Pad(bk.verse_idx, 7) + ", ");
                    writer.Write("writ_cnt: " + Pad(bk.writ_cnt, 9) + ", ");
                    writer.Write("writ_idx: " + Pad(bk.writ_idx, 9) + ", ");
                    writer.Write("name: \"" + name.ToString() + "\", ");

                    var insideDelimiter = "abbreviations: [  ";
                    int a = 0;
                    foreach (var ab in bk.abbreviations)
                    {
                        if (++a > 3)
                            break;
                        writer.Write(insideDelimiter);
                        insideDelimiter = ", ";
                        writer.Write("\"" + ab + "\"");
                    }
                    for (/**/; a < 3; a++)
                    {
                        writer.Write(insideDelimiter);
                        insideDelimiter = ", ";
                        writer.Write("\"\"");
                    }
                    writer.Write(" ]");

                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");

            // Update SDK file fo Z31
            var ostream = new StreamWriter(bom.fpath.Replace(".ix", "-Z31.ix"), false, Encoding.ASCII);
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

                    // print 16 bytes for name
                    for (int i = 0; i < 16; i++)
                    {
                        if (i < bk.name.Length)
                            bwriter.Write((byte)bk.name[i]);
                        else
                            bwriter.Write((byte)0);
                    }
                    // print 12 bytes for all abbreviations
                    int j = 0;
                    int cnt = 0;
                    foreach (var abbr in bk.abbreviations)
                    {
                        if (j > 12)
                            break;
                        for (int a = 0; a < abbr.Length; a++)
                        {
                            bwriter.Write((byte)abbr[a]);
                            j++;
                        }
                        if (++cnt < bk.abbreviations.Length)
                        {
                            bwriter.Write((byte)',');
                            j++;
                        }
                    }
                    for (/**/; j < 12; j++)
                    {
                        bwriter.Write((byte)0);
                    }
                }
            }
        }
        private void XChapter(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.rcnt.ToString() + "] = [");

            var fstream = new StreamReader(bom.fpath);
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

                    writer.Write("writ_idx: " + Pad(writIdx, 6) + ", ");
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
        private void XVerse(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.rcnt.ToString() + "] = [");

            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 0; x < bom.rcnt; x++)
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
        private void XLemma(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.Write("static " + outname + ": [" + rtype + "; " + bom.rcnt.ToString() + "] = [");

            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var pos = breader.ReadUInt32();
                    var wordKey = breader.ReadUInt16();
                    var wordClass = breader.ReadUInt16();
                    var lemmaCount = breader.ReadUInt16();

                    writer.Write("\t" + rtype + " { ");

                    writer.Write("pos: 0x" + pos.ToString("X08") + ", ");
                    writer.Write("word_key: 0x" + wordKey.ToString("X04") + ", ");
                    writer.Write("word_class: 0x" + wordClass.ToString("X04") + ", ");

                    string seperator = "lemma: [ ";
                    for (int i = 0; i < lemmaCount; i++)
                    {
                        var lemma = breader.ReadUInt16();
                        writer.Write(seperator + "0x" + lemma.ToString("X04"));
                        seperator = ", ";
                    }
                    writer.Write(" ]");
                    writer.Write(" }");
                }
            }
            writer.WriteLine("];");
        }
        private void XLemmaOOV(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.rcnt.ToString() + "] = [");

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.rcnt; x++)
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
        private void XNames(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.rcnt.ToString() + "] = [");

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    var wkey = breader.ReadUInt16();
                    var meanings = ConsoleApp.ReadByteString(breader, maxLen: 4096);
                    var meaningArray = meanings.Split('|', StringSplitOptions.RemoveEmptyEntries);

                    writer.Write("\t" + rtype + " { ");

                    writer.Write("word_key: 0x" + wkey.ToString("X04") + ", ");
                    writer.Write("meaning: [");

                    foreach (var meaning in meaningArray)
                    {
                        writer.Write("\"" + meaning + "\", ");
                    }
                    writer.Write("]");
                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");
        }

        private void XLexicon(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            // Update SDK file fo Z31
            var ostream = new StreamWriter(bom.fpath.Replace(".dxi", "-Z31.dxi"), false, Encoding.ASCII);
            var bwriter = new System.IO.BinaryWriter(ostream.BaseStream);

            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + (bom.rcnt + 1).ToString() + "] = [");
            writer.WriteLine("\t" + rtype + " { entities: 0xFFFF, search: \"\",  display: \"\", modern: \"\", pos: [ 12567, 31, 9 ] },");
            bwriter.Write((UInt16)0xFFFF);
            bwriter.Write((UInt16)3);
            bwriter.Write((UInt32)(bom.rcnt + 1));
            bwriter.Write((UInt32)31);
            bwriter.Write((UInt32) 9);
            bwriter.Write((byte) 0);
            bwriter.Write((byte) 0);
            bwriter.Write((byte) 0);

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    var entities = breader.ReadUInt16();
                    int posCnt = breader.ReadUInt16();
                    if (posCnt == 0x3117 && x >= 0x3117 + 1)
                        break;

                    UInt32[] pos = new UInt32[posCnt];
                    for (int p = 0; p < posCnt; p++)
                        pos[p] = breader.ReadUInt32();

                    var search = ConsoleApp.ReadByteString(breader);
                    var display = ConsoleApp.ReadByteString(breader);
                    var modern = ConsoleApp.ReadByteString(breader);

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
        private void XWordClass(TextWriter writer, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + bom.rcnt.ToString() + "] = [");

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    var wclass = breader.ReadUInt16();
                    var posCnt = breader.ReadUInt16();

                    writer.Write("\t" + rtype + "{ ");

                    writer.Write("word_class: 0x" + wclass.ToString("X04") + ", ");

                    writer.Write("pos: [ ");
                    for (int i = 0; i < posCnt; i++)
                    {
                        var pos = breader.ReadUInt32();
                        writer.Write("0x" + pos.ToString("X08") + ",");
                    }
                    writer.Write(" ]");
                    writer.WriteLine(" },");
                }
            }
            writer.WriteLine("];");
        }
        private void XWrit(BinaryReader breader, byte book, string rtype, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom)
        {
            var outname = bom.otype.Replace('-', '_').ToLower() + "_" + book.ToString("D02");
            var vartype = "AVX" + bom.otype[0].ToString().ToUpper() + bom.otype.Substring(1).Replace("-", "");

            var fname = Path.GetFileName(bom.fpath);
            var path = Path.Combine(this.output, "book_index", outname + ".rs");

            TextWriter writer = File.CreateText(path);

            writer.WriteLine("// This file is entirely code generated. All edits to this module will be lost.");
            writer.WriteLine("// when code is regenerated");

            writer.WriteLine();
            writer.WriteLine("static " + vartype + "_Rust_Edition    :u16 = 23108;");
            writer.WriteLine("static " + vartype + "_SDK_ZEdition    :u16 = 23107;");
            writer.WriteLine();
            writer.WriteLine("use avxlib::avx::book_index::AVXWrit;");
            writer.WriteLine();

            writer.WriteLine("static " + outname + ": [" + rtype + "; " + this.BookIndex[book].writ_cnt.ToString() + "] = [");

            var wordCnt = this.BookIndex[book].writ_cnt;
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
        }
    }
}
