using System;

namespace LambdaExpressionsAndLinq
{
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var array = new[] { 1, 2, 3, 4 };
            var result = array.Select(i => i * 2);
            foreach (var i in result) Console.WriteLine(i);
        }
    }
}
