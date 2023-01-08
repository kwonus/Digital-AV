#include <avxgen.h>
const char AVXLemmata_File[] = "AV-Lemma.dxi"; // from AV-Inventory-Z31.bom
const uint32 AVLemma_RecordCnt  =  15171;   // from AV-Inventory-Z31.bom
const uint32 AVXLemmata_FileLen = 182344;   // from AV-Inventory-Z31.bom

struct AVXLemmata {                         // from Digital-AV.pdf
    uint32  pos;
    uint16  word_key;
    uint16  word_class;
    uint16  count;
    uint16 *lemmata;
}