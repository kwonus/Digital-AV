#include <avx.h>

namespace avx
{
    struct Lemmata
    {
        u32 pos32;
        u16 word_key;
        u16 pn_pos12;
        u16 lemma_cnt;
        u16* lemmas;
    };
}
