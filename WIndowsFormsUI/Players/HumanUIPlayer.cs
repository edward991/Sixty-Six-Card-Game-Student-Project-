using GameEngine;
using GameEngine.Cards;
using GameEngine.Players;
using System;
using System.Threading;

namespace WIndowsFormsUI.Players
{
    class HumanUIPlayer : BasePlayer
    {
        private PlayerMove userMove;

        private string name;

        public AutoResetEvent Signal { get; set; }

        public event EventHandler MyTurn;
        public event EventHandler NoOneTurn;
        public event EventHandler<Card> DrawCard;
        public event EventHandler<Card> InvalidMove;
        public event EventHandler<int> Announced;
        public event EventHandler GameClosed;
        public event EventHandler<Card> TrumpChanged;

        public HumanUIPlayer(String name)
            : base()
        {
            Signal = new AutoResetEvent(false);
            this.name = name;
        }

        public String Name
        {
            get
            { return this.name; }
        }

        public void Move(PlayerMove playerMove)
        {
            this.userMove = playerMove;
        }

        public override PlayerMove GetTurn(PlayerTurnContext context)
        {
            MyTurn?.Invoke(this, null);
            PlayerMove playerMove;

            while(true)
            {
                Signal.WaitOne();

                if (userMove.Type == PlayerMoveType.PlayCard)
                {
                    Announce possibleAnnounce = PossibleAnnounce(userMove.PlayedCard, context.TrumpCard);
                    playerMove = new PlayerMove(PlayerMoveType.PlayCard, userMove.PlayedCard, possibleAnnounce);
                }
                else if (userMove.Type == PlayerMoveType.ChangeTrump)
                    playerMove = new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None);
                else if (userMove.Type == PlayerMoveType.CloseGame)
                    playerMove = new PlayerMove(PlayerMoveType.CloseGame, null, Announce.None);
                else
                    playerMove = null;

                if (PlayerMoveValidator.IsValid(playerMove, context, currentCards))
                {
                    if (playerMove.Type == PlayerMoveType.PlayCard)
                        UpdateFormAfterPlayingCard(playerMove, context);
                    else if (playerMove.Type == PlayerMoveType.ChangeTrump)
                        UpdateFormAfterTrumpChanging(context.TrumpCard);
                    else if (playerMove.Type == PlayerMoveType.CloseGame)
                        UpdateFormAfterClosing();

                    userMove = null;
                    return playerMove;
                }
                else
                {
                    if (playerMove.PlayerAnnounce != Announce.None)
                    {
                        playerMove = new PlayerMove(PlayerMoveType.PlayCard, userMove.PlayedCard, Announce.None);
                        if (PlayerMoveValidator.IsValid(playerMove, context, this.currentCards))
                        {
                            UpdateFormAfterPlayingCard(playerMove, context);

                            userMove = null;
                            return playerMove;
                        }
                    }
                    UpdateFormAfterInvalidMove(playerMove, userMove.PlayedCard);

                    continue;
                }
            }
        }

        public override void AddCard(Card card)
        {
            base.AddCard(card);
            //Thread.Sleep(150);
            DrawCard?.Invoke(this, card);
            Thread.Sleep(250);
        }

        private void UpdateFormAfterInvalidMove(PlayerMove playerMove,Card card)
        {
            InvalidMove?.Invoke(this, card);
        }

        protected virtual void UpdateFormAfterPlayingCard(PlayerMove playerMove,PlayerTurnContext context)
        {
            currentCards.Remove(playerMove.PlayedCard);
            //Thread.Sleep(1000);

            if (playerMove.PlayerAnnounce != Announce.None)
                Announced?.Invoke(this, (int)playerMove.PlayerAnnounce);

            NoOneTurn?.Invoke(this, null);

            if (!context.ImFirst())
                Thread.Sleep(1500);
        }

        protected virtual void UpdateFormAfterTrumpChanging(Card trumpCard)
        {
            currentCards.Remove(new Card(CardType.Nine, trumpCard.Suit));
            TrumpChanged?.Invoke(this, trumpCard);
        }

        protected void UpdateFormAfterClosing()
        {
            GameClosed?.Invoke(this, null);
        }
    }
}
