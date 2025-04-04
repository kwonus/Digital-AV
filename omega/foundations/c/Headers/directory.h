#ifndef AVX_DIRECTORY
#define AVX_DIRECTORY
#include <avx.h>
#include <string>
#include <artifact.h>
#include <book.h>
#include <chapter.h>
#include <written.h>
#include <lexicon.h>
#include <names.h>
#include <oov.h>
#include <lemmata.h>
#include <phonetics.h>
#include <cassert>

static const char DIRECTORY[]    = "Directory";
static const char DIR_BOOK[]     = "Book";
static const char DIR_CHAPTER[]  = "Chapter";
static const char DIR_WRITTEN[]  = "Written";
static const char DIR_LEXICON[]  = "Lexicon";
static const char DIR_LEMMATA[]  = "Lemmata";
static const char DIR_OOV[]      = "OOV-Lemmata";
static const char DIR_NAMES[]    = "Names";
static const char DIR_PHONETIC[] = "Phonetic";

namespace avx
{
    class directory
    {
    private:
        XVMem<byte> memory;

    public:
        directory();
        ~directory();

        LemmataCursor lemmata;
        LexiconCursor lexicon;
        NamesCursor names;
        PhoneticsCursor phonetics;
        oov_lemmata_cursor oov;

        const artifact* get_artifact(const char label[]);
        const byte* get_data(const char label[], artifact* details = nullptr);

        inline bool acquire(const char path[])
        {
            this->release();
            bool ok = (this->memory.Acquire(const_cast<char*>(path), false, true) != nullptr) && directory::lexicon.init();

            return ok;
        }
        inline void release()
        {
            this->memory.Release();
        }

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
    extern directory instance;
}
extern "C" void release();
extern "C" int acquire(const char* path);
extern "C" const avx::artifact* get_artifact(const char label[]);
extern "C" const byte* get_data(const char label[], avx::artifact* details);
extern "C" const byte* get_directory_data(avx::artifact* details);
extern "C" const byte* get_book_data(avx::artifact* details);
extern "C" const byte* get_chapter_data(avx::artifact* details);
extern "C" const byte* get_written_data(avx::artifact* details);
extern "C" const byte* get_lexicon_data(avx::artifact* details);
extern "C" const byte* get_lemmata_data(avx::artifact* details);
extern "C" const byte* get_oov_data(avx::artifact* details);
extern "C" const byte* get_names_data(avx::artifact* details);
extern "C" const byte* get_phonetic_data(avx::artifact* details);
extern "C" const avx::artifact* get_directory(avx::artifact* details);
extern "C" const avx::Book* get_books(avx::artifact* details);
extern "C" const avx::Book* get_book(byte num, avx::artifact* details);
extern "C" const avx::Book* get_book_ex(const char name[], avx::artifact* details);
extern "C" const avx::Chapter* get_chapter(avx::artifact* details);
extern "C" const avx::Written* get_written(avx::artifact* details);

#endif
