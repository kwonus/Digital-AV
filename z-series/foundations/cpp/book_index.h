#include <avxgen.h>
const char AVXBookIndex_File[] = "AV-Book.ix";   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_RecordLen = 44;   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_RecordCnt = 67;   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_FileLen = 2948;   // from AV-Inventory-Z31.bom

class AVXBookIndex
{
public:
    class AVXBook { 
    public:
        const uint8   num;
        const uint8   chapter_cnt;
        const uint16  chapter_idx;
        const uint16  verseCnt;
        const uint16  verseIdx;
        const uint16  writCnt;
        const uint32  writIdx;
        const char* bookname;
        const char* abbreviations[3];
    };

    AVXBookIndex()
    {
        ;
    }
    static AVXBook const index[AVXBookIndex_RecordCnt];
};