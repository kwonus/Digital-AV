#include <phonetics.h>

namespace avx
{
    PhoneticsCursor::PhoneticsCursor()
    {
        if (global_instance != nullptr)
        {
            auto cursor = (u16*)global_instance->get_phonetic_data(&details);

            for (u32 record = 0; record < details.record_cnt; record++)
            {
                const u16 key = *cursor;
                const char* ipa = (const char*)(cursor + 1);
                this->add(key, ipa);
                int len = (int) Strnlen(ipa, 100);
                const char* address = ipa + len + 1;
                cursor = (u16*)address;
            }
        }
    }
    void PhoneticsCursor::add(const u16 word_key, const char* word_ipa)
    {
        int total_len = (int) Strnlen(word_ipa, 100);

        if (total_len < 100)
        {
            char text[100];
            Strncpy(text, word_ipa, total_len);
            int slash = 0;
            int end = 0;

            for (int i = 0; i <= total_len; i++)
            {
                if (text[i] == '/')
                    text[i] = '\0';
            }
            char* segment = text;
            int len = (int) Strnlen(segment, total_len);

            while (total_len > 0)
            {
                int len = (int) Strnlen(segment, total_len);
                if (len > 0)
                {
                    ipa[(u16)word_key].push_back(const_cast<char*>(segment));
                    //ipa_lookup[segment].push_back((u16)word_key);
                }
                ++len; // consume the trailing null;
                segment += len;
                total_len -= len;
            }
        }
    }
    const std::vector<std::string> PhoneticsCursor::get_ipa(const u16 word_key)
    {
        u16 key = word_key;

       return ipa.find((u16)key) != ipa.end()
            ? ipa[(u16)word_key]
            : EMPTY_IPA;
    }
    const std::vector<u16> PhoneticsCursor::get_keys(const char* ipa)
    {
        return (ipa_lookup.find(const_cast<char*>(ipa)) != ipa_lookup.end())
            ? ipa_lookup[const_cast<char*>(ipa)]
            : this->EMPTY_KEYS;
    }
}