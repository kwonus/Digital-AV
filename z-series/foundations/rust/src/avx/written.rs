// This file is NOT code-generated, but it should be validated against current SDK docs

static AVXWritten_Rust_Edition    :u16 = 23108;
static AVXWritten_SDK_ZEdition    :u16 = 23107;

static AVXWritten_File: &'static str   = "AV-Writ.dx";
static AVXWritten_RecordLen     :usize =       22;
static AVXWritten_RecordCnt     :usize =   789651;
static AVXWritten_FileLen       :usize = 17372322;

static WordKeyBits_CAPS                  :u16 = 0xC000; // leading 2 bits
static WordKeyBits_CAPS_FirstLetter      :u16 = 0x8000;
static WordKeyBits_CAPS_AllLetters       :u16 = 0x4000;
static WordKeyBits_WordKey               :u16 = 0x3FFF; // trailing 14 bits

static Puncutation_Clause                 :u8 =   0xE0;
static Puncutation_Exclamatory            :u8 =   0x80;
static Puncutation_Interrogative          :u8 =   0xC0;
static Puncutation_Declarative            :u8 =   0xE0;
static Puncutation_Dash                   :u8 =   0xA0;
static Puncutation_Semicolon              :u8 =   0x20;
static Puncutation_Comma                  :u8 =   0x40;
static Puncutation_Colon                  :u8 =   0x60;
static Puncutation_Possessive             :u8 =   0x10;
static Puncutation_CloseParen             :u8 =   0x0C;
static Puncutation_Parenthetical          :u8 =   0x04;
static Puncutation_Italics                :u8 =   0x02;
static Puncutation_Jesus                  :u8 =   0x01;

static PersonMumber_PersonBits           :u16 = 0x3000;
static PersonMumber_NumberBits           :u16 = 0xC000;
static PersonMumber_Indefinite           :u16 = 0x0000;
static PersonMumber_Person1st            :u16 = 0x1000;
static PersonMumber_Person2nd            :u16 = 0x2000;
static PersonMumber_Person3rd            :u16 = 0x3000;
static PersonMumber_Singular             :u16 = 0x4000;
static PersonMumber_Plural               :u16 = 0x8000;
static PersonMumber_WH                   :u16 = 0xC000;

// WordClass: uint16 -- trailing 12 bits
static WordClass_NounOrPronoun           :u16 =  0x030;
static WordClass_Noun                    :u16 =  0x010;
static WordClass_Noun_nknownGender       :u16 =  0x010;
static WordClass_ProperNoun              :u16 =  0x030;
static WordClass_Pronoun                 :u16 =  0x020;
static WordClass_Pronoun_Neuter          :u16 =  0x021;
static WordClass_Pronoun_Masculine       :u16 =  0x022;
static WordClass_Pronoun_NonFeminine     :u16 =  0x023;
static WordClass_Pronoun_Feminine        :u16 =  0x024;
static WordClass_PronounOrNoun_Genitive  :u16 =  0x008;
static WordClass_Pronoun_Nominative      :u16 =  0x060;
static WordClass_Pronoun_Objective       :u16 =  0x0A0;
static WordClass_Pronoun_Reflexive       :u16 =  0x0E0;
static WordClass_Pronoun_NoCase_NoGender :u16 =  0x020;
static WordClass_Verb                    :u16 =  0x100;
static WordClass_To                      :u16 =  0x200;
static WordClass_Preposition             :u16 =  0x400;
static WordClass_Interjection            :u16 =  0x800;
static WordClass_Adjective               :u16 =  0xA00;
static WordClass_Numeric                 :u16 =  0xB00;
static WordClass_Conjunction             :u16 =  0xC00;
static WordClass_Determiner              :u16 =  0xD00;
static WordClass_Particle                :u16 =  0xE00;
static WordClass_Adverb                  :u16 =  0xF00;

// Transitions: uint8 -- leading 5 bits
static Transitions_EndBit                 :u8 =   0x10;
static Transitions_BeginningOfVerse       :u8 =   0x20;
static Transitions_EndOfVerse             :u8 =   0x30;
static Transitions_BeginningOfChapter     :u8 =   0x60;
static Transitions_EndOfChapter           :u8 =   0x70;
static Transitions_BeginningOfBook        :u8 =   0xE0;
static Transitions_EndOfBook              :u8 =   0xF0;
static Transitions_BeginningOfBible       :u8 =   0xE8;
static Transitions_EndOfBible             :u8 =   0xF8;

// Segments: uint8 -- trailing 3 bits
static Segments_HardSegmentEnd            :u8 =   0x04; // . ? !
static Segments_CoreSegmentEnd            :u8 =   0x02; // :
static Segments_SoftSegmentEnd            :u8 =   0x01; // , ; ( ) --
static Segments_RealSegmentEnd            :u8 =   0x06; // . ? ! :

struct AVXWrit {
    strongs:  [uint16; 4],
    verse_idx: uint16,
    word:      uint16,
    punc:      uint8,
    trans:     uint8,
    pnwc:      uint16,
    pos:       uint32,
    lemma:     uint16,
}
impl Default for AVXWrit {
    #[inline]
    fn default() -> AVXWrit {
        AVXWrit {
            strongs: [0,0,0,0],
            verse_idx: 0,
            word: 0,
            punc: 0,
            trans:0,
            pnwc: 0,
            pos: 0,
            lemma: 0,
        }
    }
}
