#include <oov.h>
#include <directory.h>

extern "C" AVX_API bool oov_init()
{
    return avx::instance.oov.init();
}
extern "C" AVX_API void oov_free()
{
    avx::instance.oov.free();
}
extern "C" AVX_API const char* oov_get_text(u16 oov_key)
{
    return avx::instance.oov.get_text(oov_key);
}
extern "C" AVX_API const u16 oov_get_key(const char* oov_txt)
{
    return avx::instance.oov.get_key(oov_txt);
}


namespace avx
{
    oov_lemmata_cursor::oov_lemmata_cursor()
    {
        ;
    }
    bool oov_lemmata_cursor::init()
    {
        auto cursor = (u16*)avx::instance.get_oov_data(&details);

        if (cursor != nullptr)
        {
            for (u32 record = 0; record < details.record_cnt; record++)
            {
                u16 key = *cursor;
                char* text = (char*)(cursor + 1);
                this->forward[key] = text;
                this->reverse[text] = key;
            }
            return true;
        }
        return false;
    }
    void oov_lemmata_cursor::free()
    {
        this->forward.clear();
        this->reverse.clear();
    }
    const char* oov_lemmata_cursor::get_text(const u16 word_key)
    {
        u16 key = word_key;

        return this->forward.find((u16)key) != this->forward.end()
            ? const_cast<const char*>(this->forward[(u16)word_key])
            : (const char*) nullptr;
    }
    const u16 oov_lemmata_cursor::get_key(const char* word)
    {
        char* oov = const_cast<char*>(word);

        return this->reverse.find(oov) != this->reverse.end()
            ? this->reverse[oov]
            : 0;
    }
}