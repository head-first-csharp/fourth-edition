using System;
using System.Collections.Generic;
using System.Text;

namespace EvilClone
{
    class EvilClone
    {
        public static int CloneCount = 0;
        public int CloneID { get; } = ++CloneCount;

        public EvilClone() => Console.WriteLine("Clone #{0} is wreaking havoc", CloneID);

        ~EvilClone()
        {
            Console.WriteLine("Clone #{0} destroyed", CloneID);
        }
    }
}
