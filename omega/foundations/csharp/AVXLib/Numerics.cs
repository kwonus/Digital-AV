using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX.Numerics
{
    public abstract class Written
    {
        public abstract class WordKeyBits
        {
            public const UInt16 CAPS             = 0xC000;  // leading 2 bits
            public const UInt16 CAPS_FirstLetter = 0x8000;
            public const UInt16 CAPS_AllLetters  = 0x4000;
            public const UInt16 WordKey          = 0x3FFF;  // trailing 14 bits
         }

        public abstract class Puncutation   // uint8 {
        {
            public const byte Clause        = 0xE0;
            public const byte Exclamatory   = 0x80;
            public const byte Interrogative = 0xC0;
 	        public const byte Declarative   = 0xE0;
 	        public const byte Dash          = 0xA0;
 	        public const byte Semicolon     = 0x20;
            public const byte Comma         = 0x40;
            public const byte Colon         = 0x60;
            public const byte Possessive    = 0x10;
            public const byte CloseParen    = 0x0C;
 	        public const byte Parenthetical = 0x04;
            public const byte Italics       = 0x02;
            public const byte Jesus         = 0x01;
        }

        public abstract class PersonNumber   // uint16 { // leading 4-bits
        { 
            public const UInt16 PersonBits = 0x3000;
            public const UInt16 NumberBits = 0xC000;
            public const UInt16 Indefinite = 0x0000;
            public const UInt16 Person1st  = 0x1000;
            public const UInt16 Person2nd  = 0x2000;
            public const UInt16 Person3rd  = 0x3000;
            public const UInt16 Singular   = 0x4000;
            public const UInt16 Plural     = 0x8000;
            public const UInt16 WH         = 0xC000;
        }

        public abstract class POS12
        {
            public const UInt16 NounOrPronoun           = 0x030;
            public const UInt16 Noun                    = 0x010;
            public const UInt16 Noun_nknownGender       = 0x010;
            public const UInt16 ProperNoun              = 0x030;
            public const UInt16 Pronoun                 = 0x020;
            public const UInt16 Pronoun_Neuter          = 0x021;
            public const UInt16 Pronoun_Masculine       = 0x022;
            public const UInt16 Pronoun_NonFeminine     = 0x023;
            public const UInt16 Pronoun_Feminine        = 0x024;
            public const UInt16 PronounOrNoun_Genitive  = 0x008;
            public const UInt16 Pronoun_Nominative      = 0x060;
            public const UInt16 Pronoun_Objective       = 0x0A0;
            public const UInt16 Pronoun_Reflexive       = 0x0E0;
            public const UInt16 Pronoun_NoCase_NoGender = 0x020;
            public const UInt16 Verb                    = 0x100;
            public const UInt16 To                      = 0x200;
            public const UInt16 Preposition             = 0x400;
            public const UInt16 Interjection            = 0x800;
            public const UInt16 Adjective               = 0xA00;
            public const UInt16 Numeric                 = 0xB00;
            public const UInt16 Conjunction             = 0xC00;
            public const UInt16 Determiner              = 0xD00;
            public const UInt16 Particle                = 0xE00;
            public const UInt16 Adverb                  = 0xF00;
        }

        public abstract class Transitions // uint8 { // leading 5 bits
        {
            public const UInt16 EndBit = 0x10;
            public const UInt16 BeginningOfVerse = 0x20;
            public const UInt16 EndOfVerse = 0x30;
            public const UInt16 BeginningOfChapter = 0x60;
            public const UInt16 EndOfChapter = 0x70;
            public const UInt16 BeginningOfBook = 0xE0;
            public const UInt16 EndOfBook = 0xF0;
            public const UInt16 BeginningOfBible = 0xE8;
            public const UInt16 EndOfBible = 0xF8;
        }

        public abstract class Segments   // uint8 { // trailing 3 bits
        { 
            public const byte HardSegmentEnd = 0x04; // . ? !
            public const byte CoreSegmentEnd = 0x02; // :
            public const byte SoftSegmentEnd = 0x01; // , ; ( ) --
            public const byte RealSegmentEnd = 0x06; // . ? ! :
        }
    }
    public abstract class Lexicon
    {
        public abstract class Entities
        {
            public const UInt16 Hitchcock    = 0x8000;
            public const UInt16 men          =    0x1;
            public const UInt16 women        =    0x2;
            public const UInt16 tribes       =    0x4;
            public const UInt16 cities       =    0x8;
            public const UInt16 rivers       =   0x10;
            public const UInt16 mountains    =   0x20;
            public const UInt16 animals      =   0x40;
            public const UInt16 gemstones    =   0x80;
            public const UInt16 measurements =  0x100;
        }
    }

    public abstract class OOV
    {
        public const UInt16 Marker = 0x8000;
        public const UInt16 Length = 0x0F00;
        public const UInt16 Index  = 0x00FF;
    }
}
