#include <avxgen.h>
#include <vector>
const char AVXWordClass_File[] = "AV-WordClass.dxi"; // from AV-Inventory-Z31.bom
const uint32 AVXWordClass_RecordCnt =  54;      // from AV-Inventory-Z31.bom
const uint32 AVXWordClass_FileLen   = 836;      // from AV-Inventory-Z31.bom

class AVXWordClasses
{
public:
    class AVXWordClass {
    public:
        const uint16  word_class;
        const uint16  width;
        const vector<uint32> pos;
    };

    AVXWordClasses()
    {
        ;
    }
    static AVXWordClass const classes[AVXWordClass_RecordCnt];
};