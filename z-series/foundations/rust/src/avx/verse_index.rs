// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sore NOT to add/change code into Generate-Code directives (see the following 4 lines as an example:
// unless wrapped in pair comment-directives such as [exactly as] the comments surronding these comments
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXVerseIndex: &'static str   = "AV-Verse.ix"; // from AV-Inventory-Z31.bom
static AVXVerseIndex_RecordLen:usize =       4;       // from AV-Inventory-Z31.bom
static AVXVerseIndex_RecordCnt:usize =  31_102;       // from AV-Inventory-Z31.bom
static AVXVerseIndex_FileLen  :usize = 124_408;       // from AV-Inventory-Z31.bom
// < < < Generated-Code -- Metadata < < < //

struct AVXVerse {                                     // from Digital-AV.pdf
    book:     uint8,
    chapter:  uint8,
    verse:    uint8,
    word_cnt: uint8,
}

impl Default for AVXVerse {
    #[inline]
    fn default() -> AVXVerse {
        AVXBook {
            book:  0,
            chapter: 0,
            verse:  0,
            word_cnt: 0,
        }
    }
}

// > > > Generated-Code -- Initialization > > > //

// < < < Generated-Code -- Initialization < < < //

pub const fn get_chapter_index(sdkdir: string) -> [AVXVerse; 31_102] {
    let mut index: [AVXVerse; 31_102] = Default::default();
    for vs in index.iter().enumerate() {
        let vs::book     = 1;
        let vs::chapter  = 1;
        let vs::verse    = 1;
        let vs::word_cnt = 1;
    }
    return index;
}