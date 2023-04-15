namespace AVXLib
{
    public interface IAVXObject
    {
        Framework.Written written { get;  }
        Framework.Lexicon lexicon { get; }
        Framework.Lemmata lemmata { get; }
    }
    public interface IAVXObjectSetter
    {
        Framework.Written Written { set; }
        Framework.Lexicon Lexicon { set; }
        Framework.Lemmata Lemmata { set; }
        Framework.OOV OOV         { set; }
    }

    public class ObjectTable
    {
        internal class InternalObjectTable : ObjectTable, IAVXObject, IAVXObjectSetter
        {
            public Framework.Written Written { set => _written = value; }
            public Framework.Lexicon Lexicon { set => _lexicon = value; }
            public Framework.Lemmata Lemmata { set => _lemmata = value; }
            public Framework.OOV OOV { set => _oov = value; }

            private static Type[] requirements = new Type[] { typeof(Memory.Book), typeof(Memory.Written), typeof(Memory.Lexicon), typeof(Memory.Lemmata), typeof(Memory.OOV) };

            internal InternalObjectTable(string input): base()
            {
                this.Mem = new AVXLib.Memory.Deserialization.Data(this, @"C:\src\Digital-AV\omega\AVX-Omega.data", requirements);
            }
        }
        public AVXLib.Memory.Deserialization.Data Mem { get; protected set; }

        protected Framework.Written _written;
        public virtual Framework.Written written { get => _written; }
        protected Framework.Lexicon _lexicon;
        public Framework.Lexicon lexicon { get => _lexicon; }
        protected Framework.Lemmata _lemmata;
        public Framework.Lemmata lemmata { get => _lemmata; }
        protected Framework.OOV _oov;
        public Framework.OOV oov { get => _oov; }

        protected ObjectTable()
        {
            ;
        }

        public static ObjectTable Create(string input = @"C:\src\Digital-AV\omega\AVX-Omega.data")
        {
            return new ObjectTable.InternalObjectTable(input);
        }
    }
}
