#ifndef AVX_PHONETCIS
#define AVX_PHONETCIS
#include <avx.h>
#include <artifact.h>
#include <map>
#include <vector>
#include <string>

namespace avx
{
    struct Phonetics
    {
        const u16 word_key;
        const char* ipa;
    };
    class PhoneticsCursor
    {
    private:
        std::map<u16, char*> ipa;        // one or more ipa strings delimited by '/' (one record per word_key)
#ifdef AVX_IPA_REVERSE_LOOKUP
        std::map<std::string, std::vector<u16>> ipa_lookup; // reverse lookup of a single ipa string (multiple word_keys are possible, but not common; e.g. there + their) 
#endif
        artifact details;
        void add(const u16 word_key, const char* word_ipa);
#ifdef AVX_IPA_REVERSE_LOOKUP
        const std::vector<u16> EMPTY_KEYS;
#endif
        const char* EMPTY_IPA;
    public:
        PhoneticsCursor();
        bool init();
        void free();
        const char* get_ipa(const u16 word_key);
#ifdef AVX_IPA_REVERSE_LOOKUP
        const std::vector<u16> get_keys(const char* ipa);
#endif
    };
}
extern "C" bool phonetics_init();
extern "C" void phonetics_free();
extern "C" const char* phonetics_get_ipa(const u16 key);
#ifdef AVX_IPA_REVERSE_LOOKUP
extern "C" const std::vector<u16> phonetics_get_keys(const char* ipa);
#endif
#endif