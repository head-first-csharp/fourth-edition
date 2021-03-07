using System;
using System.Collections.Generic;
using System.Text;

namespace BaseballSimulator
{
    class Ball
    {
        public event EventHandler<BallEventArgs> BallInPlay;

        public Bat GetNewBat() => new Bat(new BatCallback(OnBallInPlay));

        protected void OnBallInPlay(BallEventArgs e) => BallInPlay?.Invoke(this, e);
    }
}
