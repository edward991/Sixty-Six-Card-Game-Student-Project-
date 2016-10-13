using GameEngine;
using GameEngine.Cards;
using GameEngine.Players;
using GameEngine.RoundStates;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtificialIntelligence.MonteCarloSearchTree
{
    public class GamePosition
    {
        public List<Card> MyCards { get; private set; }
        public List<Card> PossibleOpponentCards { get; private set; }

        public int MyPoints { get; private set; }
        public int OpponentPoints { get; private set; }

        public Card PlayedCard { get; private set; }
        public Announce PlayedCardAnnounce { get; private set; }

        public BaseRoundState State { get; private set; }
        public Card TrumpCard { get; private set; }
        public int CardsLeft { get; private set; }

        public PlayerPosition ClosedByPlayer { get; private set; }

        public GamePosition(List<Card> myCards, List<Card> possibleOpponentCards, int myPoints, int opponentPoints,
            Card playedCard, Announce playedCardAnnounce, BaseRoundState state, Card trumpCard,
            int cardsLeft, PlayerPosition closedByPlayer)
        {
            this.MyCards = new List<Card>(myCards);
            this.PossibleOpponentCards = new List<Card>(possibleOpponentCards);
            this.MyPoints = myPoints;
            this.OpponentPoints = opponentPoints;
            this.PlayedCard = playedCard;
            this.PlayedCardAnnounce = playedCardAnnounce;
            this.State = state;
            this.TrumpCard = trumpCard;
            this.CardsLeft = cardsLeft;
            this.ClosedByPlayer = closedByPlayer;
        }

        public bool ImFirst
        {
            get { return this.PlayedCard == null; }
        }

        public List<PlayerMove> GetPossibleMoves()
        {
            List<PlayerMove> possibleMoves = new List<PlayerMove>();
            PlayerTurnContext context = new PlayerTurnContext(this.State, this.TrumpCard, this.PlayedCard);
            PlayerMove move;

            if (ImFirst)
            {
                move = new PlayerMove(PlayerMoveType.CloseGame, null, Announce.None);

                if (PlayerMoveValidator.IsValid(move, context, MyCards))
                    possibleMoves.Add(move);

                move = new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None);

                if (PlayerMoveValidator.IsValid(move, context, MyCards))
                    possibleMoves.Add(move);
            }

            foreach (Card card in this.MyCards)
            {
                if (ImFirst)
                {
                    Announce possibleAnnounce = this.PossibleAnnounce(card, context.TrumpCard, this.MyCards);
                    move = new PlayerMove(PlayerMoveType.PlayCard, card, possibleAnnounce);

                    if (PlayerMoveValidator.IsValid(move, context, this.MyCards))
                        possibleMoves.Add(move);
                    else
                    {
                        move.PlayerAnnounce = Announce.None;
                        if (PlayerMoveValidator.IsValid(move, context, this.MyCards))
                            possibleMoves.Add(move);
                    }
                }
                else
                {
                    move = new PlayerMove(PlayerMoveType.PlayCard, card, Announce.None);
                    if (PlayerMoveValidator.IsValid(move, context, this.MyCards))
                        possibleMoves.Add(move);
                }

            }

            return possibleMoves;
        }

        public List<GamePosition> CreateLeafs(PlayerMove move)
        {
            List<GamePosition> newGamePositions = new List<GamePosition>();

            if (move.Type == PlayerMoveType.ChangeTrump)
            {
                List<Card> myNewCards = MyCards.Where(
                    x => !x.Equals(new Card(CardType.Nine, TrumpCard.Suit))).ToList();

                myNewCards.Add(this.TrumpCard);

                GamePosition position = new GamePosition(myNewCards, PossibleOpponentCards, MyPoints, OpponentPoints,
                    null, Announce.None, State, new Card(CardType.Nine, TrumpCard.Suit),
                    CardsLeft, PlayerPosition.NoOne);

                newGamePositions.Add(position);
            }

            if (move.Type == PlayerMoveType.CloseGame)
            {
                GamePosition position = new GamePosition(MyCards, PossibleOpponentCards, MyPoints, OpponentPoints,
                    null, Announce.None, new FinalRoundState(), TrumpCard, CardsLeft,
                    PlayerPosition.FirstPlayer);

                newGamePositions.Add(position);
            }

            if (move.Type == PlayerMoveType.PlayCard)
            {
                BaseRoundState newState = this.StateInNextGamePosition(this.State, this.CardsLeft);

                if (this.ImFirst)
                {
                    foreach (var gamePosition in NewGamePositionsWhenPlayFirst(newState, move))
                        newGamePositions.Add(gamePosition);
                }
                else
                {
                    foreach (var gamePosition in NewGamePositionsWhenPlaySecond(newState, move))
                        newGamePositions.Add(gamePosition);
                }
            }

            return newGamePositions;
        }

        private List<GamePosition> NewGamePositionsWhenPlaySecond(BaseRoundState newState, PlayerMove move)
        {
            int handValue = move.PlayedCard.Value() + this.PlayedCard.Value();
            List<GamePosition> newGamePositions = new List<GamePosition>();
            PlayerTurnContext context;

            List<Card> myNewCards = this.MyCards.Where(x => !x.Equals(move.PlayedCard)).ToList();
            List<Card> possibleOpponentNewCards = this.PossibleOpponentCards.Where(
                x => !x.Equals(this.PlayedCard)).ToList();

            if (HandWinner(move.PlayedCard, this.PlayedCard, false))
            {
                if (possibleOpponentNewCards.Count == 0)
                {
                    newGamePositions.Add(new GamePosition(
                            myNewCards, possibleOpponentNewCards, MyPoints + handValue, OpponentPoints + (int)PlayedCardAnnounce,
                            null, Announce.None, newState, TrumpCard,
                            (CardsLeft - 2 > 0) ? CardsLeft - 2 : 0, ClosedByPlayer));
                }
                
                foreach (var myNewDrawnCard in possibleOpponentNewCards)
                {
                    List<Card> myCardsTempList = new List<Card>(myNewCards);
                    List<Card> opponentCardsTempList = new List<Card>(possibleOpponentNewCards);

                    if (!(newState is FinalRoundState && this.State is FinalRoundState))
                    {
                        myCardsTempList.Add(myNewDrawnCard);
                        opponentCardsTempList.Remove(myNewDrawnCard);
                    }

                    if (State is TwoCardsLeftRoundState)
                        opponentCardsTempList.Add(this.TrumpCard);

                    newGamePositions.Add(new GamePosition(
                        myCardsTempList, opponentCardsTempList, MyPoints + handValue, OpponentPoints + (int)PlayedCardAnnounce,
                        null, Announce.None, newState, TrumpCard,
                        (CardsLeft - 2 > 0) ? CardsLeft - 2 : 0, ClosedByPlayer));

                    if (newState is FinalRoundState && this.State is FinalRoundState)
                        break;
                }                
            }
            else
            {
                context = new PlayerTurnContext(newState, this.TrumpCard, null);
                Card newTrumpCard = null;
                if (PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None), context, possibleOpponentNewCards))
                {
                    newTrumpCard = new Card(CardType.Nine, this.TrumpCard.Suit);
                    possibleOpponentNewCards.Remove(new Card(CardType.Nine, this.TrumpCard.Suit));
                    possibleOpponentNewCards.Add(this.TrumpCard);
                    context.TrumpCard = new Card(CardType.Nine, this.TrumpCard.Suit);
                }

                if (possibleOpponentNewCards.Count == 0)
                {
                    newGamePositions.Add(
                                new GamePosition(myNewCards, possibleOpponentNewCards, MyPoints, OpponentPoints + handValue + (int)PlayedCardAnnounce,
                                null, Announce.None, newState, (newTrumpCard != null) ? newTrumpCard : TrumpCard,
                                (CardsLeft - 2 > 0) ? CardsLeft - 2 : 0, ClosedByPlayer));
                }
                else if (possibleOpponentNewCards.Count == 1)
                {
                    newGamePositions.Add(
                                new GamePosition(myNewCards, new List<Card>(), MyPoints, OpponentPoints + handValue + (int)PlayedCardAnnounce,
                                possibleOpponentNewCards[0], Announce.None, newState, (newTrumpCard != null) ? newTrumpCard : TrumpCard,
                                (CardsLeft - 2 > 0) ? CardsLeft - 2 : 0, ClosedByPlayer));
                }
                foreach (var opponentPlayedCard in possibleOpponentNewCards)
                {
                        foreach (var myNewDrawnCard in possibleOpponentNewCards)
                        {
                            if (!myNewDrawnCard.Equals(opponentPlayedCard)
                                && PlayerMoveValidator.IsValid(new PlayerMove(
                                    PlayerMoveType.PlayCard, opponentPlayedCard, Announce.None), context,
                                    possibleOpponentNewCards))
                            {
                                List<Card> myCardsTempList = new List<Card>(myNewCards);
                                List<Card> opponentCardsTempList = new List<Card>(possibleOpponentNewCards);
                                opponentCardsTempList.Remove(opponentPlayedCard);

                                if (!(newState is FinalRoundState && this.State is FinalRoundState))
                                {
                                    if (State is TwoCardsLeftRoundState)
                                        myCardsTempList.Add(TrumpCard);
                                    else
                                    {
                                        myCardsTempList.Add(myNewDrawnCard);
                                        opponentCardsTempList.Remove(myNewDrawnCard);
                                    }
                                }

                                newGamePositions.Add(
                                    new GamePosition(myCardsTempList, opponentCardsTempList, MyPoints, OpponentPoints + handValue + (int)PlayedCardAnnounce,
                                    opponentPlayedCard, Announce.None, newState, (newTrumpCard != null) ? newTrumpCard : TrumpCard,
                                    (CardsLeft - 2 > 0) ? CardsLeft - 2 : 0, ClosedByPlayer));

                                if ((newState is FinalRoundState && this.State is FinalRoundState) || State is TwoCardsLeftRoundState)
                                    break;
                            }
                        }
                }
            }

            return newGamePositions;
        }

        private List<GamePosition> NewGamePositionsWhenPlayFirst(BaseRoundState newState, PlayerMove move)
        {
            int handValue;

            PlayerTurnContext context = new PlayerTurnContext(this.State, this.TrumpCard, move.PlayedCard);

            List<GamePosition> newGamePositions = new List<GamePosition>();

            foreach (var opponentCard in this.PossibleOpponentCards)
            {
                PlayerMove opponentMove = new PlayerMove(PlayerMoveType.PlayCard, opponentCard, Announce.None);

                if (PlayerMoveValidator.IsValid(opponentMove, context, this.PossibleOpponentCards))
                {
                    handValue = move.PlayedCard.Value() + opponentCard.Value();

                    List<Card> myNewCards = this.MyCards.Where(x => !x.Equals(move.PlayedCard)).ToList();
                    List<Card> possibleOpponentNewCards = this.PossibleOpponentCards.Where(
                        x => !x.Equals(opponentCard)).ToList();

                    if (HandWinner(move.PlayedCard, opponentCard, true))
                    {
                        if (possibleOpponentNewCards.Count == 0)
                        {
                            newGamePositions.Add(new GamePosition(
                                myNewCards, possibleOpponentNewCards, MyPoints + (int)move.PlayerAnnounce + handValue,
                                OpponentPoints, null, Announce.None, newState, TrumpCard,
                                (CardsLeft - 2 > 0) ? CardsLeft - 2 : 0, ClosedByPlayer));
                        }
                        foreach (var myNewDrawnCard in possibleOpponentNewCards)
                        {
                            List<Card> myCardsTempList = new List<Card>(myNewCards);
                            List<Card> opponentCardsTempList = new List<Card>(possibleOpponentNewCards);

                            if (!(newState is FinalRoundState && this.State is FinalRoundState))
                            {
                                myCardsTempList.Add(myNewDrawnCard);
                                opponentCardsTempList.Remove(myNewDrawnCard);
                            }

                            if (State is TwoCardsLeftRoundState)
                                opponentCardsTempList.Add(this.TrumpCard);

                            newGamePositions.Add(new GamePosition(
                                myCardsTempList, opponentCardsTempList, MyPoints + (int)move.PlayerAnnounce + handValue,
                                OpponentPoints, null, Announce.None, newState, TrumpCard,
                                (CardsLeft - 2 > 0) ? CardsLeft - 2 : 0, ClosedByPlayer));

                            if (newState is FinalRoundState && this.State is FinalRoundState)
                                break;
                        }
                    }
                    else
                    {
                        PlayerTurnContext newContext = new PlayerTurnContext(newState, this.TrumpCard, null);
                        Card newTrumpCard = null;

                        if (PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None), context, possibleOpponentNewCards))
                        {
                            newTrumpCard = new Card(CardType.Nine, this.TrumpCard.Suit);
                            possibleOpponentNewCards.Remove(new Card(CardType.Nine, this.TrumpCard.Suit));
                            possibleOpponentNewCards.Add(this.TrumpCard);
                            context.TrumpCard = new Card(CardType.Nine, this.TrumpCard.Suit);
                        }

                        if (possibleOpponentNewCards.Count == 0)
                        {
                            newGamePositions.Add(new GamePosition(
                                        myNewCards, possibleOpponentNewCards, this.MyPoints + (int)move.PlayerAnnounce, this.OpponentPoints + handValue,
                                        null, Announce.None, newState, this.TrumpCard,
                                        (this.CardsLeft - 2 > 0) ? this.CardsLeft - 2 : 0, this.ClosedByPlayer));
                        }
                        else if (possibleOpponentNewCards.Count == 1)
                        {
                            newGamePositions.Add(new GamePosition(
                                        myNewCards, new List<Card>(), this.MyPoints + (int)move.PlayerAnnounce, this.OpponentPoints + handValue,
                                        possibleOpponentNewCards[0], Announce.None, newState, this.TrumpCard,
                                        (this.CardsLeft - 2 > 0) ? this.CardsLeft - 2 : 0, this.ClosedByPlayer));
                        }
                        foreach (var opponentPlayedCard in possibleOpponentNewCards)
                        {
                            foreach (var myNewDrawnCard in possibleOpponentNewCards)
                            {
                                if (!myNewDrawnCard.Equals(opponentPlayedCard)
                                && PlayerMoveValidator.IsValid(new PlayerMove(PlayerMoveType.PlayCard,
                                opponentPlayedCard, Announce.None), newContext, possibleOpponentNewCards))
                                {
                                    List<Card> myCardsTempList = new List<Card>(myNewCards);
                                    List<Card> opponentCardsTempList = new List<Card>(possibleOpponentNewCards);
                                    opponentCardsTempList.Remove(opponentPlayedCard);
                                    if (!(newState is FinalRoundState && this.State is FinalRoundState))
                                    {
                                        if (State is TwoCardsLeftRoundState)
                                            myCardsTempList.Add(TrumpCard);
                                        else
                                        {
                                            myCardsTempList.Add(myNewDrawnCard);
                                            opponentCardsTempList.Remove(myNewDrawnCard);
                                        }
                                    }

                                    newGamePositions.Add(new GamePosition(
                                        myCardsTempList, opponentCardsTempList, this.MyPoints + (int)move.PlayerAnnounce, this.OpponentPoints + handValue,
                                        opponentPlayedCard, Announce.None, newState, this.TrumpCard,
                                        (this.CardsLeft - 2 > 0) ? this.CardsLeft - 2 : 0, this.ClosedByPlayer));

                                    if ((newState is FinalRoundState && this.State is FinalRoundState) || State is TwoCardsLeftRoundState)
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return newGamePositions;
        }

        private BaseRoundState StateInNextGamePosition(BaseRoundState state, int cardsLeft)
        {
            if (state is StartRoundState)
                return new MoreThanTwoCardsLeftRoundState(null);
            else if (state is MoreThanTwoCardsLeftRoundState)
            {
                if (cardsLeft == 4)
                    return new TwoCardsLeftRoundState(null);
                else
                    return new MoreThanTwoCardsLeftRoundState(null);
            }
            else if (state is TwoCardsLeftRoundState)
                return new FinalRoundState();
            else
                return new FinalRoundState();
        }

        private Announce PossibleAnnounce(Card card, Card trumpCard, List<Card> currentCards)
        {
            if (card.Type == CardType.Queen)
            {
                if (currentCards.Contains(new Card(CardType.King, card.Suit)))
                {
                    if (card.Suit == trumpCard.Suit)
                        return Announce.Fourty;
                    else
                        return Announce.Twenty;
                }
            }
            else if (card.Type == CardType.King)
            {
                if (currentCards.Contains(new Card(CardType.Queen, card.Suit)))
                {
                    if (card.Suit == trumpCard.Suit)
                        return Announce.Fourty;
                    else
                        return Announce.Twenty;
                }
            }
            return Announce.None;
        }

        private bool HandWinner(Card myCard, Card opponentCard, bool ImFirst)
        {
            if (myCard.Suit == opponentCard.Suit)
            {
                if (myCard.Value() > opponentCard.Value())
                    return true;
                else
                    return false;
            }
            else if (ImFirst)
            {
                if (opponentCard.Suit == this.TrumpCard.Suit)
                    return false;
                else
                    return true;
            }
            else
            {
                if (myCard.Suit == this.TrumpCard.Suit)
                    return true;
                else
                    return false;
            }
        }
    }
}