#ifndef AVX_LEMMATA
#define AVX_LEMMATA
#include <avx.h>
#include <artifact.h>
#include <vector>

#define AV_LEMMA_CNT      15171   // from AV-Inventory-Z31.bom; consistant with Release Version 5.1 (+1 for metadata)
#define AV_LEMMA_FILE_LEN 182344  // from AV-Inventory-Z31.bom; consistant with Release Version 5.1

namespace avx
{
#pragma pack(2)
    struct Lemmata
    {
        u32 pos32;
        u16 word_key;
        u16 pn_pos12;
        u16 lemma_cnt;
        u16* lemma_array;
    };

    class LemmataCursor
    {
    private:
        u16* record_cursor;
        u16  record_index;
        artifact details;

        u16 current_key;
        u16 current_pos_bits;
        u32 current_nupos;

        bool get_next_match(std::vector<Lemmata*> &results);
        std::vector<Lemmata**> allocated_arrays;

    public:
        LemmataCursor();
        ~LemmataCursor();
        // These are not keyed or hash look-ups; they are always sequential-scans and possible multi-record returns
        const std::vector<Lemmata*> get_matches(u16 word_key, u16 word_pos_bits, u32 word_nupos); // to fetch all, pass (0,0,0)
        const Lemmata** create_match_array(u16 word_key, u16 word_pos_bits, u32 word_nupos);
        bool delete_match_array(const Lemmata** array);
    };
}
extern "C" const avx::Lemmata** lemmata_create_match_array(u16 word_key, u16 word_pos_bits, u32 word_nupos);
extern "C" bool lemmata_delete_match_array(const avx::Lemmata** array);

#endif