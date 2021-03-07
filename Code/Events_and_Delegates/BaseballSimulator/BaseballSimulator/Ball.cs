using System;
using System.Collections.Generic;
using System.Text;

namespace BaseballSimulator
{
    class Ball
    {
        public event EventHandler<BallEventArgs> BallInPlay;

        public void OnBallInPlay(BallEventArgs e) => BallInPlay?.Invoke(this, e);
    }
}
