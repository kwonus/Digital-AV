#include <avxgen.h>
const char AVXLemmataOOV_File[] = "AV-Lemma-OOV.dxi"; // from AV-Inventory-Z31.bom
const uint32 AVLemmaOOV_RecordCnt  =  771;       // from AV-Inventory-Z31.bom
const uint32 AVXLemmataOOV_FileLen = 7754;       // from AV-Inventory-Z31.bom

// OOV: uint16                                   // from Digital-AV.pdf
const uint16 OOV_Marker = 0x8000;
const uint16 OOV_length = 0x0F00;
const uint16 OOV_index  = 0x000F;

struct AVXLemmataOOV{                            // from Digital-AV.pdf
    uint16 key;
    char  *text;
}