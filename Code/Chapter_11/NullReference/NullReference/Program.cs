using System;

namespace NullReference
{
    using System.IO;

#nullable enable
    class Program
    {
        static void Main(string[] args)
        {
            using (var stringReader = new StringReader(""))
            {
                var nextLine = stringReader.ReadLine() ?? String.Empty;
                Console.WriteLine("Line length is: {0}", nextLine.Length);
            }
        }
    }

}
