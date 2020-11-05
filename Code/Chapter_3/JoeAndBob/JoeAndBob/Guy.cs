using System;
using System.Collections.Generic;
using System.Text;

namespace JoeAndBob
{
    class Guy
    {
        public string Name;
        public int Cash;

        /// <summary>
        /// Writes my name and the amount of cash I have to the console.
        /// </summary>
        public void WriteMyInfo()
        {
            Console.WriteLine(Name + " has " + Cash + " bucks.");
        }

        /// <summary>
        /// Gives some of my cash, removing it from my wallet (or printing 
        /// a message to the console if I don't have enough cash).
        /// </summary>
        /// <param name="amount">Amount of cash to give.</param>
        /// <returns>
        /// The amount of cash removed from my wallet, or 0 if I don't
        /// have enough cash (or if the amount is invalid).
        /// </returns>
        public int GiveCash(int amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine(Name + " says: " + amount + " isn't a valid amount");
                return 0;
            }
            if (amount > Cash)
            {
                Console.WriteLine(Name + " says: " +
                    "I don't have enough cash to give you " + amount);
                return 0;
            }
            Cash -= amount;
            return amount;
        }

        /// <summary>
        /// Receive some cash, adding it to my wallet (or printing
        /// a message to the console if the amount is invalid).
        /// </summary>
        /// <param name="amount">Amount of cash to give.</param>
        public void ReceiveCash(int amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine(Name + " says: " + amount + " isn't an amount I'll take");
            }
            else
            {
                Cash += amount;
            }
        }
    }
}
