using System;

namespace CreateSequencesByHand
{
    class Program
    {
        static void Main(string[] args)
        {
            var sports = new ManualSportSequence();
            foreach (var sport in sports)
                Console.WriteLine(sport);
        }
    }
}
