using System;

namespace SerializeGuys
{
    using System.Collections.Generic;
    using System.Text.Json;

    class Program
    {
        static void Main(string[] args)
        {
            var guys = new List<Guy>() {
               new Guy() { Name = "Bob", Clothes = new Outfit() { Top = "t-shirt", Bottom = "jeans" },
                  Hair = new HairStyle() { Color = HairColor.Red, Length = 3.5f }
               },
               new Guy() { Name = "Joe", Clothes = new Outfit() { Top = "polo", Bottom = "slacks" },
                  Hair = new HairStyle() { Color = HairColor.Gray, Length = 2.7f }
               },
            };

            var jsonString = JsonSerializer.Serialize(guys);
            Console.WriteLine(jsonString);

            var copyOfGuys = JsonSerializer.Deserialize<List<Guy>>(jsonString);
            foreach (var guy in copyOfGuys)
                Console.WriteLine("I deserialized this guy: {0}", guy);

            var dudes = JsonSerializer.Deserialize<Stack<Dude>>(jsonString);
            while (dudes.Count > 0)
            {
                var dude = dudes.Pop();
                Console.WriteLine($"Next dude: {dude.Name} with {dude.Hair} hair");
            }

        }
    }
}
