#include <avx.h>

namespace avx
{
    struct WordKeyBits
    {
        const u16 CAPS = 0xC000;  // leading 2 bits
        const u16 CAPS_FirstLetter = 0x8000;
        const u16 CAPS_AllLetters = 0x4000;
        const u16 WordKey = 0x3FFF;  // trailing 14 bits
    };

    struct LemmaBits
    {
        const u16 OOV_Marker = 0x8000;
        const u16 ModernizationSquelch_Marker = 0x4000;

        const u16 OOV_Lookup_Mask = 0xFFFF ^ ModernizationSquelch_Marker;
        const u16 Lexicon_Lookup_Mask = 0xFFFF ^ WordKeyBits.CAPS;
    };

    struct Punctuation   // uint8 {
    {
        const byte Clause = 0xE0;
        const byte Exclamatory = 0x80;
        const byte Interrogative = 0xC0;
        const byte Declarative = 0xE0;
        const byte Dash = 0xA0;
        const byte Semicolon = 0x20;
        const byte Comma = 0x40;
        const byte Colon = 0x60;
        const byte Possessive = 0x10;
        const byte CloseParen = 0x0C;
        const byte Parenthetical = 0x04;
        const byte Italics = 0x02;
        const byte Jesus = 0x01;
    };

    struct PersonNumber   // uint16 { // leading 4-bits
    {
        const u16 PersonBits = 0x3000;
        const u16 NumberBits = 0xC000;
        const u16 Indefinite = 0x0000;
        const u16 Person1st = 0x1000;
        const u16 Person2nd = 0x2000;
        const u16 Person3rd = 0x3000;
        const u16 Singular = 0x4000;
        const u16 Plural = 0x8000;
        const u16 WH = 0xC000;
    };

    struct POS12
    {
        const u16 NounOrPronoun = 0x030;
        const u16 Noun = 0x010;
        const u16 Noun_UnknownGender = 0x010;
        const u16 ProperNoun = 0x030;
        const u16 Pronoun = 0x020;
        const u16 Pronoun_Neuter = 0x021;
        const u16 Pronoun_Masculine = 0x022;
        const u16 Pronoun_NonFeminine = 0x023;
        const u16 Pronoun_Feminine = 0x024;
        const u16 PronounOrNoun_Genitive = 0x008;
        const u16 PronounOrNoun_Genitive_MASK = (u16)(PronounOrNoun_Genitive | Noun | Pronoun);
        const u16 Noun_Genitive = (u16)(PronounOrNoun_Genitive | Noun);
        const u16 Pronoun_Genitive = (u16)(PronounOrNoun_Genitive | Pronoun);
        const u16 Pronoun_Nominative = 0x060;
        const u16 Pronoun_Objective = 0x0A0;
        const u16 Pronoun_Reflexive = 0x0E0;
        const u16 Pronoun_NoCase_NoGender = 0x020;
        const u16 Verb = 0x100;
        const u16 To = 0x200;
        const u16 Preposition = 0x400;
        const u16 Interjection = 0x800;
        const u16 Adjective = 0xA00;
        const u16 Numeric = 0xB00;
        const u16 Conjunction = 0xC00;
        const u16 Determiner = 0xD00;
        const u16 Particle = 0xE00;
        const u16 Adverb = 0xF00;
        const u16 NonNoun = 0xF00;
    };

    struct Transitions // uint8 { // leading 5 bits
    {
        const byte EndBit = 0x10;
        const byte BeginningOfVerse = 0x20;
        const byte EndOfVerse = 0x30;
        const byte BeginningOfChapter = 0x60;
        const byte EndOfChapter = 0x70;
        const byte BeginningOfBook = 0xE0;
        const byte EndOfBook = 0xF0;
        const byte BeginningOfBible = 0xE8;
        const byte EndOfBible = 0xF8;
    };

    struct Segments   // uint8 { // trailing 3 bits
    {
        const byte HardSegmentEnd = 0x04; // . ? !
        const byte CoreSegmentEnd = 0x02; // :
        const byte SoftSegmentEnd = 0x01; // , ; ( ) --
        const byte RealSegmentEnd = 0x06; // . ? ! :
    };
    struct Entities
    {
        const u16 Hitchcock = 0x8000;
        const u16 men = 0x1;
        const u16 women = 0x2;
        const u16 tribes = 0x4;
        const u16 cities = 0x8;
        const u16 rivers = 0x10;
        const u16 mountains = 0x20;
        const u16 animals = 0x40;
        const u16 gemstones = 0x80;
        const u16 measurements = 0x100;
    };

    struct OOV
    {
        const u16 Marker = 0x8000;
        const u16 Length = 0x0F00;
        const u16 Index = 0x00FF;
    };
}
