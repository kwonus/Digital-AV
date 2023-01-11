#include <avxgen.h>
const char AVXVerseIndex_File[] = "AV-Verse.ix";    // from AV-Inventory-Z31.bom
const uint32 AVXVerseIndex_RecordLen =      4; // from AV-Inventory-Z31.bom
const uint32 AVXVerseIndex_RecordCnt =  31102; // from AV-Inventory-Z31.bom
const uint32 AVXVerseIndex_FileLen   = 124408; // from AV-Inventory-Z31.bom

class AVXVerseIndex
{
public:
    class AVXVerse {
    public:
        const uint8 book;
        const uint8 chapter;
        const uint8 verse;
        const uint8 word_cnt;
    };

    AVXVerseIndex()
    {
        ;
    }
    static AVXVerse const index[AVXVerseIndex_RecordCnt];
};