using System;
using System.Collections.Generic;

namespace Dogs
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Dog> dogs = new List<Dog>()
            {
                new Dog() { Breed = Breeds.Dachshund, Name = "Franz" },
                new Dog() { Breed = Breeds.Collie, Name = "Petunia" },
                new Dog() { Breed = Breeds.Pug, Name = "Porkchop" },
                new Dog() { Breed = Breeds.Dachshund, Name = "Brunhilda" },
                new Dog() { Breed = Breeds.Collie, Name = "Zippy" },
                new Dog() { Breed = Breeds.Corgi, Name = "Carrie" },
            };
            dogs.Sort();
            foreach (Dog dog in dogs)
                Console.WriteLine(dog);
        }
    }

}
