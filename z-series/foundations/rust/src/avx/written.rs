// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generated-Code directives.
// For example, these comments are wrapped in a pair of Generated-Coded directives.
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXWritten_Rust_Edition    :u16 = 23108;
static AVXWritten_SDK_ZEdition    :u16 = 23107;

static AVXWritten_File: &'static str = "AV-Writ-22.dx";
static AVXWritten_RecordLen   :usize =       22;
static AVXWritten_RecordCnt   :usize =   789651;
static AVXWritten_FileLen     :usize = 17372322;
// < < < Generated-Code -- Metadata < < < //

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

pub struct AVXWrit {
    strongs:  [u16; 4],
    verse_idx: u16,
    word:      u16,
    punc:      u8,
    trans:     u8,
    pnwc:      u16,
    pos:       u32,
    lemma:     u16,
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

pub struct AVXWritItem {
    book: u8,
    written: [AVXWrit],
}

mod written_01;
mod written_02;
mod written_03;
mod written_04;
mod written_05;
mod written_06;
mod written_07;
mod written_08;
mod written_09;
mod written_10;
mod written_11;
mod written_12;
mod written_13;
mod written_14;
mod written_15;
mod written_16;
mod written_17;
mod written_18;
mod written_19;
mod written_20;
mod written_21;
mod written_22;
mod written_23;
mod written_24;
mod written_25;
mod written_26;
mod written_27;
mod written_28;
mod written_29;
mod written_30;
mod written_31;
mod written_32;
mod written_33;
mod written_34;
mod written_35;
mod written_36;
mod written_37;
mod written_38;
mod written_39;
mod written_40;
mod written_41;
mod written_42;
mod written_43;
mod written_44;
mod written_45;
mod written_46;
mod written_47;
mod written_48;
mod written_49;
mod written_50;
mod written_51;
mod written_52;
mod written_53;
mod written_54;
mod written_55;
mod written_56;
mod written_57;
mod written_58;
mod written_59;
mod written_60;
mod written_61;
mod written_62;
mod written_63;
mod written_64;
mod written_65;
mod written_66;
/*

// > > > Generated-Code -- Initialization > > > //
pub mod written_01;
pub mod written_02;
pub mod written_03;
pub mod written_04;
pub mod written_05;
pub mod written_06;
pub mod written_07;
pub mod written_08;
pub mod written_09;
pub mod written_10;
pub mod written_11;
pub mod written_12;
pub mod written_13;
pub mod written_14;
pub mod written_15;
pub mod written_16;
pub mod written_17;
pub mod written_18;
pub mod written_19;
pub mod written_20;
pub mod written_21;
pub mod written_22;
pub mod written_23;
pub mod written_24;
pub mod written_25;
pub mod written_26;
pub mod written_27;
pub mod written_28;
pub mod written_29;
pub mod written_30;
pub mod written_31;
pub mod written_32;
pub mod written_33;
pub mod written_34;
pub mod written_35;
pub mod written_36;
pub mod written_37;
pub mod written_38;
pub mod written_39;
pub mod written_40;
pub mod written_41;
pub mod written_42;
pub mod written_43;
pub mod written_44;
pub mod written_45;
pub mod written_46;
pub mod written_47;
pub mod written_48;
pub mod written_49;
pub mod written_50;
pub mod written_51;
pub mod written_52;
pub mod written_53;
pub mod written_54;
pub mod written_55;
pub mod written_56;
pub mod written_57;
pub mod written_58;
pub mod written_59;
pub mod written_60;
pub mod written_61;
pub mod written_62;
pub mod written_63;
pub mod written_64;
pub mod written_65;
pub mod written_66;

pub use written_01;
pub use written_02;
pub use written_03;
pub use written_04;
pub use written_05;
pub use written_06;
pub use written_07;
pub use written_08;
pub use written_09;
pub use written_10;
pub use written_11;
pub use written_12;
pub use written_13;
pub use written_14;
pub use written_15;
pub use written_16;
pub use written_17;
pub use written_18;
pub use written_19;
pub use written_20;
pub use written_21;
pub use written_22;
pub use written_23;
pub use written_24;
pub use written_25;
pub use written_26;
pub use written_27;
pub use written_28;
pub use written_29;
pub use written_30;
pub use written_31;
pub use written_32;
pub use written_33;
pub use written_34;
pub use written_35;
pub use written_36;
pub use written_37;
pub use written_38;
pub use written_39;
pub use written_40;
pub use written_41;
pub use written_42;
pub use written_43;
pub use written_44;
pub use written_45;
pub use written_46;
pub use written_47;
pub use written_48;
pub use written_49;
pub use written_50;
pub use written_51;
pub use written_52;
pub use written_53;
pub use written_54;
pub use written_55;
pub use written_56;
pub use written_57;
pub use written_58;
pub use written_59;
pub use written_60;
pub use written_61;
pub use written_62;
pub use written_63;
pub use written_64;
pub use written_65;
pub use written_66;

static  AVXWrittenAll: [ AVXWritItem ; 66] = [
	AVXWritItem { book: 1, written: written_01 },
	AVXWritItem { book: 2, written: written_02 },
	AVXWritItem { book: 3, written: written_03 },
	AVXWritItem { book: 4, written: written_04 },
	AVXWritItem { book: 5, written: written_05 },
	AVXWritItem { book: 6, written: written_06 },
	AVXWritItem { book: 7, written: written_07 },
	AVXWritItem { book: 8, written: written_08 },
	AVXWritItem { book: 9, written: written_09 },
	AVXWritItem { book:10, written: written_10 },
	AVXWritItem { book:11, written: written_11 },
	AVXWritItem { book:12, written: written_12 },
	AVXWritItem { book:13, written: written_13 },
	AVXWritItem { book:14, written: written_14 },
	AVXWritItem { book:15, written: written_15 },
	AVXWritItem { book:16, written: written_16 },
	AVXWritItem { book:17, written: written_17 },
	AVXWritItem { book:18, written: written_18 },
	AVXWritItem { book:19, written: written_19 },
	AVXWritItem { book:20, written: written_20 },
	AVXWritItem { book:21, written: written_21 },
	AVXWritItem { book:22, written: written_22 },
	AVXWritItem { book:23, written: written_23 },
	AVXWritItem { book:24, written: written_24 },
	AVXWritItem { book:25, written: written_25 },
	AVXWritItem { book:26, written: written_26 },
	AVXWritItem { book:27, written: written_27 },
	AVXWritItem { book:28, written: written_28 },
	AVXWritItem { book:29, written: written_29 },
	AVXWritItem { book:30, written: written_30 },
	AVXWritItem { book:31, written: written_31 },
	AVXWritItem { book:32, written: written_32 },
	AVXWritItem { book:33, written: written_33 },
	AVXWritItem { book:34, written: written_34 },
	AVXWritItem { book:35, written: written_35 },
	AVXWritItem { book:36, written: written_36 },
	AVXWritItem { book:37, written: written_37 },
	AVXWritItem { book:38, written: written_38 },
	AVXWritItem { book:39, written: written_39 },
	AVXWritItem { book:40, written: written_40 },
	AVXWritItem { book:41, written: written_41 },
	AVXWritItem { book:42, written: written_42 },
	AVXWritItem { book:43, written: written_43 },
	AVXWritItem { book:44, written: written_44 },
	AVXWritItem { book:45, written: written_45 },
	AVXWritItem { book:46, written: written_46 },
	AVXWritItem { book:47, written: written_47 },
	AVXWritItem { book:48, written: written_48 },
	AVXWritItem { book:49, written: written_49 },
	AVXWritItem { book:50, written: written_50 },
	AVXWritItem { book:51, written: written_51 },
	AVXWritItem { book:52, written: written_52 },
	AVXWritItem { book:53, written: written_53 },
	AVXWritItem { book:54, written: written_54 },
	AVXWritItem { book:55, written: written_55 },
	AVXWritItem { book:56, written: written_56 },
	AVXWritItem { book:57, written: written_57 },
	AVXWritItem { book:58, written: written_58 },
	AVXWritItem { book:59, written: written_59 },
	AVXWritItem { book:60, written: written_60 },
	AVXWritItem { book:61, written: written_61 },
	AVXWritItem { book:62, written: written_62 },
	AVXWritItem { book:63, written: written_63 },
	AVXWritItem { book:64, written: written_64 },
	AVXWritItem { book:65, written: written_65 },
	AVXWritItem { book:66, written: written_66 },
];
// < < < Generated-Code -- Initialization < < < //

*/
