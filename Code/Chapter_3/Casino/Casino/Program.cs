using System;

namespace Casino
{
    class Program
    {
        static void Main(string[] args)
        {
            double odds = .75;
            Random random = new Random();

            Guy player = new Guy() { Cash = 100, Name = "The player" };

            Console.WriteLine("Welcome to the casino. The odds are " + odds);
            while (player.Cash > 0)
            {
                player.WriteMyInfo();
                Console.Write("How much do you want to bet: ");
                string howMuch = Console.ReadLine();
                if (int.TryParse(howMuch, out int amount))
                {
                    int pot = player.GiveCash(amount) * 2;
                    if (pot > 0)
                    {
                        if (random.NextDouble() > odds)
                        {
                            int winnings = pot;
                            Console.WriteLine("You win " + winnings);
                            player.ReceiveCash(winnings);
                        }
                        else
                        {
                            Console.WriteLine("Bad luck, you lose.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid number.");
                }

            }
            Console.WriteLine("The house always wins.");
        }

    }
}
