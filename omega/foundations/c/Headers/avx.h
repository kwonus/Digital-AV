#ifndef AVX_HEADER
#define AVX_HEADER
#include <XVMem.h>

#include <stdint.h>
typedef uint8_t   byte;
typedef unsigned  char uchar;

typedef uint8_t   u8;
typedef uint16_t  u16;
typedef uint32_t  u32;
typedef uint64_t  u64;

typedef int8_t    i8;
typedef int16_t   i16;
typedef int32_t   i32;
typedef int64_t   i64;

// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the AVXC_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// AVXC_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef AVX_EXPORTS
#define AVX_API ExportLib
#else
#define AVX_API ImportLib
#endif

#ifdef NEVER
// Created by VC Wizard (not needed)
// This class is exported from the dll
class AVX_API Cavxc {
public:
	Cavxc(void);
	// TODO: add your methods here.
};

extern AVX_API int navxc;
AVX_API int fnavxc(void);
#endif

#endif //AVX_HEADER
