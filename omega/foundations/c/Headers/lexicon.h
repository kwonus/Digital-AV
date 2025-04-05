#ifndef AVX_LEXICON
#define AVX_LEXICON
#include <avx.h>
#include <artifact.h>

static const u16 AV_LEX_CNT      =  12567;  // from AV-Inventory-Z31.bom; consistant with Release Version 5.1 (+1 for metadata)
static const u32 AV_LEX_FILE_LEN = 246249;  // from AV-Inventory-Z31.bom; consistant with Release Version 5.1

namespace avx
{
#pragma pack(1)
    struct Lexicon // for C/C++ native & non-lazy FFI interop ... any non-C/C++ language-client must reimplement methods on client-side
    {
        const u16 entities;
        const u16 pos_cnt;
        const u32* remainder;

        const u32* get_pos();
        const char* get_search();
        const char* get_display(bool with_cascade = true); // with_cascade will return search when display == '\0'
        const char* get_modern(bool with_cascade = true);  // with_cascade will return display when modern == '\0'
    };

#pragma pack(1)
    struct LexiconRecord // for lazy FFI interop ... this type of interop requires malloc/free
    {
        const u16 entities;
        const u16 pos_cnt;

        const u32* pos;
        const char* search;
        const char* display;
        const char* modern;

        LexiconRecord(const Lexicon& lex);
    };

    class LexiconCursor
    {
    private:
        u16* record_cursor;
        u16  record_index;
        artifact details;
        Lexicon*  cache[AV_LEX_CNT+1];                  // +1 to hold metadata in position 0;
        const Lexicon* get_first();
        const Lexicon* get_next();
        bool init();
        void quit();

    public:
        friend class directory;
        LexiconCursor();
        const Lexicon* get(const u16 key);
    };
}
extern "C" AVX_API const avx::Lexicon* lexicon_get(const u16 key);

#endif
