using AVX.Numerics;
using AVXLib.Framework;
using System.Runtime.CompilerServices;

namespace AVXLib
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var data = new Deserialization.Data(@"C:\src\Digital-AV\omega\AVX-Omega.data");
            Console.WriteLine("Digital-AV is initialized.");
        }
    }
}