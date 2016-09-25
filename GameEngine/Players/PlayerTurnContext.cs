using GameEngine.Cards;
using GameEngine.RoundStates;

namespace GameEngine.Players
{
    public class PlayerTurnContext
    {
        public Card TrumpCard { get; set; }
        public Card FirstPlayedCard { get; internal set; }
        public Announce FirstPlayerCardAnnounce { get; internal set; }
        public Card SecondPlayedCard { get; internal set; }
        public BaseRoundState State { get; internal set; }
        public int CardsLeft { get; private set; }
        public int FirstPlayerRoundPoints { get; private set; }
        public int SecondPlayerRoundPoints { get; private set; }
        public PlayerPosition ClosedByPlayer { get; private set; }

        public PlayerTurnContext(BaseRoundState state, Card trumpCard, int cardsLeft,int firstPlayerPoints,int secondPlayerPoints, PlayerPosition closedByPlayer)
        {
            this.TrumpCard = trumpCard;
            this.State = state;
            FirstPlayedCard = null;
            SecondPlayedCard = null;
            this.CardsLeft = cardsLeft;
            FirstPlayerRoundPoints = firstPlayerPoints;
            SecondPlayerRoundPoints = secondPlayerPoints;
            this.ClosedByPlayer = closedByPlayer;
        }

        public PlayerTurnContext(BaseRoundState state,Card trumpCard,Card firstPlayedCard)
        {
            this.State = state;
            this.TrumpCard = trumpCard;
            this.FirstPlayedCard = firstPlayedCard;
            this.CardsLeft = this.FirstPlayerRoundPoints = this.SecondPlayerRoundPoints = 0;
        }

        public bool ImFirst()
        {
            return this.FirstPlayedCard == null;
        } 
    }
}
