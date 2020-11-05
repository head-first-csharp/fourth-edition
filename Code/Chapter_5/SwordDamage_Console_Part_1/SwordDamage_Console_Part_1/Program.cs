using System;

namespace SwordDamage_Console_Part_1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Random random = new Random();
            SwordDamage swordDamage = new SwordDamage();
            while (true)
            {
                Console.Write("0 for no magic/flaming, 1 for magic, 2 for flaming, " +
                               "3 for both, anything else to quit: ");
                char key = Console.ReadKey().KeyChar;
                if (key != '0' && key != '1' && key != '2' && key != '3') return;
                int roll = random.Next(1, 7) + random.Next(1, 7) + random.Next(1, 7);
                swordDamage.Roll = roll;
                swordDamage.SetMagic(key == '1' || key == '3');
                swordDamage.SetFlaming(key == '2' || key == '3');
                Console.WriteLine("\nRolled " + roll + " for " + swordDamage.Damage + " HP\n");
            }
        }
    }
}
