
namespace GameEngine.RoundStates
{
    public class MoreThanTwoCardsLeftRoundState : BaseRoundState
    {
        public MoreThanTwoCardsLeftRoundState(Round round)
            : base(round)
        {
        }

        public override bool CanAnnounce20Or40()
        {
            return true;
        }

        public override bool CanClose()
        {
            return true;
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
            if (cardsLeftInDeck == 2)
            {
                this.round.State = new TwoCardsLeftRoundState(this.round);
            }
        }
    }
}
