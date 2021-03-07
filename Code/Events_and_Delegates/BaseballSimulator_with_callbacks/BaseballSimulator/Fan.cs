using System;
using System.Collections.Generic;
using System.Text;

namespace BaseballSimulator
{
    class Fan
    {
        private int pitchNumber = 0;

        public Fan(Ball ball) => ball.BallInPlay += BallInPlayEventHandler;

        void BallInPlayEventHandler(object sender, EventArgs e)
        {
            pitchNumber++;
            if (e is BallEventArgs ballEventArgs)
            {
                if (ballEventArgs.Distance > 400 && ballEventArgs.Angle > 30)
                    Console.WriteLine($"Pitch #{pitchNumber}: Home run! I’m going for the ball!");
                else
                    Console.WriteLine($"Pitch #{pitchNumber}: Woo-hoo! Yeah!");
            }
        }
    }
}
