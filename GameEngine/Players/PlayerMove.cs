using GameEngine.Cards;

namespace GameEngine.Players
{
    public class PlayerMove
    {
        public Card PlayedCard { get; private set; }
        public PlayerMoveType Type { get; private set; }
        public Announce PlayerAnnounce { get; set; }

        public PlayerMove(PlayerMoveType type, Card playedCard, Announce playerAnnounce)
        {
            this.Type = type;
            this.PlayedCard = playedCard;
            this.PlayerAnnounce = playerAnnounce;
        }
    }
}
