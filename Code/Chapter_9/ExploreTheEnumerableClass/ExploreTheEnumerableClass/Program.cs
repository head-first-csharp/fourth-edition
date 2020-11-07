using System;

namespace ExploreTheEnumerableClass
{
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var emptyInts = Enumerable.Empty<int>(); // an empty sequence of ints
            var emptyComics = Enumerable.Empty<Comic>(); // an empty sequence of Comic references

            var oneHundredThrees = Enumerable.Repeat(3, 100);
            var twelveYesStrings = Enumerable.Repeat("yes", 12);
            var eightyThreeObjects = Enumerable.Repeat(
                new { cost = 12.94M, sign = "ONE WAY", isTall = false }, 83);

        }
    }
}
