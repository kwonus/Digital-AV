#include <lemmata.h>
#include <directory.h>

avx::LemmataCursor lemmata_instance;

extern "C" AVX_API const avx::Lemmata** lemmata_create_match_array(u16 word_key, u16 word_pos_bits, u32 word_nupos)
{
    return lemmata_instance.create_match_array(word_key, word_pos_bits, word_nupos);
}
extern "C" AVX_API bool lemmata_delete_match_array(const avx::Lemmata** array)
{
    return lemmata_instance.delete_match_array(array);
}

namespace avx
{
    LemmataCursor::LemmataCursor()
    {
        this->record_cursor = nullptr;

        u16 current_key = 0;
        u16 current_pos_bits = 0;
        u32 current_nupos = 0;
    }
    LemmataCursor::~LemmataCursor()
    {
        for (auto element : this->allocated_arrays)
        {
            delete element;
        }
    }

    const std::vector<Lemmata*> LemmataCursor::get_matches(u16 word_key, u16 word_pos_bits, u32 word_nupos)
    {
        this->record_cursor = (u16*)instance.get_lexicon_data(&details);
        this->record_index = 0;

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
        return true;
    }
    const avx::Lemmata** LemmataCursor::create_match_array (u16 word_key, u16 word_pos_bits, u32 word_nupos)
    {
        auto results = this->get_matches(word_key, word_pos_bits, word_nupos);

        auto len = results.size();
        auto array = new avx::Lemmata*[len+1];

        for (int i = 0; i < len; i++)
            array[i] = results[i];

        array[len] = nullptr;
        this->allocated_arrays.push_back(array);
        return const_cast<const avx::Lemmata**>(array);
    }
    bool LemmataCursor::delete_match_array(const avx::Lemmata** address)
    {
        auto old_len = this->allocated_arrays.size();
        auto element = const_cast<avx::Lemmata**>(address);

        this->allocated_arrays.erase(std::remove(
                this->allocated_arrays.begin(),
                this->allocated_arrays.end(),
                element),
                this->allocated_arrays.end());

        auto new_len = this->allocated_arrays.size();

        bool found = (new_len < old_len);

        if (found)
            delete address;

        return found;
    }
}