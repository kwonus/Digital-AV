namespace Generator
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public static class BOM
    {
//      public const string Omega_Version = "-Ω42";

#pragma warning disable SYSLIB0045
        public static HashAlgorithm? hasher { get; private set; } = HashAlgorithm.Create(HashAlgorithmName.MD5.ToString());
#pragma warning restore SYSLIB0045

        public const byte IGNORE = 0xEE;
        public const byte UNDEFINED = 0xFF;
        public const byte DIRECTORY = 0;
        public const byte Book = 1;
        public const byte Chapter = 2;
        public const byte Written = 3;
        public const byte Lexicon = 4;
        public const byte Lemmata = 5;
        public const byte OOV = 6;
        public const byte Names = 7;
        public const byte Phonetics = 8;

        public static Dictionary<byte, TOC> Inventory = new()
        {
            {BOM.DIRECTORY,      new TOC(DIRECTORY, "Directory") },
            {BOM.Book,           new TOC(Book,      "Book") },
            {BOM.Chapter,        new TOC(Chapter,   "Chapter") },
            {BOM.Written,        new TOC(Written,   "Written") },
            {BOM.Lexicon,        new TOC(Lexicon,   "Lexicon") },
            {BOM.Lemmata,        new TOC(Lemmata,   "Lemmata") },
            {BOM.OOV,            new TOC(OOV,       "OOV-Lemmata") },
            {BOM.Names,          new TOC(Names,     "Names") },
            {BOM.Phonetics,      new TOC(Phonetics, "Phonetics") },
        };
    }
    public class TOC
    {
        public TOC(byte id, string label)
        {
            this.ID = id;
            this.label = new byte[16];
            for (int i = 0; i < label.Length && i < 16; i++)
                this.label[i] = (byte)label[i];
            for (int i = label.Length; i < 16; i++)
                this.label[i] = (byte)0;
            this.offset = 0;
            this.length = 0;
            this.hash = new UInt64[2];
            for (int i = 0; i < 2; i++)
                this.hash[i] = 0;
            this.recordCount = 0;
            this.recordLength = 0;
        }
        public byte[] label;
        public UInt32 originalOffset;
        public UInt32 offset;
        public UInt32 length;
        public UInt64[] hash;
        public UInt32 recordLength;
        public UInt32 recordCount;
        public byte ID { get; private set; }
    }

    public enum ORDER
    {
        UNDEFINED = BOM.UNDEFINED,
        Directory = BOM.DIRECTORY,
        Book = BOM.Book,
        Chapter = BOM.Chapter,
        Written = BOM.Written,
        Lexicon = BOM.Lexicon,
        Lemmata = BOM.Lemmata,
        OOV = BOM.OOV,
        Names = BOM.Names,
        Phonetics = BOM.Phonetics,
    }
}
