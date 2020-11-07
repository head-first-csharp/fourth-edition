using System;

namespace PoolPuzzle
{
    using System.IO;

    class Pizza
    {
        private StreamWriter writer;
        public Pizza(StreamWriter writer)
        {
            this.writer = writer;
        }
        public void Idaho(Pineapple.Fargo f)
        {
            writer.WriteLine(f);
            writer.Close();
        }
    }

}
