using System;
using System.Collections.Generic;
using System.Text;

namespace BaseballSimulator
{
    class BallEventArgs : EventArgs
    {
        public int Angle { get; private set; }
        public int Distance { get; private set; }

        public BallEventArgs(int angle, int distance)
        {
            this.Angle = angle;
            this.Distance = distance;
        }
    }
}
