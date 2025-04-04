#ifndef AVX_OOV_LEMMATA
#define AVX_OOV_LEMMATA

#include <avx.h>
#include <artifact.h>
#include <map>
#include <string>

namespace avx
{
    struct oov_lemmata
    {
        const u16 oov_key;
        const char* oov_text;
    };

    class oov_lemmata_cursor
    {
    private:
        std::map<std::string, u16> reverse;
        std::map<u16, char*> forward;
        artifact details;

    public:
        oov_lemmata_cursor();
        bool init();
        void free();
        const char* get_text(u16 oov_key);
        const u16 get_key(const char* oov_txt);
    };
}
extern "C" const char* get_oov_text(u16 oov_key);
extern "C" const u16 get_oov_key(const char* oov_txt);

#endif
