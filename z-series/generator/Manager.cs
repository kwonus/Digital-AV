namespace DigitalAV.Migration
{
    using FoundationsGenerator;
    using SerializeFromSDK;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public interface IInventoryManager
    {
        public void Manage();
    }

    public abstract class AVXManager
    {
        public static void Main()
        {
            IInventoryManager z31 = new ManageZ31();
            z31.Manage();

            IInventoryManager z32 = new ManageZ32();
            //z31.Manage();

            IInventoryManager omega = new ManageOmega();
            //omega.Manage();

            var cpp = new CSrcGen(BOM.baseSDK, BOM.csrc_z);
            cpp.Generate();

            var rust = new RustSrcGen(BOM.baseSDK, BOM.rsrc_z);
            rust.Generate();
        }
        internal static BinaryWriter? OpenTextWriter(string suffix, string extent)
        {
            try
            {
                string file = BOM.baseSDK + "AV-Inventory" + suffix + extent;
                var stream = new FileStream(file, FileMode.Create);
                return new BinaryWriter(stream, Encoding.ASCII);
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
    }
}
