namespace Generator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HashMapper
    {
        public Dictionary<UInt32, HashSet<UInt16>> PosToBits { get; private set; } = new();
        public Dictionary<UInt16, HashSet<UInt32>> BitsToPos { get; private set; } = new();

        public void Add(UInt32 key32, UInt16 key16)
        {
            bool alloc32 = !PosToBits.ContainsKey(key32);
            bool alloc16 = !BitsToPos.ContainsKey(key16);

            HashSet<UInt16> bits = alloc32 ? new() : PosToBits[key32];
            HashSet<UInt32> pos  = alloc16 ? new() : BitsToPos[key16];

            if (!bits.Contains(key16))
                bits.Add(key16);
            if (!pos.Contains(key32))
                pos.Add(key32);

            if (alloc32)
                PosToBits[key32] = bits;
            if (alloc16)
                BitsToPos[key16] = pos;
        }
        public void Print()
        {
#pragma warning disable CS0219
            int i = 0;
#pragma warning restore CS0219
        }
    }
}
