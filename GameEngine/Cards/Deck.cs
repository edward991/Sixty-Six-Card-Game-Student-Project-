using System;
using System.Collections.Generic;

namespace GameEngine.Cards
{
    public class Deck
    {
        private List<Card> cards;
        public Card TrumpCard { get; private set; }

        public Deck()
        {
            this.cards = new List<Card>();
            this.AddCardsInList();
            this.ShuffleDeck();
            this.TrumpCard = this.cards[0];
        }

        public Deck(List<Card> cards, Card trumpCard)
        {
            this.cards = new List<Card>(cards);
            this.ShuffleDeck();
            int i = this.cards.FindIndex(x => x.Equals(trumpCard));
            Card tempCard = this.cards[0];
            this.cards[0] = this.cards[i];
            this.cards[i] = tempCard;
            this.TrumpCard = this.cards[0];
        }

        public Card GetNextCard()
        {
            Card card = this.cards[this.cards.Count - 1];
            this.cards.RemoveAt(this.cards.Count - 1);
            return card;
        }

        public int CardsLeft()
        {
            return this.cards.Count;
        }

        public void ChangeTrumpCard(Card card)
        {
            this.TrumpCard = card;
            if (this.cards.Count > 0)
            {
                this.cards[0] = card;
            }
        }

        private void AddCardsInList()
        {
            foreach (CardType type in Enum.GetValues(typeof(CardType)))
            {
                foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                {
                    cards.Add(new Card(type, suit));
                }
            }
        }

        private void ShuffleDeck()
        {
            Random random = new Random();
            int n = this.cards.Count;
            for (int i = 0; i < n; i++)
            {
                int r = i + random.Next(0, n - i);
                Card temp = this.cards[i];
                this.cards[i] = this.cards[r];
                this.cards[r] = temp;
            }
        }
    }
}
