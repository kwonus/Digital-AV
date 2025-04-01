#include <avx.h>
#include <directory.h>

namespace avx
{
#pragma pack(1)
    struct Lexicon
    {
        const u16 entities;
        const u16 pos_cnt;
        const u32* remainder;

        const u32* get_pos();
        const char* get_search();
        const char* get_display(bool with_cascade = true); // with_cascade will return search when display == '\0'
        const char* get_modern(bool with_cascade = true);  // with_cascade will return display when modern == '\0'
    };

    struct LexiconRecord // for FFI interop
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
    public:
        LexiconCursor();
        const Lexicon* get_first();
        const Lexicon* get_next();
    };
}
