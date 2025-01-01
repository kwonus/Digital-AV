namespace DigitalAV.Migration
{
    using FoundationsGenerator;
    using SerializeFromSDK;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using static System.Net.WebRequestMethods;

    public class ManageOmega : IInventoryManager
    {
        private TextWriter bomOmega_MD5;
        private TextWriter bomOmega;
        private TextReader bomZ32;

        internal ManageOmega() // Do not modify ... the new Omega generator is in the omega folder (sibling of z-series folder)
        {
            this.bomOmega_MD5 = AVXManager.OpenTextWriter("-Omega-3910", ".md5", "AVX"); // Do not modify
            this.bomOmega = AVXManager.OpenTextWriter("-Omega-3910", ".txt", "AVX");     // Do not modify
            this.bomZ32 = AVXManager.OpenTextReader(BOM.Z_32, ".bom");
        }
        private static char[] whitespace = new char[] { ' ', '\t' };
        private void ReadInventory()
        {
            if (this.bomZ32 != null)
            {
                for (string? line = this.bomZ32.ReadLine(); line != null; line = this.bomZ32.ReadLine())
                {
                    if (!line.StartsWith("AV-"))
                        continue;
                    string[] fields = line.Split(whitespace, StringSplitOptions.RemoveEmptyEntries);

                    FoundationsGenerator.Directory? bom = null;
                    if (fields.Length == 5)
                    {
                        string label = fields[0].Substring("AV-".Length);

                        if (label.StartsWith("Book"))
                            bom = BOM.Inventory[BOM.Book];
                        else if (label.StartsWith("Chapter"))
                            bom = BOM.Inventory[BOM.Chapter];
                        else if (label.StartsWith("Lexicon"))
                            bom = BOM.Inventory[BOM.Lexicon];
                        else if (label.StartsWith("Names"))
                            bom = BOM.Inventory[BOM.Names];
                        else if (label.Contains("OOV"))
                            bom = BOM.Inventory[BOM.OOV];
                        else if (label.Contains("Lemma"))
                            bom = BOM.Inventory[BOM.Lemmata];
                        else if (label.Contains("Phonetics"))
                            bom = BOM.Inventory[BOM.Phonetics];
                    }
                    if (bom != null)
                    {
                        bom.hash = fields[1];
                        bom.recordLength = UInt32.Parse(fields[2]);
                        bom.recordCount = UInt32.Parse(fields[3]);
                        bom.length = UInt32.Parse(fields[4]);
                    }
                }
            }
        }
        private void UpdatePhoneticsBOM()
        {
            var fstream = new StreamReader(BOM.GetZ_Path(ORDER.Phonetics));

            var bom = BOM.Inventory[BOM.Phonetics];
            bom.recordLength = 0;
            bom.recordCount = BOM.Inventory[BOM.OOV].recordCount + BOM.Inventory[BOM.Lexicon].recordCount - 1; // subtract 1 for zeroth-entry in lexicon
            UInt32 len = 0;

            if (fstream.CurrentEncoding == Encoding.UTF8)
            {
                using (var breader = new System.IO.BinaryReader(fstream.BaseStream))
                {
                    for (int x = 1; x <= bom.recordCount; x++)
                    {
                        var wkey = breader.ReadUInt16();
                        len += 2;

                        for (byte c = breader.ReadByte(); c != 0; c = breader.ReadByte())
                        {
                            len++;
                        }
                        len++;
                    }
                }
            }
            bom.length = len;
        }
        private void AddDirectoryRecord(BinaryWriter writer, FoundationsGenerator.Directory bom)
        {
            // Phonetics bom was over-written, because it was not actually in Z SDK. Restore values here:
            if (bom.length == 0)
            {
                UpdatePhoneticsBOM();
            }

            this.bomOmega.Write(AVXManager.PadRight(bom.label, 16)); this.bomOmega.Write("\t");
            this.bomOmega.Write(AVXManager.PadLeft(bom.offset.ToString(), 8)); this.bomOmega.Write("\t");
            this.bomOmega.Write(AVXManager.PadLeft(bom.length.ToString(), 8)); this.bomOmega.Write("\t");
            this.bomOmega.Write(AVXManager.PadLeft(bom.recordLength.ToString(), 8)); this.bomOmega.Write("\t");
            this.bomOmega.Write(AVXManager.PadLeft(bom.recordCount.ToString(), 8)); this.bomOmega.Write("\t");
            this.bomOmega.Write(bom.hash); this.bomOmega.Write("\n");

            for (int i = 0; i < bom.label.Length; i++)
                writer.Write((byte)bom.label[i]);
            for (int i = bom.label.Length; i < 16; i++)
                writer.Write((byte)0);
            writer.Write(bom.offset);
            writer.Write(bom.length);
            writer.Write(bom.recordLength);
            writer.Write(bom.recordCount);

            if (bom.label == "Directory")
            {
                writer.Write((UInt64)0);
                writer.Write((UInt64)0x3201);
            }
            else if (bom.hash.Length == 32)
            {
                var left = bom.hash.Substring(0, 16);
                var right = bom.hash.Substring(16);

                var ileft = UInt64.Parse(left, System.Globalization.NumberStyles.HexNumber);
                var iright = UInt64.Parse(left, System.Globalization.NumberStyles.HexNumber);

                writer.Write(ileft);
                writer.Write(iright);
            }
            else
            {
                writer.Write((UInt64) 0);
                writer.Write((UInt64) 0);
            }
        }
        private void CreateDirectory(BinaryWriter writer)
        {
            FoundationsGenerator.Directory directory = BOM.Inventory[BOM.DIRECTORY];
            directory.hash = "00000000000000000000000000003909";
            directory.offset =  0;
            directory.recordLength = 48;
            directory.recordCount = 9;
            directory.length = (UInt32) (directory.recordLength * directory.recordCount);

            this.AddDirectoryRecord(writer, directory);

            var previous = directory;
            foreach (ORDER order in from artifact in BOM.Inventory orderby artifact.Key
                                                                   where  (artifact.Key != BOM.UNDEFINED && artifact.Key != BOM.DIRECTORY)
                                                                   select (ORDER)artifact.Key)
            {
                var id = (byte)order;
                string ifile = BOM.GetZ_Path(order, release: BOM.Z_32);
                var bytes = System.IO.File.ReadAllBytes(ifile);
                var bom = BOM.Inventory[id];
                bom.offset = previous.offset + previous.length;

                if (string.IsNullOrEmpty(bom.hash) && (BOM.hasher != null))
                {
                    var hash = BOM.hasher.ComputeHash(bytes);
                    if (hash != null)
                        bom.hash = AVXManager.BytesToHex(hash);
                }
                this.AddDirectoryRecord(writer, bom);
                previous = bom;
            }
        }
        public static Dictionary<byte, UInt32> Size { get; private set; } = new();
        private void CreateArtifact(BinaryWriter writer, ORDER order)
        {
            string file = BOM.GetZ_Path(order, release: BOM.Z_32);
            var bytes = System.IO.File.ReadAllBytes(file);
            Size[(byte)order] = (UInt32) bytes.Length;

            writer.Write(bytes);
        }
        public void Manage()
        {
            Console.WriteLine("Read Inventory from the Z32 bom");
            this.ReadInventory();

            string ofile = Path.Combine(AVXManager.SDK_BASELINE, "AVX-Omega-3910.data");
            var ostream = new StreamWriter(ofile, false, Encoding.ASCII);
            using (BinaryWriter writer = new BinaryWriter(ostream.BaseStream))
            {
                this.CreateDirectory(writer);

                Console.WriteLine("Read Existing binary content files & and upgrade outdated files");

                foreach (ORDER order in from artifact in BOM.Inventory
                                        orderby artifact.Key
                                        where (artifact.Key != BOM.UNDEFINED && artifact.Key != BOM.DIRECTORY)
                                        select (ORDER)artifact.Key)
                {
                    CreateArtifact(writer, order);
                }
                CreateArtifact(writer, ORDER.Phonetics);
            }
            this.bomZ32.Close();
            this.bomOmega.Close();

            var bytes = System.IO.File.ReadAllBytes(ofile);

            var hash = BOM.hasher != null ? BOM.hasher.ComputeHash(bytes) : null;
            var md5 = hash != null ? AVXManager.BytesToHex(hash) : "ERROR";
            this.bomOmega_MD5.WriteLine(md5);

            this.bomOmega_MD5.Close();
        }
    }
}
