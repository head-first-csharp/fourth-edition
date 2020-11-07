using System;

namespace SharpenYourPencil
{
    using System.Linq;

    class Program
    {
        static string Output(Suits suit, int number) =>
            $"Suit is {suit} and number is {number}";

        static void Main(string[] args)
        {
            var deck = new Deck();
            var processedCards = deck
                .Take(3)
                .Concat(deck.TakeLast(3))
                .OrderByDescending(card => card)
                .Select(card => card.Value switch
                {
                    Values.King => Output(card.Suit, 7),
                    Values.Ace => $"It's an ace! {card.Suit}",
                    Values.Jack => Output((Suits)card.Suit - 1, 9),
                    Values.Two => Output(card.Suit, 18),
                    _ => card.ToString(),
                });

            foreach (var output in processedCards)
            {
                Console.WriteLine(output);
            }
        }
    }
}
