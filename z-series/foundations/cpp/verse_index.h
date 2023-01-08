#include <avxgen.h>
const char AVXVerseIndex_File[] = "AV-Verse.ix";    // from AV-Inventory-Z31.bom
const uint32 AVXVerseIndex_RecordLen =      4; // from AV-Inventory-Z31.bom
const uint32 AVXVerseIndex_RecordCnt =  31102; // from AV-Inventory-Z31.bom
const uint32 AVXVerseIndex_FileLen   = 124408; // from AV-Inventory-Z31.bom

typedef struct avx_verse {                              // from Digital-AV.pdf
    uint8 book;
    uint8 chapter;
    uint8 verse;
    uint8 word_cnt;
}   AVXVerse;