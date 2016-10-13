using GameEngine.Cards;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.Players
{
    public class PlayerMoveValidator
    {
        public static bool IsValid(PlayerMove move, PlayerTurnContext context, List<Card> cards)
        {
            if (!context.ImFirst() && move.PlayerAnnounce != Announce.None)
            {
                return false;
            }

            if (move.Type == PlayerMoveType.PlayCard)
            {
                if (!cards.Contains(move.PlayedCard))
                {
                    return false;
                }

                if (move.PlayerAnnounce != Announce.None && !context.State.CanAnnounce20Or40())
                {
                    return false;
                }

                if (move.PlayerAnnounce != Announce.None)
                {
                    if (move.PlayedCard.Type != CardType.Queen && move.PlayedCard.Type != CardType.King)
                    {
                        move.PlayerAnnounce = Announce.None;
                    }
                }

                if (context.State.ShouldObserveRules())
                {
                    if (!context.ImFirst())
                    {
                        if (move.PlayedCard.Suit == context.FirstPlayedCard.Suit)
                        {
                            if (move.PlayedCard.Value() < context.FirstPlayedCard.Value())
                            {
                                bool hasBigger = cards.Any(c => c.Value() > context.FirstPlayedCard.Value() && c.Suit == context.FirstPlayedCard.Suit);

                                if (hasBigger)
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            bool hasSuit = cards.Any(c => c.Suit == context.FirstPlayedCard.Suit);

                            if (hasSuit)
                            {
                                return false;
                            }

                            if (move.PlayedCard.Suit != context.TrumpCard.Suit)
                            {
                                bool hasTrump = cards.Any(c => c.Suit == context.TrumpCard.Suit);

                                if (hasTrump)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            if (move.Type == PlayerMoveType.CloseGame)
            {
                if (!context.State.CanClose() || !context.ImFirst())
                {
                    return false;
                }
            }

            if (move.Type == PlayerMoveType.ChangeTrump)
            {
                if (!context.State.CanChangeTrump() || !context.ImFirst())
                {
                    return false;
                }
                if (!cards.Contains(new Card(CardType.Nine, context.TrumpCard.Suit)))
                {
                    return false;
                }
            }

            return true;
        }

        public static List<Card> GetPossibleCardsToPlay(PlayerTurnContext context, List<Card> cards)
        {
            List<Card> possibleCardsToPlay = new List<Card>();

            foreach (var card in cards)
            {
                if (PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.PlayCard, card, Announce.None), context, cards))
                {
                    possibleCardsToPlay.Add(card);
                }
            }

            return possibleCardsToPlay;
        }

    }
}
