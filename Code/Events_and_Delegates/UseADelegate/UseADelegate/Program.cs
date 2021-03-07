using System;

namespace UseADelegate
{
    class Program
    {
        delegate string IntToString(int i);

        public static string AddNumberSign(int i) => $"#{i}";
        public static string PlusOne(int i) => $"{i} plus one equals {i + 1}";

        static void Main(string[] args)
        {
            IntToString methodRef = AddNumberSign;
            Console.WriteLine(methodRef(12345));

            methodRef = PlusOne;
            Console.WriteLine(methodRef(12345));
        }
    }
}
