using System;

namespace StructDog
{
    class Program
    {
        static void Main(string[] args)
        {
            Canine spot = new Canine("Spot", "pug");
            Canine bob = spot;
            bob.Name = "Spike";
            bob.Breed = "beagle";
            spot.Speak();
            Dog jake = new Dog("Jake", "poodle");
            Dog betty = jake;
            betty.Name = "Betty";
            betty.Breed = "pit bull";
            jake.Speak();
        }
    }
}
