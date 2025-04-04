#ifndef AVX_NAMES
#define AVX_NAMES

#include <avx.h>
#include <artifact.h>
#include <map>
#include <vector>
#include<string>

namespace avx
{
    struct Names
    {
        u16 word_key;
        char* meanings;
    };
    class NamesCursor
    {
    private:
        std::map<u16, char*> meanings;
        artifact details;

    public:
        NamesCursor();
        bool init();
        void free();
        const char* get_meanings(const u16 word_key);
    };
}
extern "C" bool names_init();
extern "C" void names_free();
extern "C" const char* names_get_meaning(const u16 key);

#endif
