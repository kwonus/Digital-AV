#include <avx.h>
#include <directory.h>
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
        std::map<u16, std::vector<std::string>> ipa;        // one or more ipa strings delimited by '/' (one record per word_key)
        std::map<std::string, std::vector<u16>> ipa_lookup; // reverse lookup of a single ipa string (multiple word_keys are possible, but not common; e.g. there + their) 
        artifact details;
        void add(const u16 word_key, const char* word_ipa);
        const std::vector<u16> EMPTY_KEYS;
        const std::vector<std::string> EMPTY_IPA;
    public:
        PhoneticsCursor();
        const std::vector<std::string> get_ipa(const u16 word_key);
        const std::vector<u16> get_keys(const char* ipa);
    };
}