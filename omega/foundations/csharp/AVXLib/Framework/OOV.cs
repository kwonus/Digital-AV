﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVXLib.Framework
{
    public class OOV
    {
        private static Dictionary<UInt16, ReadOnlyMemory<char>> map = new();
        public static (OOV oov, bool valid) GetEntry(UInt16 key)
        {
            (OOV oov, bool valid) result = (new OOV(), false);

            if (OOV.map.ContainsKey(key))
            {
                result.oov.oovKey = key;
                result.oov.text = OOV.map[key];
                result.valid = true;
            }
            return result;
        }

        public UInt16 oovKey;
        public ReadOnlyMemory<char> text;

        public static (Dictionary<UInt16, ReadOnlyMemory<char>> result, bool okay, string message) Read(System.IO.BinaryReader reader, Dictionary<string, Artifact> directory)
        {
            var oov = new Dictionary<UInt16, ReadOnlyMemory<char>>();

            if (!directory.ContainsKey("OOV-Lemmata"))
                return (oov, false, "OOV-Lemmata is missing from directory");
            Artifact artifact = directory["OOV-Lemmata"];

            Span<byte> buffer = stackalloc byte[24];

            var needed = artifact.offset + artifact.length;

            if (reader.BaseStream.Length < needed)
                return (oov, false, "Input stream has insufficient data");

            reader.BaseStream.Seek(artifact.offset, SeekOrigin.Begin);

            for (int o = 0; o < artifact.recordCount; o++)
            {
                var key = reader.ReadUInt16();
                var val = Deserialization.ReadDelimitedMemory(reader, '\0', buffer);
                if ((val.length > 0) && (key > 0) && !val.overflow)
                    oov[key] = val.text;
            }
            OOV.map = oov;
            return (oov, true, "");
        }
    }
}