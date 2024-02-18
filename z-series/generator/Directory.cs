using DigitalAV.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoundationsGenerator
{
    public static class BOM
    {
        public const string Omega_Version_Updated = "-Ω3909";
        public const string Omega_Version = "-Ω32";
        public const string Z_31 = "-Z31";
        public const string Z_32 = "-Z32";

#pragma warning disable SYSLIB0045
        public static HashAlgorithm? hasher { get; private set; } = HashAlgorithm.Create(HashAlgorithmName.MD5.ToString());
#pragma warning restore SYSLIB0045

        public const byte IGNORE    = 0xEE;
        public const byte UNDEFINED = 0xFF;
        public const byte DIRECTORY = 0;
        public const byte Book      = 1;
        public const byte Chapter   = 2;
        public const byte Written   = 3;
        public const byte Lexicon   = 4;
        public const byte Lemmata   = 5;
        public const byte OOV       = 6;
        public const byte Names     = 7;
        public const byte Phonetics = 8;
        public const byte Verse     = UNDEFINED;    // not defined in 3.2 versions

        public static Dictionary<byte, Directory> Inventory = new()
        {
            {BOM.DIRECTORY,      new Directory("Directory") },
            {BOM.Book,           new Directory("Book") },
            {BOM.Chapter,        new Directory("Chapter") },
            {BOM.Written,        new Directory("Written") },
            {BOM.Lexicon,        new Directory("Lexicon") },
            {BOM.Lemmata,        new Directory("Lemmata") },
            {BOM.OOV,            new Directory("OOV-Lemmata") },
            {BOM.Names,          new Directory("Names") },
            {BOM.Phonetics,      new Directory("Phonetics") },
            {BOM.UNDEFINED,      new Directory("Verse") }
        };
        public static int GetRecordLength(ORDER id, string version)
        {
            switch (id)
            {
                case ORDER.Directory:   return 64;
                case ORDER.Book:        return  version.Contains("32") ? 48 : 50;
                case ORDER.Chapter:     return  version.Contains("32") ?  6 : 10;   // Z14 is also 8 bytes; only Z31 was 10 bytes
                case ORDER.UNDEFINED:   return  4; // Verse 
                case ORDER.Written:     return  version.Contains("32") ? 24 : 22; 
            }
            return 0;
        }

        public static string GetZ_Name(ORDER id)
        {
            if (id == ORDER.UNDEFINED)
                return "Verse";

            var entry = BOM.Inventory[(byte)id];

            if (entry.label.Contains("OOV"))
                return "Lemma-OOV";
            if (entry.label.Contains("Lemma"))
                return "Lemma";
            if (entry.label.Contains("Writ"))
                return "Writ";

            return entry.label;
        }
        private static string IX(string itype)
        {
            return AVXManager.baseSDK + "AV-" + itype + ".ix";
        }
        private static string DX(string itype)
        {
            return AVXManager.baseSDK + "AV-" + itype + ".dx";
        }
        private static string DXI(string itype)
        {
            return AVXManager.baseSDK + "AV-" + itype + ".dxi";
        }
        public static string GetC_Type(ORDER id)
        {
            switch (id)
            {
                case ORDER.UNDEFINED: // verse
                case ORDER.Book:
                case ORDER.Chapter:
                    return BOM.Inventory[(byte)id].label.ToLower() + "_index";
            }
            return BOM.Inventory[(byte)id].label.Replace("-", "_").ToLower();
        }
        public static string GetPascal_Type(ORDER id)
        {
            switch (id)
            {
                case ORDER.UNDEFINED: // verse
                case ORDER.Book:
                case ORDER.Chapter:
                    return BOM.Inventory[(byte)id].label + "Index";
            }
            return BOM.Inventory[(byte)id].label.Replace("-", "");
        }
        public static string GetZ_Path(string zname, string explicitSuffix = "", string release = "")
        {
            string suffix = explicitSuffix;

            if (explicitSuffix.Length == 0)
            {
                if (zname == "Book" && release == BOM.Z_31)
                    suffix = "-50";
                else if (zname == "Chapter" && release == BOM.Z_31)
                    suffix = "-10";
                else if (zname == "Writ" && release == BOM.Z_31)
                    suffix = "-22";
            }

            if (zname.Contains("Writ"))
                return DX(zname + suffix);
            if (zname == "Verse" || zname == "Chapter" || zname == "Book")
                return IX(zname + suffix);

            return DXI(zname + suffix);
        }
        public static string GetZ_Path(ORDER id, string suffix = "", string release = "")
        {
            var zname = GetZ_Name(id);
            return GetZ_Path(zname, suffix, release);
        }
        public static string GetZ_IType(ORDER id)
        {
            if (id == ORDER.UNDEFINED)
                return "Verse";

            var entry = BOM.Inventory[(byte)id];

            if (entry.label.Contains("OOV"))
                return "Lemma-OOV";
            if (entry.label.Contains("Lemma"))
                return "Lemma";
            if (entry.label.Contains("Writ"))
                return "Writ";

            return entry.label;
        }
        public static string GetZ_OType(ORDER id)
        {
            if (id == ORDER.UNDEFINED)
                return "Verse";

            var entry = BOM.Inventory[(byte)id];

            if (entry.label.Contains("OOV"))
                return "Lemma-OOV";
            if (entry.label.Contains("Lemma"))
                return "Lemma";
            if (entry.label.Contains("Writ"))
                return "Writ";

            return entry.label;
        }
    }
    public class Directory
    {
        public Directory(string label)
        {
            this.label = label;
            this.offset = 0;
            this.length = 0;
            this.hash   = "";
            this.recordCount = 0;
            this.recordLength = 0;
        }
        public string label;
        public UInt32 offset;
        public UInt32 length;
        public string hash;
        public UInt32 recordLength;
        public UInt32 recordCount;

        public string GetRustSource(string version, string extent = ".rs")
        {
            if (string.IsNullOrEmpty(this.label) || string.IsNullOrEmpty(version))
                return "ERROR" + extent;

            string filename = this.label.Replace("-", "_").ToLower(); ;

            switch (this.label)
            {
                case "Verse":
                case "Chapter":
                case "Book":    filename += "_index"; break;
            }
            filename += extent;

            switch (version[1])
            {
                case 'Ω':   return Path.Combine(AVXManager.RUST_SOURCE, filename);
                case 'Z':   return Path.Combine(AVXManager.RUST_SOURCE, filename);
            }
            return "ERROR" + extent;
        }
        public string GetCppSource(string version, string extent = ".cpp")
        {
            if (string.IsNullOrEmpty(this.label) || string.IsNullOrEmpty(version))
                return "ERROR" + extent;

            string filename = this.label.Replace("-", "_").ToLower(); ;

            switch (this.label)
            {
                case "Verse":
                case "Chapter":
                case "Book": filename += "_index"; break;
                case "Written": filename = Path.Combine(filename, this.label); break;
            }
            filename += extent;

            switch (version[1])
            {
                case 'Ω': return Path.Combine(AVXManager.CPP_SOURCE, filename);
                case 'Z': return Path.Combine(AVXManager.CPP_SOURCE, filename);
            }
            return "ERROR" + extent;
        }
    }

    public enum ORDER
    {
        UNDEFINED    = BOM.UNDEFINED,
        Directory    = BOM.DIRECTORY,
        Book         = BOM.Book,
        Chapter      = BOM.Chapter,
        Written      = BOM.Written,
        Lexicon      = BOM.Lexicon,
        Lemmata      = BOM.Lemmata,
        OOV          = BOM.OOV,
        Names        = BOM.Names,
        Phonetics    = BOM.Phonetics,
    }
}
