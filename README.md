# Digital AV

It's been over twenty-five years since the release of AV-Bible for Windows (also known as AV-1995). In its day, it was the first free bible software with a graphical user interface that ran on Microsoft Windows. This "FreeWare" was maintaineded for more than a decade (AV-1996, AV-1997, ...). Eventually, it evolved into a software development kit (SDK). Accordingly, it was renamed Digital-AV some years ago.

You can rely on its foundation, with decades of deployments and text validation. Digital-AV has travelled the world and back with feedback from Christian missionaries. They used it in their ministries. I a recieved treasured quality-control feedback. By 2008, all of the textual bugs had been ironed out. Not coincidentally, 2008 was the first realease of the SDK. Never making a dime, and pouring a healthy chuck of my life into it. This has been a labor of love.

It's been over four hundred years since the original printing of the KJV bible, but I can assure you that I will not be maintaining Digital-AV four hundred years from now.

However, I am pleased to announce the availability of the new Alpha-Release of Digital-AV. The format has been drastically simplified to enable easy deserialization, but with no loss of features. The new 3.2 release is being dubbed the Omega Series of the SDK.  The documentation and format is different enough that it warranted a new name (it functionally replaces Z-Series; and there is obviously only one letter that follows Z).

Each revision of the SDK can be found in this repository. Current releases can be found in the /omega/ folder; the 3.1 release is still available in /z-series/ and more historical releases can be found in /legacy/

The Digital-AV SDK is a single 19mb file that can be used to manifest the entire text of the bible, including Strong's numbers, Lemmatizations, Part-of-Speech tags, and other linguistic features. But even with all of that, it's still the KJV Bible at the core.

The new SDK documentation can be found here:
https://github.com/kwonus/Digital-AV/blob/master/omega/Digital-AV-Ω32.pdf

The SDK now includes a foundations folder, with language-specific implementations of SDK artifacts. Initially, these include C++ and Rust. If you poke around in the repo, you will find the source generators in the /z-series/generation folder. However, the generated source itself can be found in /omega/foundations

An additional reference implementation for C# can be found in a companion repo named AVXText. It is based upon the Z14 release. It is soon to be replaced with a foundations release as per the above documentation link.

Finally, all of the text in each of these digital editions remains faithful to the original AV Bible and the orthography is consistent with the 1769 edition.

The Lord gave the word: great was the company of those that published it. -- Psalm 68:11

The project home is http://Digital-AV.org
