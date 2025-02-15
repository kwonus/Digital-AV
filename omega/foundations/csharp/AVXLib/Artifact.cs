namespace AVXLib
{
    using static AVXLib.Framework.Numerics;
    using System.Text;

    public class Artifact
    {
        public Artifact(string label)
        {
            this.SKIP = true;
            this.label = label;
            this.offset = 0;
            this.length = 0;
            this.hash = "";
            this.recordCount = 0;
            this.recordLength = 0;
            this.ERROR = false;
            this.DONE = false;
        }
        public Artifact(System.IO.BinaryReader reader, Dictionary<string, Artifact> directory, Type[]? selections)
        {
            this.SKIP = (selections != null);
            this.label = "";
            this.offset = 0;
            this.length = 0;
            this.hash = "";
            this.recordCount = 0;
            this.recordLength = 0;

            Artifact? dir = directory.ContainsKey("Directory") ? directory["Directory"] : null;
            this.DONE = false;

            byte[] label = new byte[16];
            this.ERROR = (directory.Count > 0 && dir == null) || (directory.Count > 0 && reader.BaseStream.Position > dir.length) || (16 != reader.Read(label));

            int len = 0;
            if (!this.ERROR)
            {
                var buffer = new StringBuilder(16); len += 16;
                for (int i = 0; i < 16 && label[i] != 0; i++)
                    buffer.Append((char)(label[i]));
                this.label = buffer.ToString();
                this.offset = reader.ReadUInt32(); len += 4;
                this.length = reader.ReadUInt32(); len += 4;
                this.recordLength  = reader.ReadUInt32(); len += 4;
                this.recordCount   = reader.ReadUInt32(); len += 4;
                if (selections != null)
                    foreach (var selection in selections)
                    {
                        bool include = selection.Name.StartsWith(this.label, StringComparison.InvariantCultureIgnoreCase)
                                    || this.label.StartsWith(selection.Name, StringComparison.InvariantCultureIgnoreCase);
                        if (include)
                        {
                            this.SKIP = false;
                            break;
                        }
                    }
                this.hash_1 = reader.ReadUInt64(); len += 8;
                this.hash_2 = reader.ReadUInt64(); len += 8;
                this.hash = this.hash_1.ToString("X016") + this.hash_2.ToString("X016");
                this.ERROR = dir != null && len != dir.recordLength;

                directory[this.label] = this;

                if (dir != null)
                    this.DONE = (dir.length <= reader.BaseStream.Position);
            }
        }
        public bool   SKIP;
        public bool   DONE;
        public bool   ERROR;
        public string label;
        public UInt32 offset;
        public UInt32 length;
        public UInt64 hash_1;
        public UInt64 hash_2;
        public string hash;
        public UInt32 recordLength;
        public UInt32 recordCount;
    }
}
