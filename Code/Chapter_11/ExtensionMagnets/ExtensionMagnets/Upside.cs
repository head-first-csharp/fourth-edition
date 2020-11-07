using System;
using System.Collections.Generic;
using System.Text;

namespace Upside
{
    public static class Margin
    {
        public static void SendIt(this string s)
        {
            Console.Write(s);
        }
        public static string ToPrice(this int n)
        {
            if (n == 1)
                return "a buck ";
            else
                return " more bucks";
        }
        public static string Green(this bool b)
        {
            if (b == true)
                return "be";
            else
                return "gets";
        }
    }
}