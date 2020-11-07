using System;

namespace ExceptionMagnets
{

    class ExTestDrive
    {
        public static void Zero(string test)
        {
            try
            {
                Console.Write("t");

                DoRisky(test);

                Console.Write("o");

            }
            catch (MyException)
            {

                Console.Write("a");

            }
            finally
            {

                Console.Write("w");

            }

            Console.Write("s");

        }

        static void DoRisky(String t)
        {
            Console.Write("h");

            if (t == "yes")
            {

                throw new MyException();

            }

            Console.Write("r");
        }
    }
}