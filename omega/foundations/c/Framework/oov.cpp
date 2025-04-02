#include <oov.h>

namespace avx
{
    oov_lemmata_cursor::oov_lemmata_cursor()
    {
        if (global_instance != nullptr)
        {
            auto cursor = (u16*)global_instance->get_names_data(&details);

            for (u32 record = 0; record < details.record_cnt; record++)
            {
                u16 key = *cursor;
                char* text = (char*)(cursor + 1);
                this->forward[key] = text;
                this->reverse[text] = key;
            }
        }
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