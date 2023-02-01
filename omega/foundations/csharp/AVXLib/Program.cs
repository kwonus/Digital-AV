using AVX.Numerics;
using System.Runtime.CompilerServices;

namespace AVXLib
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var input = @"C:\src\Digital-AV\omega\AVX-Omega.data";

            var fstream = new StreamReader(input);
            using (var reader = new System.IO.BinaryReader(fstream.BaseStream))
            {
                for (var entry = new Artifact(reader); !entry.ERROR; entry = new Artifact(reader))
                {
                    Console.WriteLine(entry.hash + ": " + entry.label);

                    if (entry.DONE)
                        break;
                }
            }
        }
    }
}