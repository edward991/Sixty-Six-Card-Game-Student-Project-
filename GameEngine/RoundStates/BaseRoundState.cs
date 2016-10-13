
namespace GameEngine.RoundStates
{
    public abstract class BaseRoundState
    {
        protected Round round;

        protected BaseRoundState(Round round)
        {
            this.round = round;
        }

        public abstract bool CanAnnounce20Or40();

        public abstract bool CanClose();

        public abstract bool CanChangeTrump();

        public abstract bool ShouldObserveRules();

        public abstract bool ShouldDrawCard();

        internal abstract void PlayHand(int cardsLeftInDeck);

        internal void Close()
        {
            if (this.CanClose())
            {
                this.round.State = new FinalRoundState(this.round);
            }
        }
    }
}
