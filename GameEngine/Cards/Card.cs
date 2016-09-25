using System;

namespace GameEngine.Cards
{
    public class Card : IComparable<Card>
    {
        public CardType Type { get; private set; }
        public CardSuit Suit { get; private set; }

        public Card(CardType type, CardSuit suit)
        {
            this.Type = type;
            this.Suit = suit;
        }

        public override bool Equals(object obj)
        {
            if (obj is Card)
            {
                Card other = (Card)obj;
                return this.Type == other.Type && this.Suit == other.Suit;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)this.Suit * (int)this.Type;
        }

        public override string ToString()
        {
            String s;
            switch (this.Type)
            {
                case CardType.Ace:
                    s = "A";
                    break;
                case CardType.Jack:
                    s = "J";
                    break;
                case CardType.King:
                    s = "K";
                    break;
                case CardType.Nine:
                    s = "9";
                    break;
                case CardType.Queen:
                    s = "Q";
                    break;
                case CardType.Ten:
                    s = "10";
                    break;
                default:
                    s = "";
                    break;
            }

            switch (this.Suit)
            {
                case CardSuit.Club:
                    s += "♣";
                    break;
                case CardSuit.Diamond:
                    s += "♦";
                    break;
                case CardSuit.Heart:
                    s += "♥";
                    break;
                case CardSuit.Spade:
                    s += "♠";
                    break;
                default:
                    break;
            }

            return s;
        }

        public int Value()
        {
            switch (this.Type)
            {
                case CardType.Nine:
                    return 0;
                case CardType.Jack:
                    return 2;
                case CardType.Queen:
                    return 3;
                case CardType.King:
                    return 4;
                case CardType.Ten:
                    return 10;
                case CardType.Ace:
                    return 11;
                default:
                    return -1;
            }
        }

        public int CompareTo(Card other)
        {
            return this.Value().CompareTo(other.Value());
        }
    }
}
