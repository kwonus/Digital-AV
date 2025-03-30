#include <avx.h>

namespace avx
{
#pragma pack(1)
    struct Written
    {
        u16 strongs[4];
        byte bcv_wc[4];
        u16 word_key;
        u16 pn_pos12;
        u32 pos32;
        u16 lemma;
        byte punctuation;
        byte transition;
    };
}
