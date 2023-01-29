#include <avxgen.h>
const char AVXLemmata_File[] = "AV-Lemma.dxi"; // from AV-Inventory-Z31.bom
const uint32 AVXLemmata_RecordCnt  =  15171;   // from AV-Inventory-Z31.bom
const uint32 AVXLemmata_FileLen    = 182344;   // from AV-Inventory-Z31.bom

class AVXLemmataRecords
{
public:
    class AVXLemmata {
    public:
        const uint32  pos32;
        const uint16  word_key;
        const uint16  pn_pos;
        const uint16  lemmata[3];
    };

    AVXLemmataRecords()
    {
        ;
    }
    static AVXLemmata const records[AVXLemmata_RecordCnt];
};