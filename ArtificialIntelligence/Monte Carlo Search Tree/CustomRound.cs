using GameEngine;
using GameEngine.Cards;
using GameEngine.Players;
using GameEngine.RoundStates;
using System.Collections.Generic;

namespace ArtificialIntelligence.MonteCarloSearchTree
{
    class CustomRound : Round
    {
        public CustomRound(BasePlayer firstPlayer, BasePlayer secondPlayer, PlayerPosition firstToPlay,
            int firstPlayerRoundPoints, int secondPlayerRoundPoints, List<Card> firstPlayerCards,
            List<Card> secondPlayerCards, List<Card> deckCards, Card trumpCard, BaseRoundState state, PlayerPosition ClosedBy)
            : base(firstPlayer, secondPlayer, firstToPlay)
        {
            this.FirstPlayerRoundPoints = firstPlayerRoundPoints;
            this.SecondPlayerRoundPoints = secondPlayerRoundPoints;
            this.firstPlayerCards = new List<Card>(firstPlayerCards);
            this.secondPlayerCards = new List<Card>(secondPlayerCards);
            this.ClosedByPlayer = ClosedBy;
            this.deck = new Deck(deckCards, trumpCard);
            if (state is StartRoundState)
                this.State = new StartRoundState(this);
            else if (state is MoreThanTwoCardsLeftRoundState)
                this.State = new MoreThanTwoCardsLeftRoundState(this);
            else if (state is TwoCardsLeftRoundState)
                this.State = new TwoCardsLeftRoundState(this);
            else if (state is FinalRoundState)
                this.State = new FinalRoundState(this);
        }


        public int SimulateToTheEndOfRound()
        {
            while (!this.IsFinished())
            {
                this.PlayHand();
            }

            return SimulationReward();
        }

        private int SimulationReward()
        {
            if (this.ClosedByPlayer == PlayerPosition.FirstPlayer && this.FirstPlayerRoundPoints < 66)
                return -3;

            if (this.ClosedByPlayer == PlayerPosition.SecondPlayer && this.SecondPlayerRoundPoints < 66)
                return +3;

            if (this.FirstPlayerRoundPoints > this.SecondPlayerRoundPoints)
            {
                if (this.SecondPlayerRoundPoints >= 33)
                    return 1;
                else if (this.SecondPlayerRoundPoints > 0)
                    return 2;
                else
                    return 3;
            }
            else if (this.FirstPlayerRoundPoints < this.SecondPlayerRoundPoints)
            {
                if (this.FirstPlayerRoundPoints >= 33)
                    return -1;
                else if (this.FirstPlayerRoundPoints > 0)
                    return -2;
                else
                    return -3;
            }

            return 0;
        }
    }
}
