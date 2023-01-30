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
        const uint8  b;
        const uint8  c;
        const uint8  v;
        const uint8  wc;
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
    static AVXWrit const written_01[38262];
    static AVXWrit const written_02[32685];
    static AVXWrit const written_03[24541];
    static AVXWrit const written_04[32896];
    static AVXWrit const written_05[28352];
    static AVXWrit const written_06[18854];
    static AVXWrit const written_07[18966];
    static AVXWrit const written_08[2574];
    static AVXWrit const written_09[25048];
    static AVXWrit const written_10[20600];
    static AVXWrit const written_11[24513];
    static AVXWrit const written_12[23517];
    static AVXWrit const written_13[20365];
    static AVXWrit const written_14[26069];
    static AVXWrit const written_15[7440];
    static AVXWrit const written_16[10480];
    static AVXWrit const written_17[5633];
    static AVXWrit const written_18[18098];
    static AVXWrit const written_19[42704];
    static AVXWrit const written_20[15038];
    static AVXWrit const written_21[5579];
    static AVXWrit const written_22[2658];
    static AVXWrit const written_23[37036];
    static AVXWrit const written_24[42654];
    static AVXWrit const written_25[3411];
    static AVXWrit const written_26[39401];
    static AVXWrit const written_27[11602];
    static AVXWrit const written_28[5174];
    static AVXWrit const written_29[2033];
    static AVXWrit const written_30[4216];
    static AVXWrit const written_31[669];
    static AVXWrit const written_32[1320];
    static AVXWrit const written_33[3152];
    static AVXWrit const written_34[1284];
    static AVXWrit const written_35[1475];
    static AVXWrit const written_36[1616];
    static AVXWrit const written_37[1130];
    static AVXWrit const written_38[6443];
    static AVXWrit const written_39[1781];
    static AVXWrit const written_40[23684];
    static AVXWrit const written_41[15166];
    static AVXWrit const written_42[25939];
    static AVXWrit const written_43[19094];
    static AVXWrit const written_44[24245];
    static AVXWrit const written_45[9422];
    static AVXWrit const written_46[9462];
    static AVXWrit const written_47[6065];
    static AVXWrit const written_48[3084];
    static AVXWrit const written_49[3022];
    static AVXWrit const written_50[2183];
    static AVXWrit const written_51[1979];
    static AVXWrit const written_52[1837];
    static AVXWrit const written_53[1022];
    static AVXWrit const written_54[2244];
    static AVXWrit const written_55[1666];
    static AVXWrit const written_56[896];
    static AVXWrit const written_57[430];
    static AVXWrit const written_58[6897];
    static AVXWrit const written_59[2304];
    static AVXWrit const written_60[2476];
    static AVXWrit const written_61[1553];
    static AVXWrit const written_62[2517];
    static AVXWrit const written_63[298];
    static AVXWrit const written_64[294];
    static AVXWrit const written_65[608];
    static AVXWrit const written_66[11995];

    static AVXWrit const *written[67];
};