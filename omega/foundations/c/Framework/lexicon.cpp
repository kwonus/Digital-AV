#include <lexicon.h>
#include <directory.h>

extern "C" const avx::Lexicon* lexicon_get(const u16 key)
{
    avx::instance.lexicon.get(key);
}

namespace avx
{
    const u32* Lexicon::get_pos()
    {
        return this->pos_cnt > 0 ? this->remainder : nullptr;
    }
    const char* Lexicon::get_search()
    {
        return (char*)(this->remainder + pos_cnt);
    }
    const char* Lexicon::get_display(bool with_cascade)
    {
        const char* search = (char*)(this->remainder + pos_cnt);
        const char* display = (char*)(this->remainder + pos_cnt) + 1 + Strnlen(search, 24);
        return (*display != '\0') || !with_cascade
            ? display
            : search;
    }
    const char* Lexicon::get_modern(bool with_cascade)
    {
        const char* search = (char*)(this->remainder + pos_cnt);
        const char* display = (char*)(this->remainder + pos_cnt) + 1 + Strnlen(search, 24);
        const char* modern = (char*)(this->remainder + pos_cnt) + 2 + Strnlen(search, 24) + Strnlen(display, 24);
        return (*modern != '\0') || !with_cascade
            ? modern
            : (*display != '\0')
            ? display
            : search;
    }

    LexiconCursor::LexiconCursor()
    {
        this->record_cursor = nullptr;
        this->quit();
    }
    void LexiconCursor::quit()
    {
        for (u16 key = 0; key <= AV_LEX_CNT; key++)
        {
            this->cache[key] = nullptr;
        }
    }
    bool LexiconCursor::init()
    {
        int cnt = 0;
        this->quit();

        for (const avx::Lexicon* lex = avx::instance.lexicon.get_first(); lex != nullptr; lex = avx::instance.lexicon.get_next())
            cnt++;

        return cnt - 1 == AV_LEX_CNT;
    }
    const Lexicon* LexiconCursor::get_first()
    {
        this->record_cursor = (u16*)instance.get_lexicon_data(&details);
        this->record_index = 0;

        return this->get_next();
    }
    const Lexicon* LexiconCursor::get(const u16 key)
    {
        this->record_index = key;
        return this->get_next();
    }
    const Lexicon* LexiconCursor::get_next()
    {
        Lexicon* record = this->record_index <= AV_LEX_CNT ? this->cache[this->record_index] : nullptr;

        if (record != nullptr)
        {
            this->record_index++;
        }
        else if (this->record_cursor != nullptr && details.record_cnt > 0 && this->record_index < this->details.record_cnt)
        {
            byte* address = (byte*) this->record_cursor;

            record = (Lexicon*)this->record_cursor;
            if (this->record_index <= AV_LEX_CNT)
                this->cache[this->record_index] = record;

            address += (2 * sizeof(u16));
            address += (record->pos_cnt * sizeof(u32));
            address += Strnlen(record->get_search(), 24);
            address += Strnlen(record->get_display(false), 24);
            address += Strnlen(record->get_modern(false), 24);
            address += 3; // three null terminators on the three strings

            this->record_cursor = (u16*)address;

            this->record_index ++;
        }
        return record;
    }

    LexiconRecord::LexiconRecord(const Lexicon& lex)
        : entities(lex.entities)
        , pos_cnt(lex.pos_cnt)
        , pos(lex.pos_cnt > 0 ? lex.remainder : nullptr)
        , search( (char*)(lex.remainder + lex.pos_cnt))
        , display((char*)(this->search  + 1 + Strnlen(this->search,  24)))
        , modern( (char*)(this->display + 1 + Strnlen(this->display, 24)))
    {
        ;
    }
}