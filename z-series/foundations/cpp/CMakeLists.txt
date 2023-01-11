cmake_minimum_required(VERSION 3.24)
project(cpp)

set(CMAKE_CPP_STANDARD 20)

include_directories(.)

add_executable(cpp
        book_index.cpp
        book_index.h
        chapter_index.cpp
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
        written.cpp
        written.h avxgen.h)
