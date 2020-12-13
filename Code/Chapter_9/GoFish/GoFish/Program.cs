using System;

namespace GoFish
{
    using System.Collections.Generic;
    using System.Linq;

    class Program
    {
        /// <summary>
        /// The GameController to manage the game
        /// </summary>
        static GameController gameController;

        /// <summary>
        /// Play a game of Go Fish!
        /// </summary>
        static void Main(string[] args)
        {
            Console.Write("Enter your name: ");
            var humanName = Console.ReadLine();

            Console.Write("Enter the number of computer opponents: ");
            int opponentCount;
            while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out opponentCount)
                || opponentCount < 1 || opponentCount > 4)
            {
                Console.WriteLine("Please enter a number from 1 to 4");
            }
            Console.WriteLine($"{Environment.NewLine}Welcome to the game, {humanName}");


            gameController = new GameController("Human",
                     Enumerable.Range(1, opponentCount).Select(i => $"Computer #{i}"));
            Console.WriteLine(gameController.Status);

            while (!gameController.GameOver)
            {
                while (!gameController.GameOver)
                {
                    Console.WriteLine($"Your hand:");
                    foreach (var card in gameController.HumanPlayer.Hand
                        .OrderBy(card => card.Suit)
                        .OrderBy(card => card.Value))
                        Console.WriteLine(card);

                    var value = PromptForAValue();

                    var player = PromptForAnOpponent();

                    gameController.NextRound(player, value);

                    Console.WriteLine(gameController.Status);
                }

                Console.WriteLine("Press Q to quit, any other key for a new game.");
                if (Console.ReadKey(true).KeyChar.ToString().ToUpper() == "N")
                    gameController.NewGame();
            }
        }

        /// <summary>
        /// Prompt the human player for a card value that's in their hand
        /// </summary>
        /// <returns>The value that the player asked for</returns>
        static Values PromptForAValue()
        {
            var handValues = gameController.HumanPlayer.Hand.Select(card => card.Value).ToList();
            Console.Write("What card value do you want to ask for? ");
            while (true)
            {
                if (Enum.TryParse(typeof(Values), Console.ReadLine(), out var value) &&
                    handValues.Contains((Values)value))
                    return (Values)value;
                else
                    Console.WriteLine("Please enter a value in your hand.");
            }
        }

        /// <summary>
        /// Prompt the human player for an opponent to ask for a card
        /// </summary>
        /// <returns>The opponent to ask for a card</returns>
        static Player PromptForAnOpponent()
        {
            var opponents = gameController.Opponents.ToList();
            for (int i = 1; i <= opponents.Count(); i++)
                Console.WriteLine($"{i}. {opponents[i - 1]}");
            Console.Write("Who do you want to ask for a card? ");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int selection)
                    && selection >= 1 && selection <= opponents.Count())
                    return opponents[selection - 1];
                else
                    Console.Write($"Please enter a number from 1 to {opponents.Count()}: ");
            }
        }
    }
}
