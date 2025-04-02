#include <names.h>

namespace avx
{
    NamesCursor::NamesCursor()
    {
        if (global_instance != nullptr)
        {
            auto cursor = (u16*)global_instance->get_names_data(&details);

            for (u32 record = 0; record < details.record_cnt; record++)
            {
                u16 key = *cursor;
                char* text = (char*)(cursor + 1);
                this->meanings[key] = text;
            }
        }
    }
    const char * NamesCursor::get_meanings(const u16 word_key)
    {
        u16 key = word_key;

        return meanings.find((u16)key) != meanings.end()
            ? const_cast<const char*>(meanings[(u16)word_key])
            : (const char*) nullptr;
    }
}