using GameEngine.Cards;
using GameEngine.Players;
using GameEngine.RoundStates;
using System.Collections.Generic;

namespace GameEngine
{
    public class Round
    {
        protected Deck deck;
        protected BasePlayer firstPlayer;
        protected BasePlayer secondPlayer;
        protected List<Card> firstPlayerCards;
        protected List<Card> secondPlayerCards;

        public int FirstPlayerRoundPoints { get; protected set; }
        public int SecondPlayerRoundPoints { get; protected set; }

        public PlayerPosition ClosedByPlayer { get; protected set; }
        public PlayerPosition FirstToPlay { get; private set; }

        public BaseRoundState State { get; set; }

        public Round(BasePlayer firstPlayer, BasePlayer secondPlayer, PlayerPosition firstToPlay)
        {
            deck = new Deck();

            FirstPlayerRoundPoints = 0;
            SecondPlayerRoundPoints = 0;

            this.firstPlayer = firstPlayer;
            this.secondPlayer = secondPlayer;

            firstPlayerCards = new List<Card>();
            secondPlayerCards = new List<Card>();

            this.FirstToPlay = firstToPlay;

            ClosedByPlayer = PlayerPosition.NoOne;
            State = new StartRoundState(this);
        }

        public void Start()
        {
            DealCards();
            firstPlayer.StartRound();
            secondPlayer.StartRound();

            while (!IsFinished())
            {
                PlayHand();
            }
        }

        protected void PlayHand()
        {
            Hand hand = 
                new Hand(FirstToPlay, firstPlayer, firstPlayerCards, secondPlayer, secondPlayerCards, State, deck, FirstPlayerRoundPoints, SecondPlayerRoundPoints,ClosedByPlayer);

            UpdateRoundInfo();

            hand.Start();

            FirstToPlay = hand.HandWinner;

            firstPlayerCards.Remove(hand.FirstPlayerCard);
            secondPlayerCards.Remove(hand.SecondPlayerCard);

            UpdatePoints(hand);

            if (hand.GameClosedInThisHand == PlayerPosition.FirstPlayer || hand.GameClosedInThisHand == PlayerPosition.SecondPlayer)
            {
                ClosedByPlayer = hand.GameClosedInThisHand;
            }

            if (!IsFinished())
            {
                DrawNewCards();
            }

            State.PlayHand(deck.CardsLeft());
        }

        private void DrawNewCards()
        {
            if (State.ShouldDrawCard())
            {
                if (FirstToPlay == PlayerPosition.FirstPlayer)
                {
                    GiveCardToFirstPlayer();
                    GiveCardToSecondPlayer();
                }
                else if (this.FirstToPlay == PlayerPosition.SecondPlayer)
                {
                    GiveCardToSecondPlayer();
                    GiveCardToFirstPlayer();
                }
            }
        }

        protected virtual void UpdateRoundInfo()
        {
        }

        protected virtual void UpdatePoints(Hand hand)
        {
            if (hand.HandWinner == PlayerPosition.FirstPlayer)
            {
                FirstPlayerRoundPoints += hand.FirstPlayerCard.Value() + hand.SecondPlayerCard.Value();
                if (firstPlayerCards.Count == 0 && ClosedByPlayer == PlayerPosition.NoOne)
                {
                    FirstPlayerRoundPoints += 10;
                }
            }
            else if (hand.HandWinner == PlayerPosition.SecondPlayer)
            {
                SecondPlayerRoundPoints += hand.FirstPlayerCard.Value() + hand.SecondPlayerCard.Value();
                if (secondPlayerCards.Count == 0 && ClosedByPlayer == PlayerPosition.NoOne)
                {
                    SecondPlayerRoundPoints += 10;
                }
            }

            FirstPlayerRoundPoints += (int)hand.FirstPlayerAnnounce;
            SecondPlayerRoundPoints += (int)hand.SecondPlayerAnnounce;
        }

        private void GiveCardToSecondPlayer()
        {
            Card card = deck.GetNextCard();

            secondPlayer.AddCard(card);
            secondPlayerCards.Add(card);
        }

        private void GiveCardToFirstPlayer()
        {
            Card card = deck.GetNextCard();

            firstPlayer.AddCard(card);
            firstPlayerCards.Add(card);
        }

        private void DealCards()
        {
            firstPlayer.ResetCards();
            secondPlayer.ResetCards();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GiveCardToFirstPlayer();
                }
                for (int j = 0; j < 3; j++)
                {
                    GiveCardToSecondPlayer();
                }
            }
        }

        protected bool IsFinished()
        {
            if (FirstPlayerRoundPoints >= 66 || SecondPlayerRoundPoints >= 66)
            {
                return true;
            }
            if (firstPlayerCards.Count == 0 || secondPlayerCards.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}
