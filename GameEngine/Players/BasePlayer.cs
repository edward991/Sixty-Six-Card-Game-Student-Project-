using GameEngine.Cards;
using System.Collections.Generic;

namespace GameEngine.Players
{
    public abstract class BasePlayer
    {
        protected List<Card> currentCards;

        public BasePlayer()
        {
            currentCards = new List<Card>();
        }

        public virtual void AddCard(Card card)
        {
            currentCards.Add(card);
        }

        public abstract PlayerMove GetTurn(PlayerTurnContext context);

        public virtual void StartRound()
        {
        }

        public virtual void EndTurn(PlayerTurnContext context)
        {
        }

        public void ResetCards()
        {
            currentCards.Clear();
        }

        protected Announce PossibleAnnounce(Card card, Card trumpCard)
        {
            if (card.Type == CardType.Queen)
            {
                if (currentCards.Contains(new Card(CardType.King, card.Suit)))
                {
                    if (card.Suit == trumpCard.Suit)
                    {
                        return Announce.Fourty;
                    }
                    else
                    {
                        return Announce.Twenty;
                    }
                }
            }
            else if (card.Type == CardType.King)
            {
                if (currentCards.Contains(new Card(CardType.Queen, card.Suit)))
                {
                    if (card.Suit == trumpCard.Suit)
                    {
                        return Announce.Fourty;
                    }
                    else
                    {
                        return Announce.Twenty;
                    }
                }
            }
            return Announce.None;
        }
    }
}
