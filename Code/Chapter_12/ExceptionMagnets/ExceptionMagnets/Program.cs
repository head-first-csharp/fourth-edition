using System;

namespace ExceptionMagnets
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("when it ");
            ExTestDrive.Zero("yes");
            Console.Write(" it ");
            ExTestDrive.Zero("no");
            Console.WriteLine(".");
        }
    }
}
