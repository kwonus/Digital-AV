#include <avxgen.h>
#include <written.h>
#include <chapter_index.h>
#include <verse_index.h>
const char AVXBookIndex_File[] = "AV-Book.ix";   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_RecordLen = 44;   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_RecordCnt = 67;   // from AV-Inventory-Z31.bom
const uint32 AVXBookIndex_FileLen = 2948;   // from AV-Inventory-Z31.bom

class AVXBookIndex
{
public:
    class AVXBook { 
    public:
        const uint8     num;
        const uint8     chapter_cnt;
        const uint16    chapter_idx;
        const uint16    verseCnt;
        const uint16    verseIdx;
        const uint16    writCnt;
        const uint32    writIdx;
        const char     *bookname;
        const char      abbr2[3];  // strlen == 2 || strlen == 0
        const char      abbr3[4];  // strlen == 3
        const char      abbr4[5];  // <-- Most common // use this for display // strlen <= 4
        const char     *abbrAltA;  // Alternate Abbreviation: unknown size
        const char     *abbrAltB;  // Alternate Abbreviation: unknown size
        
        inline const AVXWritten::AVXWrit* const getWrit()
        {
            return AVXWritten::written[num];
        }
        inline const AVXWritten::AVXWrit* const getWrit(uint8 chapter)
        {
            if (chapter > this->chapter_cnt || chapter < 1)
                return nullptr;

            auto ichapter = AVXChapterIndex::index[this->chapter_idx + chapter - 1];
            return AVXWritten::written[num] + ichapter.writ_idx - this->writIdx;
        }
        const AVXWritten::AVXWrit* const getWrit(uint8 chapter, uint8 verse)
        {
            if (chapter > this->chapter_cnt || chapter < 1)
                return nullptr;

            auto ichapter = AVXChapterIndex::index[this->chapter_idx + chapter - 1];

            if (verse > ichapter.verse_cnt || verse < 1)
                return nullptr;

            auto iverse = &(AVXVerseIndex::index[ichapter.verse_idx + verse - 1]);

            uint32 offset = ichapter.writ_idx - this->writIdx;

            for (uint8 v = 1; v < verse; v++)
            {
                offset += iverse->word_cnt;
                iverse++;
            }
            return AVXWritten::written[num] + offset;
        }
    };

    AVXBookIndex()
    {
        ;
    }
    static AVXBook const index[AVXBookIndex_RecordCnt];

    inline const AVXWritten::AVXWrit* const getWrit(uint8 bknum)
    {
        return nullptr;
    }
    inline const AVXWritten::AVXWrit* const getWrit(uint8 bknum, uint8 chapter)
    {
        return nullptr;
    }
    inline const AVXWritten::AVXWrit* const getWrit(uint8 bknum, uint8 chapter, uint8 verse)
    {
        return nullptr;
    }
};