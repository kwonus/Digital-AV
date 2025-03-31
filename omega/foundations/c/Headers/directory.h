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
        XVMem<byte> memory;

    public:
        directory(const char path[]);
        ~directory();

        const artifact* get_artifact(const char label[]);
        const byte* get_data(const char label[], artifact* details = nullptr);

        inline const byte* get_directory_data(artifact* details = nullptr)
        {
            return this->get_data(DIRECTORY, details);
        }
        inline const byte* get_book_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_BOOK, details);
        }
        inline const byte* get_chapter_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_CHAPTER, details);
        }
        inline const byte* get_written_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_WRITTEN, details);
        }
        inline const byte* get_lexicon_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_LEXICON, details);
        }
        inline const byte* get_lemmata_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_LEMMATA, details);
        }
        inline const byte* get_oov_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_OOV, details);
        }
        inline const byte* get_names_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_NAMES, details);
        }
        inline const byte* get_phonetic_data(artifact* details = nullptr)
        {
            return this->get_data(DIR_PHONETIC, details);
        }

        // For fixed length records, we have additional methods
        //
        inline const artifact* get_directory(artifact* details = nullptr)
        {
            return (artifact*) get_directory_data(details);
        }
        inline const Book* get_books(artifact* details = nullptr)
        {
            return (Book*) get_book_data(details);
        }
        const Book* get_book(byte num, artifact* details = nullptr);
        const Book* get_book(const char name[], artifact* details = nullptr);

        inline const Chapter* get_chapter(artifact* details = nullptr)
        {
            return (Chapter*) get_chapter_data(details);
        }
        inline const Written* get_written(artifact* details = nullptr)
        {
            return (Written*)get_written_data(details);
        }
    };
}
