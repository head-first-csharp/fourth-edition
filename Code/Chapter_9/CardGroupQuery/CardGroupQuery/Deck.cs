using System;
using System.Collections.Generic;
using System.Text;

namespace CardGroupQuery
{
    /* 
     * NOTE: If you did the Blazor version of the "Two Decks" project, your Deck
     * class will extend List<Card> and not ObservableCollection<Card>
     */

    using System.Collections.ObjectModel;

    class Deck : ObservableCollection<Card> 
    {
        private static Random random = new Random();

        public Deck()
        {
            Reset();
        }

        public Card Deal(int index)
        {
            Card cardToDeal = base[index];
            RemoveAt(index);
            return cardToDeal;
        }

        public void Reset()
        {
            Clear();
            for (int suit = 0; suit <= 3; suit++)
                for (int value = 1; value <= 13; value++)
                    Add(new Card((Values)value, (Suits)suit));
        }

        public Deck Shuffle()
        {
            List<Card> copy = new List<Card>(this);
            Clear();
            while (copy.Count > 0)
            {
                int index = random.Next(copy.Count);
                Card card = copy[index];
                copy.RemoveAt(index);
                Add(card);
            }

            return this;
        }

        public void Sort()
        {
            List<Card> sortedCards = new List<Card>(this);
            sortedCards.Sort(new CardComparerByValue());
            Clear();
            foreach (Card card in sortedCards)
            {
                Add(card);
            }
        }
    }
}
