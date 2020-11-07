using System;
using System.Collections.Generic;
using System.Text;

namespace AmazeballsSerum
{
    static class AmazeballsSerum
    {
        public static string BreakWalls(this OrdinaryHuman h, double wallDensity)
        {
            return ($"I broke through a wall of {wallDensity} density.");
        }
    }
}
