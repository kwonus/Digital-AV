#include <avxgen.h>
const char AVXWordClass_File[] = "AV-WordClass.dxi"; // from AV-Inventory-Z31.bom
const uint32 AVXWordClass_RecordCnt =  54;      // from AV-Inventory-Z31.bom
const uint32 AVXWordClass_FileLen   = 836;      // from AV-Inventory-Z31.bom

struct AVXWordClass {
    uint16  word_class;
    uint16  width;
    uint32 *pos;
}