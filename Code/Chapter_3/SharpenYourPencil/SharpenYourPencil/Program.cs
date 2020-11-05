using System;

namespace SharpenYourPencil
{
    class Program
    {
        static void Main(string[] args)
        {
            Clown oneClown = new Clown();
            oneClown.Name = "Boffo";
            oneClown.Height = 14;
            oneClown.TalkAboutYourself();      // My name is _______ and I'm ____ inches tall."

            Clown anotherClown = new Clown();
            anotherClown.Name = "Biff";
            anotherClown.Height = 16;
            anotherClown.TalkAboutYourself();  // My name is _______ and I'm ____ inches tall."

            Clown clown3 = new Clown();
            clown3.Name = anotherClown.Name;
            clown3.Height = oneClown.Height - 3;
            clown3.TalkAboutYourself();        // My name is _______ and I'm ____ inches tall."

            anotherClown.Height *= 2;
            anotherClown.TalkAboutYourself();  // My name is _______ and I'm ____ inches tall."
        }
    }
}
