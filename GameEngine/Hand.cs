using GameEngine.Cards;
using GameEngine.Players;
using GameEngine.RoundStates;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class Hand
    {
        private BasePlayer firstPlayer;
        private BasePlayer secondPlayer;

        private List<Card> firstPlayerCards;
        private List<Card> secondPlayerCards;

        private int firstPlayerPoints;
        private int secondPlayerPoints;

        private BaseRoundState state;
        private Deck deck;

        public PlayerPosition FirstToPlay { get; private set; }
        public PlayerPosition HandWinner { get; private set; }

        public Card FirstPlayerCard { get; private set; }
        public Card SecondPlayerCard { get; private set; }

        public Announce FirstPlayerAnnounce { get; private set; }
        public Announce SecondPlayerAnnounce { get; private set; }

        public PlayerPosition GameClosedInThisHand { get; private set; }
        public PlayerPosition ClosedByPlayer { get; private set; }

        public Hand(PlayerPosition firstToPlay, BasePlayer firstPlayer, List<Card> firstPlayerCards, BasePlayer secondPlayer, List<Card> secondPlayerCards, BaseRoundState state, Deck deck, int firstPlayerPoints, int secondPlayerPoints, PlayerPosition closedByPlayer)
        {
            this.FirstToPlay = firstToPlay;

            this.firstPlayer = firstPlayer;
            this.firstPlayerCards = firstPlayerCards;
            this.firstPlayerPoints = firstPlayerPoints;

            this.secondPlayer = secondPlayer;
            this.secondPlayerCards = secondPlayerCards;
            this.secondPlayerPoints = secondPlayerPoints;

            this.state = state;
            this.deck = deck;
            GameClosedInThisHand = PlayerPosition.NoOne;
            this.ClosedByPlayer = closedByPlayer;
        }

        public void Start()
        {
            BasePlayer firstToPlayInHand;
            BasePlayer secondToPlayInHand;

            List<Card> firstToPlayInHandCards;
            List<Card> secondToPlayInHandCards;

            if (this.FirstToPlay == PlayerPosition.FirstPlayer)
            {
                firstToPlayInHand = this.firstPlayer;
                firstToPlayInHandCards = this.firstPlayerCards;
                secondToPlayInHand = this.secondPlayer;
                secondToPlayInHandCards = this.secondPlayerCards;
            }
            else
            {
                firstToPlayInHand = this.secondPlayer;
                firstToPlayInHandCards = this.secondPlayerCards;
                secondToPlayInHand = this.firstPlayer;
                secondToPlayInHandCards = this.firstPlayerCards;
            }


            PlayerTurnContext handContext = new PlayerTurnContext(this.state, deck.TrumpCard, deck.CardsLeft(), this.firstPlayerPoints, this.secondPlayerPoints,this.ClosedByPlayer);

            PlayerMove firstToPlayMove = null;

            do
            {
                firstToPlayMove = this.FirstToPlayInHandMove(firstToPlayInHand, handContext);
            }
            while (firstToPlayMove.Type != PlayerMoveType.PlayCard);

            handContext.FirstPlayedCard = firstToPlayMove.PlayedCard;
            handContext.FirstPlayerCardAnnounce = firstToPlayMove.PlayerAnnounce;

            PlayerMove secondToPlayMove = secondToPlayInHand.GetTurn(handContext);

            handContext.SecondPlayedCard = secondToPlayMove.PlayedCard;

            //ovo mozes u poseban metod
            if (firstToPlayInHand == this.firstPlayer)
            {
                this.FirstPlayerCard = firstToPlayMove.PlayedCard;
                this.FirstPlayerAnnounce = firstToPlayMove.PlayerAnnounce;
                this.SecondPlayerCard = secondToPlayMove.PlayedCard;
                this.SecondPlayerAnnounce = secondToPlayMove.PlayerAnnounce;
            }
            else
            {
                this.FirstPlayerCard = secondToPlayMove.PlayedCard;
                this.FirstPlayerAnnounce = secondToPlayMove.PlayerAnnounce;
                this.SecondPlayerCard = firstToPlayMove.PlayedCard;
                this.SecondPlayerAnnounce = firstToPlayMove.PlayerAnnounce;
            }

            this.HandWinner = this.WhoWinTheHand();

            firstToPlayInHand.EndTurn(handContext);
            secondToPlayInHand.EndTurn(handContext);
        }

        private PlayerMove FirstToPlayInHandMove(BasePlayer player, PlayerTurnContext handContext)
        {
            PlayerMove firstToPlayAction = player.GetTurn(handContext);

            if (firstToPlayAction.Type == PlayerMoveType.CloseGame)
            {
                handContext.State.Close();
                handContext.State = new FinalRoundState();
                this.state = new FinalRoundState();

                if (player == this.firstPlayer)
                {
                    this.GameClosedInThisHand = PlayerPosition.FirstPlayer;
                }
                else
                    this.GameClosedInThisHand = PlayerPosition.SecondPlayer;
            }

            if (firstToPlayAction.Type == PlayerMoveType.ChangeTrump)
            {
                Card newTrumpCard = new Card(CardType.Nine, deck.TrumpCard.Suit);
                Card oldTrumCard = deck.TrumpCard;
                handContext.TrumpCard = newTrumpCard;
                this.deck.ChangeTrumpCard(newTrumpCard);

                if (player == firstPlayer)
                {
                    this.firstPlayerCards.Remove(newTrumpCard);
                    this.firstPlayerCards.Add(oldTrumCard);
                    this.firstPlayer.AddCard(oldTrumCard);
                }
                else
                {
                    this.secondPlayerCards.Remove(newTrumpCard);
                    this.secondPlayerCards.Add(oldTrumCard);
                    this.secondPlayer.AddCard(oldTrumCard);
                }
            }

            return firstToPlayAction;
        }

        private PlayerPosition WhoWinTheHand()
        {
            if (this.FirstPlayerCard.Suit == this.SecondPlayerCard.Suit)
            {
                if (this.FirstPlayerCard.Value() > this.SecondPlayerCard.Value())
                    return PlayerPosition.FirstPlayer;
                else
                    return PlayerPosition.SecondPlayer;
            }

            if (this.FirstToPlay == PlayerPosition.FirstPlayer)
            {
                if (this.SecondPlayerCard.Suit == this.deck.TrumpCard.Suit)
                    return PlayerPosition.SecondPlayer;
                else
                    return PlayerPosition.FirstPlayer;
            }
            else
            {
                if (this.FirstPlayerCard.Suit == this.deck.TrumpCard.Suit)
                    return PlayerPosition.FirstPlayer;
                else
                    return PlayerPosition.SecondPlayer;
            }
        }
    }
}
