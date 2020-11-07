using System;

namespace AddSubtract
{
    class Program
    {
        static void Main(string[] args)
        {
            AddSubtract a = new AddSubtract() { Value = 5 }
                .Add(5)
                .Subtract(3)
                .Add(9)
                .Subtract(12);
            Console.WriteLine($"Result: {a.Value}");
        }
    }
}
