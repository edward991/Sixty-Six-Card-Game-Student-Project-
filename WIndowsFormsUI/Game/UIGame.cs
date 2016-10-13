using GameEngine;
using GameEngine.Cards;
using GameEngine.Players;
using System;
using System.Threading;
using WIndowsFormsUI.Players;

namespace WIndowsFormsUI
{
    class UIGame:Game
    {
        public event EventHandler<Tuple<int,int,bool>> UpdatePoints;
        public event EventHandler<Tuple<int, int, bool>> UpdateRoundPoints;
        public event EventHandler<Tuple<Card,bool>> PrintRoundInfo;
        public event EventHandler RestartRound;

        public UIGame(HumanUIPlayer humanPlayer, ComputerUIPlayer computerPlayer, PlayerPosition firstToPlay)
        : base(humanPlayer, computerPlayer, firstToPlay)
        {
        }

        protected override void UpdatePointsAfterRound(Round round)
        {
            base.UpdatePointsAfterRound(round);
            UpdatePoints?.Invoke(this, new Tuple<int, int, bool>(FirstPlayerGamePoints, SecondPlayerGamePoints, IsFinished()));
        }

        protected override void Restart()
        {
            base.Restart();
            Thread.Sleep(1000);
            if (!IsFinished())
            {
                UpdateRoundPoints?.Invoke(this, new Tuple<int, int, bool>(0, 0, false));
                RestartRound?.Invoke(this, null);
            }
        }

        protected override Round GenerateRound()
        {
            UIRound round = new UIRound((HumanUIPlayer)this.FirstPlayer, (ComputerUIPlayer)this.SecondPlayer, this.FirstToPlayInNextRound);
            round.UpdateRoundPoints += UpdateRoundPoints;
            round.PrintRoundInfo += PrintRoundInfo;
            return round;
        }
    }
}
