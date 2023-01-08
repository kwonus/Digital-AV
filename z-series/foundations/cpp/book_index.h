#include <avxgen.h>
const char AVXBookIndex_File[] = "AV-Book.ix";   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_RecordLen = 32;   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_RecordCnt = 66;   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_FileLen = 2112;   // from AV-Inventory-Z31.bom

typedef struct avx_book {                            // from Digital-AV.pdf
    uint8   num;
    uint8   chapter_cnt;
    uint16  chapter_idx;
    char    name[17];
    int     abbreviationCnt;
    char  **abbreviations;
}   AVXBook;