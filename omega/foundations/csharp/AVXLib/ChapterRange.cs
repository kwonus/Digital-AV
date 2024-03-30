using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVXLib
{
    public class ChapterRange
    {
        public byte From { get; set; }
        public byte? Unto { get; set; }
        public ChapterRange()
        {
            From = 0;
            Unto = null;
        }
        public ChapterRange(byte from , byte? unto = null)
        {
            from = from;
            unto = unto;
        }
    }
}
