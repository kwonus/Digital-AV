cmake_minimum_required(VERSION 3.24)
project(cpp)

set(CMAKE_CPP_STANDARD 20)

include_directories(.)

add_executable(cpp
        book_index.cpp
        book_index.h
        chapter_index.cppgit
        chapter_index.h
        lemmata.cpp
        lemmata.h
        lemmata_oov.cpp
        lemmata_oov.h
        lexicon.cpp
        lexicon.h
        names.cpp
        names.h
        verse_index.cpp
        verse_index.h
        word_classes.cpp
        word_classes.h
        written.h,
        avxgen.h,

        written/written_01.cpp,
        written/written_02.cpp,
        written/written_03.cpp,
        written/written_04.cpp,
        written/written_05.cpp,
        written/written_06.cpp,
        written/written_07.cpp,
        written/written_08.cpp,
        written/written_09.cpp,
        written/written_10.cpp,

        written/written_11.cpp,
        written/written_12.cpp,
        written/written_13.cpp,
        written/written_14.cpp,
        written/written_15.cpp,
        written/written_16.cpp,
        written/written_17.cpp,
        written/written_18.cpp,
        written/written_19.cpp,
        written/written_20.cpp,

        written/written_31.cpp,
        written/written_32.cpp,
        written/written_33.cpp,
        written/written_34.cpp,
        written/written_35.cpp,
        written/written_36.cpp,
        written/written_37.cpp,
        written/written_38.cpp,
        written/written_39.cpp,
        written/written_40.cpp,

        written/written_51.cpp,
        written/written_52.cpp,
        written/written_53.cpp,
        written/written_54.cpp,
        written/written_55.cpp,
        written/written_56.cpp,
        written/written_57.cpp,
        written/written_58.cpp,
        written/written_59.cpp,
        written/written_60.cpp,

        written/written_61.cpp,
        written/written_62.cpp,
        written/written_63.cpp,
        written/written_64.cpp,
        written/written_65.cpp,
        written/written_66.cpp
)
