// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generated-Code directives.
// For example, these comments are wrapped in a pair of Generated-Coded directives.
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXChapterIndex_Rust_Edition    :u16 = 23108;
static AVXChapterIndex_SDK_ZEdition    :u16 = 23107;

static AVXChapterIndex_File: &'static str = "AV-Chapter-10.ix";
static AVXChapterIndex_RecordLen   :usize =       10;
static AVXChapterIndex_RecordCnt   :usize =     1189;
static AVXChapterIndex_FileLen     :usize =     7134;
// < < < Generated-Code -- Metadata < < < //

struct AVXChapter {
    writ_idx:    u16,
	writ_cnt:    u16,
    verse_idx:   u16,
	verse_cnt:   u8,		// uint16 in binary-file baseline SDK asset
}

impl Default for AVXChapter {
    #[inline]
    fn default() -> AVXChapter {
		AVXChapter {
            writ_idx:  0,
			writ_cnt:  0,
			verse_idx: 0,
            verse_cnt: 0,
        }
    }
}

// > > > Generated-Code -- Initialization > > > //
static chapter_index: [AVXChapter; 1189] = [
];
// < < < Generated-Code -- Initialization < < < //

/*
use crate::avx::book_index;
pub const fn get_chapter_index(sdkdir: String) -> [AVXChapter; 1189] {
    let index = chapter_index;
}
*/
