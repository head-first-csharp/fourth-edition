using System;

namespace HippoAndWolf
{
    class Program
    {
        static void Main(string[] args)
        {
            Animal[] animals =
            {
            new Wolf(false),
            new Hippo(),
            new Wolf(true),
            new Wolf(false),
            new Hippo()
        };

            foreach (Animal animal in animals)
            {
                animal.MakeNoise();

                if (animal is ISwimmer swimmer)
                {
                    swimmer.Swim();
                }

                if (animal is IPackHunter hunter)
                {
                    hunter.HuntInPack();
                }

                Console.WriteLine();
            }
        }
    }
}
