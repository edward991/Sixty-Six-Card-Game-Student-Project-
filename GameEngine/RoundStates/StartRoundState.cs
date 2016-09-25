
namespace GameEngine.RoundStates
{
    public class StartRoundState : BaseRoundState
    {
        public StartRoundState(Round round)
            : base(round)
        {
        }

        public override bool CanAnnounce20Or40()
        {
            return false;
        }

        public override bool CanClose()
        {
            return false;
        }

        public override bool CanChangeTrump()
        {
            return false;
        }

        public override bool ShouldObserveRules()
        {
            return false;
        }

        public override bool ShouldDrawCard()
        {
            return true;
        }

        internal override void PlayHand(int cardsLeftInDeck)
        {
            this.round.State = new MoreThanTwoCardsLeftRoundState(this.round);
        }
    }
}
