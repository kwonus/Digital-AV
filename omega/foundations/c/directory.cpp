#include <directory.h>

static avx::directory* instance = nullptr;
extern "C" int release()
{
    if (instance != nullptr)
    {
        delete instance;
        instance = nullptr;
        return 0;
    }
    return -1;
}
extern "C" int acquire(const char* path)
{
    release();
    instance = new avx::directory(path);
    return instance->get_directory_data() != nullptr ? 0 : -1;
}

extern "C" const avx::artifact* get_artifact(const char label[]) {
    return instance != nullptr ? instance->get_artifact(label) : nullptr;
}
extern "C" const byte* get_data(const char label[], avx::artifact* details) {
    return instance != nullptr ? instance->get_data(label, details) : nullptr;
}
extern "C" const byte* get_directory_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_directory_data(details) : nullptr;
}
extern "C" const byte* get_book_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_book_data(details) : nullptr;
}
extern "C" const byte* get_chapter_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_chapter_data(details) : nullptr;
}
extern "C" const byte* get_written_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_written_data(details) : nullptr;
}
extern "C" const byte* get_lexicon_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_lexicon_data(details) : nullptr;
}
extern "C" const byte* get_lemmata_data(avx::artifact* details = nullptr) {
    return instance != nullptr ? instance->get_lemmata_data(details) : nullptr;
}
extern "C" const byte* get_oov_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_oov_data(details) : nullptr;
}
extern "C" const byte* get_names_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_names_data(details) : nullptr;
}
extern "C" const byte* get_phonetic_data(avx::artifact* details) {
    return instance != nullptr ? instance->get_phonetic_data(details) : nullptr;
}
extern "C" const avx::artifact* get_directory(avx::artifact* details) {
    return instance != nullptr ? instance->get_directory(details) : nullptr;
}
extern "C" const avx::Book* get_books(avx::artifact* details) {
    return instance != nullptr ? instance->get_books(details) : nullptr;
}
extern "C" const avx::Book* get_book(byte num, avx::artifact* details) {
    return instance != nullptr ? instance->get_book(num, details) : nullptr;
}
extern "C" const avx::Book* get_book_ex(const char name[], avx::artifact* details) {
    return instance != nullptr ? instance->get_book(name, details) : nullptr;
}
extern "C" const avx::Chapter* get_chapter(avx::artifact* details) {
    return instance != nullptr ? instance->get_chapter(details) : nullptr;
}
extern "C" const avx::Written* get_written(avx::artifact* details) {
    return instance != nullptr ? instance->get_written(details) : nullptr;
}

namespace avx
{
    directory::directory(const char path[])
    {
        this->memory.Acquire(const_cast<char*>(path), false, true);
    }
    directory::~directory()
    {
        this->memory.Release();
    }
    const artifact* directory::get_artifact(const char label[])
    {
        artifact* entry = (artifact*)memory.GetData();
        u32 cnt = entry[0].record_cnt;
        u32 len = entry[0].record_len;

        assert(len == sizeof(artifact));
        assert(Strnicmp(entry[0].label, DIRECTORY, sizeof(artifact::label) - 1) == 0);

        for (u32 i = 0; i < cnt; i++)
        {
            if (Strnicmp(entry[i].label, label, sizeof(artifact::label) - 1) == 0)
                return entry + i;
        }
        return nullptr;
    }
    const byte* directory::get_data(const char label[], artifact* details)
    {
        byte* data = memory.GetData();
        const artifact* entry = get_artifact(label);
        if (details != nullptr)
        {
            if (entry == nullptr)
            {
                details->length = 0;
                details->offset = 0;
                details->label[0] = '\0';
                details->record_cnt = 0;
                details->record_len = 0;
                details->hash_1 = 0;
                details->hash_2 = 0;
            }
            else
            {
                *details = *entry;
            }
        }
        byte* address = data != nullptr ? data + entry->offset : nullptr;

        return (data != nullptr
            && entry != nullptr
            && entry->offset < memory.GetSize()
            && (entry->offset + entry->offset + (entry->record_cnt * entry->record_len)) <= memory.GetSize())
             ? address
             : nullptr;
    }

    const Book* directory::get_book(byte num, artifact* details)
    {
        auto books = (Book*)get_book_data(details);
        auto entry = (artifact*)get_artifact(DIR_BOOK);

        return (books != nullptr && entry != nullptr && num < entry->record_cnt)
            ? books + num
            : nullptr;
    }
    const Book* directory::get_book(const char name[], artifact* details)
    {
        auto books = (Book*)get_book_data(details);
        auto entry = (artifact*)get_artifact(DIR_BOOK);

        if (books != nullptr && entry != nullptr && name != nullptr)
        {
            for (int num = 1; num < (int) entry->record_cnt; num++)
            {
                if (Strnicmp(name, books[num].name, sizeof(Book::name) - 1) == 0)
                    return books + num;
                if (Strnicmp(name, books[num].abbr, 2) == 0)
                    return books + num;
                if (Strnicmp(name, books[num].abbr + 3, 3) == 0)
                    return books + num;
                if (Strnicmp(name, books[num].abbr + 7, 4) == 0)
                    return books + num;

                int size = sizeof(Book::alts) - 1;
                int len = (int) Strnlen(books[num].alts, size);
                for (char* alt = books[num].alts; len > 0 && size > 0; alt += len)
                {
                    if (Strnicmp(name, alt, len) == 0)
                        return books + num;
                    size -= (len + 1);
                    len = (int) Strnlen(alt, size);
                }
            }
        }
        return nullptr;
    }
}