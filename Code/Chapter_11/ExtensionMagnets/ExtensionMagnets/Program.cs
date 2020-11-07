using System;

namespace Sideways
{
    using Upside;
    class Program
    {
        static void Main(string[] args)
        {
            int i = 1;
            string s = i.ToPrice();
            s.SendIt();
            bool b = true;
            b.Green().SendIt();
            b = false;
            b.Green().SendIt();
            i = 3;
            i.ToPrice().SendIt();
        }
    }
}