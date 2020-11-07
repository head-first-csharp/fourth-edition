using System;
using System.Collections.Generic;
using System.Text;

namespace StructDog
{
    public struct Dog
    {
        public string Name { get; set; }
        public string Breed { get; set; }

        public Dog(string name, string breed)
        {
            this.Name = name;
            this.Breed = breed;
        }

        public void Speak()
        {
            Console.WriteLine("My name is {0} and I'm a {1}.", Name, Breed);
        }
    }
}
