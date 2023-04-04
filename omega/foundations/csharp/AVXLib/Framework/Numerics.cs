namespace AVXLib.Framework
{
    public abstract class Numerics
    {
        public abstract class WordKeyBits
        {
            public const ushort CAPS = 0xC000;  // leading 2 bits
            public const ushort CAPS_FirstLetter = 0x8000;
            public const ushort CAPS_AllLetters = 0x4000;
            public const ushort WordKey = 0x3FFF;  // trailing 14 bits
        }

        public abstract class Punctuation   // uint8 {
        {
            public const byte Clause = 0xE0;
            public const byte Exclamatory = 0x80;
            public const byte Interrogative = 0xC0;
            public const byte Declarative = 0xE0;
            public const byte Dash = 0xA0;
            public const byte Semicolon = 0x20;
            public const byte Comma = 0x40;
            public const byte Colon = 0x60;
            public const byte Possessive = 0x10;
            public const byte CloseParen = 0x0C;
            public const byte Parenthetical = 0x04;
            public const byte Italics = 0x02;
            public const byte Jesus = 0x01;
        }

        public abstract class PersonNumber   // uint16 { // leading 4-bits
        {
            public const ushort PersonBits = 0x3000;
            public const ushort NumberBits = 0xC000;
            public const ushort Indefinite = 0x0000;
            public const ushort Person1st = 0x1000;
            public const ushort Person2nd = 0x2000;
            public const ushort Person3rd = 0x3000;
            public const ushort Singular = 0x4000;
            public const ushort Plural = 0x8000;
            public const ushort WH = 0xC000;
        }

        public abstract class POS12
        {
            public const ushort NounOrPronoun = 0x030;
            public const ushort Noun = 0x010;
            public const ushort Noun_nknownGender = 0x010;
            public const ushort ProperNoun = 0x030;
            public const ushort Pronoun = 0x020;
            public const ushort Pronoun_Neuter = 0x021;
            public const ushort Pronoun_Masculine = 0x022;
            public const ushort Pronoun_NonFeminine = 0x023;
            public const ushort Pronoun_Feminine = 0x024;
            public const ushort PronounOrNoun_Genitive = 0x008;
            public const ushort Pronoun_Nominative = 0x060;
            public const ushort Pronoun_Objective = 0x0A0;
            public const ushort Pronoun_Reflexive = 0x0E0;
            public const ushort Pronoun_NoCase_NoGender = 0x020;
            public const ushort Verb = 0x100;
            public const ushort To = 0x200;
            public const ushort Preposition = 0x400;
            public const ushort Interjection = 0x800;
            public const ushort Adjective = 0xA00;
            public const ushort Numeric = 0xB00;
            public const ushort Conjunction = 0xC00;
            public const ushort Determiner = 0xD00;
            public const ushort Particle = 0xE00;
            public const ushort Adverb = 0xF00;
        }

        public abstract class Transitions // uint8 { // leading 5 bits
        {
            public const ushort EndBit = 0x10;
            public const ushort BeginningOfVerse = 0x20;
            public const ushort EndOfVerse = 0x30;
            public const ushort BeginningOfChapter = 0x60;
            public const ushort EndOfChapter = 0x70;
            public const ushort BeginningOfBook = 0xE0;
            public const ushort EndOfBook = 0xF0;
            public const ushort BeginningOfBible = 0xE8;
            public const ushort EndOfBible = 0xF8;
        }

        public abstract class Segments   // uint8 { // trailing 3 bits
        {
            public const byte HardSegmentEnd = 0x04; // . ? !
            public const byte CoreSegmentEnd = 0x02; // :
            public const byte SoftSegmentEnd = 0x01; // , ; ( ) --
            public const byte RealSegmentEnd = 0x06; // . ? ! :
        }
        public abstract class Entities
        {
            public const ushort Hitchcock = 0x8000;
            public const ushort men = 0x1;
            public const ushort women = 0x2;
            public const ushort tribes = 0x4;
            public const ushort cities = 0x8;
            public const ushort rivers = 0x10;
            public const ushort mountains = 0x20;
            public const ushort animals = 0x40;
            public const ushort gemstones = 0x80;
            public const ushort measurements = 0x100;
        }

        public abstract class OOV
        {
            public const ushort Marker = 0x8000;
            public const ushort Length = 0x0F00;
            public const ushort Index = 0x00FF;
        }
    }
}
