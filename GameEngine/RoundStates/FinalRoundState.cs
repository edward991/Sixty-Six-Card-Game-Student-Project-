
namespace GameEngine.RoundStates
{
    public class FinalRoundState : BaseRoundState
    {
        public FinalRoundState(Round round)
            : base(round)
        {
        }

        public FinalRoundState()
            : base(null)
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
            return true;
        }

        public override bool ShouldDrawCard()
        {
            return false;
        }

        internal override void PlayHand(int cardsLeftInDeck)
        {
        }
    }
}
