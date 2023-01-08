// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sore NOT to add/change code into Generate-Code directives (see the following 4 lines as an example:
// unless wrapped in pair comment-directives such as [exactly as] the comments surronding these comments
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXBookIndex: &'static str = "AV-Book.ix"; // from AV-Inventory-Z31.bom
static AVXBookIndex_RecordLen:usize =    32;      // from AV-Inventory-Z31.bom
static AVXBookIndex_RecordCnt:usize =    66;      // from AV-Inventory-Z31.bom
static AVXBookIndex_FileLen  :usize = 2_112;      // from AV-Inventory-Z31.bom
// < < < Generated-Code -- Metadata < < < //

struct AVXBook {                                  // from Digital-AV.pdf
    num:            uint8,
    chapter_cnt:    uint8,
    chapter_idx:    uint16,
    name:           [char; 16],
    abbreviations:  [char; 12],
}
impl Default for AVXBook {
    #[inline]
    fn default() -> AVXBook {
        AVXBook {
            num: 0,
            chapter_cnt: 0,
            chapter_idx: 0,
            name:  ['\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0'],
            abbreviations:             ['\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0','\0'],
        }
    }
}

// > > > Generated-Code -- Initialization > > > //

// < < < Generated-Code -- Initialization < < < //

pub const fn get_book_index(sdkdir: string) -> [AVXBook; 66] {
    let mut index: [AVBook; 66] = Default::default();
    let mut num: uint8 = 1;
    for bk in index.iter().enumerate() {
        let bk::num = num;
        let num = num + 1;

        let vs::chapter_cnt = 1;
        let vs::chapter_idx = 1;
        //let vs::name;
        //let vs::word_cnt;
    }
    return index;
}