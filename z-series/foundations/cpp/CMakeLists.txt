cmake_minimum_required(VERSION 3.24)
project(cpp C)

set(CMAKE_C_STANDARD 11)

include_directories(.)

add_executable(cpp
        book_index.c
        book_index.h
        chapter_index.c
        chapter_index.h
        lemmata.c
        lemmata.h
        lemmata_oov.c
        lemmata_oov.h
        lexicon.c
        lexicon.h
        names.c
        names.h
        verse_index.c
        verse_index.h
        word_classes.c
        word_classes.h
        written.c
        written.h avxgen.h)
