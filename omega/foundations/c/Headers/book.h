#include <avx.h>

namespace avx
{
#pragma pack(1)
    struct Book
    {
        byte book_num;
        byte chapter_cnt;
        u16 chapter_idx;
        u16 writ_cnt;
        u16 writ_idx;
        char name[16];
        char abbr[12];
        char alts[10];
    };
}
