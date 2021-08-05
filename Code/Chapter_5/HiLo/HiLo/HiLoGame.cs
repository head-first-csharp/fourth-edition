using System;
using System.Collections.Generic;
using System.Text;

namespace HiLo
{
    /*
     * In some early versions of Head First C# (4th edition) there was a minor technical issue
     * with this exercise. This code corresponds to the updated version, which you can find here:
     * https://github.com/head-first-csharp/fourth-edition/raw/master/Additional_material/Corrected_pages_243_and_244.pdf
     */

    static class HiLoGame
    {
        public const int MAXIMUM = 10;
        private static Random random = new Random();
        private static int currentNumber = random.Next(1, MAXIMUM + 1);
        private static int nextNumber = random.Next(1, MAXIMUM + 1);
        private static int pot = 10;

        public static int GetPot() { return pot; }

        public static void Guess(bool higher)
        {
            if ((higher && nextNumber >= currentNumber) ||
                    (!higher && nextNumber <= currentNumber))
            {
                Console.WriteLine("You guessed right!");
                pot++;
            }
            else
            {
                Console.WriteLine("Bad luck, you guessed wrong.");
                pot--;
            }
            currentNumber = nextNumber;
            nextNumber = random.Next(1, MAXIMUM + 1);
            Console.WriteLine($"The current number is {currentNumber}");
        }

        public static void Hint()
        {
            int half = MAXIMUM / 2;
            if (nextNumber >= half)
                Console.WriteLine($"The current number is {currentNumber}, the next number is at least {half}");
            else Console.WriteLine($"The current number is {currentNumber}, the next is at most {half}");
            pot--;
        }
    }

}
