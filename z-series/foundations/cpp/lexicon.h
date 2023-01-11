#include <avxgen.h>
const char AVXLexicon_File[] = "AV-Lexicon.dxi"; // from AV-Inventory-Z31.bom
const uint32 AV_Lexicon_RecordCnt =  12567; // from AV-Inventory-Z31.bom
const uint32 AV_Lexicon_FileLen   = 246249; // from AV-Inventory-Z31.bom

// Entities: uint16
const uint16 Entity_Hitchcock     = 0x8000;
const uint16 Entity_men           =    0x1;
const uint16 Entity_women         =    0x2;
const uint16 Entity_tribes        =    0x4;
const uint16 Entity_cities        =    0x8;
const uint16 Entity_rivers        =   0x10;
const uint16 Entity_mountains     =   0x20;
const uint16 Entity_animals       =   0x40;
const uint16 Entity_gemstones     =   0x80;
const uint16 Entity_measurements  =  0x100;

class AVXLexicon
{
public:
    class AVXLexItem {
    public:
        const uint16  entities;
        const uint16  count;
        const uint32* pos;
        const char* search;
        const char* display;
        const char* modern;
    };

    AVXLexicon()
    {
        ;
    }
    static AVXLexItem const items[AV_Lexicon_RecordCnt];
};

