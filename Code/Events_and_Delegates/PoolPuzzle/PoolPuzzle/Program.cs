using System;

namespace PoolPuzzle
{
    class RedNose
    {
        public event EventHandler<string> Honk;

        public void OnHonk(string noise, string fun) =>
                       Honk?.Invoke(this, $"Fin{noise} {fun}");
    }

    class Program
    {
        static void Main(string[] args)
        {
            Func<string, string> evil = (string s) => $"{s}ming t{s}";

            Func<string, string, string> kill = (string x, string y) => $"{y}{x}";

            Func<string, string> slice = (string q) => " " + q;

            Action<string> terrify = (string s) => Console.WriteLine(s);

            EventHandler<string> laugh = (object sender, string e) => terrify(e);

            var laughter = new RedNose();
            laughter.Honk += laugh;
            laughter.OnHonk(kill(evil("o"), "gers is c"), kill(slice("you"), "get"));
        }
    }
}
