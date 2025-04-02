#include <lemmata.h>

namespace avx
{
    LemmataCursor::LemmataCursor()
    {
        this->record_cursor = nullptr;

        u16 current_key = 0;
        u16 current_pos_bits = 0;
        u32 current_nupos = 0;
    }
    const std::vector<Lemmata*> LemmataCursor::get_matches(u16 word_key, u16 word_pos_bits, u32 word_nupos)
    {
        if (global_instance != nullptr)
        {
            this->record_cursor = (u16*)global_instance->get_lexicon_data(&details);
            this->record_index = 0;
        }
        this->current_key = word_key;
        this->current_pos_bits = word_pos_bits;
        this->current_nupos = word_nupos;

        std::vector<Lemmata*> results;

        for (bool ok = this->get_next_match(results); ok; ok = this->get_next_match(results))
        {
            ;
        }
        this->current_key = 0;
        this->current_pos_bits = 0;
        this->current_nupos = 0;

        return results;
    }
    bool LemmataCursor::get_next_match(std::vector<Lemmata*>& results)
    {
        if (this->record_cursor != nullptr && details.record_cnt > 0 && this->record_index < this->details.record_cnt)
        {
            byte* address = (byte*) this->record_cursor;

            auto record = (Lemmata*)this->record_cursor;

            address += sizeof(Lemmata);
            address -= sizeof(Lemmata::lemma_array);
            address += (sizeof(u16) * record->lemma_cnt);

            this->record_cursor = (u16*)address;
            this->record_index++;

            bool match = (this->current_key      == 0 || record->word_key == this->current_key)
                      && (this->current_pos_bits == 0 || record->pn_pos12 == this->current_pos_bits)
                      && (this->current_nupos    == 0 || record->pos32    == this->current_nupos);

            if (match)
            {
                results.push_back(record);
            }
        }
    }
}