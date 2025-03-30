#include <written.h>

namespace avx
{
#ifdef NEVER
    using System.Text;
    using AVXLib.Memory;
    using static System.Net.Mime.MediaTypeNames;
    using static AVXLib.Framework.Numerics;

    public class Written
    {
        private Deserialization.Data Data;
        private ReadOnlyMemory<AVXLib.Memory.Book> Book{ get = > this.Data.Book; }
        private ReadOnlyMemory<AVXLib.Memory.Written> Writ{ get = > this.Data.Written; }

            //      private static char[] subtract = new char[] { '-', ' ', '\'', '(', ')', '.', ':', ';', '!', '?', ',' };
        private static HashSet<char> subtract = new(){ '-', ' ' };

        public Written(Deserialization.Data data)
        {
            this.Data = data;
        }
        private static string Keyify(ReadOnlyMemory<char> input)
        {
            return Keyify(input.ToString());
        }
        public static string Keyify(string input)
        {
            string result = input.ToLower();

            int idx = -1;
            foreach(var c in subtract)
            {
                int tmp = result.IndexOf(c);
                if (tmp < 0)
                    continue;
                if (tmp < idx || idx < 0)
                    idx = tmp;
            }
            if (idx >= 0)
            {
                int len = result.Length;
                var replacement = (idx == 0) ? new StringBuilder(len - 1) : new StringBuilder(result.Substring(0, idx), len - 1);

                for (++idx; idx < len; idx++)
                    if (!subtract.Contains(result[idx]))
                        replacement.Append(result[idx]);
                return replacement.ToString();
            }
            return result;
        }
    }
#endif
}
