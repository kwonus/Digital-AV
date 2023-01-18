# Digital AV

The Digital AV is a software development kit (SDK) for the Authorized Version (AV) of the King James Bible. It was first digitally released in 2008, but after a handful of bug fixes and minor revisions it became known as the 2011 edition.  The original SDK was just three binary files and a document that described the binary record layouts.  The final release of 2011 edition was 2017-08-15

Later the SDK was enhanced with additional features and released again as the 2018 edition.  The 2018 edition increased the number of binary files.  Each file manifests additional features and capabilities.  The most recent release of the 2018 edition was 2020-08-17.

The latest SDK is known as the Z-series edition.  With still more binary files, it vastly improves the number of intrinsic labguage featires. Its release can be found here. Please note that files in the Z-series/pos folder are not formally part of the SDK; and are therefore not described on Digital-AV.pdf specification (The text files in the pos folder are informational and used to generate some of the binary files that compose the SDK)

Each revision of the SDK can be found in this repository.  This repo also contains the earlier legacy releases: each with its own SDK specifications (Legacy editions can be found in the folder named ./legacy/; versioning of the current Z-series edition uses normal revisions in a folder named ./z-series/).

The SDK is primarily a set of binary files that can be used to manifest the entire text of the the bible, along with Strong's numbers, Lemmatizations, and Part-of-Speech tags.  For details, consult the included documentation at:

https://github.com/kwonus/Digital-AV/blob/master/z-series/Digital-AV.pdf

Still, there are also reference implementations:
- AVXText (a companion repo) is a C# implementation based upon the Z14 revision of the SDK, which deserializes all SDK files into a framework for searching and/or printing.
- In the ./foundations/ folder of this repo, there are two addional reference implementations that require zero deserialization. One for Rust; Another for C++. Each of these is derived from the Z31 release.

Finally, all of the text in each of these digital editions remains faithful to the original AV Bible and the orthography is consistent with the 1769 edition.

The project home is http://Digital-AV.org
