namespace AVXLib.Framework
{
    using AVXLib.Memory;

    public class OOV
    {
        private Dictionary<UInt16, ReadOnlyMemory<char>> map;
        private Dictionary<string, UInt16> reverseMap = new();
        public (Memory.OOV oov, bool valid) GetEntry(UInt16 key)
        {
            (Memory.OOV oov, bool valid) result = (new Memory.OOV(), false);

            if (this.map.ContainsKey(key))
            {
                result.oov.oovKey = key;
                result.oov.text = this.map[key];
                result.valid = true;
            }
            return result;
        }
        public IEnumerable<UInt16> Keys
        {
            get => this.map.Keys;
        }
        public UInt16 GetReverseEntry(string text)
        {
            if (this.reverseMap.ContainsKey(text))
            {
                return this.reverseMap[text];
            }
            var normalized = text.ToLower().Replace("-", "");
            if (this.reverseMap.ContainsKey(normalized))
            {
                return this.reverseMap[normalized];
            }
            return 0;
        }

        public OOV(Deserialization.Data data)
        {
            this.map = data.OOVLemmata;

            foreach (var kv in this.map)
            {
                var key = kv.Key;
                var val = kv.Value.ToArray();
                string text = new string(val);
                this.reverseMap[text] = key;
            }
        }
    }
}
