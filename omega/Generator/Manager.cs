﻿namespace Generator
{
    using System;
    using System.IO;
    using System.Text;

    public abstract class AVXManager
    {
        public static string omegaSDK { get; private set; } = @"C:\src\AVX\omega\";

        public static void Main()
        {
            // The global bom gets messed up when we build 3.1 and 3.2 at the same time.
            // Choose which one:

            var omega = new ManageOmega("-3911".ToUpper(), "-4220".ToUpper());
            omega.Manage();
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal static (BinaryWriter? writer, string path) CreateSDK(string filename, string extent = ".data")
        {
            try
            {
                string path = AVXManager.omegaSDK + filename + extent;
                var stream = new FileStream(path, FileMode.Create);
                return (new BinaryWriter(stream, Encoding.UTF8), path);
            }
            catch
            {
                return (null, string.Empty);
            }
        }
        internal static TextWriter? OpenTextWriter(string filename, string extent)
        {
            try
            {
                string path = AVXManager.omegaSDK + filename + extent;
                TextWriter textWriter = new StreamWriter(path);
                return textWriter;
            }
            catch
            {
                return null;
            }
        }
        internal static TextReader? OpenTextReader(string filename, string extent)
        {
            try
            {
                string path = AVXManager.omegaSDK + filename + extent;
                TextReader textReader = new StreamReader(path);
                return textReader;
            }
            catch
            {
                return null;
            }
        }
        internal static BinaryReader? OpenBinaryReader(string filename, string extent = ".data")
        {
            try
            {
                string path = AVXManager.omegaSDK + filename + extent;
                var stream = new FileStream(path, FileMode.Open);
                return new BinaryReader(stream, Encoding.UTF8);
            }
            catch
            {
                return null;
            }
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
        internal static UInt32 GetRecordLength(string itype)
        {
            if (!itype.EndsWith('x'))
                return 0; // this is variable length // i.e .dxi

            var filename = Path.GetFileNameWithoutExtension(itype).ToLower();

            if (filename.Length < 3+3)
                return 0;

            string digits = filename.Substring(filename.Length-3);

            if ((digits[0] == '-') && char.IsDigit(digits[1]) && char.IsDigit(digits[2])) // this file identifies it's size
            {
                return (UInt32) (((digits[1] - (int) '0') * 10) + (digits[2] - (int)'0'));
            }
            else switch (filename.Substring(3, 4))
                {
                    case "writ": return 24;
                    case "book": return 48;
                    case "chap": return  6;
                    case "vers": return  4;
                }
            return 0;
        }
    }
}
