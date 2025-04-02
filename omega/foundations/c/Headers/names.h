#include <avx.h>
#include <directory.h>
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
        const char* get_meanings(const u16 word_key);
    };
}
