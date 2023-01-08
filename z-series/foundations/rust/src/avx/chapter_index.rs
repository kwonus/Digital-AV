// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sore NOT to add/change code into Generate-Code directives (see the following 4 lines as an example:
// unless wrapped in pair comment-directives such as [exactly as] the comments surronding these comments
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXChapterIndex: &'static str   = "AV-Chapter.ix";  // from AV-Inventory-Z31.bom
static AVXChapterIndex_RecordLen:usize =     8;            // from AV-Inventory-Z31.bom
static AVXBookIndex_RecordCnt   :usize = 1_189;            // from AV-Inventory-Z31.bom
static AVXChapterIndex_FileLen  :usize = 9_512;            // from AV-Inventory-Z31.bom
// < < < Generated-Code -- Metadata < < < //

struct AVXChapter {
    writ_idx:    uint32,
    verse_idx:   uint16,
    word_cnt:    uint16,
}

impl Default for AVXChapter {
    #[inline]
    fn default() -> AVXChapter {
        AVXBook {
            writ_idx:  0,
            verse_idx: 0,
            word_cnt:  0,
        }
    }
}

// > > > Generated-Code -- Initialization > > > //

// < < < Generated-Code -- Initialization < < < //

pub const fn get_chapter_index(sdkdir: string) -> [AVChapter; 1189] {
    let mut index: [AVChapter; 1189] = Default::default();
    for ch in index.iter().enumerate() {
        let ch::writ_idx  = 1;
        let ch::verse_idx = 1;
        let ch::word_cnt  = 1;
    }
    return index;
}
