using GameEngine;
using GameEngine.Cards;
using GameEngine.Players;
using System.Threading;
using System;

using ArtificialIntelligence;

namespace WIndowsFormsUI.Players
{
    class ComputerUIPlayer:ComputerPlayer
    {
        public event EventHandler DrawCard;
        public event EventHandler MyTurn;
        public event EventHandler NoOneTurn;
        public event EventHandler<Tuple<Card,int,int,int>> PlayedCard;
        public event EventHandler<Tuple<Card,int>> TrumpChanged;
        public event EventHandler GameClosed;

        public ComputerUIPlayer(GameLevel level)
            : base(level)
        {

        }

        public override PlayerMove GetTurn(PlayerTurnContext context)
        {
            MyTurn?.Invoke(this, null);

            PlayerMove playerMove = base.GetTurn(context);

            Thread.Sleep(1000);
            if (playerMove.Type == PlayerMoveType.PlayCard)
                this.UpdateFormAfterPlayingCard(playerMove, context);
            if (playerMove.Type == PlayerMoveType.ChangeTrump)
                this.UpdateFormAfterTrumpChanging(context.TrumpCard);
            if (playerMove.Type == PlayerMoveType.CloseGame)
                GameClosed?.Invoke(this, null);

            return playerMove;
        }

        public override void AddCard(Card card)
        {
            base.AddCard(card);
            DrawCard?.Invoke(this, null);
            Thread.Sleep(250);
        }

        protected void UpdateFormAfterPlayingCard(PlayerMove playerMove, PlayerTurnContext context)
        {
            int index1 = this.currentCards.FindIndex(c => c.Equals(playerMove.PlayedCard));
            this.currentCards.Remove(playerMove.PlayedCard);
            int index2 = -1;
            if (playerMove.PlayerAnnounce != Announce.None)
            {
                Card card = this.currentCards.Find(c => c.Suit == playerMove.PlayedCard.Suit && (c.Type == CardType.Queen || c.Type == CardType.King) && !c.Equals(playerMove.PlayedCard));

                index2 = this.currentCards.FindIndex(c => c == card);//mozda i equals
            }
            PlayedCard?.Invoke(this, new Tuple<Card, int, int, int>(playerMove.PlayedCard, index1, index2, (int)playerMove.PlayerAnnounce));
            NoOneTurn?.Invoke(this, null);
            if (!context.ImFirst())
                Thread.Sleep(1500);
        }

        protected void UpdateFormAfterTrumpChanging(Card trumpCard)
        {
            int index = this.currentCards.FindIndex(c => c.Equals(new Card(CardType.Nine, trumpCard.Suit)));
            currentCards.Remove(new Card(CardType.Nine, trumpCard.Suit));
            TrumpChanged?.Invoke(this, new Tuple<Card, int>(trumpCard, index));
        }

    }

}
