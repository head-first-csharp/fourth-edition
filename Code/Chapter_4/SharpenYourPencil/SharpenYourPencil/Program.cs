using System;

namespace SharpenYourPencil
{
    class Program
    {
        private static void Main(string[] args)
        {
            Elephant[] elephants = new Elephant[7];
            elephants[0] = new Elephant() { Name = "Lloyd", EarSize = 40 };
            elephants[1] = new Elephant() { Name = "Lucinda", EarSize = 33 };
            elephants[2] = new Elephant() { Name = "Larry", EarSize = 42 };
            elephants[3] = new Elephant() { Name = "Lucille", EarSize = 32 };
            elephants[4] = new Elephant() { Name = "Lars", EarSize = 44 };
            elephants[5] = new Elephant() { Name = "Linda", EarSize = 37 };
            elephants[6] = new Elephant() { Name = "Humphrey", EarSize = 45 };

            Elephant biggestEars = elephants[0];
            for (int i = 1; i < elephants.Length; i++)
            {
                Console.WriteLine("Iteration #" + i);

                if (elephants[i].EarSize > biggestEars.EarSize)
                {
                    biggestEars = elephants[i];
                }


                Console.WriteLine(biggestEars.EarSize.ToString());
            }
        }
    }
}
