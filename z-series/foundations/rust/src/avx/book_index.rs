// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generated-Code directives.
// For example, these comments are wrapped in a pair Generated-Coded directives.
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXBookIndex_Rust_Edition    :u16 = 23108;
static AVXBookIndex_SDK_ZEdition    :u16 = 23107;

static AVXBookIndex_File: &'static str = "AV-Book.ix";
static AVXBookIndex_RecordLen   :usize =       50;
static AVXBookIndex_RecordCnt   :usize =       67;
static AVXBookIndex_FileLen     :usize =     3350;
// < < < Generated-Code -- Metadata < < < //

pub struct AVXBook { // from Digital-AV.pdf
    num:            u8,
    chapter_cnt:    u8,
    chapter_idx:    u16,
	verse_cnt:		u16,		// uint32 in binary-file baseline SDL asset,
	verse_idx:		u16,
	writ_idx:		u32,
	writ_cnt:		u16,		// uint32 in binary-file baseline SDK asset,
    name:           &'static str,
	abbr2:			&'static str,	// strlen == 2 || strlen == 0
	abbr3:			&'static str,	// strlen == 3
	abbr4:			&'static str,	// <-- Most common // use this for display // strlen <= 4
	abbrAltA:		&'static str,	// Alternate Abbreviation: unknown size
	abbrAltB:		&'static str,	// Alternate Abbreviation: unknown size
}

// > > > Generated-Code -- Initialization > > > //
static books: [AVXBook; 67] = [
	AVXBook{ num:  0, chapter_cnt:   0, chapter_idx:    0, verse_cnt:       0, verse_idx:       0, writ_cnt:         0, writ_idx:     12556, name: "Z31c", abbr2: "", abbr3: "", abbr4: "", abbrAltA: "", abbrAltB: "Revision",  },
	AVXBook{ num:  1, chapter_cnt:  50, chapter_idx:    0, verse_cnt:    3066, verse_idx:       0, writ_cnt:     38262, writ_idx:         0, name: "Genesis", abbr2: "Ge", abbr3: "Gen", abbr4: "Gen", abbrAltA: "Gn", abbrAltB: "",  },
	AVXBook{ num:  2, chapter_cnt:  40, chapter_idx:   50, verse_cnt:    2426, verse_idx:    1533, writ_cnt:     32685, writ_idx:     38262, name: "Exodus", abbr2: "Ex", abbr3: "Exo", abbr4: "Exo", abbrAltA: "Exod", abbrAltB: "",  },
	AVXBook{ num:  3, chapter_cnt:  27, chapter_idx:   90, verse_cnt:    1718, verse_idx:    2746, writ_cnt:     24541, writ_idx:     70947, name: "Leviticus", abbr2: "Le", abbr3: "Lev", abbr4: "Lev", abbrAltA: "Lv", abbrAltB: "",  },
	AVXBook{ num:  4, chapter_cnt:  36, chapter_idx:  117, verse_cnt:    2576, verse_idx:    3605, writ_cnt:     32896, writ_idx:     95488, name: "Numbers", abbr2: "Nu", abbr3: "Num", abbr4: "Numb", abbrAltA: "Nb", abbrAltB: "",  },
	AVXBook{ num:  5, chapter_cnt:  34, chapter_idx:  153, verse_cnt:    1918, verse_idx:    4893, writ_cnt:     28352, writ_idx:    128384, name: "Deuteronomy", abbr2: "Dt", abbr3: "D't", abbr4: "Deut", abbrAltA: "De", abbrAltB: "",  },
	AVXBook{ num:  6, chapter_cnt:  24, chapter_idx:  187, verse_cnt:    1316, verse_idx:    5852, writ_cnt:     18854, writ_idx:    156736, name: "Joshua", abbr2: "Js", abbr3: "Jsh", abbr4: "Josh", abbrAltA: "Jos", abbrAltB: "",  },
	AVXBook{ num:  7, chapter_cnt:  21, chapter_idx:  211, verse_cnt:    1236, verse_idx:    6510, writ_cnt:     18966, writ_idx:    175590, name: "Judges", abbr2: "Jg", abbr3: "Jdg", abbr4: "Judg", abbrAltA: "Jdgs", abbrAltB: "",  },
	AVXBook{ num:  8, chapter_cnt:   4, chapter_idx:  232, verse_cnt:     170, verse_idx:    7128, writ_cnt:      2574, writ_idx:    194556, name: "Ruth", abbr2: "Ru", abbr3: "Rth", abbr4: "Ruth", abbrAltA: "Rut", abbrAltB: "",  },
	AVXBook{ num:  9, chapter_cnt:  31, chapter_idx:  236, verse_cnt:    1620, verse_idx:    7213, writ_cnt:     25048, writ_idx:    197130, name: "1 Samuel", abbr2: "1S", abbr3: "1Sm", abbr4: "1Sam", abbrAltA: "1Sa", abbrAltB: "",  },
	AVXBook{ num: 10, chapter_cnt:  24, chapter_idx:  267, verse_cnt:    1390, verse_idx:    8023, writ_cnt:     20600, writ_idx:    222178, name: "2 Samuel", abbr2: "2S", abbr3: "2Sm", abbr4: "2Sam", abbrAltA: "1Sa", abbrAltB: "",  },
	AVXBook{ num: 11, chapter_cnt:  22, chapter_idx:  291, verse_cnt:    1632, verse_idx:    8718, writ_cnt:     24513, writ_idx:    242778, name: "1 Kings", abbr2: "1K", abbr3: "1Ki", abbr4: "1Kgs", abbrAltA: "1Kg", abbrAltB: "1Kin",  },
	AVXBook{ num: 12, chapter_cnt:  25, chapter_idx:  313, verse_cnt:    1438, verse_idx:    9534, writ_cnt:     23517, writ_idx:    267291, name: "2 Kings", abbr2: "2K", abbr3: "2Ki", abbr4: "2Kgs", abbrAltA: "2Kg", abbrAltB: "2Kin",  },
	AVXBook{ num: 13, chapter_cnt:  29, chapter_idx:  338, verse_cnt:    1884, verse_idx:   10253, writ_cnt:     20365, writ_idx:    290808, name: "1 Chronicles", abbr2: "", abbr3: "1Ch", abbr4: "1Chr", abbrAltA: "1Chron", abbrAltB: "",  },
	AVXBook{ num: 14, chapter_cnt:  36, chapter_idx:  367, verse_cnt:    1644, verse_idx:   11195, writ_cnt:     26069, writ_idx:    311173, name: "2 Chronicles", abbr2: "", abbr3: "2Ch", abbr4: "2Chr", abbrAltA: "2Chron", abbrAltB: "",  },
	AVXBook{ num: 15, chapter_cnt:  10, chapter_idx:  403, verse_cnt:     560, verse_idx:   12017, writ_cnt:      7440, writ_idx:    337242, name: "Ezra", abbr2: "", abbr3: "Ezr", abbr4: "Ezra", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 16, chapter_cnt:  13, chapter_idx:  413, verse_cnt:     812, verse_idx:   12297, writ_cnt:     10480, writ_idx:    344682, name: "Nehemiah", abbr2: "Ne", abbr3: "Neh", abbr4: "Neh", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 17, chapter_cnt:  10, chapter_idx:  426, verse_cnt:     334, verse_idx:   12703, writ_cnt:      5633, writ_idx:    355162, name: "Esther", abbr2: "Es", abbr3: "Est", abbr4: "Est", abbrAltA: "Esth", abbrAltB: "",  },
	AVXBook{ num: 18, chapter_cnt:  42, chapter_idx:  436, verse_cnt:    2140, verse_idx:   12870, writ_cnt:     18098, writ_idx:    360795, name: "Job", abbr2: "Jb", abbr3: "Job", abbr4: "Job", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 19, chapter_cnt: 150, chapter_idx:  478, verse_cnt:    4922, verse_idx:   13940, writ_cnt:     42704, writ_idx:    378893, name: "Psalms", abbr2: "Ps", abbr3: "Psa", abbr4: "Pslm", abbrAltA: "Psm", abbrAltB: "Pss",  },
	AVXBook{ num: 20, chapter_cnt:  31, chapter_idx:  628, verse_cnt:    1830, verse_idx:   16401, writ_cnt:     15038, writ_idx:    421597, name: "Proverbs", abbr2: "Pr", abbr3: "Pro", abbr4: "Prov", abbrAltA: "Prv", abbrAltB: "",  },
	AVXBook{ num: 21, chapter_cnt:  12, chapter_idx:  659, verse_cnt:     444, verse_idx:   17316, writ_cnt:      5579, writ_idx:    436635, name: "Ecclesiastes", abbr2: "Ec", abbr3: "Ecc", abbr4: "Eccl", abbrAltA: "Eccle", abbrAltB: "Qoh",  },
	AVXBook{ num: 22, chapter_cnt:   8, chapter_idx:  671, verse_cnt:     234, verse_idx:   17538, writ_cnt:      2658, writ_idx:    442214, name: "Song of Solomon", abbr2: "So", abbr3: "SoS", abbr4: "Song", abbrAltA: "SS", abbrAltB: "Cant",  },
	AVXBook{ num: 23, chapter_cnt:  66, chapter_idx:  679, verse_cnt:    2584, verse_idx:   17655, writ_cnt:     37036, writ_idx:    444872, name: "Isaiah", abbr2: "Is", abbr3: "Isa", abbr4: "Isa", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 24, chapter_cnt:  52, chapter_idx:  745, verse_cnt:    2728, verse_idx:   18947, writ_cnt:     42654, writ_idx:    481908, name: "Jeremiah", abbr2: "Je", abbr3: "Jer", abbr4: "Jer", abbrAltA: "Jeremy", abbrAltB: "Jr",  },
	AVXBook{ num: 25, chapter_cnt:   5, chapter_idx:  797, verse_cnt:     308, verse_idx:   20311, writ_cnt:      3411, writ_idx:    524562, name: "Lamentations", abbr2: "La", abbr3: "Lam", abbr4: "Lam", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 26, chapter_cnt:  48, chapter_idx:  802, verse_cnt:    2546, verse_idx:   20465, writ_cnt:     39401, writ_idx:    527973, name: "Ezekiel", abbr2: "", abbr3: "Eze", abbr4: "Ezek", abbrAltA: "Ezk", abbrAltB: "",  },
	AVXBook{ num: 27, chapter_cnt:  12, chapter_idx:  850, verse_cnt:     714, verse_idx:   21738, writ_cnt:     11602, writ_idx:    567374, name: "Daniel", abbr2: "Da", abbr3: "Dan", abbr4: "Dan", abbrAltA: "Dn", abbrAltB: "",  },
	AVXBook{ num: 28, chapter_cnt:  14, chapter_idx:  862, verse_cnt:     394, verse_idx:   22095, writ_cnt:      5174, writ_idx:    578976, name: "Hosea", abbr2: "Ho", abbr3: "Hos", abbr4: "Hos", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 29, chapter_cnt:   3, chapter_idx:  876, verse_cnt:     146, verse_idx:   22292, writ_cnt:      2033, writ_idx:    584150, name: "Joel", abbr2: "Jl", abbr3: "Jol", abbr4: "Joel", abbrAltA: "Joe", abbrAltB: "",  },
	AVXBook{ num: 30, chapter_cnt:   9, chapter_idx:  879, verse_cnt:     292, verse_idx:   22365, writ_cnt:      4216, writ_idx:    586183, name: "Amos", abbr2: "Am", abbr3: "Amo", abbr4: "Amos", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 31, chapter_cnt:   1, chapter_idx:  888, verse_cnt:      42, verse_idx:   22511, writ_cnt:       669, writ_idx:    590399, name: "Obadiah", abbr2: "Ob", abbr3: "Obd", abbr4: "Obad", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 32, chapter_cnt:   4, chapter_idx:  889, verse_cnt:      96, verse_idx:   22532, writ_cnt:      1320, writ_idx:    591068, name: "Jonah", abbr2: "", abbr3: "Jnh", abbr4: "Jona", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 33, chapter_cnt:   7, chapter_idx:  893, verse_cnt:     210, verse_idx:   22580, writ_cnt:      3152, writ_idx:    592388, name: "Micah", abbr2: "Mc", abbr3: "Mic", abbr4: "Mica", abbrAltA: "Mi", abbrAltB: "",  },
	AVXBook{ num: 34, chapter_cnt:   3, chapter_idx:  900, verse_cnt:      94, verse_idx:   22685, writ_cnt:      1284, writ_idx:    595540, name: "Nahum", abbr2: "Na", abbr3: "Nah", abbr4: "Nah", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 35, chapter_cnt:   3, chapter_idx:  903, verse_cnt:     112, verse_idx:   22732, writ_cnt:      1475, writ_idx:    596824, name: "Habakkuk", abbr2: "Hb", abbr3: "Hab", abbr4: "Hab", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 36, chapter_cnt:   3, chapter_idx:  906, verse_cnt:     106, verse_idx:   22788, writ_cnt:      1616, writ_idx:    598299, name: "Zephaniah", abbr2: "Zp", abbr3: "Zep", abbr4: "Zeph", abbrAltA: "Zph", abbrAltB: "",  },
	AVXBook{ num: 37, chapter_cnt:   2, chapter_idx:  909, verse_cnt:      76, verse_idx:   22841, writ_cnt:      1130, writ_idx:    599915, name: "Haggai", abbr2: "Hg", abbr3: "Hag", abbr4: "Hag", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 38, chapter_cnt:  14, chapter_idx:  911, verse_cnt:     422, verse_idx:   22879, writ_cnt:      6443, writ_idx:    601045, name: "Zechariah", abbr2: "Zc", abbr3: "Zec", abbr4: "Zech", abbrAltA: "Zch", abbrAltB: "",  },
	AVXBook{ num: 39, chapter_cnt:   4, chapter_idx:  925, verse_cnt:     110, verse_idx:   23090, writ_cnt:      1781, writ_idx:    607488, name: "Malachi", abbr2: "Ml", abbr3: "Mal", abbr4: "Mal", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 40, chapter_cnt:  28, chapter_idx:  929, verse_cnt:    2142, verse_idx:   23145, writ_cnt:     23684, writ_idx:    609269, name: "Matthew", abbr2: "Mt", abbr3: "Mat", abbr4: "Matt", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 41, chapter_cnt:  16, chapter_idx:  957, verse_cnt:    1356, verse_idx:   24216, writ_cnt:     15166, writ_idx:    632953, name: "Mark", abbr2: "Mk", abbr3: "Mrk", abbr4: "Mark", abbrAltA: "Mk", abbrAltB: "Mr",  },
	AVXBook{ num: 42, chapter_cnt:  24, chapter_idx:  973, verse_cnt:    2302, verse_idx:   24894, writ_cnt:     25939, writ_idx:    648119, name: "Luke", abbr2: "Lk", abbr3: "Luk", abbr4: "Luke", abbrAltA: "Lu", abbrAltB: "",  },
	AVXBook{ num: 43, chapter_cnt:  21, chapter_idx:  997, verse_cnt:    1758, verse_idx:   26045, writ_cnt:     19094, writ_idx:    674058, name: "John", abbr2: "Jn", abbr3: "Jhn", abbr4: "John", abbrAltA: "Joh", abbrAltB: "",  },
	AVXBook{ num: 44, chapter_cnt:  28, chapter_idx: 1018, verse_cnt:    2014, verse_idx:   26924, writ_cnt:     24245, writ_idx:    693152, name: "Acts", abbr2: "Ac", abbr3: "Act", abbr4: "Acts", abbrAltA: "Ats", abbrAltB: "",  },
	AVXBook{ num: 45, chapter_cnt:  16, chapter_idx: 1046, verse_cnt:     866, verse_idx:   27931, writ_cnt:      9422, writ_idx:    717397, name: "Romans", abbr2: "Ro", abbr3: "Rom", abbr4: "Rom", abbrAltA: "Rm", abbrAltB: "",  },
	AVXBook{ num: 46, chapter_cnt:  16, chapter_idx: 1062, verse_cnt:     874, verse_idx:   28364, writ_cnt:      9462, writ_idx:    726819, name: "1 Corinthians", abbr2: "", abbr3: "1Co", abbr4: "1Cor", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 47, chapter_cnt:  13, chapter_idx: 1078, verse_cnt:     514, verse_idx:   28801, writ_cnt:      6065, writ_idx:    736281, name: "2 Corinthians", abbr2: "", abbr3: "2Co", abbr4: "2Cor", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 48, chapter_cnt:   6, chapter_idx: 1091, verse_cnt:     298, verse_idx:   29058, writ_cnt:      3084, writ_idx:    742346, name: "Galatians", abbr2: "Ga", abbr3: "Gal", abbr4: "Gal", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 49, chapter_cnt:   6, chapter_idx: 1097, verse_cnt:     310, verse_idx:   29207, writ_cnt:      3022, writ_idx:    745430, name: "Ephesians", abbr2: "Ep", abbr3: "Eph", abbr4: "Eph", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 50, chapter_cnt:   4, chapter_idx: 1103, verse_cnt:     208, verse_idx:   29362, writ_cnt:      2183, writ_idx:    748452, name: "Philippians", abbr2: "Pp", abbr3: "Php", abbr4: "Phil", abbrAltA: "Philip", abbrAltB: "",  },
	AVXBook{ num: 51, chapter_cnt:   4, chapter_idx: 1107, verse_cnt:     190, verse_idx:   29466, writ_cnt:      1979, writ_idx:    750635, name: "Colossians", abbr2: "Co", abbr3: "Col", abbr4: "Col", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 52, chapter_cnt:   5, chapter_idx: 1111, verse_cnt:     178, verse_idx:   29561, writ_cnt:      1837, writ_idx:    752614, name: "1 Thessalonians", abbr2: "", abbr3: "1Th", abbr4: "1Th", abbrAltA: "1Thess", abbrAltB: "",  },
	AVXBook{ num: 53, chapter_cnt:   3, chapter_idx: 1116, verse_cnt:      94, verse_idx:   29650, writ_cnt:      1022, writ_idx:    754451, name: "2 Thessalonians", abbr2: "", abbr3: "2Th", abbr4: "2Th", abbrAltA: "2Thess", abbrAltB: "",  },
	AVXBook{ num: 54, chapter_cnt:   6, chapter_idx: 1119, verse_cnt:     226, verse_idx:   29697, writ_cnt:      2244, writ_idx:    755473, name: "1 Timothy", abbr2: "", abbr3: "1Ti", abbr4: "1Tim", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 55, chapter_cnt:   4, chapter_idx: 1125, verse_cnt:     166, verse_idx:   29810, writ_cnt:      1666, writ_idx:    757717, name: "2 Timothy", abbr2: "", abbr3: "2Ti", abbr4: "2Tim", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 56, chapter_cnt:   3, chapter_idx: 1129, verse_cnt:      92, verse_idx:   29893, writ_cnt:       896, writ_idx:    759383, name: "Titus", abbr2: "Ti", abbr3: "Ti", abbr4: "Ti", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 57, chapter_cnt:   1, chapter_idx: 1132, verse_cnt:      50, verse_idx:   29939, writ_cnt:       430, writ_idx:    760279, name: "Philemon", abbr2: "Pm", abbr3: "Phm", abbr4: "Phm", abbrAltA: "Philem", abbrAltB: "",  },
	AVXBook{ num: 58, chapter_cnt:  13, chapter_idx: 1133, verse_cnt:     606, verse_idx:   29964, writ_cnt:      6897, writ_idx:    760709, name: "Hebrews", abbr2: "", abbr3: "Heb", abbr4: "Heb", abbrAltA: "Hbr", abbrAltB: "Hbrs",  },
	AVXBook{ num: 59, chapter_cnt:   5, chapter_idx: 1146, verse_cnt:     216, verse_idx:   30267, writ_cnt:      2304, writ_idx:    767606, name: "James", abbr2: "Jm", abbr3: "Jam", abbr4: "Jam", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 60, chapter_cnt:   5, chapter_idx: 1151, verse_cnt:     210, verse_idx:   30375, writ_cnt:      2476, writ_idx:    769910, name: "1 Peter", abbr2: "1P", abbr3: "1Pe", abbr4: "1Pet", abbrAltA: "1Pt", abbrAltB: "",  },
	AVXBook{ num: 61, chapter_cnt:   3, chapter_idx: 1156, verse_cnt:     122, verse_idx:   30480, writ_cnt:      1553, writ_idx:    772386, name: "2 Peter", abbr2: "2P", abbr3: "2Pe", abbr4: "2Pet", abbrAltA: "2Pt", abbrAltB: "",  },
	AVXBook{ num: 62, chapter_cnt:   5, chapter_idx: 1159, verse_cnt:     210, verse_idx:   30541, writ_cnt:      2517, writ_idx:    773939, name: "1 John", abbr2: "1J", abbr3: "1Jn", abbr4: "1Jn", abbrAltA: "1Jn", abbrAltB: "1Jhn",  },
	AVXBook{ num: 63, chapter_cnt:   1, chapter_idx: 1164, verse_cnt:      26, verse_idx:   30646, writ_cnt:       298, writ_idx:    776456, name: "2 John", abbr2: "2J", abbr3: "2Jn", abbr4: "2Jn", abbrAltA: "1Jn", abbrAltB: "1Jhn",  },
	AVXBook{ num: 64, chapter_cnt:   1, chapter_idx: 1165, verse_cnt:      28, verse_idx:   30659, writ_cnt:       294, writ_idx:    776754, name: "3 John", abbr2: "3J", abbr3: "3Jn", abbr4: "3Jn", abbrAltA: "1Jn", abbrAltB: "1Jhn",  },
	AVXBook{ num: 65, chapter_cnt:   1, chapter_idx: 1166, verse_cnt:      50, verse_idx:   30673, writ_cnt:       608, writ_idx:    777048, name: "Jude", abbr2: "Jd", abbr3: "Jd", abbr4: "Jude", abbrAltA: "", abbrAltB: "",  },
	AVXBook{ num: 66, chapter_cnt:  22, chapter_idx: 1167, verse_cnt:     808, verse_idx:   30698, writ_cnt:     11995, writ_idx:    777656, name: "Revelation", abbr2: "Re", abbr3: "Rev", abbr4: "Rev", abbrAltA: "", abbrAltB: "",  },
];
// < < < Generated-Code -- Initialization < < < //

/*
use crate::avx::book_index;

pub const fn get_book_index(sdkdir: String) -> [AVXBook; 67] {
    let index = books;
}

error[E0106]: missing lifetime specifier
   --> src\avx\book_index.rs:107:49
    |
107 | pub const fn get_book_index(sdkdir: String) -> [AVXBook; 67] {
    |                                                 ^^^^^^^ expected named lifetime parameter
    |
    = help: this function's return type contains a borrowed value, but there is no value for it to be borrowed from
help: consider using the `'static` lifetime
    |
107 | pub const fn get_book_index(sdkdir: String) -> [AVXBook<'static>; 67] {
    |
*/
