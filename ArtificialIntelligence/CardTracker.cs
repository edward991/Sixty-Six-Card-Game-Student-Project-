using System;
using GameEngine.Cards;
using System.Collections.Generic;

namespace ArtificialIntelligence
{
    public class CardTracker
    {
        public Card TrumpCard { get; private set; }

        public List<Card> PossibleOpponentCards { get; private set; }
        public List<Card> PlayedCards { get; private set; }

        public CardTracker()
        {
            this.Clear();
        }

        public void Clear()
        {
            this.TrumpCard = null;
            this.PossibleOpponentCards = new List<Card>();

            foreach (CardType type in Enum.GetValues(typeof(CardType)))
                foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
                    PossibleOpponentCards.Add(new Card(type, suit));

            this.PlayedCards = new List<Card>();
        }

        public void CardPlayed(Card card)
        {
            if (card == null)
            {
                return;
            }

            this.PossibleOpponentCards.Remove(card);
            this.PlayedCards.Add(card);
        }

        public void ChangeTrumpCard(Card card)
        {
            this.TrumpCard = new Card(CardType.Nine, card.Suit);

            this.PossibleOpponentCards.Add(card);

            this.PossibleOpponentCards.Remove(this.TrumpCard);
        }

        public void TrumpCardSaw(Card newCard)
        {
            if (!Card.Equals(newCard, this.TrumpCard))
            {
                this.PossibleOpponentCards.Remove(newCard);
                if (this.TrumpCard != null)
                {
                    this.PossibleOpponentCards.Add(this.TrumpCard);
                }

                this.TrumpCard = newCard;
            }
        }
    }
}
