using System;

namespace dotnetUsesUnicode
{
    using System.IO;
    using System.Text;
    using System.Text.Json;

    class Program
    {
        static void Main(string[] args)
        {
            File.WriteAllText("eureka.txt", "Eureka!");
            byte[] eurekaBytes = File.ReadAllBytes("eureka.txt");
            foreach (byte b in eurekaBytes)
                Console.Write("{0} ", b);
            Console.WriteLine(Encoding.UTF8.GetString(eurekaBytes));

            foreach (byte b in eurekaBytes)
                Console.Write("{0:x2} ", b);
            Console.WriteLine();

            File.WriteAllText("eureka.txt", "םולש", Encoding.Unicode);

            Console.WriteLine(JsonSerializer.Serialize("ש"));

            File.WriteAllText("elephant1.txt", "\uD83D\uDC18");
            File.WriteAllText("elephant2.txt", "\U0001F418");

        }
    }
}
