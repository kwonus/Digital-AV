#include <avxgen.h>
const char AVXChapterIndex_File[] = "AV-Chapter.ix";    // from AV-Inventory-Z31.bom
const uint32 AVXChapterIndex_RecordLen =    8;     // from AV-Inventory-Z31.bom
const uint32 AVXChapterIndex_RecordCnt = 1189;     // from AV-Inventory-Z31.bom
const uint32 AVXChapterIndex_FileLen   = 9512;     // from AV-Inventory-Z31.bom

class AVXChapterIndex
{
public:
    class AVXChapter {
    public:
        const uint32 writ_idx;
        const uint16 writ_cnt;
        const uint16 verse_idx;
        const uint16 verse_cnt;
    };

    AVXChapterIndex()
    {
        ;
    }
    static AVXChapter const index[AVXChapterIndex_RecordCnt];
};