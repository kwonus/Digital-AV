# Digital AV

The Digital AV is a software development kit (SDK) for the Authorized Version (AV) of the King James Bible. It was first digitally released in 2008, but after a handful of bug fixes and minor revisions it became known as the 2011 edition.  The original SDK was just three binary files and a document that described the binary record layouts.  The final release of 2011 edition was 2017-08-15

Later the SDK was enhanced with additional features and released again as the 2018 edition.  The 2018 edition more than tripled the number of binary files.  Each file manifests additional features and capabilities.  The most recent release of the 2018 edition was 2020-08-17.

The latest edition of the SDK is being called the Z-series edition.  Packed with additional features, and still more binary files, the public release can be found here.  Please give us feedback if you're using it and you discover any anomalies. Please note that files in the Z-series/pos folder are not formally part of the SDK; and are therefore not described on Digital-AV.pdf (The text files in that pos folder are used to generate some of the binary files that compose the SDK)

Each revision of the SDK can be found in this repository.  And within each folder, including legacy releases, an SDK document can be found that describes the folder contents.  Editions prior to the Z-series can be found in the folder named ./legacy/.

Some of the folders contain sample source code, but the SDK is primarily a set of binary files that can be used to manifest the entire text of the the bible, along with Strong's numbers, Lemmatizations, and Part-of-Speech tags.  For details, consult the included documentation at:

https://github.com/kwonus/Digital-AV/blob/master/z-series/Digital-AV.pdf

A reference implementation, in C++ using the latest SDK, can be found in the companion repositories called AVXLib / AVXTest.

***Privacy Policy:***
While AV-Bible reads data from the AppData/Digital-AV folder, it ONLY reads files that it first downloaded. On first execution, AV-Bible downloads the KJV files as compact encoded binaries from Digital-AV.org.  It needs these files for search & display of the bible text. The only other access to your local system is to store and retrieve AV-Bible program settings. AV-Bible accesses no other files on the user's file-system.  In short, AV-Bible does not invade your privacy, neither does it read any data that it it did not first write.  In fact, AV-Bible never even records what you search on; and it certainly never sends any data to any servers.  It runs stealthily entirely on your Windows desktop.

Finally, all of the text in each of these digital editions remains faithful to the original AV Bible and the orthography is consistent with the 1769 edition.

The project home is http://Digital-AV.org
