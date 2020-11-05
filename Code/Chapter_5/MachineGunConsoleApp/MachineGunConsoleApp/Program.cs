using System;

namespace MachineGunConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfBullets = ReadInt(20, "Number of bullets");
            int magazineSize = ReadInt(16, "Magazine size");

            Console.Write($"Loaded [false]: ");
            bool.TryParse(Console.ReadLine(), out bool isLoaded);

            MachineGun gun = new MachineGun(numberOfBullets, magazineSize, isLoaded);

            while (true)
            {
                Console.WriteLine($"{gun.Bullets} bullets, {gun.BulletsLoaded} loaded");
                if (gun.IsEmpty()) Console.WriteLine("WARNING: You're out of ammo");
                Console.WriteLine("Space to shoot, r to reload, + to add ammo, q to quit");
                char key = Console.ReadKey(true).KeyChar;
                if (key == ' ') Console.WriteLine($"Shooting returned {gun.Shoot()}");
                else if (key == 'r') gun.Reload();
                else if (key == '+') gun.Bullets += gun.MagazineSize;
                else if (key == 'q') return;
            }
        }

        static int ReadInt(int lastUsedValue, string prompt)
        {
            Console.Write(prompt + " [" + lastUsedValue + "]: ");
            string line = Console.ReadLine();
            if (int.TryParse(line, out int value))
            {
                Console.WriteLine("   using value " + value);
                return value;
            }
            else
            {
                Console.WriteLine("   using default value " + lastUsedValue);
                return lastUsedValue;
            }
        }

    }
}
