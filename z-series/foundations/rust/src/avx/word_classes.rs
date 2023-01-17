// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generated-Code directives.
// For example, these comments are wrapped in a pair Generated-Coded directives.
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXWordClasses_Rust_Edition    :u16 = 23108;
static AVXWordClasses_SDK_ZEdition    :u16 = 23107;

static AVXWordClasses_File: &'static str = "AV-WordClass.dxi";
static AVXWordClasses_RecordLen   :usize =        0;
static AVXWordClasses_RecordCnt   :usize =       54;
static AVXWordClasses_FileLen     :usize =      836;
// < < < Generated-Code -- Metadata < < < //

struct AVXWordClass {
    word_class: u16,
    pos: Vec<u32>,
}

// > > > Generated-Code -- Initialization > > > //
static word_classes: [AVXWordClass; 54] = [
	AVXWordClass{ word_class: 0x0000, pos: vec![ 0x00000000] },
	AVXWordClass{ word_class: 0x0010, pos: vec![ 0x00003950, 0x4000394E, 0x40075ACE, 0x40075AC7] },
	AVXWordClass{ word_class: 0x00E0, pos: vec![ 0x40080470] },
	AVXWordClass{ word_class: 0x0100, pos: vec![ 0x00005852, 0x00005849, 0x00005842, 0x00005882, 0x00005889, 0x00005AC9, 0x00005AC2, 0x0000585A, 0x00005ACE, 0x00005AC4, 0x000059A2, 0x00005884, 0x00005ADA, 0x00005904, 0x00005902, 0x0000584E, 0x0000588E, 0x0000589A, 0x0000591A, 0x00005909, 0x00005AC7, 0x00005847, 0x000059A4, 0x00005ADD, 0x00005887, 0x00005907, 0x000B0893, 0x000B349D, 0x000B3458] },
	AVXWordClass{ word_class: 0x0400, pos: vec![ 0x80004206] },
	AVXWordClass{ word_class: 0x0800, pos: vec![ 0x000002A8, 0x80005518, 0x8000550E, 0x800AA036, 0x800AA1D0, 0x81540E51] },
	AVXWordClass{ word_class: 0x0A00, pos: vec![ 0x0000000A, 0x00000318, 0x00000153, 0x00000143, 0x00000150, 0x40000155, 0x4000294E, 0x40055ACE, 0x40055AC7, 0x80050D4E, 0x80054D4E, 0x40AB59D5] },
	AVXWordClass{ word_class: 0x0B00, pos: vec![ 0x00000E44, 0x00003E44] },
	AVXWordClass{ word_class: 0x0C00, pos: vec![ 0x00000073, 0x00000063, 0x00000C78, 0x00000E74, 0x40018470, 0x80318470] },
	AVXWordClass{ word_class: 0x0D00, pos: vec![ 0x00000004, 0x00000094, 0x00000087, 0x00000083, 0x00000093, 0x00000098] },
	AVXWordClass{ word_class: 0x0F00, pos: vec![ 0x00000036, 0x800006C4, 0x800006CA, 0x800006D8, 0x800006C3, 0x40008470, 0x8000D94E, 0x8000D953, 0x8000D82E, 0x8000D883, 0x8000D893, 0x8000D943, 0x8000D898, 0x801B5ACE, 0x801B5AC7] },
	AVXWordClass{ word_class: 0x20C0, pos: vec![ 0x00083BBD] },
	AVXWordClass{ word_class: 0x3020, pos: vec![ 0x00000209, 0x00004138] },
	AVXWordClass{ word_class: 0x3028, pos: vec![ 0x00004127] },
	AVXWordClass{ word_class: 0x30C0, pos: vec![ 0x00083BDC] },
	AVXWordClass{ word_class: 0x3100, pos: vec![ 0x0000590E] },
	AVXWordClass{ word_class: 0x3E00, pos: vec![ 0x81018470] },
	AVXWordClass{ word_class: 0x4010, pos: vec![ 0x000001DC, 0x8007702E, 0x1DCE585A] },
	AVXWordClass{ word_class: 0x4018, pos: vec![ 0x000038FC, 0x00E540FC, 0xC0E3F14E, 0xDC7E5ACE] },
	AVXWordClass{ word_class: 0x4028, pos: vec![ 0x00083F9C] },
	AVXWordClass{ word_class: 0x4030, pos: vec![ 0x00003A1C] },
	AVXWordClass{ word_class: 0x4038, pos: vec![ 0x000740FC] },
	AVXWordClass{ word_class: 0x4080, pos: vec![ 0x01074F9C, 0x01074FBC] },
	AVXWordClass{ word_class: 0x40C0, pos: vec![ 0x01073F9C] },
	AVXWordClass{ word_class: 0x4100, pos: vec![ 0x0008639C] },
	AVXWordClass{ word_class: 0x4120, pos: vec![ 0x0000584D] },
	AVXWordClass{ word_class: 0x6028, pos: vec![ 0x00083FBC] },
	AVXWordClass{ word_class: 0x60C0, pos: vec![ 0x01073FBC] },
	AVXWordClass{ word_class: 0x60C8, pos: vec![ 0x01071FBC] },
	AVXWordClass{ word_class: 0x6100, pos: vec![ 0x0000591D, 0x000059BD, 0x000B0BB2, 0x000B109D, 0x000B209D, 0x000B589D, 0x000863BC] },
	AVXWordClass{ word_class: 0x7028, pos: vec![ 0x00083FDC] },
	AVXWordClass{ word_class: 0x7080, pos: vec![ 0x01074FDC] },
	AVXWordClass{ word_class: 0x70C0, pos: vec![ 0x01073FDC] },
	AVXWordClass{ word_class: 0x70C8, pos: vec![ 0x01071FDC] },
	AVXWordClass{ word_class: 0x7100, pos: vec![ 0x0000589D, 0x000863DC] },
	AVXWordClass{ word_class: 0x8010, pos: vec![ 0x000001DD, 0x00003A1D, 0x80003BAA, 0x00072A1D, 0x8007754E, 0x80EEDAC7, 0x80EED887] },
	AVXWordClass{ word_class: 0x8018, pos: vec![ 0x000038FD, 0x00E540FD, 0xC0E3F54E] },
	AVXWordClass{ word_class: 0x8028, pos: vec![ 0x00083F9D] },
	AVXWordClass{ word_class: 0x8080, pos: vec![ 0x01074F9D] },
	AVXWordClass{ word_class: 0x80C0, pos: vec![ 0x01073F9D] },
	AVXWordClass{ word_class: 0x80C8, pos: vec![ 0x01071F9D] },
	AVXWordClass{ word_class: 0x8100, pos: vec![ 0x000B0892, 0x0008639D, 0x016113B3, 0x016113B2] },
	AVXWordClass{ word_class: 0xA028, pos: vec![ 0x00083FBD] },
	AVXWordClass{ word_class: 0xA0C8, pos: vec![ 0x01071FBD] },
	AVXWordClass{ word_class: 0xA100, pos: vec![ 0x000863BD] },
	AVXWordClass{ word_class: 0xB020, pos: vec![ 0x0000413D] },
	AVXWordClass{ word_class: 0xB028, pos: vec![ 0x00083FDD] },
	AVXWordClass{ word_class: 0xB080, pos: vec![ 0x01074FDD] },
	AVXWordClass{ word_class: 0xB0C0, pos: vec![ 0x01073FDD] },
	AVXWordClass{ word_class: 0xB0C8, pos: vec![ 0x01071FDD] },
	AVXWordClass{ word_class: 0xB100, pos: vec![ 0x000863DD] },
	AVXWordClass{ word_class: 0xC010, pos: vec![ 0x40070E51] },
	AVXWordClass{ word_class: 0xC020, pos: vec![ 0x40088E51, 0x40090E51, 0x81278E51, 0x81178E51, 0x81238E51] },
	AVXWordClass{ word_class: 0xCC00, pos: vec![ 0x40018E51] },
];
// < < < Generated-Code -- Initialization < < < //

