using GameEngine.Cards;
using GameEngine.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtificialIntelligence.MonteCarloSearchTree
{
    class RandomMovesPlayer : BasePlayer
    {
        private static Random random = new Random();
        private Card playedCard;
        private Announce playedCardAnnounce;

        public RandomMovesPlayer(List<Card> currentCards)
        {
            this.currentCards = new List<Card>(currentCards);
            this.playedCard = null;
            this.playedCardAnnounce = Announce.None;
        }
        public RandomMovesPlayer(List<Card> currentCards, Card playedCard, Announce playedCardAnnounce)
            : base()
        {
            this.playedCard = playedCard;
            this.currentCards = new List<Card>(currentCards);
            this.playedCardAnnounce = playedCardAnnounce;
        }

        public override PlayerMove GetTurn(PlayerTurnContext context)
        {
            Announce possibleAnnounce;
            PlayerMove move;

            if (this.playedCard != null)
            {
                move = new PlayerMove(PlayerMoveType.PlayCard, playedCard, this.playedCardAnnounce);
                this.playedCard = null;
                return move;
            }

            PlayerMove selectedMove;

            if (PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None),
                context, this.currentCards))
                selectedMove = new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None);
            else if (this.CloseGame(context))
                selectedMove = new PlayerMove(PlayerMoveType.CloseGame, null, Announce.None);
            else
            {
                List<PlayerMove> possibleMoves = new List<PlayerMove>();

                foreach (var card in this.currentCards)
                {
                    possibleAnnounce = this.PossibleAnnounce(card, context.TrumpCard);
                    PlayerMove currentMove;
                    if (context.ImFirst())
                    {
                        currentMove = new PlayerMove(PlayerMoveType.PlayCard, card, possibleAnnounce);
                        if (PlayerMoveValidator.IsValid(currentMove, context, this.currentCards))
                        {
                            this.currentCards.Remove(card);
                            return currentMove;
                            //possibleMoves.Add(currentMove);
                        }
                        else
                        {
                            currentMove.PlayerAnnounce = Announce.None;
                            if (PlayerMoveValidator.IsValid(currentMove, context, this.currentCards))
                                possibleMoves.Add(currentMove);
                        }
                    }
                    else
                    {
                        currentMove = new PlayerMove(PlayerMoveType.PlayCard, card, Announce.None);
                        if (PlayerMoveValidator.IsValid(currentMove, context, this.currentCards))
                            possibleMoves.Add(currentMove);
                    }

                }

                selectedMove = possibleMoves[random.Next(possibleMoves.Count)];
            }

            if (selectedMove.Type == PlayerMoveType.ChangeTrump)
                this.currentCards.Remove(new Card(CardType.Nine, context.TrumpCard.Suit));
            if (selectedMove.Type == PlayerMoveType.PlayCard)
                this.currentCards.Remove(selectedMove.PlayedCard);

            return selectedMove;
        }

        private bool CloseGame(PlayerTurnContext context)
        {
            if (!PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.CloseGame, null, Announce.None), context, this.currentCards))
            {
                return false;
            }

            if (this.currentCards.Count(x => x.Suit == context.TrumpCard.Suit) == 5)
            {
                return true;
            }

            return false;
        }

    }
}
