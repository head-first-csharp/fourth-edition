using System;

namespace CreateAnonymousTypes
{
    public class Program
    {
        public static void Main()
        {
            var whatAmI = new { Color = "Blue", Flavor = "Tasty", Height = 37 };
            Console.WriteLine(whatAmI);
        }
    }
}
