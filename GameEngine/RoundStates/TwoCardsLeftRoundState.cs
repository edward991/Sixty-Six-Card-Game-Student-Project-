
namespace GameEngine.RoundStates
{
    public class TwoCardsLeftRoundState : BaseRoundState
    {
        public TwoCardsLeftRoundState(Round round)
            : base(round)
        {
        }

        public override bool CanAnnounce20Or40()
        {
            return true;
        }

        public override bool CanClose()
        {
            return false;
        }

        public override bool CanChangeTrump()
        {
            return true;
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
            this.round.State = new FinalRoundState(this.round);
        }
    }
}
