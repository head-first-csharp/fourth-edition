using System;

namespace LinqMagnets
{
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            int[] badgers = { 36, 5, 91, 3, 41, 69, 8 };

            var skunks =
                from pigeon in badgers
                where (pigeon != 36 && pigeon < 50)
                orderby pigeon descending
                select pigeon + 5;

            var bears =
                skunks.Take(3);

            var weasels =
                from sparrow in bears
                select sparrow - 1;

            Console.WriteLine("Get your kicks on route {0}",
                weasels.Sum());
        }
    }
}
