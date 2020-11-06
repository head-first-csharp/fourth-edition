using System;
using System.Collections.Generic;
using System.Text;

namespace PigeonAndOstrich
{
    abstract class Bird
    {
        public static Random Randomizer = new Random();
        public abstract Egg[] LayEggs(int numberOfEggs);
    }
}
