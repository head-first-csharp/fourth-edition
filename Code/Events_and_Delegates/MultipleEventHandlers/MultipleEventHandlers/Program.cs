using System;

namespace MultipleEventHandlers
{
    class Program
    {
        static int count;

        static void SaySomething(object sender, TalkEventArgs e)
        {
            Console.WriteLine($"Call #{count++}: I said something: {e.Message}");
        }

        static void SaySomethingElse(object sender, TalkEventArgs e)
        {
            Console.WriteLine($"Call #{count++}: I said something else: {e.Message}");
        }

        static void Main(string[] args)
        {
            var myEvent = new Talker();
            while (true)
            {
                Console.Write("1 to chain SaySomething, 2 to chain SaySomethingElse, or a message: ");
                var line = Console.ReadLine();
                switch (line)
                {
                    case "1":
                        Console.WriteLine("Adding SaySomething");
                        myEvent.TalkToMe += SaySomething;
                        break;
                    case "2":
                        Console.WriteLine("Adding SaySomethingElse");
                        myEvent.TalkToMe += SaySomethingElse;
                        break;
                    case "":
                        return;
                    default:
                        count = 1;
                        Console.WriteLine("Raising the TalkToMe event");
                        myEvent.OnTalkToMe(line);
                        break;
                }
            }
        }
    }
}
