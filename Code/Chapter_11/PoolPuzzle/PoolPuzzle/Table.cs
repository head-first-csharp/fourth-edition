using System;
using System.Collections.Generic;
using System.Text;

namespace PoolPuzzle
{
    public struct Table
    {
        public string stairs;
        public Hinge floor;

        public void Set(Hinge b) => floor = b;

        public void Lamp(object oil)
        {
            if (oil is int oilInt)
                floor.bulb = oilInt;
            else if (oil is string oilString)
                stairs = oilString;
            else if (oil is Hinge vine)
                Console.WriteLine(
                  $"{vine.Table()} {    floor.bulb} {stairs}");
        }
    }
}
