#include <avxgen.h>
const char AVXNames_File[] = "AV-Names.dxi";  // from AV-Inventory-Z31.bom
const uint32 AVXNames_RecordCnt =  2470; // from AV-Inventory-Z31.bom
const uint32 AVXNames_FileLen   = 60727; // from AV-Inventory-Z31.bom

class AVXNames
{
public:
    class AVXName {
    public:
        const uint16 word_key;
        const uint16 count;
        const char  *meanings[5];
    };

    AVXNames()
    {
        ;
    }
    static AVXName const names[AVXNames_RecordCnt];
};