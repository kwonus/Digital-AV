// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generated-Code directives.
// For example, these comments are wrapped in a pair of Generated-Coded directives.
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXVerseIndex_Rust_Edition    :u16 = 23108;
static AVXVerseIndex_SDK_ZEdition    :u16 = 23107;

static AVXVerseIndex_File: &'static str = "AV-Verse.ix";
static AVXVerseIndex_RecordLen   :usize =        4;
static AVXVerseIndex_RecordCnt   :usize =        0;
static AVXVerseIndex_FileLen     :usize =        0;
// < < < Generated-Code -- Metadata < < < //

struct AVXVerse {                                     // from Digital-AV.pdf
    book:     u8,
    chapter:  u8,
    verse:    u8,
    word_cnt: u8,
}

impl Default for AVXVerse {
    #[inline]
    fn default() -> AVXVerse {
        AVXVerse {
            book:  0,
            chapter: 0,
            verse:  0,
            word_cnt: 0,
        }
    }
}

// > > > Generated-Code -- Initialization > > > //
static verse_index: [AVXVerse; 0] = [
];
// < < < Generated-Code -- Initialization < < < //

/*
use crate::avx;
pub const fn get_verse_index(sdkdir: String) -> [AVXVerse; 31_102] {
    let index = verses;
}
*/

