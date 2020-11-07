using System;

namespace CheckTemperature
{
    class Program
    {
        static void CheckTemperature(double temp, double tooHigh = 99.5, double tooLow = 96.5)
        {
            if (temp < tooHigh && temp > tooLow)
                Console.WriteLine("{0} degrees F - feeling good!", temp);
            else
                Console.WriteLine("Uh-oh {0} degrees F -- better see a doctor!", temp);
        }

        static void Main(string[] args)
        {
            // Those values are fine for your average person
            CheckTemperature(101.3);

            // A dog's temperature should be between 100.5 and 102.5 Fahrenheit
            CheckTemperature(101.3, 102.5, 100.5);

            // Bob's temperature is always a little low, so set tooLow to 95.5
            CheckTemperature(96.2, tooLow: 95.5);
        }
    }
}
