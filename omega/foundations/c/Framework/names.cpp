#include <names.h>
#include <directory.h>

extern "C" bool names_init()
{
    return avx::instance.names.init();
}
extern "C" void names_free()
{
    avx::instance.names.free();
}
extern "C" const char* names_get_meaning(const u16 key)
{
    avx::instance.names.get_meanings(key);
}

namespace avx
{
    NamesCursor::NamesCursor()
    {
        ;
    }
    bool NamesCursor::init()
    {
        auto cursor = (u16*)instance.get_names_data(&details);

        if (cursor != nullptr)
        {
            for (u32 record = 0; record < details.record_cnt; record++)
            {
                u16 key = *cursor;
                char* text = (char*)(cursor + 1);
                this->meanings[key] = text;
            }
            return true;
        }
        return false;
    }
    void NamesCursor::free()
    {
        this->meanings.clear();
    }
    const char * NamesCursor::get_meanings(const u16 word_key)
    {
        u16 key = word_key;

        return meanings.find((u16)key) != meanings.end()
            ? const_cast<const char*>(meanings[(u16)word_key])
            : (const char*) nullptr;
    }
}