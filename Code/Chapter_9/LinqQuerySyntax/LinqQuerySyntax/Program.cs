using System;

namespace LinqQuerySyntax
{
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            int[] values = new int[] { 0, 12, 44, 36, 92, 54, 13, 8 };
            IEnumerable<int> result =
                      from v in values
                      where v < 37
                      orderby -v
                      select v;
            // use a foreach loop to print the results
            foreach (int i in result)
                Console.Write($"{i} ");

        }
    }
}
