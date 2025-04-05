unit AVXLib;

interface

uses Windows, SysUtils;

procedure Release;
function Acquire(path: PChar): Integer;

implementation

  type
  TArtifact = packed record
    section: array[0..15] of Char;
    offset: UInt32;
    length: UInt32;
    hash_1: UInt64;
    hash_2: UInt64;
    record_len: UInt32;
    record_cnt: UInt32;
  end;

  TBook = packed record
    book_num: Byte;
    chapter_cnt: Byte;
    chapter_idx: UInt16;
    writ_cnt: UInt16;
    writ_idx: UInt16;
    name: array[0..15] of Char;
    abbr: array[0..11] of Char;
    alts: array[0.. 9] of Char;
  end;

  TChapter = packed record
    writ_idx: UInt16;
    writ_cnt: UInt16;
    book_num: Byte;
    verse_cnt: Byte;
  end;

  type
  TWritten = packed record
    strongs: array[0..3] of UInt16;
    bcv_wc: array[0..3] of Byte;
    word_key: UInt16;
    pn_pos12: UInt16;
    pos32: UInt32;
    lemma: UInt16;
    punctuation: Byte;
    transition: Byte;
  end;

  PArtifact = ^TArtifact;
  PBook = ^TBook;
  PChapter = ^TChapter;
  PWritten = ^TWritten;

  TWordKeyBits = record
    const CAPS = $C000;
    const CAPS_FirstLetter = $8000;
    const CAPS_AllLetters = $4000;
    const WordKey = $3FFF;
  end;

  TLemmaBits = record
    const OOV_Marker = $8000;
    const ModernizationSquelch_Marker = $4000;
    const OOV_Lookup_Mask = $FFFF xor ModernizationSquelch_Marker;
    const Lexicon_Lookup_Mask = $FFFF xor TWordKeyBits.CAPS;
  end;

  TPunctuation = record
    const Clause = $E0;
    const Exclamatory = $80;
    const Interrogative = $C0;
    const Declarative = $E0;
    const Dash = $A0;
    const Semicolon = $20;
    const Comma = $40;
    const Colon = $60;
    const Possessive = $10;
    const CloseParen = $0C;
    const Parenthetical = $04;
    const Italics = $02;
    const Jesus = $01;
  end;

  TPersonNumber = record
    const PersonBits = $3000;
    const NumberBits = $C000;
    const Indefinite = $0000;
    const Person1st = $1000;
    const Person2nd = $2000;
    const Person3rd = $3000;
    const Singular = $4000;
    const Plural = $8000;
    const WH = $C000;
  end;

  TPOS12 = record
    const NounOrPronoun = $030;
    const Noun = $010;
    const Noun_UnknownGender = $010;
    const ProperNoun = $030;
    const Pronoun = $020;
    const Pronoun_Neuter = $021;
    const Pronoun_Masculine = $022;
    const Pronoun_NonFeminine = $023;
    const Pronoun_Feminine = $024;
    const PronounOrNoun_Genitive = $008;
    const PronounOrNoun_Genitive_MASK = PronounOrNoun_Genitive or Noun or Pronoun;
    const Noun_Genitive = PronounOrNoun_Genitive or Noun;
    const Pronoun_Genitive = PronounOrNoun_Genitive or Pronoun;
    const Pronoun_Nominative = $060;
    const Pronoun_Objective = $0A0;
    const Pronoun_Reflexive = $0E0;
    const Pronoun_NoCase_NoGender = $020;
    const Verb = $100;
    const PrepTo = $200;    // To is a reserved word
    const Preposition = $400;
    const Interjection = $800;
    const Adjective = $A00;
    const Numeric = $B00;
    const Conjunction = $C00;
    const Determiner = $D00;
    const Particle = $E00;
    const Adverb = $F00;
    const NonNoun = $F00;
  end;

  TTransitions = record
    const EndBit = $10;
    const BeginningOfVerse = $20;
    const EndOfVerse = $30;
    const BeginningOfChapter = $60;
    const EndOfChapter = $70;
    const BeginningOfBook = $E0;
    const EndOfBook = $F0;
    const BeginningOfBible = $E8;
    const EndOfBible = $F8;
  end;

  TSegments = record
    const HardSegmentEnd = $04;
    const CoreSegmentEnd = $02;
    const SoftSegmentEnd = $01;
    const RealSegmentEnd = $06;
  end;

  TEntities = record
    const Hitchcock = $8000;
    const Men = $1;
    const Women = $2;
    const Tribes = $4;
    const Cities = $8;
    const Rivers = $10;
    const Mountains = $20;
    const Animals = $40;
    const Gemstones = $80;
    const Measurements = $100;
  end;

  TOOV = record
    const Marker = $8000;
    const Length = $0F00;
    const Index = $00FF;
  end;

 var
   avx: Thandle;
   avx_acquire:        function (const omega: PChar): Integer; cdecl;
   avx_release:        function (): Integer; cdecl;
   get_artifact:       function (const section: PChar): PArtifact; cdecl;
   get_data:           function (const section: PChar; artifact: PArtifact): PChar; cdecl;
   get_directory_data: function (artifact: PArtifact): PChar; cdecl;
   get_book_data:      function (artifact: PArtifact): PChar; cdecl;
   get_chapter_data:   function (artifact: PArtifact): PChar; cdecl;
   get_written_data:   function (artifact: PArtifact): PChar; cdecl;
   get_lexicon_data:   function (artifact: PArtifact): PChar; cdecl;
   get_lemmata_data:   function (artifact: PArtifact): PChar; cdecl;
   get_oov_data:       function (artifact: PArtifact): PChar; cdecl;
   get_names_data:     function (artifact: PArtifact): PChar; cdecl;
   get_phonetic_data:  function (artifact: PArtifact): PChar; cdecl;
   get_directory:      function (artifact: PArtifact): PArtifact; cdecl;
   get_books:          function (artifact: PArtifact): PBook; cdecl;
   get_book:           function (artifact: PArtifact): PBook; cdecl;
   get_book_ex:        function (artifact: PArtifact): PBook; cdecl;
   get_chapter:        function (artifact: PArtifact): PChapter; cdecl;
   get_written:        function (artifact: PArtifact): PWritten; cdecl;

 procedure Release;
 begin
   if avx >= 32
   then begin
     avx_release();
     FreeLibrary(avx);
     avx := 0;
   end;
 end;

 function Acquire(path: PChar): Integer;
 begin
   Release;
   avx := LoadLibrary('C:\src\Digital-AV\omega\foundations\c\x64\Release\avxc.dll');

   if avx >= 32
   then begin
     avx_acquire       := GetProcAddress(avx, 'avx_acquire');
     avx_release       := GetProcAddress(avx, 'avx_release');
     get_artifact      := GetProcAddress(avx, 'get_artifact');
     get_data          := GetProcAddress(avx, 'get_data');
     get_directory_data:= GetProcAddress(avx, 'get_directory_data');
     get_book_data     := GetProcAddress(avx, 'get_book_data');
     get_chapter_data  := GetProcAddress(avx, 'get_chapter_data');
     get_written_data  := GetProcAddress(avx, 'get_written_data');
     get_lexicon_data  := GetProcAddress(avx, 'get_lexicon_data');
     get_lemmata_data  := GetProcAddress(avx, 'get_lemmata_data');
     get_oov_data      := GetProcAddress(avx, 'get_oov_data');
     get_names_data    := GetProcAddress(avx, 'get_names_data');
     get_phonetic_data := GetProcAddress(avx, 'get_phonetic_data');
     get_directory     := GetProcAddress(avx, 'get_directory');
     get_books         := GetProcAddress(avx, 'get_books');
     get_book          := GetProcAddress(avx, 'get_book');
     get_book_ex       := GetProcAddress(avx, 'get_book_ex');
     get_chapter       := GetProcAddress(avx, 'get_chapter');
     get_written       := GetProcAddress(avx, 'get_written');

     try
       result := avx_acquire(path);
       except on E: Exception do
         MessageBox(0, PChar(E.Message), 'Exception', 0);
       end;
   end
   else begin
     result := -100;
   end;
 end;

Initialization
  avx := 0;
end.