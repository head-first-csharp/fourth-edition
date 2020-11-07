using System;

namespace PowersOfTwo
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (int i in new PowersOfTwo())
                Console.Write($" {i}");
        }
    }
}
