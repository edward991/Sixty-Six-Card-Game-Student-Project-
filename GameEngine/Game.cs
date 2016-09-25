using GameEngine.Players;

namespace GameEngine
{
    public class Game
    {
        public int FirstPlayerGamePoints { get; private set; }
        public int SecondPlayerGamePoints { get; private set; }

        public BasePlayer FirstPlayer { get; private set; }
        public BasePlayer SecondPlayer { get; private set; }

        public PlayerPosition FirstToPlayInNextRound { get; private set; }

        public Game(BasePlayer firstPlayer, BasePlayer secondPlayer, PlayerPosition firstToPlayInNextRound)
        {
            FirstPlayerGamePoints = 0;
            SecondPlayerGamePoints = 0;
            this.FirstPlayer = firstPlayer;
            this.SecondPlayer = secondPlayer;
            this.FirstToPlayInNextRound = firstToPlayInNextRound;
        }

        public void Start()
        {
            while (!IsFinished())
            {
                PlayRound();
                Restart();
            }
        }

        protected virtual void Restart()
        {
        }

        protected virtual Round GenerateRound()
        {
            return new Round(FirstPlayer, SecondPlayer, FirstToPlayInNextRound);
        }

        private void PlayRound()
        {
            Round round = GenerateRound();
            round.Start();
            UpdatePointsAfterRound(round);
        }

        protected virtual void UpdatePointsAfterRound(Round round)
        {
            if (round.ClosedByPlayer == PlayerPosition.FirstPlayer && round.FirstPlayerRoundPoints < 66)
            {
                SecondPlayerGamePoints += 3;
                FirstToPlayInNextRound = PlayerPosition.FirstPlayer;
                return;
            }

            if (round.ClosedByPlayer == PlayerPosition.SecondPlayer && round.SecondPlayerRoundPoints < 66)
            {
                FirstPlayerGamePoints += 3;
                FirstToPlayInNextRound = PlayerPosition.SecondPlayer;
                return;
            }

            if (round.FirstPlayerRoundPoints > round.SecondPlayerRoundPoints)
            {
                FirstToPlayInNextRound = PlayerPosition.SecondPlayer;
                if (round.SecondPlayerRoundPoints >= 33)
                    FirstPlayerGamePoints += 1;
                else if (round.SecondPlayerRoundPoints > 0)
                    FirstPlayerGamePoints += 2;
                else
                    FirstPlayerGamePoints += 3;
            }
            else if (round.FirstPlayerRoundPoints < round.SecondPlayerRoundPoints)
            {
                FirstToPlayInNextRound = PlayerPosition.FirstPlayer;
                if (round.FirstPlayerRoundPoints >= 33)
                    SecondPlayerGamePoints += 1;
                else if (round.FirstPlayerRoundPoints > 0)
                    SecondPlayerGamePoints += 2;
                else
                    SecondPlayerGamePoints += 3;

            }
        }

        protected bool IsFinished()
        {
            if (FirstPlayerGamePoints >= 11 || SecondPlayerGamePoints >= 11)
                return true;
            else
                return false;
        }
    }
}
