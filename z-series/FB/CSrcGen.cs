using AVX.FlatBuf;
using DigitalAV.Migration;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SerializeFromSDK
{
    public class CSrcGen
    {
        private string output;
        private string sdk;
        private Dictionary<string, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize)> inventory;

        public CSrcGen(string sdk, string src, Dictionary<string, (string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize)> inventory)
        {
            this.inventory= inventory;
            this.sdk = sdk;
            this.output = src;
        }
        public bool Generate()
        {
            Console.WriteLine("Create source initializers for C.");
            foreach (var item in inventory.Keys)
            {
                var record = inventory[item];
                var select = Path.GetFileNameWithoutExtension(item).Substring(3);

                switch (select)
                {
                    case "Book":        this.XBook(     record, "AVXBookIndex::AVXBook index[]");           break;
                    case "Chapter":     this.XChapter(  record, "AVXChapterIndex::AVXChapter index[]");     break;
                    case "Verse":       this.XVerse(    record, "AVXVerseIndex::AVXVerse index[]");         break;
                    case "Lemma":       this.XLemma(    record, "AVXLemmataRecords::AVXLemmata records[]"); break;
                    case "Lemma-OOV":   this.XLemmaOOV( record, "AVXLemmataOOV::AVXLemmaOOV vocabulary[]"); break;
                    case "Lexicon":     this.XLexicon(  record, "AVXLexicon::AVXLexItem items[]");          break;
                    case "Names":       this.XNames(    record, "AVXNames::AVXName names[]");               break;
                    case "WordClass":   this.XWordClass(record, "AVXWordClasses::AVXWordClass classes[]");  break;
                    case "Writ":        this.XWrit(     record, "AVXWritten::AVXWrit written[]");           break;
                }
            }
            return true;
        }
        private string Pad<T>(T num, int width)
        {
            var val = num != null ? num.ToString() : "";
            var fmt = "{0," + width.ToString() + "}";
            return string.Format(fmt, val);
        }

        private TextWriter XInitialize(string otype, string inializerVar)
        {
            var outname = otype.Replace('-', '_').ToLower();

            TextWriter writer = File.CreateText(Path.Combine(this.output, outname + ".cpp"));
            writer.WriteLine("#include \"" + outname + ".h\"");
            writer.Write("static const " + inializerVar + " = {");

            return writer;
        }
        private void XBook((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var fstream = new StreamReader(bom.fpath);
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
                    var babbr      = breader.ReadBytes(12);

                    var name = new StringBuilder();
                    var abbr = new StringBuilder();

                    for (int i = 0; i < bname.Length && bname[i] != 0; i++)
                        name.Append((char)bname[i]);

//                  Z14-style field
//                  for (int i = 0; i < babbr.Length && babbr[i] != 0; i++)
//                      abbr.Append((char)babbr[i]);
//
//                  var abbreviations = abbr.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);

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
                        var abbreviations = RustSrcGen.BK[name.ToString()];
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
                        writer.Write(alt1.Length > 0 ? "\"" + alt1 + "\", " : "nullptr, ");
                        writer.Write(alt2.Length > 0 ? "\"" + alt2 + "\""   : "nullptr");
                    }
                    else
                    {
                        writer.Write("nullptr, nullptr, nullptr, nullptr, nullptr");
                    }
                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XChapter((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var writIdx = breader.ReadUInt32();
                    var writCnt = breader.ReadUInt16();
                    var verseIdx = breader.ReadUInt16();
                    var verseCnt = breader.ReadUInt16();

                    writer.Write("\t{ ");

                    writer.Write(Pad(writIdx, 6) + ", ");
                    writer.Write(Pad(writCnt, 4) + ", ");
                    writer.Write(Pad(verseIdx, 5) + ", ");
                    writer.Write(Pad(verseCnt, 4));

                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XVerse((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.rcnt; x++)
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
        private void XLemma((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

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
        private void XLemmaOOV((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.rcnt; x++)
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
        private void XNames((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var wkey = breader.ReadUInt16();
                    var meanings = ConsoleApp.ReadByteString(breader, maxLen: 4096);
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

        private void XLexicon((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            UInt32 recordCount;
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 0; x < bom.rcnt; x++)
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

                    var search =  ConsoleApp.ReadByteString(breader);
                    var display = ConsoleApp.ReadByteString(breader);
                    var modern =  ConsoleApp.ReadByteString(breader);

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
        private void XWordClass((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var buffer = new char[24];
            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var wclass = breader.ReadUInt16();
                    var posCnt = breader.ReadUInt16();

                    writer.Write("\t{ ");

                    writer.Write("0x" + wclass.ToString("X04") + ", ");
                    writer.Write(posCnt.ToString() + ", ");

                    string seperator = "{ ";
                    for (int i = 0; i < posCnt; i++)
                    {
                        var pos = breader.ReadUInt32();
                        writer.Write(seperator + "0x" + pos.ToString("X08"));
                        seperator = ", ";
                    }
                    writer.Write(" }");
                    writer.Write(" }");
                }
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
        private void XWrit((string md5, string fpath, string otype, UInt32 rlen, UInt32 rcnt, UInt32 fsize) bom, string inializerVar)
        {
            TextWriter writer = XInitialize(bom.otype, inializerVar);

            var fstream = new StreamReader(bom.fpath);
            using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                string delimiter = "\n";
                for (int x = 1; x <= bom.rcnt; x++)
                {
                    writer.Write(delimiter);
                    if (delimiter.Length < 2)
                        delimiter = ",\n";

                    var Strongs = breader.ReadUInt64();
                    var VerseIdx = breader.ReadUInt16();
                    var Word = breader.ReadUInt16();
                    var Punc = breader.ReadByte();
                    var Trans = breader.ReadByte();
                    var Pnwc = breader.ReadUInt16();
                    var Pos = breader.ReadUInt32();
                    var Lemma = breader.ReadUInt16();

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
            }
            writer.WriteLine("\n};");
            writer.Close();
        }
    }
}
