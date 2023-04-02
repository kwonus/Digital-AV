using AVX.Numerics;
using AVXLib.Framework;
using System.Runtime.CompilerServices;

namespace AVXLib
{
    internal class Program
    {
        void LibTest(string[] args)
        {
            var data = new Deserialization.Data(@"C:\src\Digital-AV\omega\AVX-Omega.data");

            if (args.Length >= 2)
            {
                var ortho = new Orthographical(data);
                var book = ortho.FindBook(args[0]);
                Console.Write(book.book.abbr4.ToString() + " " + args[1]);

                if (book.found)
                {
                    var cv = args[1].Split(':');
                    if (cv.Length == 2)
                    {
                        var verse = int.Parse(cv[1]);
                        var chapt = int.Parse(cv[0]);

                        if (chapt > 0 && chapt <= book.book.chapterCnt)
                        {
                            var chapter = data.Chapter.Span[book.book.chapterIdx + chapt - 1];
                            var writ = book.book.written.Span[chapter.writIdx];
                            var space = "\n";
                            int v = 1;
                            for (int w = 0; w < chapter.writCnt && v <= verse; /**/)
                            {
                                if (v == verse)
                                {
                                    for (int wordCount = writ.BCVWc.WC; wordCount > 0; wordCount--)
                                    {
                                        Console.Write(space + ortho.GetDisplayWithPunctuation(book.book.bookNum, (UInt16)(chapter.writIdx + w++)));
                                        space = " ";
                                    }
                                    break;
                                }
                                w += writ.BCVWc.WC;
                                v++;
                                writ = book.book.written.Span[chapter.writIdx + w];

                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Digital-AV is initialized.");
            }
        }
    }
}