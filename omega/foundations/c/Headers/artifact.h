#ifndef AVX_ARTIFACT
#define AVX_ARTIFACT

#include <avx.h>
#include <string>

#pragma pack(1)
namespace avx
{
    struct artifact
    {
        char label[16];
        u32 offset;
        u32 length;
        u64 hash_1;
        u64 hash_2;
        u32 record_len;
        u32 record_cnt;
    };
}

#endif
