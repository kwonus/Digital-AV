using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class LexRecord
    {
        private UInt16 Key;
        public UInt16 Entities;
        public UInt32[] POS;
        public string[] NUPOS;
        public string[] Text;

        public LexRecord(UInt16 key, UInt16 entities, UInt32[] pos, string[] text)
        {
            this.Key = key;
            this.Entities = entities;
            this.POS = pos;
            this.Text = text;
            this.NUPOS = new string[pos.Length];

            for (int i = 0; i < pos.Length; i++)
            {
                this.NUPOS[i] = Blueprint.Blue.FiveBitEncoding.DecodePOS(pos[i]);
            }
        }
        /*
         * Lexicon:
         *   - Key: 123
         *     Entities: 456
         *     POS: [1, 2, 3]
         *     Text: ["example", "sample", "test"]
         *     NUPOS: ["noun", "verb", "adjective"]
         *   - Key: 789
         *     Entities: 101
         *     POS: [4, 5, 6]
         *     Text: ["hello", "world", "test"]
         *     NUPOS: ["greeting", "noun", "adjective"]
         */
        internal void InsertSpaces(TextWriter writer, int cnt)
        {
            for (int i = cnt; i < 50; i++)
            {
                writer.Write(' ');
            }
        }
        internal static int AppendArray(TextWriter writer, string[] array)
        {
            if (array.Length == 0)
            {
                writer.Write("[ ]");
                return 3;
            }

            char delimiter = '[';
            int length = 0;

            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(delimiter); writer.Write(' ');
                length += 2;
                delimiter = ',';

                writer.Write(array[i]);
                length += array[i].Length;
            }
            writer.Write(" ]");
            return length + 2;
        }
        internal static int AppendArray(TextWriter writer, UInt32[] array)
        {
            if (array.Length == 0)
            {
                writer.Write("[ ]");
                return 3;
            }

            char delimiter = '[';
            int length = 0;

            for (int i = 0; i < array.Length; i++)
            {
                writer.Write(delimiter); writer.Write(' ');
                length += 2;
                delimiter = ',';

                writer.Write("0x" + array[i].ToString("X8"));
                length += (2+8);
            }
            writer.Write(" ]");
            return length + 2;
        }
        internal void AppendLabel(TextWriter writer)
        {
            writer.Write(" #");
            writer.Write(this.Text[0]);
            writer.Write("/");
            writer.Write(this.Text[2]);
            writer.WriteLine("/");
        }
        internal static void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("Lexicon:");
        }
        internal void WriteRecord(TextWriter writer)
        {
            int len;
            writer.Write("  - Key: ");       writer.Write("0x" + this.Key.     ToString("X4")); InsertSpaces(writer, 3 + 2+4); AppendLabel(writer);
            writer.Write("    Text: ");      len = LexRecord.AppendArray(writer, this.Text);    InsertSpaces(writer, 4 + len); AppendLabel(writer);
            writer.Write("    Entities: ");  writer.Write("0x" + this.Entities.ToString("X4")); InsertSpaces(writer, 8 + 2+4); AppendLabel(writer);
            writer.Write("    POS: ");       len = LexRecord.AppendArray(writer, this.POS);     InsertSpaces(writer, 3 + len); AppendLabel(writer);
            writer.Write("    NUPOS: ");     len = LexRecord.AppendArray(writer, this.NUPOS);   InsertSpaces(writer, 5 + len); AppendLabel(writer);
        }
    }
    public class YamlLexicon: Dictionary<UInt16, LexRecord>
    {
        internal static YamlLexicon Entries = new YamlLexicon();
        public static void WriteAll(TextWriter writer)
        {
            LexRecord.WriteHeader(writer);
            foreach (var entry in Entries.Values)
            {
                entry.WriteRecord(writer);
            }
        }

        public static LexRecord Add(UInt16 key, UInt16 entities, UInt32[] pos, string search, string display, string modern)
        {
            string[] text = [ search, display, modern ];
            var record = new LexRecord(key, entities, pos, text);
            YamlLexicon.Entries.Add(key, record);
            return record;
        }
        public static LexRecord Add(UInt16 key, UInt16 entities, UInt32[] pos, string[] text)
        {
            var record = new LexRecord(key, entities, pos, text);
            YamlLexicon.Entries.Add(key, record);
            return record;
        }
    }    
}        
         