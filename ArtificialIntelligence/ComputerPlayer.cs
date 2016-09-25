using System;
using GameEngine.Cards;
using GameEngine.Players;
using GameEngine;
using System.Collections.Generic;
using System.Linq;
using ArtificialIntelligence.MonteCarloSearchTree;
using GameEngine.RoundStates;

namespace ArtificialIntelligence
{
    public class ComputerPlayer : BasePlayer
    {
        private static Random random = new Random();
        private CardTracker cardTracker = new CardTracker();
        public GameLevel level { get; private set; }

        public ComputerPlayer(GameLevel level)
            : base()
        {
            this.level = level;
        }

        public override PlayerMove GetTurn(PlayerTurnContext context)
        {
            this.cardTracker.TrumpCardSaw(context.TrumpCard);

            if (context.FirstPlayedCard != null)
                this.cardTracker.CardPlayed(context.FirstPlayedCard);

            if (PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None),
                context, this.currentCards))
            {
                this.cardTracker.ChangeTrumpCard(context.TrumpCard);
                return new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None);
            }

            if (this.CloseGame(context))
                return new PlayerMove(PlayerMoveType.CloseGame, null, Announce.None);

            switch (this.level)
            {
                case GameLevel.Easy:
                    return this.PlayRandomMove(context);
                case GameLevel.Normal:
                    return this.PlayNormalLevelMove(context);
                case GameLevel.Hard:
                    return this.PlayMCSTMove(context);
                default:
                    return null;
            }
        }

        public override void AddCard(Card card)
        {
            base.AddCard(card);
            this.cardTracker.PossibleOpponentCards.Remove(card);
        }

        public override void StartRound()
        {
            base.StartRound();
            this.cardTracker.Clear();
            foreach (var card in this.currentCards)
            {
                this.cardTracker.PossibleOpponentCards.Remove(card);
            }
        }

        public override void EndTurn(PlayerTurnContext context)
        {
            base.EndTurn(context);

            if (context.State is TwoCardsLeftRoundState)
                this.cardTracker.PossibleOpponentCards.Add(context.TrumpCard);

            this.cardTracker.CardPlayed(context.SecondPlayedCard);
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

        private PlayerMove PlayRandomMove(PlayerTurnContext context)
        {
            List<PlayerMove> possibleMoves = new List<PlayerMove>();

            foreach (var card in PlayerMoveValidator.GetPossibleCardsToPlay(context, this.currentCards))
            {
                Announce possibleAnnounce = this.PossibleAnnounce(card, context.TrumpCard);
                PlayerMove move = new PlayerMove(PlayerMoveType.PlayCard, card, possibleAnnounce);
                if (PlayerMoveValidator.IsValid(move, context, this.currentCards)) //ovo is Valid da li menja MOve
                    possibleMoves.Add(move);
            }

            return possibleMoves[random.Next(possibleMoves.Count)];
        }

        private PlayerMove PlayMCSTMove(PlayerTurnContext context)
        {
            PlayerPosition temp;
            switch (context.ClosedByPlayer)
            {
                case PlayerPosition.FirstPlayer:
                    temp = PlayerPosition.SecondPlayer;
                    break;
                case PlayerPosition.SecondPlayer:
                    temp = PlayerPosition.FirstPlayer;
                    break;
                default:
                    temp = PlayerPosition.NoOne;
                    break;
            }

            MCTS mcst = new MCTS(new GamePosition(this.currentCards, this.cardTracker.PossibleOpponentCards,
                context.SecondPlayerRoundPoints, context.FirstPlayerRoundPoints,
                context.FirstPlayedCard, context.FirstPlayerCardAnnounce, context.State, context.TrumpCard,
                context.CardsLeft, temp),
                5000, 10);

            return mcst.MTCSAlgorithm();
        }

        #region Normal Move
        private PlayerMove PlayNormalLevelMove(PlayerTurnContext context)
        {
            List<Card> possibleCardsToPlay = PlayerMoveValidator.GetPossibleCardsToPlay(context, this.currentCards);

            if (context.State.ShouldObserveRules())
            {
                if (context.ImFirst())
                    return this.ChooseCardWhenPlayingFirstAndRulesApply(context, possibleCardsToPlay);
                else
                    return this.ChooseCardWhenPlayingSecondAndRulesApply(context, possibleCardsToPlay);
            }
            else
            {
                if (context.ImFirst())
                    return this.ChooseCardWhenPlayingFirstAndRulesDoNotApply(context, possibleCardsToPlay);
                else
                    return this.ChooseCardWhenPlayingSecondAndRulesDoNotApply(context, possibleCardsToPlay);
            }
        }

        private PlayerMove ChooseCardWhenPlayingSecondAndRulesDoNotApply(PlayerTurnContext context, List<Card> possibleCardsToPlay)
        {
            Card biggerCard =
                possibleCardsToPlay.Where(
                    x => x.Suit == context.FirstPlayedCard.Suit && x.Value() > context.FirstPlayedCard.Value())
                    .OrderByDescending(x => x.Value())
                    .FirstOrDefault();

            if (biggerCard != null)
            {
                if (biggerCard.Type != CardType.Queen || !this.currentCards.Contains(new Card(CardType.King, biggerCard.Suit)))
                {
                    if (biggerCard.Type != CardType.King || !this.currentCards.Contains(new Card(CardType.Queen, biggerCard.Suit)))
                    {
                        return new PlayerMove(PlayerMoveType.PlayCard, biggerCard, Announce.None);
                    }
                }
            }

            if (context.FirstPlayedCard.Type == CardType.Ace || context.FirstPlayedCard.Type == CardType.Ten)
            {
                if (possibleCardsToPlay.Contains(new Card(CardType.Nine, context.TrumpCard.Suit))
                    && context.TrumpCard.Type == CardType.Jack)
                    return new PlayerMove(PlayerMoveType.PlayCard, new Card(CardType.Nine, context.TrumpCard.Suit), Announce.None);

                if (possibleCardsToPlay.Contains(new Card(CardType.Jack, context.TrumpCard.Suit)))
                    return new PlayerMove(PlayerMoveType.PlayCard, new Card(CardType.Jack, context.TrumpCard.Suit), Announce.None);

                if (possibleCardsToPlay.Contains(new Card(CardType.Queen, context.TrumpCard.Suit))
                    && this.cardTracker.PlayedCards.Contains(new Card(CardType.King, context.TrumpCard.Suit)))
                    return new PlayerMove(PlayerMoveType.PlayCard, new Card(CardType.Queen, context.TrumpCard.Suit), Announce.None);

                if (possibleCardsToPlay.Contains(new Card(CardType.King, context.TrumpCard.Suit))
                    && this.cardTracker.PlayedCards.Contains(new Card(CardType.Queen, context.TrumpCard.Suit)))
                    return new PlayerMove(PlayerMoveType.PlayCard, new Card(CardType.King, context.TrumpCard.Suit), Announce.None);

                if (possibleCardsToPlay.Contains(new Card(CardType.Ten, context.TrumpCard.Suit)))
                    return new PlayerMove(PlayerMoveType.PlayCard, new Card(CardType.Ten, context.TrumpCard.Suit), Announce.None);

                if (possibleCardsToPlay.Contains(new Card(CardType.Ace, context.TrumpCard.Suit)))
                    return new PlayerMove(PlayerMoveType.PlayCard, new Card(CardType.Ace, context.TrumpCard.Suit), Announce.None);
            }

            Card smallestCard = possibleCardsToPlay.OrderBy(x => x.Value()).FirstOrDefault();
            return new PlayerMove(PlayerMoveType.PlayCard, smallestCard, Announce.None);
        }

        private PlayerMove ChooseCardWhenPlayingFirstAndRulesDoNotApply(PlayerTurnContext context, List<Card> possibleCardsToPlay)
        {
            PlayerMove move = this.TryToAnnounce(context, possibleCardsToPlay);

            if (move != null)
                return move;

            Card cardToWinTheGame = this.GetTrumpCardWhichWillSurelyWinTheGame(context.TrumpCard.Suit, context.FirstPlayerRoundPoints, possibleCardsToPlay);

            if (cardToWinTheGame != null)
                return new PlayerMove(PlayerMoveType.PlayCard, cardToWinTheGame, Announce.None);

            Card cardToPlay =
               possibleCardsToPlay.Where(x => x.Suit != context.TrumpCard.Suit)
                   .OrderBy(x => this.cardTracker.PossibleOpponentCards.Count(y => y.Suit == x.Suit))
                   .ThenBy(x => x.Value())
                   .FirstOrDefault();

            if (cardToPlay != null)
                return new PlayerMove(PlayerMoveType.PlayCard, cardToPlay, Announce.None);

            cardToPlay = possibleCardsToPlay.OrderBy(x => x.Value()).FirstOrDefault();
            return new PlayerMove(PlayerMoveType.PlayCard, cardToPlay, Announce.None);
        }

        private Card GetTrumpCardWhichWillSurelyWinTheGame(CardSuit trumpSuit, int firstPlayerRoundPoints, List<Card> possibleCardsToPlay)
        {
            Card opponentBiggestTrumpCard =
                this.cardTracker.PossibleOpponentCards.Where(x => x.Suit == trumpSuit)
                    .OrderByDescending(x => x.Value())
                    .FirstOrDefault();

            var myBiggestTrumpCards =
                possibleCardsToPlay.Where(x => x.Suit == trumpSuit).OrderByDescending(x => x.Value());

            int sumOfPoints = 0;

            foreach (var myTrumpCard in myBiggestTrumpCards)
            {
                sumOfPoints += myTrumpCard.Value();

                if (firstPlayerRoundPoints >= 66 - sumOfPoints)
                {
                    if (opponentBiggestTrumpCard == null
                        || myTrumpCard.Value() > opponentBiggestTrumpCard.Value())
                        return myTrumpCard;
                }
            }

            return null;
        }

        private PlayerMove ChooseCardWhenPlayingSecondAndRulesApply(PlayerTurnContext context, List<Card> possibleCardsToPlay)
        {
            Card biggerCard =
               possibleCardsToPlay.Where(
                   x => x.Suit == context.FirstPlayedCard.Suit && x.Value() > context.FirstPlayedCard.Value())
                   .OrderByDescending(x => x.Value())
                   .FirstOrDefault();

            if (biggerCard != null)
                return new PlayerMove(PlayerMoveType.PlayCard, biggerCard, Announce.None);

            Card smallestTrumpCard =
                possibleCardsToPlay.Where(x => x.Suit == context.TrumpCard.Suit)
                    .OrderBy(x => x.Value())
                    .FirstOrDefault();

            if (smallestTrumpCard != null)
                return new PlayerMove(PlayerMoveType.PlayCard, smallestTrumpCard, Announce.None);

            Card cardToPlay = possibleCardsToPlay.OrderBy(x => x.Value()).FirstOrDefault();
            return new PlayerMove(PlayerMoveType.PlayCard, cardToPlay, Announce.None);

        }

        private PlayerMove ChooseCardWhenPlayingFirstAndRulesApply(PlayerTurnContext context, List<Card> possibleCardsToPlay)
        {
            bool opponentHasTrump = this.cardTracker.PossibleOpponentCards.Any(x => x.Suit == context.TrumpCard.Suit);

            Card trumpCard = this.GetCardWhichWillSurelyWinTheTrick(context.TrumpCard.Suit, opponentHasTrump);
            if (trumpCard != null)
            {
                if (PossibleAnnounce(trumpCard, context.TrumpCard) == Announce.Fourty)
                    return new PlayerMove(PlayerMoveType.PlayCard, trumpCard, Announce.Fourty);
                else
                    return new PlayerMove(PlayerMoveType.PlayCard, trumpCard, Announce.None);
            }

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                Card possibleCard = this.GetCardWhichWillSurelyWinTheTrick(suit, opponentHasTrump);
                if (possibleCard != null)
                    return new PlayerMove(PlayerMoveType.PlayCard, possibleCard, Announce.None);
            }

            PlayerMove move = this.TryToAnnounce(context, possibleCardsToPlay);
            if (move != null)
                return move;

            Card cardToPlay =
                possibleCardsToPlay.Where(x => x.Suit != context.TrumpCard.Suit)
                    .OrderBy(x => x.Value())
                    .FirstOrDefault();

            if (cardToPlay != null)
                return new PlayerMove(PlayerMoveType.PlayCard, cardToPlay, Announce.None);

            cardToPlay = possibleCardsToPlay.OrderBy(x => x.Value()).FirstOrDefault();
            return new PlayerMove(PlayerMoveType.PlayCard, cardToPlay, Announce.None);
        }

        private Card GetCardWhichWillSurelyWinTheTrick(CardSuit suit, bool opponentHasTrump)
        {
            var myBiggestCard = this.currentCards.Where(
                x => x.Suit == suit).OrderByDescending(x => x.Value()).FirstOrDefault();

            if (myBiggestCard == null)
                return null;

            var opponentBiggestCard = this.cardTracker.PossibleOpponentCards.Where(x => x.Suit == suit)
                    .OrderByDescending(x => x.Value())
                    .FirstOrDefault();

            if (!opponentHasTrump && opponentBiggestCard == null)
                return myBiggestCard;

            if (opponentBiggestCard != null && opponentBiggestCard.Value() < myBiggestCard.Value())
                return myBiggestCard;

            return null;
        }

        private PlayerMove TryToAnnounce(PlayerTurnContext context, List<Card> possibleCardsToPlay)
        {
            foreach (var card in possibleCardsToPlay)
            {
                if (this.PossibleAnnounce(card, context.TrumpCard) == Announce.Fourty)
                    if (PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.PlayCard, card, Announce.Fourty), context, possibleCardsToPlay))
                        return new PlayerMove(PlayerMoveType.PlayCard, card, Announce.Fourty);
            }

            foreach (var card in possibleCardsToPlay)
            {
                if (this.PossibleAnnounce(card, context.TrumpCard) == Announce.Twenty)
                    if (PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.PlayCard, card, Announce.Twenty), context, possibleCardsToPlay))
                        return new PlayerMove(PlayerMoveType.PlayCard, card, Announce.Twenty);
            }

            return null;
        }

        #endregion
    }
}
