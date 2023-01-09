// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generate-Code directives.
// For example, these comments are wrapped in a pair Generated-Code directives.
// < < < Generated-Code -- Header < < < //

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

// > > > Generated-Code -- Metadata > > > //
static AVXBookIndex_Rust_Edition    :u16 = 23108;
static AVXBookIndex_SDK_ZEdition    :u16 = 23107;

static AVXBookIndex_File: &'static str = "AV-Book.ix";
static AVXBookIndex_RecordLen   :usize =       32;
static AVXBookIndex_RecordCnt   :usize =       66;
static AVXBookIndex_FileLen     :usize =     2112;
// < < < Generated-Code -- Metadata < < < //

struct AVXBook {                                  // from Digital-AV.pdf
    num:            uint8,
    chapter_cnt:    uint8,
    chapter_idx:    uint16,
	verse_cnt:		uint16,
	verse_idx:		uint16,
	writ_idx:		uint32,
	writ_cnt:		uint32,
    name:           string,
    abbreviations:  [string; 3],
}

// > > > Generated-Code -- Initialization > > > //
static books: [AVXBook; 67] = [
	AVXBook{ num:  0, chapter_cnt:   0, chapter_idx:    0, verse_cnt:       0, verse_idx:       0, writ_cnt:         0, writ_idx:         0, name: "", abbreviations: [  "", "", "" ] },
	AVXBook{ num:  1, chapter_cnt:  50, chapter_idx:    0, verse_cnt:       0, verse_idx:       0, writ_cnt:     38262, writ_idx:         0, name: "Genesis", abbreviations: [  "Ge", "", "" ] },
	AVXBook{ num:  2, chapter_cnt:  40, chapter_idx:   50, verse_cnt:    1533, verse_idx:    1533, writ_cnt:     32685, writ_idx:     38262, name: "Exodus", abbreviations: [  "Ex", "", "" ] },
	AVXBook{ num:  3, chapter_cnt:  27, chapter_idx:   90, verse_cnt:    2746, verse_idx:    2746, writ_cnt:     24541, writ_idx:     70947, name: "Leviticus", abbreviations: [  "Le", "", "" ] },
	AVXBook{ num:  4, chapter_cnt:  36, chapter_idx:  117, verse_cnt:    3605, verse_idx:    3605, writ_cnt:     32896, writ_idx:     95488, name: "Numbers", abbreviations: [  "Nu", "", "" ] },
	AVXBook{ num:  5, chapter_cnt:  34, chapter_idx:  153, verse_cnt:    4893, verse_idx:    4893, writ_cnt:     28352, writ_idx:    128384, name: "Deuteronomy", abbreviations: [  "De", "", "" ] },
	AVXBook{ num:  6, chapter_cnt:  24, chapter_idx:  187, verse_cnt:    5852, verse_idx:    5852, writ_cnt:     18854, writ_idx:    156736, name: "Joshua", abbreviations: [  "Jos", "", "" ] },
	AVXBook{ num:  7, chapter_cnt:  21, chapter_idx:  211, verse_cnt:    6510, verse_idx:    6510, writ_cnt:     18966, writ_idx:    175590, name: "Judges", abbreviations: [  "j'd", "Ju", "" ] },
	AVXBook{ num:  8, chapter_cnt:   4, chapter_idx:  232, verse_cnt:    7128, verse_idx:    7128, writ_cnt:      2574, writ_idx:    194556, name: "Ruth", abbreviations: [  "Ru", "", "" ] },
	AVXBook{ num:  9, chapter_cnt:  31, chapter_idx:  236, verse_cnt:    7213, verse_idx:    7213, writ_cnt:     25048, writ_idx:    197130, name: "1 Samuel", abbreviations: [  "1Sa", "", "" ] },
	AVXBook{ num: 10, chapter_cnt:  24, chapter_idx:  267, verse_cnt:    8023, verse_idx:    8023, writ_cnt:     20600, writ_idx:    222178, name: "2 Samuel", abbreviations: [  "2Sa", "", "" ] },
	AVXBook{ num: 11, chapter_cnt:  22, chapter_idx:  291, verse_cnt:    8718, verse_idx:    8718, writ_cnt:     24513, writ_idx:    242778, name: "1 Kings", abbreviations: [  "1Ki", "", "" ] },
	AVXBook{ num: 12, chapter_cnt:  25, chapter_idx:  313, verse_cnt:    9534, verse_idx:    9534, writ_cnt:     23517, writ_idx:    267291, name: "2 Kings", abbreviations: [  "2Ki", "", "" ] },
	AVXBook{ num: 13, chapter_cnt:  29, chapter_idx:  338, verse_cnt:   10253, verse_idx:   10253, writ_cnt:     20365, writ_idx:    290808, name: "1 Chronicles", abbreviations: [  "1Ch", "", "" ] },
	AVXBook{ num: 14, chapter_cnt:  36, chapter_idx:  367, verse_cnt:   11195, verse_idx:   11195, writ_cnt:     26069, writ_idx:    311173, name: "2 Chronicles", abbreviations: [  "2Ch", "", "" ] },
	AVXBook{ num: 15, chapter_cnt:  10, chapter_idx:  403, verse_cnt:   12017, verse_idx:   12017, writ_cnt:      7440, writ_idx:    337242, name: "Ezra", abbreviations: [  "Ezr", "", "" ] },
	AVXBook{ num: 16, chapter_cnt:  13, chapter_idx:  413, verse_cnt:   12297, verse_idx:   12297, writ_cnt:     10480, writ_idx:    344682, name: "Nehemiah", abbreviations: [  "Ne", "", "" ] },
	AVXBook{ num: 17, chapter_cnt:  10, chapter_idx:  426, verse_cnt:   12703, verse_idx:   12703, writ_cnt:      5633, writ_idx:    355162, name: "Esther", abbreviations: [  "Es", "", "" ] },
	AVXBook{ num: 18, chapter_cnt:  42, chapter_idx:  436, verse_cnt:   12870, verse_idx:   12870, writ_cnt:     18098, writ_idx:    360795, name: "Job", abbreviations: [  "Jo", "", "" ] },
	AVXBook{ num: 19, chapter_cnt: 150, chapter_idx:  478, verse_cnt:   13940, verse_idx:   13940, writ_cnt:     42704, writ_idx:    378893, name: "Psalms", abbreviations: [  "Ps", "", "" ] },
	AVXBook{ num: 20, chapter_cnt:  31, chapter_idx:  628, verse_cnt:   16401, verse_idx:   16401, writ_cnt:     15038, writ_idx:    421597, name: "Proverbs", abbreviations: [  "Pr", "", "" ] },
	AVXBook{ num: 21, chapter_cnt:  12, chapter_idx:  659, verse_cnt:   17316, verse_idx:   17316, writ_cnt:      5579, writ_idx:    436635, name: "Ecclesiastes", abbreviations: [  "Ec", "", "" ] },
	AVXBook{ num: 22, chapter_cnt:   8, chapter_idx:  671, verse_cnt:   17538, verse_idx:   17538, writ_cnt:      2658, writ_idx:    442214, name: "Song of Solomon", abbreviations: [  "Ca", "So", "SoS" ] },
	AVXBook{ num: 23, chapter_cnt:  66, chapter_idx:  679, verse_cnt:   17655, verse_idx:   17655, writ_cnt:     37036, writ_idx:    444872, name: "Isaiah", abbreviations: [  "Is", "", "" ] },
	AVXBook{ num: 24, chapter_cnt:  52, chapter_idx:  745, verse_cnt:   18947, verse_idx:   18947, writ_cnt:     42654, writ_idx:    481908, name: "Jeremiah", abbreviations: [  "Je", "", "" ] },
	AVXBook{ num: 25, chapter_cnt:   5, chapter_idx:  797, verse_cnt:   20311, verse_idx:   20311, writ_cnt:      3411, writ_idx:    524562, name: "Lamentations", abbreviations: [  "La", "", "" ] },
	AVXBook{ num: 26, chapter_cnt:  48, chapter_idx:  802, verse_cnt:   20465, verse_idx:   20465, writ_cnt:     39401, writ_idx:    527973, name: "Ezekiel", abbreviations: [  "Eze", "", "" ] },
	AVXBook{ num: 27, chapter_cnt:  12, chapter_idx:  850, verse_cnt:   21738, verse_idx:   21738, writ_cnt:     11602, writ_idx:    567374, name: "Daniel", abbreviations: [  "Da", "", "" ] },
	AVXBook{ num: 28, chapter_cnt:  14, chapter_idx:  862, verse_cnt:   22095, verse_idx:   22095, writ_cnt:      5174, writ_idx:    578976, name: "Hosea", abbreviations: [  "Ho", "", "" ] },
	AVXBook{ num: 29, chapter_cnt:   3, chapter_idx:  876, verse_cnt:   22292, verse_idx:   22292, writ_cnt:      2033, writ_idx:    584150, name: "Joel", abbreviations: [  "Joe", "", "" ] },
	AVXBook{ num: 30, chapter_cnt:   9, chapter_idx:  879, verse_cnt:   22365, verse_idx:   22365, writ_cnt:      4216, writ_idx:    586183, name: "Amos", abbreviations: [  "Am", "", "" ] },
	AVXBook{ num: 31, chapter_cnt:   1, chapter_idx:  888, verse_cnt:   22511, verse_idx:   22511, writ_cnt:       669, writ_idx:    590399, name: "Obadiah", abbreviations: [  "Ob", "", "" ] },
	AVXBook{ num: 32, chapter_cnt:   4, chapter_idx:  889, verse_cnt:   22532, verse_idx:   22532, writ_cnt:      1320, writ_idx:    591068, name: "Jonah", abbreviations: [  "Jon", "", "" ] },
	AVXBook{ num: 33, chapter_cnt:   7, chapter_idx:  893, verse_cnt:   22580, verse_idx:   22580, writ_cnt:      3152, writ_idx:    592388, name: "Micah", abbreviations: [  "Mic", "", "" ] },
	AVXBook{ num: 34, chapter_cnt:   3, chapter_idx:  900, verse_cnt:   22685, verse_idx:   22685, writ_cnt:      1284, writ_idx:    595540, name: "Nahum", abbreviations: [  "Na", "", "" ] },
	AVXBook{ num: 35, chapter_cnt:   3, chapter_idx:  903, verse_cnt:   22732, verse_idx:   22732, writ_cnt:      1475, writ_idx:    596824, name: "Habakkuk", abbreviations: [  "Hab", "", "" ] },
	AVXBook{ num: 36, chapter_cnt:   3, chapter_idx:  906, verse_cnt:   22788, verse_idx:   22788, writ_cnt:      1616, writ_idx:    598299, name: "Zephaniah", abbreviations: [  "Zep", "", "" ] },
	AVXBook{ num: 37, chapter_cnt:   2, chapter_idx:  909, verse_cnt:   22841, verse_idx:   22841, writ_cnt:      1130, writ_idx:    599915, name: "Haggai", abbreviations: [  "Hag", "", "" ] },
	AVXBook{ num: 38, chapter_cnt:  14, chapter_idx:  911, verse_cnt:   22879, verse_idx:   22879, writ_cnt:      6443, writ_idx:    601045, name: "Zechariah", abbreviations: [  "Zec", "", "" ] },
	AVXBook{ num: 39, chapter_cnt:   4, chapter_idx:  925, verse_cnt:   23090, verse_idx:   23090, writ_cnt:      1781, writ_idx:    607488, name: "Malachi", abbreviations: [  "Mal", "", "" ] },
	AVXBook{ num: 40, chapter_cnt:  28, chapter_idx:  929, verse_cnt:   23145, verse_idx:   23145, writ_cnt:     23684, writ_idx:    609269, name: "Matthew", abbreviations: [  "Mat", "Mt", "" ] },
	AVXBook{ num: 41, chapter_cnt:  16, chapter_idx:  957, verse_cnt:   24216, verse_idx:   24216, writ_cnt:     15166, writ_idx:    632953, name: "Mark", abbreviations: [  "Mar", "Mk", "" ] },
	AVXBook{ num: 42, chapter_cnt:  24, chapter_idx:  973, verse_cnt:   24894, verse_idx:   24894, writ_cnt:     25939, writ_idx:    648119, name: "Luke", abbreviations: [  "Lu", "", "" ] },
	AVXBook{ num: 43, chapter_cnt:  21, chapter_idx:  997, verse_cnt:   26045, verse_idx:   26045, writ_cnt:     19094, writ_idx:    674058, name: "John", abbreviations: [  "Joh", "", "" ] },
	AVXBook{ num: 44, chapter_cnt:  28, chapter_idx: 1018, verse_cnt:   26924, verse_idx:   26924, writ_cnt:     24245, writ_idx:    693152, name: "Acts", abbreviations: [  "Ac", "", "" ] },
	AVXBook{ num: 45, chapter_cnt:  16, chapter_idx: 1046, verse_cnt:   27931, verse_idx:   27931, writ_cnt:      9422, writ_idx:    717397, name: "Romans", abbreviations: [  "Ro", "", "" ] },
	AVXBook{ num: 46, chapter_cnt:  16, chapter_idx: 1062, verse_cnt:   28364, verse_idx:   28364, writ_cnt:      9462, writ_idx:    726819, name: "1 Corinthians", abbreviations: [  "1Co", "", "" ] },
	AVXBook{ num: 47, chapter_cnt:  13, chapter_idx: 1078, verse_cnt:   28801, verse_idx:   28801, writ_cnt:      6065, writ_idx:    736281, name: "2 Corinthians", abbreviations: [  "2Co", "", "" ] },
	AVXBook{ num: 48, chapter_cnt:   6, chapter_idx: 1091, verse_cnt:   29058, verse_idx:   29058, writ_cnt:      3084, writ_idx:    742346, name: "Galatians", abbreviations: [  "Ga", "", "" ] },
	AVXBook{ num: 49, chapter_cnt:   6, chapter_idx: 1097, verse_cnt:   29207, verse_idx:   29207, writ_cnt:      3022, writ_idx:    745430, name: "Ephesians", abbreviations: [  "Eph", "", "" ] },
	AVXBook{ num: 50, chapter_cnt:   4, chapter_idx: 1103, verse_cnt:   29362, verse_idx:   29362, writ_cnt:      2183, writ_idx:    748452, name: "Philippians", abbreviations: [  "phili", "Ph'p", "" ] },
	AVXBook{ num: 51, chapter_cnt:   4, chapter_idx: 1107, verse_cnt:   29466, verse_idx:   29466, writ_cnt:      1979, writ_idx:    750635, name: "Colossians", abbreviations: [  "Co", "", "" ] },
	AVXBook{ num: 52, chapter_cnt:   5, chapter_idx: 1111, verse_cnt:   29561, verse_idx:   29561, writ_cnt:      1837, writ_idx:    752614, name: "1 Thessalonians", abbreviations: [  "1Th", "", "" ] },
	AVXBook{ num: 53, chapter_cnt:   3, chapter_idx: 1116, verse_cnt:   29650, verse_idx:   29650, writ_cnt:      1022, writ_idx:    754451, name: "2 Thessalonians", abbreviations: [  "2Th", "", "" ] },
	AVXBook{ num: 54, chapter_cnt:   6, chapter_idx: 1119, verse_cnt:   29697, verse_idx:   29697, writ_cnt:      2244, writ_idx:    755473, name: "1 Timothy", abbreviations: [  "1Ti", "", "" ] },
	AVXBook{ num: 55, chapter_cnt:   4, chapter_idx: 1125, verse_cnt:   29810, verse_idx:   29810, writ_cnt:      1666, writ_idx:    757717, name: "2 Timothy", abbreviations: [  "2Ti", "", "" ] },
	AVXBook{ num: 56, chapter_cnt:   3, chapter_idx: 1129, verse_cnt:   29893, verse_idx:   29893, writ_cnt:       896, writ_idx:    759383, name: "Titus", abbreviations: [  "Tit", "", "" ] },
	AVXBook{ num: 57, chapter_cnt:   1, chapter_idx: 1132, verse_cnt:   29939, verse_idx:   29939, writ_cnt:       430, writ_idx:    760279, name: "Philemon", abbreviations: [  "Ph'", "phile", "" ] },
	AVXBook{ num: 58, chapter_cnt:  13, chapter_idx: 1133, verse_cnt:   29964, verse_idx:   29964, writ_cnt:      6897, writ_idx:    760709, name: "Hebrews", abbreviations: [  "Heb", "", "" ] },
	AVXBook{ num: 59, chapter_cnt:   5, chapter_idx: 1146, verse_cnt:   30267, verse_idx:   30267, writ_cnt:      2304, writ_idx:    767606, name: "James", abbreviations: [  "Ja", "j'm", "" ] },
	AVXBook{ num: 60, chapter_cnt:   5, chapter_idx: 1151, verse_cnt:   30375, verse_idx:   30375, writ_cnt:      2476, writ_idx:    769910, name: "1 Peter", abbreviations: [  "1P", "", "" ] },
	AVXBook{ num: 61, chapter_cnt:   3, chapter_idx: 1156, verse_cnt:   30480, verse_idx:   30480, writ_cnt:      1553, writ_idx:    772386, name: "2 Peter", abbreviations: [  "2P", "", "" ] },
	AVXBook{ num: 62, chapter_cnt:   5, chapter_idx: 1159, verse_cnt:   30541, verse_idx:   30541, writ_cnt:      2517, writ_idx:    773939, name: "1 John", abbreviations: [  "1J", "", "" ] },
	AVXBook{ num: 63, chapter_cnt:   1, chapter_idx: 1164, verse_cnt:   30646, verse_idx:   30646, writ_cnt:       298, writ_idx:    776456, name: "2 John", abbreviations: [  "2J", "", "" ] },
	AVXBook{ num: 64, chapter_cnt:   1, chapter_idx: 1165, verse_cnt:   30659, verse_idx:   30659, writ_cnt:       294, writ_idx:    776754, name: "3 John", abbreviations: [  "3J", "", "" ] },
	AVXBook{ num: 65, chapter_cnt:   1, chapter_idx: 1166, verse_cnt:   30673, verse_idx:   30673, writ_cnt:       608, writ_idx:    777048, name: "Jude", abbreviations: [  "Ju", "", "" ] },
	AVXBook{ num: 66, chapter_cnt:  22, chapter_idx: 1167, verse_cnt:   30698, verse_idx:   30698, writ_cnt:     11995, writ_idx:    777656, name: "Revelation", abbreviations: [  "Re", "", "" ] },
];
// < < < Generated-Code -- Initialization < < < //

pub const fn get_book_index(sdkdir: string) -> [AVXBook; 66] {
    let index: avxlib::avx::book_index::books;
}
