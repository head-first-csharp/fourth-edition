using System;
using System.Collections.Generic;
using System.Text;

namespace PoolPuzzle
{
    class Q
    {
        public Q(bool add)
        {
            if (add) Op = "+";
            else Op = "*";
            N1 = R.Next(1, 10);
            N2 = R.Next(1, 10);
        }

        public static Random R = new Random();
        public int N1 { get; private set; }
        public string Op { get; private set; }
        public int N2 { get; private set; }

        public bool Check(int a)
        {
            if (Op == "+") return (a == N1 + N2);
            else return (a == N1 * N2);
        }
    }
}
