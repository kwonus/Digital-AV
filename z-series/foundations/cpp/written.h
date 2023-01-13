#include <avxgen.h>
const char AVXWritten_File[]        = "AV-Writ.dx"; // from AV-Inventory-Z31.bom
const uint32 AVXWritten_RecordLen   =       22;     // from AV-Inventory-Z31.bom
const uint32 AVXWritten_RecordCnt   =   789651;     // from AV-Inventory-Z31.bom
const uint32 AVXWritten_FileLen     = 17372322;     // from AV-Inventory-Z31.bom

const uint16 WordKeyBits_CAPS                   = 0xC000; // leading 2 bits
const uint16 WordKeyBits_CAPS_FirstLetter       = 0x8000;
const uint16 WordKeyBits_CAPS_AllLetters        = 0x4000;
const uint16 WordKeyBits_WordKey                = 0x3FFF; // trailing 14 bits

const uint8 Puncutation_Clause                  =   0xE0;
const uint8 Puncutation_Exclamatory             =   0x80;
const uint8 Puncutation_Interrogative           =   0xC0;
const uint8 Puncutation_Declarative             =   0xE0;
const uint8 Puncutation_Dash                    =   0xA0;
const uint8 Puncutation_Semicolon               =   0x20;
const uint8 Puncutation_Comma                   =   0x40;
const uint8 Puncutation_Colon                   =   0x60;
const uint8 Puncutation_Possessive              =   0x10;
const uint8 Puncutation_CloseParen              =   0x0C;
const uint8 Puncutation_Parenthetical           =   0x04;
const uint8 Puncutation_Italics                 =   0x02;
const uint8 Puncutation_Jesus                   =   0x01;

const uint16 PersonMumber_PersonBits            = 0x3000;
const uint16 PersonMumber_NumberBits            = 0xC000;
const uint16 PersonMumber_Indefinite            = 0x0000;
const uint16 PersonMumber_Person1st             = 0x1000;
const uint16 PersonMumber_Person2nd             = 0x2000;
const uint16 PersonMumber_Person3rd             = 0x3000;
const uint16 PersonMumber_Singular              = 0x4000;
const uint16 PersonMumber_Plural                = 0x8000;
const uint16 PersonMumber_WH                    = 0xC000;

// WordClass: uint16 -- trailing 12 bits
const uint16 WordClass_NounOrPronoun            =  0x030;
const uint16 WordClass_Noun                     =  0x010;
const uint16 WordClass_Noun_nknownGender        =  0x010;
const uint16 WordClass_ProperNoun               =  0x030;
const uint16 WordClass_Pronoun                  =  0x020;
const uint16 WordClass_Pronoun_Neuter           =  0x021;
const uint16 WordClass_Pronoun_Masculine        =  0x022;
const uint16 WordClass_Pronoun_NonFeminine      =  0x023;
const uint16 WordClass_Pronoun_Feminine         =  0x024;
const uint16 WordClass_PronounOrNoun_Genitive   =  0x008;
const uint16 WordClass_Pronoun_Nominative       =  0x060;
const uint16 WordClass_Pronoun_Objective        =  0x0A0;
const uint16 WordClass_Pronoun_Reflexive        =  0x0E0;
const uint16 WordClass_Pronoun_NoCase_NoGender  =  0x020;
const uint16 WordClass_Verb                     =  0x100;
const uint16 WordClass_To                       =  0x200;
const uint16 WordClass_Preposition              =  0x400;
const uint16 WordClass_Interjection             =  0x800;
const uint16 WordClass_Adjective                =  0xA00;
const uint16 WordClass_Numeric                  =  0xB00;
const uint16 WordClass_Conjunction              =  0xC00;
const uint16 WordClass_Determiner               =  0xD00;
const uint16 WordClass_Particle                 =  0xE00;
const uint16 WordClass_Adverb                   =  0xF00;

// Transitions: uint8 -- leading 5 bits
 const uint8 Transitions_EndBit                 =   0x10;
 const uint8 Transitions_BeginningOfVerse       =   0x20;
 const uint8 Transitions_EndOfVerse             =   0x30;
 const uint8 Transitions_BeginningOfChapter     =   0x60;
 const uint8 Transitions_EndOfChapter           =   0x70;
 const uint8 Transitions_BeginningOfBook        =   0xE0;
 const uint8 Transitions_EndOfBook              =   0xF0;
 const uint8 Transitions_BeginningOfBible       =   0xE8;
 const uint8 Transitions_EndOfBible             =   0xF8;

// Segments: uint8 -- trailing 3 bits
const uint8 Segments_HardSegmentEnd             =   0x04; // . ? !
const uint8 Segments_CoreSegmentEnd             =   0x02; // :
const uint8 Segments_SoftSegmentEnd             =   0x01; // , ; ( ) --
const uint8 Segments_RealSegmentEnd             =   0x06; // . ? ! :

class AVXWritten
{
public:
    class AVXWrit {
    public:
        const uint16 strongs[4];
        const uint16 verse_idx;
        const uint16 word;
        const uint8  punc;
        const uint8  trans;
        const uint16 pnwc;
        const uint32 pos;
        const uint16 lemma;
    };

    AVXWritten()
    {
        ;
    }
    static AVXWrit const written_01[3066];
    static AVXWrit const written_02[2426];
    static AVXWrit const written_03[1718];
    static AVXWrit const written_04[2576];
    static AVXWrit const written_05[1918];
    static AVXWrit const written_06[1316];
    static AVXWrit const written_07[1236];
    static AVXWrit const written_08[170];
    static AVXWrit const written_9[1620];
    static AVXWrit const written_10[1390];
    static AVXWrit const written_11[1632];
    static AVXWrit const written_12[1438];
    static AVXWrit const written_13[1884];
    static AVXWrit const written_14[1644];
    static AVXWrit const written_15[560];
    static AVXWrit const written_16[812];
    static AVXWrit const written_17[334];
    static AVXWrit const written_18[2140];
    static AVXWrit const written_19[4922];
    static AVXWrit const written_20[1830];
    static AVXWrit const written_21[444];
    static AVXWrit const written_22[234];
    static AVXWrit const written_23[2584];
    static AVXWrit const written_24[2728];
    static AVXWrit const written_25[308];
    static AVXWrit const written_26[2546];
    static AVXWrit const written_27[714];
    static AVXWrit const written_28[394];
    static AVXWrit const written_29[146];
    static AVXWrit const written_30[292];
    static AVXWrit const written_31[42];
    static AVXWrit const written_32[96];
    static AVXWrit const written_33[210];
    static AVXWrit const written_34[94];
    static AVXWrit const written_35[112];
    static AVXWrit const written_36[106];
    static AVXWrit const written_37[76];
    static AVXWrit const written_38[422];
    static AVXWrit const written_39[110];
    static AVXWrit const written_40[2142];
    static AVXWrit const written_41[1356];
    static AVXWrit const written_42[2302];
    static AVXWrit const written_43[1758];
    static AVXWrit const written_44[2014];
    static AVXWrit const written_45[866];
    static AVXWrit const written_46[874];
    static AVXWrit const written_47[514];
    static AVXWrit const written_48[298];
    static AVXWrit const written_49[310];
    static AVXWrit const written_50[208];
    static AVXWrit const written_51[190];
    static AVXWrit const written_52[178];
    static AVXWrit const written_53[94];
    static AVXWrit const written_54[226];
    static AVXWrit const written_55[166];
    static AVXWrit const written_56[92];
    static AVXWrit const written_57[50];
    static AVXWrit const written_58[606];
    static AVXWrit const written_59[216];
    static AVXWrit const written_60[210];
    static AVXWrit const written_61[122];
    static AVXWrit const written_62[210];
    static AVXWrit const written_63[26];
    static AVXWrit const written_64[28];
    static AVXWrit const written_65[50];
    static AVXWrit const written_66[808];
};