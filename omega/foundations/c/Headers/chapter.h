#include <avx.h>

namespace avx
{
#pragma pack(1)
    struct Chapter
    {
        u16 writ_idx;
        u16 writ_cnt;
        byte book_num;
        byte verse_cnt;
    };
}
