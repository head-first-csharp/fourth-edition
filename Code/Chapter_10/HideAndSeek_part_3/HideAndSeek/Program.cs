using System;

namespace HideAndSeek
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var gameController = new GameController();
                while (!gameController.GameOver)
                {
                    Console.WriteLine(gameController.Status);
                    Console.Write(gameController.Prompt);
                    Console.WriteLine(gameController.ParseInput(Console.ReadLine()));
                }

                Console.WriteLine($"You won the game in {gameController.MoveNumber} moves!");
                Console.WriteLine("Press P to play again, any other key to quit.");
                if (Console.ReadKey(true).KeyChar.ToString().ToUpper() != "P") return;
            }
        }
    }
}