using System;

namespace JimmyLinq
{
    class Program
    {
        static void Main(string[] args)
        {
            var done = false;
            while (!done)
            {
                Console.WriteLine(
                    "\nPress G to group comics by price, R to get reviews, any other key to quit\n");
                done = Console.ReadKey(true).KeyChar.ToString().ToUpper() switch
                {
                    "G" => GroupComicsByPrice(),
                    "R" => GetReviews(),
                    _ => true,
                };
            }
        }


        private static bool GroupComicsByPrice()
        {
            var groups = ComicAnalyzer.GroupComicsByPrice(Comic.Catalog, Comic.Prices);
            foreach (var group in groups)
            {
                Console.WriteLine($"{group.Key} comics:");
                foreach (var comic in group)
                    Console.WriteLine($"#{comic.Issue} {comic.Name}: {Comic.Prices[comic.Issue]:c}");
            }
            return false;
        }

        private static bool GetReviews()
        {
            var reviews = ComicAnalyzer.GetReviews(Comic.Catalog, Comic.Reviews);
            foreach (var review in reviews)
                Console.WriteLine(review);
            return false;
        }
    }
}
