using GameEngine;
using GameEngine.Cards;
using GameEngine.Players;
using System;
using System.Threading;
using WIndowsFormsUI.Players;

namespace WIndowsFormsUI
{
    class UIRound : Round
    {
        public event EventHandler<Tuple<int, int, bool>> UpdateRoundPoints;
        public event EventHandler<Tuple<Card,bool>> PrintRoundInfo;

        
        public UIRound(HumanUIPlayer humanPlayer, ComputerUIPlayer computerPlayer, PlayerPosition playerPosition)
            : base(humanPlayer, computerPlayer, playerPosition)
        {
        }
        

        protected override void UpdateRoundInfo()
        {
            base.UpdateRoundInfo();
            PrintRoundInfo?.Invoke(this, new Tuple<Card, bool>(this.deck.TrumpCard, this.State is GameEngine.RoundStates.StartRoundState));
        }

        protected override void UpdatePoints(Hand hand)
        {
            base.UpdatePoints(hand);

            UpdateRoundPoints?.Invoke(this, new Tuple<int, int, bool>(FirstPlayerRoundPoints, SecondPlayerRoundPoints, IsFinished()));
            if (this.IsFinished())
                Thread.Sleep(2000);
        }
    }
}
