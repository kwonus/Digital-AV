namespace AVXLib.Framework
{
    using AVXLib.Memory;

    public class Lemmata
    {
        ReadOnlyMemory<AVXLib.Memory.Lemmata> Lemmas;

        public Lemmata(Deserialization.Data data)
        {
            this.Lemmas = data.Lemmata;
        }

        public UInt16[] FindLemmataUsingWordKey(UInt16 key)
        {
            HashSet<UInt16> lemmata = new();
            foreach (var record in this.Lemmas.Span)
            {
                if (record.WordKey == key)
                {
                    foreach (var lemma in record.Lemmas.Span)
                    {
                        if (!lemmata.Contains(lemma))
                            lemmata.Add(lemma);
                    }
                }
            }
            return lemmata.ToArray();
        }
        public UInt16[] FindLemmataInList(UInt16 key)
        {
            HashSet<UInt16> lemmata = new();
            foreach (var record in this.Lemmas.Span)
            {
                foreach (var lemma in record.Lemmas.Span)
                {
                    if ((lemma == key) && !lemmata.Contains(lemma))
                        lemmata.Add(record.WordKey);
                }
            }
            return lemmata.ToArray();
        }
    }
}
