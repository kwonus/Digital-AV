#include <avx.h>

namespace avx
{
    struct Lexicon
    {
        u16 entities;
        u16 pos_cnt;
        u32* pos;
        char* search;
        char* display;
        char* moderns;
    };
}
/*
    public abstract class LEXICON
    {
        public static string ToSearchString(Lexicon lex)
        {
            return lex.Search.ToString();
        }
        public static string ToDisplayString(Lexicon lex)
        {
            return (lex.Display.Length > 0) ? lex.Display.ToString() : lex.Search.ToString(); 
        }
        public static string ToModernString(Lexicon lex)
        {
            return (lex.Modern.Length  > 0) ? lex.Modern.ToString()  : (lex.Display.Length > 0) ? lex.Display.ToString() : lex.Search.ToString();
        }
        public static bool IsModernSameAsDisplay(Lexicon lex)
        {
            return (lex.Modern.Length == 0);
        }
        public static bool IsHyphenated(Lexicon lex)
        {
            return (lex.Display.Length != 0);
        }
    }
}
*/
