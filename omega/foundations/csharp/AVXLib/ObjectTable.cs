using PhonemeEmbeddings;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AVXLib
{
    public interface IAVXObject
    {
        Framework.Written written { get; }
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

    public class OmegaData
    {
        [DllImport("omega_data.dll", EntryPoint = "get_library_revision")]
        public static extern UInt16 get_library_revision();

        [DllImport("omega_data.dll", EntryPoint = "acquire_data")]
        private static extern UInt64 acquire_data(byte[] data, UInt64 size);

        [DllImport("omega_data.dll", EntryPoint = "md5_hash")]
        public static extern UInt64 md5_hash(byte idx); // 0 <= idx <= 1

        public static MemoryStream? Contents
        {
            get
            {
                byte[] array = new byte[0];

                try
                {
                    UInt64 size = OmegaData.acquire_data(array, 0);

                    if (size > 0)
                    {
                        array = new byte[size];

                        GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
                        try
                        {
                            if (size == acquire_data(array, size))
                            {
                                MemoryStream stream = new MemoryStream(array);
                                stream.Position = 0;
                                handle.Free();
                                return stream;
                            }
                        }
                        catch
                        {
                            handle.Free();
                        }
                    }
                }
                catch
                {
                    ;
                }
                return null;
            }
        }
    }

    public class ObjectTable
    {
        private static ObjectTable? Instance = null;
#if OBSOLETE_PATH_BASED_SDK
        public static string SDK { set; protected get; }
#endif
        public static ObjectTable AVXObjects
        {
            get
            {
                if (ObjectTable.Instance != null)
                {
                    return ObjectTable.Instance;
                }
#if OBSOLETE_PATH_BASED_SDK
                ObjectTable.Instance = AVXLib.ObjectTable.Create(ObjectTable.SDK);
#else
                try
                {
                    using (MemoryStream mem = AVXLib.OmegaData.Contents)
                    {
                        if (mem != null)
                        {
                            using (BinaryReader reader = new BinaryReader(mem))
                            {
                                ObjectTable.Instance = AVXLib.ObjectTable.Create(reader);
                            }
                        }
                        else
                        {
                            ObjectTable.Instance = null;
                        }
                    }
                }
                catch
                {
                    ;
                }
#endif
                return ObjectTable.Instance;
            }
        }
        internal class InternalObjectTable : ObjectTable, IAVXObject, IAVXObjectSetter
        {
            public Framework.Written Written { set => _written = value; }
            public Framework.Lexicon Lexicon { set => _lexicon = value; }
            public Framework.Lemmata Lemmata { set => _lemmata = value; }
            public Framework.OOV OOV { set => _oov = value; }

            private static Type[] requirements = new Type[] { typeof(Memory.Book), typeof(Memory.Written), typeof(Memory.Lexicon), typeof(Memory.Lemmata), typeof(Memory.OOV), typeof(Memory.Phonetics), typeof(Memory.Chapter), typeof(Memory.Names) };

            internal InternalObjectTable(string input): base()
            {
                this.Mem = new AVXLib.Memory.Deserialization.Data(this, input, requirements); // Revision 3.9
            }
            internal InternalObjectTable(BinaryReader stream) : base()
            {
                this.Mem = new AVXLib.Memory.Deserialization.Data(this, stream, requirements); // Revision 5.1
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

        private static ObjectTable Create(string input) // e.g. "AVX-Omega-4217.data"
        {
            var obj = new ObjectTable.InternalObjectTable(input);
            return obj;
        }
        private static ObjectTable Create(BinaryReader data) // from dotnet resource
        {
            var obj = new ObjectTable.InternalObjectTable(data);
            return obj;
        }
    }
}
