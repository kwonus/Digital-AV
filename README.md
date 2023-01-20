# Digital AV

The Digital AV is a software development kit (SDK) for the Authorized Version (AV) of the King James Bible. It was first digitally released in 2008, but after a handful of bug fixes and minor revisions it became known as the 2011 edition.  The original SDK was just three binary files and a document that described the binary record layouts.

Later the SDK was enhanced with additional features and released again as the 2018 edition.  The 2018 edition increased the number of binary files.  Each file manifests additional features and capabilities.  The latest SDK is known as the Z-series edition.  With still more binary files, it vastly improves the number of intrinsic language features. Its release can be found here.

Each revision of the SDK can be found in this repository.  This repo also contains the earlier legacy releases: each with its own SDK specifications (Legacy editions can be found in the folder named ./legacy/; versioning of the current Z-series edition uses normal revisions in a folder named ./z-series/).

The SDK is primarily a set of binary files that can be used to manifest the entire text of the the bible, along with Strong's numbers, Lemmatizations, and Part-of-Speech tags.  For details, consult the included documentation at:

https://github.com/kwonus/Digital-AV/blob/master/z-series/Digital-AV.pdf

Only files explicitly mentioned in the above spec are part of the official baseline SDK. Some files, and the Z-series/pos folder in particular, are informational. Many are inputs to the generation of the baseline SDK itself.

New in the Z31 release is the addition of the z-series/foundations folder. The SDK foundations folder contains language-specific implementations of SDK artifacts as native arrays of structs. Initially, these include C++ and Rust. If you poke arounf in the repo, it should be easy to find the C# source generators for these foundations. I don't want to cite the current folder URL here, because refactoring plans are to move them to a more sensible location.

An additional reference implementation for C# can be found in a companion repo named AVXText. It is based upon the Z14 release.  The C# AVText project performs deserialization of the SDK files. This is differennt from the /foundations/ implementations which require no deserialization.


Finally, all of the text in each of these digital editions remains faithful to the original AV Bible and the orthography is consistent with the 1769 edition.

The project home is http://Digital-AV.org
