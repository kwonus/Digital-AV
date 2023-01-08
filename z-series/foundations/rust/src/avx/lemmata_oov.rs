// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sore NOT to add/change code into Generate-Code directives (see the following 4 lines as an example:
// unless wrapped in pair comment-directives such as [exactly as] the comments surronding these comments
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXLemmataOOV: &'static str = "AV-lemma-OOV.dxi"; // from AV-Inventory-Z31.bom
static AVLemmaOOV_RecordCnt :usize =   771;              // from AV-Inventory-Z31.bom
static AVXLemmataOOV_FileLen:usize = 7_754;              // from AV-Inventory-Z31.bom
// < < < Generated-Code -- Metadata < < < //

// OOV: uint16                                           // from Digital-AV.pdf
static OOV_Marker :u16 = 0x8000;
static OOV_length :u16 = 0x0F00;
static OOV_index  :u16 = 0x000F;

struct AVXLemmataOOV{                                    // from Digital-AV.pdf
    key:  uint16;
    word: string;
}

// > > > Generated-Code -- Initialization > > > //

// < < < Generated-Code -- Initialization < < < //
