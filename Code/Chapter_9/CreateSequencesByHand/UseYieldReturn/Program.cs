using System;

namespace UseYieldReturn
{
    using System.Collections.Generic;

    class Program
    {
        static IEnumerable<string> SimpleEnumerable()
        {
            yield return "apples";
            yield return "oranges";
            yield return "bananas";
            yield return "unicorns";
        }
        static void Main(string[] args)
        {
            foreach (var s in SimpleEnumerable()) Console.WriteLine(s);
        }

    }
}
