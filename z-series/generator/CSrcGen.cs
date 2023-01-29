using DigitalAV.Migration;
using FoundationsGenerator;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SerializeFromSDK
{
    public class CSrcGen
    {
        private string output;
        private string sdk;

        private
        (
            byte chapter_cnt,
            UInt16 chapter_idx,
            UInt16 verse_cnt,
            UInt16 verse_idx,
            UInt32 writ_idx,
            UInt32 writ_cnt
        )[] BookIndex = new (byte, UInt16, UInt16, UInt16, UInt32, UInt32)[67];

        public CSrcGen(string sdk, string src)
        {
            this.sdk = sdk;
            this.output = src;
        }
        public bool Generate()
        {
            Console.WriteLine("Create source initializers for C.");

            this.XBook(     "AVXBookIndex::AVXBook const AVXBookIndex::index[]");              
            this.XChapter(  "AVXChapterIndex::AVXChapter const AVXChapterIndex::index[]");     
            this.XVerse(    "AVXVerseIndex::AVXVerse const AVXVerseIndex::index[]");
            this.XLemma(    "AVXLemmataRecords::AVXLemmata const AVXLemmataRecords::records[]");
            this.XLemmaOOV( "AVXLemmataOOV::AVXLemmaOOV const AVXLemmataOOV::vocabulary[]");   
            this.XLexicon(  "AVXLexicon::AVXLexItem const AVXLexicon::items[]");               
            this.XNames(    "AVXNames::AVXName const AVXNames::names[]");                      

            // This needs to be done last and in this order (XWrit differs from processing of other files)
            this.XWrit(     "AVXWritten::AVXWrit const AVXWritten::written[]", "static AVXWrit const written[]");
            return true;
        }
        private string Pad<T>(T num, int width)
        {
            var val = num != null ? num.ToString() : "";
            var fmt = "{0," + width.ToString() + "}";
            return string.Format(fmt, val);
        }

        private TextWriter XInitializeWrit(string inializerVar, string memberVar, byte bookNum)
        {
            var bom = BOM.Inventory[BOM.Written];
            var suffix = (bookNum <= 9 ? "_0" : "_") + bookNum.ToString();
            var path = bom.GetCppSource(BOM.Z_31, suffix + ".cpp");
            var header = Path.GetFileName(bom.GetCppSource(BOM.Z_31, ".h"));

            TextWriter writer = File.CreateText(path);
            writer.WriteLine("#include \"" + header + "\"");
            writer.Write(inializerVar.Replace("[]", suffix + "[]") + " = {");

            Console.WriteLine(memberVar.Replace("[]", suffix + "[" + BookIndex[bookNum].writ_cnt.ToString() + "];"));

            return writer;
        }
        private TextWriter XInitialize(ORDER id, string inializerVar)
        {
            var outname = BOM.GetC_Type(id);

            TextWriter writer = File.CreateText(Path.Combine(this.output, outname + ".cpp"));
            writer.WriteLine("#include \"" + outname + ".h\"");
            writer.Write(inializerVar + " = {");

            return writer;
        }
        private void XBook(string inializerVar)
        {
            var bom = BOM.Inventory[BOM.Book];

            TextWriter writer = XInitialize(ORDER.Book, inializerVar);

            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.Book, release: BOM.Z_31));
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                byte bookNum = 0;
                for (int x = 0; x <= 66; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    bookNum        = breader.ReadByte();
                    var chapterCnt = breader.ReadByte();
                    var chapterIdx = breader.ReadUInt16();
                    var verseCnt   = breader.ReadUInt16();
                    var verseIdx   = breader.ReadUInt16();
                    var writCnt    = breader.ReadUInt32();
                    var writIdx    = breader.ReadUInt32();
                    var bname      = breader.ReadBytes(16);
                    var babbr      = breader.ReadBytes(18);
                    if (x != bookNum)
                        break;

                    this.BookIndex[bookNum].chapter_cnt = chapterCnt;
                    this.BookIndex[bookNum].chapter_idx = chapterIdx;
                    this.BookIndex[bookNum].verse_cnt = verseCnt;
                    this.BookIndex[bookNum].verse_idx = verseIdx;
                    this.BookIndex[bookNum].writ_cnt = writCnt;
                    this.BookIndex[bookNum].writ_idx = writIdx;
                    var name = new StringBuilder();
                    var abbr = new StringBuilder();

                    for (int i = 0; i < bname.Length && bname[i] != 0; i++)
                        name.Append((char)bname[i]);

                    writer.Write("\t{ ");

                    writer.Write(Pad(bookNum, 2) + ", ");
                    writer.Write(Pad(chapterCnt, 3) + ", ");
                    writer.Write(Pad(chapterIdx, 4) + ", ");
                    writer.Write(Pad(verseCnt, 5) + ", ");
                    writer.Write(Pad(verseIdx, 6) + ", ");
                    writer.Write(Pad(writCnt, 7) + ", ");
                    writer.Write(Pad(writIdx, 8) + ", ");
                    writer.Write("\"" + name.ToString() + "\", ");

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

                        writer.Write("\"" + abbreviations.a2 + "\", ");
                        writer.Write("\"" + abbreviations.a3 + "\", ");
                        writer.Write("\"" + abbreviations.a4 + "\", ");
                        writer.Write("\"" + alt1 + "\", ");
                        writer.Write("\"" + alt2 + "\", ");
                    }
                    else
                    {
                        writer.Write("\"\", \"\", \"\", \"\", \"\"");
                    }
                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private UInt32 NormalizeWritIdx(UInt32 writIdx)
        {
            UInt32 bk = 0;
            for (byte n = 1; n <= 66; n++)
            {
                if (writIdx >= BookIndex[n].writ_idx)
                    bk = n;
                else
                    break;
            }
            return writIdx - BookIndex[bk].writ_idx; ;
        }
        private void XChapter(string inializerVar)
        {
            var bom = BOM.Inventory[BOM.Chapter];

            TextWriter writer = XInitialize(ORDER.Chapter, inializerVar);
            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.Chapter, release: BOM.Z_31));

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var writIdx = breader.ReadUInt32();
                    var writCnt = breader.ReadUInt16();
                    var verseIdx = breader.ReadUInt16();
                    var verseCnt = breader.ReadUInt16();

                    writer.Write("\t{ ");

                    writer.Write(Pad(NormalizeWritIdx(writIdx), 5) + ", ");
                    writer.Write(Pad(writCnt, 4) + ", ");
                    writer.Write(Pad(verseIdx, 5) + ", ");
                    writer.Write(Pad(verseCnt, 4));

                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XVerse(string inializerVar)
        {
            var bom = BOM.Inventory[BOM.Verse];

            TextWriter writer = XInitialize(ORDER.UNDEFINED, inializerVar);
            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.UNDEFINED));

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var book = breader.ReadByte();
                    var chapter = breader.ReadByte();
                    var verse = breader.ReadByte();
                    var wordCnt = breader.ReadByte();

                    writer.Write("\t{ ");

                    writer.Write(Pad(book, 2) + ", ");
                    writer.Write(Pad(chapter, 3) + ", ");
                    writer.Write(Pad(verse, 3) + ", ");
                    writer.Write(Pad(wordCnt, 3));
                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XLemma(string inializerVar)
        {
            var bom = BOM.Inventory[BOM.Lemmata];

            TextWriter writer = XInitialize(ORDER.Lemmata, inializerVar);
            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.Lemmata));

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var pos = breader.ReadUInt32();
                    var wordKey = breader.ReadUInt16();
                    var wordClass = breader.ReadUInt16();
                    var lemmaCount = breader.ReadUInt16();

                    writer.Write("\t{ ");

                    writer.Write("0x" + pos.ToString("X08") + ", ");
                    writer.Write("0x" + wordKey.ToString("X04") + ", ");
                    writer.Write("0x" + wordClass.ToString("X04"));

                    int i;
                    for (i = 0; i < lemmaCount; i++)
                    {
                        var lemma = breader.ReadUInt16();
                        writer.Write(", 0x" + lemma.ToString("X04"));
                    }
                    for (/**/; i < 3; i++)
                    {
                        writer.Write(", 0x0000");
                    }
                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XLemmaOOV(string inializerVar)
        {
            var bom = BOM.Inventory[BOM.OOV];

            TextWriter writer = XInitialize(ORDER.OOV, inializerVar);
            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.OOV));

            var buffer = new char[24];

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var oovKey = breader.ReadUInt16();

                    int i = 0;
                    byte c = 0;
                    for (c = breader.ReadByte(); c != 0 && i < 24; c = breader.ReadByte())
                        buffer[i++] = (char)c;
                    buffer[i] = '\0';
                    if (c != 0) for (c = breader.ReadByte(); c != 0; c = breader.ReadByte()) // discard ... this should not happen ... check in debugger
                        Console.WriteLine("Bad stuff!!!");

                    var oovString = new string(buffer, 0, i);

                    writer.Write("\t{ ");

                    writer.Write("0x" + oovKey.ToString("X") + ", ");
                    writer.Write("\"" + oovString + "\"");

                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XNames(string inializerVar)
        {
            var bom = BOM.Inventory[BOM.Names];

            TextWriter writer = XInitialize(ORDER.Names, inializerVar);
            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.Names));

            var buffer = new char[24];
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.recordCount; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var wkey = breader.ReadUInt16();
                    var meanings = AVXManager.ReadByteString(breader, maxLen: 4096);
                    var meaningArray = meanings.Split('|', StringSplitOptions.RemoveEmptyEntries);

                    writer.Write("\t{ ");

                    writer.Write("0x" + wkey.ToString("X04") + ", ");
                    writer.Write(meaningArray.Length.ToString() + ", ");

                    string seperator = "{ ";
                    foreach (var meaning in meaningArray)
                    {
                        writer.Write(seperator + "\"" + meaning + "\"");
                        seperator = ", ";
                    }
                    writer.Write(" }");
                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XLexicon(string inializerVar)
        {
            var bom = BOM.Inventory[BOM.Lexicon];

            TextWriter writer = XInitialize(ORDER.Lexicon, inializerVar);
            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.Lexicon));

            var buffer = new char[24];
            UInt32 recordCount;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 0; x < bom.recordCount; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var entities = breader.ReadUInt16();
                    int posCnt = breader.ReadUInt16();

                    UInt32[] pos = new UInt32[posCnt];
                    for (int p = 0; p < posCnt; p++)
                    {
                        pos[p] = breader.ReadUInt32();

                        if (x == 0)
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

                    var search =  AVXManager.ReadByteString(breader);
                    var display = AVXManager.ReadByteString(breader);
                    var modern =  AVXManager.ReadByteString(breader);

                    writer.Write("\t{ ");

                    writer.Write("0x" + entities.ToString("X04") + ", ");
 //                 writer.Write(posCnt.ToString() + ", ");     // C++ code uses std::vector. Therfore, count is not needed as a seperate field

                    string seperator = "{ ";
                    foreach (var p in pos)
                    {
                        writer.Write(seperator + "0x" + p.ToString("X08"));
                        seperator = ", ";
                    }
                    if (seperator == "{ ")
                    {
                        writer.Write(seperator + "0x" + 0.ToString("X08"));
                        seperator = ", ";
                    }
                    writer.Write(" }, ");

                    if (!string.IsNullOrEmpty(search))
                        writer.Write("\"" + search + "\", ");
                    else
                        writer.Write("nullptr, ");

                    if (!string.IsNullOrEmpty(display))
                        writer.Write("\"" + display + "\", ");
                    else
                        writer.Write("nullptr, ");

                    if (!string.IsNullOrEmpty(modern))
                        writer.Write("\"" + modern + "\"");
                    else
                        writer.Write("nullptr");

                    writer.Write(" }");
                }
                writer.WriteLine("\n};");
                writer.Close();
            }
        }
        private void XWrit(string inializerVar, string memberVar)
        {
            var file = BOM.GetZ_Path(ORDER.Written, release: BOM.Z_31);
            var fstream = new StreamReader(file);

            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (byte n = 1; n <= 66; n++)
                {
                    string delimiter = "\n";

                    TextWriter writer = XInitializeWrit(inializerVar, memberVar, n);
                    for (UInt32 w = 0; w < BookIndex[n].writ_cnt; w++)
                    {
                        writer.Write(delimiter);
                        if (delimiter.Length < 2)
                            delimiter = ",\n";

                        var Strongs  = breader.ReadUInt64();
                        var VerseIdx = breader.ReadUInt16();
                        var Word     = breader.ReadUInt16();
                        var Punc     = breader.ReadByte();
                        var Trans    = breader.ReadByte();
                        var Pnwc     = breader.ReadUInt16();
                        var Pos      = breader.ReadUInt32();
                        var Lemma    = breader.ReadUInt16();

                        writer.Write("\t{ ");

                        int shift = 3*16;
                        for (int index = 0; index < 4; shift -= 16, index++)
                        {
                            var strongs = (UInt16)((Strongs >> shift) & 0xFFFF);
                            writer.Write(Pad(strongs, 4) + ", ");
                        }
                        writer.Write(Pad(VerseIdx, 5) + ", ");
                        writer.Write("0x" + Word.ToString("X04") + ", ");
                        writer.Write("0x" + Punc.ToString("X02") + ", ");
                        writer.Write("0x" + Trans.ToString("X02") + ", ");
                        writer.Write("0x" + Pnwc.ToString("X04") + ", ");
                        writer.Write("0x" + Pos.ToString("X08") + ", ");
                        writer.Write("0x" + Lemma.ToString("X04"));

                        writer.Write(" }");
                    }
                    writer.WriteLine("\n};");
                    writer.Close();
                }
            }
        }
    }
}
