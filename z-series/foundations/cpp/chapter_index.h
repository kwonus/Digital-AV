#include <avxgen.h>
const char AVXChapterIndex_File[] = "AV-Chapter.ix";    // from AV-Inventory-Z31.bom
const uint32 AVXChapterIndex_RecordLen =    8;     // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_RecordCnt    = 1189;     // from AV-Inventory-Z31.bom
const uint32 AVXChapterIndex_FileLen   = 9512;     // from AV-Inventory-Z31.bom

typedef struct avx_chapter {                             // from Digital-AV.pdf
    uint32 writ_idx;
    uint16 writ_cnt;
    uint16 verse_idx;
    uint16 verse_cnt;
}   AVXChapter;