using System;
using System.Collections.Generic;
using System.Text;

namespace PoolPuzzle
{
    class Clowns : Picasso
    {
        public Clowns() : base("Clowns") { }

        public override int Ear()
        {
            return 7;
        }
    }


}
