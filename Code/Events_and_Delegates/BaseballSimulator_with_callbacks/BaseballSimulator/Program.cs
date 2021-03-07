using System;

namespace BaseballSimulator
{
    class Program
    {
        static readonly Ball ball = new Ball();
        static readonly Pitcher pitcher = new Pitcher(ball);
        static readonly Fan fan = new Fan(ball);

        static void Main(string[] args)
        {
            var running = true;
            while (running)
            {
                Console.Write("Enter a number for the angle (or anything else to quit): ");
                if (int.TryParse(Console.ReadLine(), out int angle))
                {
                    Console.Write("Enter a number for the distance (or anything else to quit): ");
                    if (int.TryParse(Console.ReadLine(), out int distance))
                    {
                        BallEventArgs ballEventArgs = new BallEventArgs(angle, distance);
                        var bat = ball.GetNewBat();
                        bat.HitTheBall(ballEventArgs);
                    }
                    else
                        running = false;
                }
                else
                    running = false;
            }
            Console.WriteLine("Thanks for playing!");
        }
    }
}
