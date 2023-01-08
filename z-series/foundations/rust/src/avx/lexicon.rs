// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generate-Code directives.
// For example, these comments are wrapped in a pair Generated-Code directives.
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXLexicon: &'static str    = "AV-Lexicon.dxi"; // from AV-Inventory-Z31.bom
static AVXLexicon_RecordCnt :usize =  12_567;          // from AV-Inventory-Z31.bom
static AVXLexicon_FileLen   :usize = 246_249;          // from AV-Inventory-Z31.bom
// < < < Generated-Code -- Metadata < < < //

// Entities: uint16
static Entity_Hitchcock    :u16 = 0x8000;
static Entity_men          :u16 =    0x1;
static Entity_women        :u16 =    0x2;
static Entity_tribes       :u16 =    0x4;
static Entity_cities       :u16 =    0x8;
static Entity_rivers       :u16 =   0x10;
static Entity_mountains    :u16 =   0x20;
static Entity_animals      :u16 =   0x40;
static Entity_gemstones    :u16 =   0x80;
static Entity_measurements :u16 =  0x100;

struct AVXLexItem {
    entities: u16;
    search:   string;
    display:  string;
    modern:   string;
    pos:      Vec<u32>;
}

// > > > Generated-Code -- Initialization > > > //

// < < < Generated-Code -- Initialization < < < //
