using AmazingExtensions;
using System;

namespace ExtendingString
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = "Evil clones are wreaking havoc. Help!";
            Console.WriteLine(message.IsDistressCall());
        }
    }
}
