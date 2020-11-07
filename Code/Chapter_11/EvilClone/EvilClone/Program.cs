using System;

namespace EvilClone
{
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var clones = new List<EvilClone>();
            while (true)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case 'a':
                        clones.Add(new EvilClone());
                        break;
                    case 'c':
                        Console.WriteLine("Clearing list at time {0}", stopwatch.ElapsedMilliseconds);
                        clones.Clear();
                        break;
                    case 'g':
                        Console.WriteLine("Collecting at time {0}", stopwatch.ElapsedMilliseconds);
                        GC.Collect();
                        break;
                };
            }
        }
    }
}
