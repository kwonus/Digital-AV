#include <avxgen.h>
const char AVXNames_File[] = "AV-Names.dxi";  // from AV-Inventory-Z31.bom
const uint32 AVXNames_RecordCnt =  2470; // from AV-Inventory-Z31.bom
const uint32 AVXNames_FileLen   = 60727; // from AV-Inventory-Z31.bom

struct AVXName {
    uint16 word_key;
    uint16 count;
    char** meanings;
}