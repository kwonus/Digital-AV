#include <avx.h>
#include <string>
#include <artifact.h>
#include <book.h>
#include <chapter.h>
#include <written.h>
#include <cassert>

#define DIRECTORY   "Directory"
#define DIR_BOOK     "Book"
#define DIR_CHAPTER  "Chapter"
#define DIR_WRITTEN  "Written"
#define DIR_LEXICON  "Lexicon"
#define DIR_LEMMATA  "Lemmata"
#define DIR_OOV      "OOV-Lemmata"
#define DIR_NAMES    "Names"
#define DIR_PHONETIC "Phonetic"

namespace avx
{
    class directory
    {
    private:
        XVMem<byte> &memory;

    public:
        directory(XVMem<byte>& xvmem);
        ~directory();

        const artifact* get_artifact(const char label[]);

        inline const byte* get_directory_data()
        {
            auto entry = get_artifact(DIRECTORY);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_book_data()
        {
            auto entry = get_artifact(DIR_BOOK);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_chapter_data()
        {
            auto entry = get_artifact(DIR_CHAPTER);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_written_data()
        {
            auto entry = get_artifact(DIR_WRITTEN);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_lexicon_data()
        {
            auto entry = get_artifact(DIR_LEXICON);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_lemmata_data()
        {
            auto entry = get_artifact(DIR_LEMMATA);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_oov_data()
        {
            auto entry = get_artifact(DIR_OOV);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_names_data()
        {
            auto entry = get_artifact(DIR_NAMES);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }
        inline const byte* get_phonetic_data()
        {
            auto entry = get_artifact(DIR_PHONETIC);
            return (entry != nullptr) ? memory.GetData() + entry->offset : nullptr;
        }

        // For fixed length records, we have additional inlines
        //
        inline const artifact* get_directory()
        {
            return (artifact*) get_directory_data();
        }
        inline const Book* get_books()
        {
            return (Book*) get_book_data();
        }
        inline const Book* get_book(byte num)
        {
            auto books = (Book*) get_book_data();
            auto entry = (artifact*)get_artifact(DIR_BOOK);

            return (books != nullptr && entry != nullptr && num < entry->record_cnt)
                ? books + num
                : nullptr;
        }
        inline const Book* get_book(const char name[])
        {
            auto books = (Book*)get_book_data();
            auto entry = (artifact*)get_artifact(DIR_BOOK);

            if (books != nullptr && entry != nullptr && name != nullptr)
            {
                for (int num = 1; num < entry->record_cnt; num++)
                {
                    if (Strnicmp(name, books[num].name, sizeof(Book::name) - 1) == 0)
                        return books + num;
                    if (Strnicmp(name, books[num].abbr, 2) == 0)
                        return books + num;
                    if (Strnicmp(name, books[num].abbr+3, 3) == 0)
                        return books + num;
                    if (Strnicmp(name, books[num].abbr + 7, 4) == 0)
                        return books + num;

                    int size = sizeof(Book::alts) - 1;
                    int len = Strnlen(books[num].alts, size);
                    for (char* alt = books[num].alts; len > 0 && size > 0; alt += len)
                    {
                        if (Strnicmp(name, alt, len) == 0)
                            return books + num;
                        size -= (len+1);
                        len = Strnlen(alt, size);
                    }
                }
            }
            return nullptr;
        }
        inline const Chapter* get_chapter()
        {
            return (Chapter*) get_chapter_data();
        }
        inline const Written* get_written()
        {
            return (Written*)get_written_data();
        }
    };
}
