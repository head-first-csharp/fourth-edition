using System;
using System.Collections.Generic;
using System.Text;

namespace JimmyLinq
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ComicAnalyzer
    {
        private static PriceRange CalculatePriceRange(Comic comic, IReadOnlyDictionary<int, decimal> prices)
        {
            if (prices[comic.Issue] < 100)
                return PriceRange.Cheap;
            else
                return PriceRange.Expensive;
        }

        public static IEnumerable<IGrouping<PriceRange, Comic>> GroupComicsByPrice(
                    IEnumerable<Comic> comics, IReadOnlyDictionary<int, decimal> prices)
        {
            var grouped =
              comics
              .OrderBy(comic => prices[comic.Issue])
              .GroupBy(comic => CalculatePriceRange(comic, prices));

            return grouped;
        }

        public static IEnumerable<string> GetReviews(
                          IEnumerable<Comic> comics, IEnumerable<Review> reviews)
        {
            var joined =
              comics
              .OrderBy(comic => comic.Issue)
              .Join(
                reviews,
                comic => comic.Issue,
                review => review.Issue,
                (comic, review) =>
                        $"{review.Critic} rated #{comic.Issue} '{comic.Name}' {review.Score:0.00}");

            return joined;
        }
    }
}
