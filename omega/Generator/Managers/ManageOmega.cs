namespace Generator
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization;
    using Blueprint.Blue;
    using BlueprintBlue.Model;
    using YamlDotNet.Core;
    using System.Runtime.CompilerServices;

    public class ManageOmega
    {
        private TextWriter? bomOmega_MD5;
        private TextWriter? bomOmega;

        private TextReader? yamlLexiconUpdates;
        private TextWriter? yamlLexicon;
        private TextWriter? yamlLexiconOriginal;

        private YamlLexicon oldlex;
        private YamlLexicon newlex;
        private List<LexRecord> updatelex;

        // This was used to add NUPhone to the 3911 release // It is not needed for 4xyz releases
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
        private BinaryReader? phoneticReader;  
#endif
        private BinaryReader? sdkReader;
        private BinaryWriter? newWriter;
        private UInt64 version;
        private char[] VERSION;
        private string SDK;
        
        public ManageOmega(string baseSDK, string newSDK)
        {
            this.bomOmega_MD5 = AVXManager.OpenTextWriter("AVX-Omega" + newSDK, ".md5");
            this.bomOmega = AVXManager.OpenTextWriter("AVX-Omega" + newSDK, ".txt");

            this.yamlLexiconUpdates = AVXManager.OpenTextReader("AV-Lexicon" + "-Updates", ".yml");
            this.yamlLexiconOriginal = AVXManager.OpenTextWriter("AV-Lexicon" + baseSDK, ".yml");
            this.yamlLexicon = AVXManager.OpenTextWriter("AV-Lexicon" + newSDK, ".yml");

            this.oldlex = new();
            this.newlex = new();
            this.updatelex = ManageOmega.ImportLexicon(this.yamlLexiconUpdates);

            this.sdkReader = AVXManager.OpenBinaryReader("AVX-Omega" + baseSDK);
            var sdk = AVXManager.CreateSDK("AVX-Omega" + newSDK);
            this.newWriter = sdk.writer;
            this.SDK = sdk.path;
            this.VERSION = new char[4];

            // This was used to add NUPhone to the 3911 release // It is not needed for subsequent releases
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
            this.phoneticReader = AVXManager.OpenBinaryReader("AV-Phonetics", ".binary");
#endif
            version = 0;
            string str = newSDK.ToUpper();
            for (int i = 1; i < str.Length; i++)
            {
                char c = str[i];
                byte val = (byte) c;
                int digit =  c >= (byte)'0' && c <= (byte)'9' ? val - (byte)'0' : c >= (byte)'A' && c <= (byte)'F' ? (val - (byte)'A') + 10 : 0;
                version *= 0x10;
                version += (byte) digit;
                if (i-1 < VERSION.Length)
                    VERSION[i-1] = c;
            }
        }
        public void CloseAllButMD5()
        {
            if (this.bomOmega != null)
                this.bomOmega.Close();

            if (this.sdkReader != null)
                this.sdkReader.Close();
            if (this.newWriter != null)
                this.newWriter.Close();

            if (this.yamlLexiconUpdates != null)
                this.yamlLexiconUpdates.Close();
            if (this.yamlLexicon != null)
                this.yamlLexicon.Close();
            if (this.yamlLexiconOriginal != null)
                this.yamlLexiconOriginal.Close();

            this.bomOmega = null;
            this.sdkReader = null;
            this.newWriter = null;
            this.yamlLexicon = null;
            this.yamlLexiconOriginal = null;
            this.yamlLexiconUpdates = null;

            // This was used to add NUPhone to the 3911 release // It is not needed for subsequent releases
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
            if (this.phoneticReader != null)
                this.phoneticReader.Close();
            this.newWriter = null;
#endif
        }
        public void CloseMD5()
        {
            if (this.bomOmega_MD5 != null)
                this.bomOmega_MD5.Close();
        }
        private static char[] whitespace = new char[] { ' ', '\t' };

        // This was used to add NUPhone to the 3911 release // It is not needed for 4xyz releases
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
        private void UpdatePhoneticsBOM()
        {
            if (this.phoneticReader != null)
            {
                this.phoneticReader.BaseStream.Seek(0, SeekOrigin.Begin);

                var label = "Phonetic".ToArray();
                var bom = BOM.Inventory[BOM.Phonetics];
                for (int i = 0; i < label.Length; i++)
                    bom.label[i] = (byte)label[i];
                for (int i = label.Length; i < bom.label.Length; i++)
                    bom.label[i] = (byte)0;
                bom.recordLength = 0;
                bom.recordCount = BOM.Inventory[BOM.OOV].recordCount + BOM.Inventory[BOM.Lexicon].recordCount - 1; // subtract 1 for zeroth-entry in lexicon
                bom.offset = (UInt32)(BOM.Inventory[(byte)(BOM.Phonetics - 1)].offset + BOM.Inventory[(byte)(BOM.Phonetics - 1)].length);
                UInt32 len = 0;

                for (int x = 1; x <= bom.recordCount; x++)
                {
                    var wkey = this.phoneticReader.ReadUInt16();
                    len += 2;

                    for (byte c = this.phoneticReader.ReadByte(); c != 0; c = this.phoneticReader.ReadByte())
                    {
                        len++;
                    }
                    len++;
                }
                bom.length = len;

                this.phoneticReader.BaseStream.Seek(0, SeekOrigin.Begin);
                bom.hash = this.CalculateMD5(this.phoneticReader.ReadBytes((int)len));
            }
        }
#endif
        private void WriteBinaryBOM(TOC bom)
        {
            if (this.newWriter != null)
            {
                this.newWriter.Write(bom.label);
                this.newWriter.Write(bom.offset);
                this.newWriter.Write(bom.length);
                this.newWriter.Write(bom.recordLength);
                this.newWriter.Write(bom.recordCount);
                this.newWriter.Write(bom.hash[0]);
                this.newWriter.Write(bom.hash[1]);
             }
        }
        private void WriteTextBOM(TOC bom)
        {
            if (this.bomOmega != null)
            {
                if (bom.label != null)
                {
                    int i = 0;
                    for (/**/; i < bom.label.Length; i++)
                        if (bom.label[i] != 0)
                            this.bomOmega.Write((char)bom.label[i]);
                        else
                            break;
                    for (/**/; i < 16; i++)
                        this.bomOmega.Write(' ');
                    this.bomOmega.Write("\t");
                }
                else
                {
                    for (int i = 0; i < 16; i++)
                        this.bomOmega.Write('-');
                    this.bomOmega.Write("\t");
                }
                this.bomOmega.Write(AVXManager.PadLeft(bom.offset.ToString(), 8)); this.bomOmega.Write("\t");
                this.bomOmega.Write(AVXManager.PadLeft(bom.length.ToString(), 8)); this.bomOmega.Write("\t");
                this.bomOmega.Write(AVXManager.PadLeft(bom.recordLength.ToString(), 8)); this.bomOmega.Write("\t");
                this.bomOmega.Write(AVXManager.PadLeft(bom.recordCount.ToString(), 8)); this.bomOmega.Write("\t");
                this.bomOmega.Write(bom.hash[0].ToString("X16") + bom.hash[1].ToString("X16"));
                this.bomOmega.Write("\n");
            }
        }
        private void RewriteDirectory()
        {
            if (this.newWriter != null)
            {
                this.newWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                for (byte idx = 0; idx <= BOM.Phonetics; idx++)
                {
                    TOC toc = BOM.Inventory[idx];

                    if (idx == BOM.DIRECTORY)
                    {
                        toc.hash[0] = 0;
                        toc.hash[1] = this.version;
                        toc.recordCount = (UInt32)(BOM.Phonetics + 1);
                        toc.length = (UInt32)(toc.recordCount * toc.recordLength);
                    }
                    else
                    {
                        byte prev = (byte)(idx - 1);
                        toc.offset = (UInt32)(BOM.Inventory[prev].offset + BOM.Inventory[prev].length);
                    }
                    WriteBinaryBOM(toc);
                }
            }
        }
        private void CloneDirectory()
        {
            if (this.sdkReader != null && this.newWriter != null)
            {
                TOC bom;
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
                for (byte idx = 0; idx < BOM.Phonetics; idx++)
#else
                for (byte idx = 0; idx <= BOM.Phonetics; idx++)
#endif
                {
                    bom = BOM.Inventory[idx];
                    bom.hash = new UInt64[2];

                    bom.label = this.sdkReader.ReadBytes(16);
                    bom.originalOffset = this.sdkReader.ReadUInt32();
                    bom.offset = 0;
                    bom.length = this.sdkReader.ReadUInt32();
                    bom.recordLength = this.sdkReader.ReadUInt32();
                    bom.recordCount = this.sdkReader.ReadUInt32();
                    bom.hash[0] = this.sdkReader.ReadUInt64();
                    bom.hash[1] = this.sdkReader.ReadUInt64();

                    if (idx == BOM.DIRECTORY)
                    {
                        bom.hash[0] = 0;
                        bom.hash[1] = this.version;
                        bom.recordCount = (UInt32)(BOM.Phonetics + 1);
                        bom.length = (UInt32)(bom.recordCount * bom.recordLength);
                    }
                    else
                    {
                        byte prev = (byte)(idx - 1);
                        bom.offset = (UInt32) (BOM.Inventory[prev].offset + BOM.Inventory[prev].length);
                    }
                    WriteBinaryBOM(bom);
                }
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
                this.UpdatePhoneticsBOM();
                bom = BOM.Inventory[BOM.Phonetics];
                WriteBinaryBOM(bom);
#endif
            }
        }
        private bool WriteBulkData(TOC toc, byte[] data)
        {
            RecalculateMD5(toc, data);
            bool ok = (this.newWriter != null);

            if (ok)
            {
                toc.length = (UInt32) data.Length;
                this.newWriter.Write(data);
            }
            return ok;
        }
        private void RecalculateMD5(TOC toc, byte[] data)
        {
            toc.hash = CalculateMD5(data);
        }

        public static List<LexRecord> ImportLexicon(TextReader? yaml)
        {
            if (yaml == null)
                return new();

            string text = yaml.ReadToEnd();
            var deserializer = new DeserializerBuilder().Build();
            List<LexRecord> lex = deserializer.Deserialize<List<LexRecord>>(text);

            return lex;
        }

        private void ProcessLexicon(YamlLexicon lex)
        {
            if (this.sdkReader != null && this.newWriter != null)
            {
                var bom = BOM.Inventory[BOM.Lexicon];
                this.sdkReader.BaseStream.Seek((int)bom.originalOffset, SeekOrigin.Begin);

                for (UInt32 i = 0; i < bom.recordCount; i++)
                {
                    UInt16 entities = this.sdkReader.ReadUInt16();
                    UInt16 size     = this.sdkReader.ReadUInt16();
                    UInt32[] pos = new UInt32[size];
                    for (int p = 0; p < size; p++)
                        pos[p] = this.sdkReader.ReadUInt32();
                    string[] text = new string[3];
                    StringBuilder sb = new StringBuilder(16);
                    for (int t = 0; t < 3; t++)
                    {
                        sb.Clear();
                        for (char c = this.sdkReader.ReadChar(); c != 0; c = this.sdkReader.ReadChar())
                            sb.Append(c);
                        text[t] = sb.ToString();
                    }
                    lex.Add((UInt16)i, entities, pos, text);
                }
            }
            if (this.yamlLexiconOriginal != null)
                lex.WriteAll(this.yamlLexiconOriginal);
        }
        private void FixBadModernTranslations()
        {
//          byte[] art = new byte[] {   44,   17,   29,    4 }; // coordinates[] B C V W Acts 17:29 (4th to last word)
//          byte[] art = new byte[] { 0x2C, 0x11, 0x1D, 0x04 }; // coordinates[] B C V W Acts 17:29 (4th to last word)
            UInt32 coord_of_art_as_a_noun = (UInt32) 0x_04_1D_11_2C;

            UInt16 noun12 = 0x0010;
            UInt32 noun32 = 0;


            if (this.sdkReader != null && this.newWriter != null)
            {
                using (var mem = new MemoryStream())
                {
                    var mwriter = new BinaryWriter(mem, Encoding.UTF8);

                    var bom = BOM.Inventory[BOM.Written];
                    this.sdkReader.BaseStream.Seek((int)bom.originalOffset, SeekOrigin.Begin);

                    for (UInt32 w = 0; w < bom.recordCount; w++)
                    {
                        int len = 0;
                        var strongs = this.sdkReader.ReadUInt64(); len += 8;
                        var bcvw = this.sdkReader.ReadUInt32(); len += 4;
                        var wkey = this.sdkReader.ReadUInt16(); len += 2;
                        var punc = this.sdkReader.ReadByte(); len ++;
                        var tran = this.sdkReader.ReadByte(); len ++;
                        var pnPos12 = this.sdkReader.ReadUInt16(); len += 2;
                        var pos32 = this.sdkReader.ReadUInt32(); len += 4;
                        var lemma = this.sdkReader.ReadUInt16(); len += 2;

                        if ((lemma & 0x4000) != 0) // this was a legacy usage of this bit and no longer applies
                        {
                            lemma ^= 0x4000;
                        }

                        if (noun32 == 0 && pnPos12 == noun12)
                        {
                            noun32 = pos32;
                        }
                        if (bcvw == coord_of_art_as_a_noun)
                        {
                            lemma |= 0x4000; // modernization-squelch-bit ("art" here should *not* be modernized into "are")
                            pnPos12 = noun12;
                            pos32 = noun32;
                        }

                        if (len != 24)
                        {
                            break;
                        }
                        mwriter.Write(strongs);
                        mwriter.Write(bcvw);
                        mwriter.Write(wkey);
                        mwriter.Write(punc);
                        mwriter.Write(tran);
                        mwriter.Write(pnPos12);
                        mwriter.Write(pos32);
                        mwriter.Write(lemma);
                    }
                    var mreader = new BinaryReader(mwriter.BaseStream, Encoding.UTF8);
                    mreader.BaseStream.Seek(0, SeekOrigin.Begin);
                    var data = mreader.ReadBytes((int)bom.length);
                    this.WriteBulkData(bom, data);                   
                    mreader.Close();
                }
            }
        }
        private void FixBookArtifactWithCurrentVersion()
        {
            if (this.sdkReader != null && this.newWriter != null)
            {
                using (var mem = new MemoryStream())
                {
                    var mwriter = new BinaryWriter(mem, Encoding.UTF8);

                    (UInt32 idx, UInt16 cnt) previous = (0, 0);
                    var bom = BOM.Inventory[BOM.Book];
                    this.sdkReader.BaseStream.Seek((int)bom.originalOffset, SeekOrigin.Begin);

                    for (byte b = 0; b < bom.recordCount; b++)
                    {
                        int len = 0;
                        var bookNum = this.sdkReader.ReadByte(); len++;
                        var chapterCnt = this.sdkReader.ReadByte(); len++;
                        var chapterIdx = this.sdkReader.ReadUInt16(); len += 2;
                        var writCnt = this.sdkReader.ReadUInt16(); len += 2;
                        var writIdx = this.sdkReader.ReadUInt32(); len += 4;

                        var name = this.sdkReader.ReadBytes(16); len += name.Length;
                        var abbr = this.sdkReader.ReadBytes(22); len += abbr.Length;

                        if (len != 48)
                        {
                            break;
                        }
                        if (b == 0)
                        {
                            writIdx = (UInt32)this.version;
                            for (int i = "Omega ".Length; i < name.Length; i++) // map version to new one; e.g. 3.9.11 => 4.2.17
                            {
                                if (this.VERSION.Length == 4)
                                {
                                    switch (i)
                                    {
                                        case 0: name[i] = (byte)this.VERSION[0]; break;
                                        case 2: name[i] = (byte)this.VERSION[1]; break;
                                        case 4: name[i] = (byte)this.VERSION[2]; break;
                                        case 5: name[i] = (byte)this.VERSION[3]; break;
                                    }
                                }
                                else
                                {
                                    switch (i)
                                    {
                                        case 0: name[i] = (byte)'X'; break;
                                        case 2: name[i] = (byte)'X'; break;
                                        case 4: name[i] = (byte)'X'; break;
                                        case 5: name[i] = (byte)'X'; break;
                                    }                                }
                                if (i >= 5)
                                    break;
                            }
                            char major = this.VERSION.Length >= 1 ? this.VERSION[0] : 'X';
                            char minor = this.VERSION.Length >= 2 ? this.VERSION[1] : 'X';
                            int cnt = 0;
                            int two = 0;
                            for (int i = 0; i < abbr.Length; i++) // 35 => 39
                            {
                                if (two == 2)
                                {
                                    two = 0;
                                    cnt++;
                                }
                                if (cnt == 3)
                                {
                                    break;
                                }
                                byte c = abbr[i];

                                // if it's a numeric, it needs to be replaced
                                if ( (c >= (byte)'0' && c <= (byte)'9') 
                                ||   (c >= (byte)'A' && c <= (byte)'F')
                                ||   (c >= (byte)'a' && c <= (byte)'f') )
                                {
                                    switch(two)
                                    {
                                        case 0: abbr[i] = (byte) major; break;
                                        case 1: abbr[i] = (byte) minor; break;
                                    }
                                    two++;
                                }
                            }
                        }
                        else if (b == 1)
                        {
                            previous = (0, writCnt);
                            writIdx = 0;
                        }
                        else
                        {
                            writIdx = previous.idx + previous.cnt;
                            previous = (writIdx, writCnt);
                        }
                        mwriter.Write(bookNum);
                        mwriter.Write(chapterCnt);
                        mwriter.Write(chapterIdx);
                        mwriter.Write(writCnt);
                        mwriter.Write(writIdx);
                        mwriter.Write(name);
                        mwriter.Write(abbr);
                    }
                    var mreader = new BinaryReader(mwriter.BaseStream, Encoding.UTF8);
                    mreader.BaseStream.Seek(0, SeekOrigin.Begin);
                    var data = mreader.ReadBytes((int)bom.length);
                    this.WriteBulkData(bom, data);
                    mreader.Close();
                }
            }
        }
        private void ExportLexiconAsYaml(TextWriter? writer)
        {
            if (writer != null)
            {
            }
        }
        private void UpdateLexiconFromYaml()
        {
            ;
        }
        private void CreateExistingArtifact(byte order)
        {
            if (this.sdkReader != null && this.newWriter != null)
            {
                var bom = BOM.Inventory[(byte)order];
                this.sdkReader.BaseStream.Seek((int)bom.originalOffset, SeekOrigin.Begin);
                var bytes = this.sdkReader.ReadBytes((int)bom.length);

                this.newWriter.Write(bytes);
            }
        }
        private void CreateNewArtifact(byte order, BinaryReader? reader)
        {
            if (reader != null && this.newWriter != null)
            {
                var bom = BOM.Inventory[(byte)order];
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                var bytes = reader.ReadBytes((int)bom.length);

                this.newWriter.Write(bytes);
            }
        }
        public void Manage()
        {
            this.CloneDirectory();
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
            for (byte idx = 1; idx < BOM.Phonetics; idx++)
#else
            for (byte idx = 1; idx <= BOM.Phonetics; idx++)
#endif
            {
#if BASELINE_IS_EARLIER_THAN_3911
                if (idx == BOM.Book)
                    this.FixBookArtifact();
                else
#endif
                if (idx == BOM.Book)
                {
                    this.FixBookArtifactWithCurrentVersion();
                }
                else if (idx == BOM.Written)
                {
                    this.FixBadModernTranslations();
                }
                else if (idx == BOM.Lexicon)
                {
                    this.ProcessLexicon(this.oldlex);
                    this.newlex.Clone(this.oldlex);
                    this.newlex.Replace(this.updatelex);
                    this.newlex.WriteAll(this.yamlLexicon);
                    this.CreateExistingArtifact(idx);
                }
                else
                {
                    this.CreateExistingArtifact(idx);
                }
            }
            // This was used to add NUPhone to the 3911 release // It is not needed for 4xyz releases
#if REQUIRES_EXTERNAL_NUPHONE_BINARY
            this.CreateNewArtifact(BOM.Phonetics, this.phoneticReader);
#endif
            for (byte idx = 0; idx <= BOM.Phonetics; idx++)
            {
                var bom = BOM.Inventory[idx];
                this.WriteTextBOM(bom);
            }

            this.RewriteDirectory();
            this.CloseAllButMD5();

            if (this.bomOmega_MD5 != null)
            {
                var bytes = System.IO.File.ReadAllBytes(this.SDK);
                var pair = CalculateMD5(bytes);

                this.bomOmega_MD5.Write(pair[0].ToString("X16") + pair[1].ToString("X16"));
            }
            this.CloseMD5();

        }
        private UInt64[] CalculateMD5(byte[] bytes)
        {
            UInt64[] result = new ulong[2] { 0, 0 };

            if (BOM.hasher != null)
            {
                var bval = BOM.hasher.ComputeHash(bytes);
                var sval = AVXManager.BytesToHex(bval);

                result[0] = UInt64.Parse(sval.Substring(0, 16), NumberStyles.HexNumber);
                result[1] = UInt64.Parse(sval.Substring(16), NumberStyles.HexNumber);
            }
            return result;
        }
        internal static string ReadByteString(BinaryReader breader, UInt16 maxLen = 24)
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
        internal static void WriteByteString(BinaryWriter bwriter, string token)
        {
            for (int i = 0; i < token.Length; i++)
            {
                bwriter.Write((byte)token[i]);
            }
            bwriter.Write((byte)0);
        }
        internal static string PadLeft(string input, int cnt, char padding = ' ')
        {
            string output = input;
            for (int len = output.Length; len < cnt; len++)
                output = padding + output;
            return output;
        }
        internal static string PadRight(string input, int cnt, char padding = ' ')
        {
            string output = input;
            for (int len = output.Length; len < cnt; len++)
                output += padding;
            return output;
        }
        internal static string BytesToHex(byte[] bytes)
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
    }
}
