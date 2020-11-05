using System;

namespace PoolPuzzle
{
    class Program
    {
        public static void Main(string[] args)
        {
            Q q = new Q(Q.R.Next(2) == 1);
            while (true)
            {
                Console.Write($"{q.N1   } {q.Op   } {q.N2   } = ");
                if (!int.TryParse(Console.ReadLine(), out int i))
                {
                    Console.WriteLine("Thanks for playing!");
                    return;
                }
                if (q.Check(i))
                {
                    Console.WriteLine("Right!");
                    q = new Q(Q.R.Next(2) == 1);
                }
                else Console.WriteLine("Wrong! Try again.");
            }
        }
    }
}
