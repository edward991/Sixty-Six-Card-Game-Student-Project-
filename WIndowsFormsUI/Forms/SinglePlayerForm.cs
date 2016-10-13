using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using WIndowsFormsUI.Players;
using System.Drawing.Imaging;
using System.Threading;
using GameEngine;
using GameEngine.Players;
using GameEngine.Cards;
using ArtificialIntelligence;
using WIndowsFormsUI.Properties;

namespace WIndowsFormsUI.Forms
{
    public partial class SinglePlayerForm : Form
    {
        private HumanUIPlayer userPlayer;
        private ComputerUIPlayer computerPlayer;
        private UIGame game;

        private List<CardControl> userCardsControls;
        private List<CardControl> computerCardsControls;

        private Image userCharacterImage;
        private Image computerCharacterImage;

        private Dictionary<Card, Bitmap> cardsImages;
        private Bitmap cardBackImage;
        private Bitmap deckImage;

        private int cardsInDeck;

        private Point location = Point.Empty;

        private Point humanPlayedCardLocation = new Point(570, 210);
        private Point computerPlayedCardLocation = new Point(450, 180);

        GameLevel level;
        private bool enableSound;
        private bool enableNotifications;

        public bool isMyTurn;
        public Card PlayedCard { get; set; }
        public bool CloseGame { get; set; }
        public bool ChangeTrump { get; set; }

        public SinglePlayerForm(string playerName, GameLevel level, CardDeckType deckType, CardBackType cardBack, bool enableSound, bool enableNotifications, int characterImage)
        {
            InitializeComponent();

            InitializeCardControls();

            ChooseCharactersImages(characterImage);

            this.level = level;

            if (deckType == CardDeckType.Classic)
            {
                MappClassicCards();
            }
            if (deckType == CardDeckType.Simple)
            {
                MappSimpleCards();
            }

            if (cardBack == CardBackType.Blue)
            {
                cardBackImage = new Bitmap(Resources.blueBack);
                deckImage = new Bitmap(Resources.deckBlue);
            }
            if (cardBack == CardBackType.Red)
            {
                cardBackImage = new Bitmap(Resources.redBack);
                deckImage = new Bitmap(Resources.deck);
            }

            this.cardsInDeck = 24;
            DrawDeck(24);

            userNameLabel.Text = playerName;

            this.enableSound = enableSound;
            this.enableNotifications = enableNotifications;

            this.computerPlayerMessageCloud.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
        }

        #region DrawingBackground

        private void DrawRoundedRectangle(Graphics gfx, Rectangle Bounds, int CornerRadius, Pen DrawPen, Color FillColor)
        {
            int strokeOffset = Convert.ToInt32(Math.Ceiling(DrawPen.Width));
            Bounds = Rectangle.Inflate(Bounds, -strokeOffset, -strokeOffset);

            DrawPen.EndCap = DrawPen.StartCap = LineCap.Round;

            GraphicsPath gfxPath = new GraphicsPath();
            gfxPath.AddArc(Bounds.X, Bounds.Y, CornerRadius, CornerRadius, 180, 90);
            gfxPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y, CornerRadius, CornerRadius, 270, 90);
            gfxPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
            gfxPath.AddArc(Bounds.X, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
            gfxPath.CloseAllFigures();

            gfx.FillPath(new SolidBrush(FillColor), gfxPath);
            gfx.DrawPath(DrawPen, gfxPath);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

            Rectangle rectangle1 = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height / 2 + 1);
            Rectangle rectangle2 =
                new Rectangle(0, ClientRectangle.Height / 2, ClientRectangle.Width, ClientRectangle.Height - ClientRectangle.Height / 2);


            using (

               LinearGradientBrush brush = new LinearGradientBrush(rectangle1,
                                                              Color.FromArgb(2, 55, 55),
                                                              Color.FromArgb(6, 90, 90),
                                                              90F)
                                                              )

            {
                e.Graphics.FillRectangle(brush, rectangle1);
            }
            using (
                LinearGradientBrush brush = new LinearGradientBrush(rectangle2,
                                                              Color.FromArgb(6, 90, 90),
                                                              Color.FromArgb(2, 55, 55),
                                                              90F)
                                                              )

            {
                e.Graphics.FillRectangle(brush, rectangle2);
            }


            DrawRoundedRectangle(e.Graphics, new Rectangle(111, 197, 115, 161), 15, new Pen(Color.FromArgb(35, 120, 120), 3), Color.Transparent);
            //DrawRoundedRectangle(e.Graphics, new Rectangle(humanPlayedCardLocation.X, humanPlayedCardLocation.Y, 115, 161), 15, new Pen(Color.FromArgb(35, 120, 120), 3), Color.Transparent);
            //DrawRoundedRectangle(e.Graphics, new Rectangle(computerPlayedCardLocation.X, computerPlayedCardLocation.Y, 115, 161), 15, new Pen(Color.FromArgb(35, 120, 120), 3), Color.Transparent);
        }

        #endregion

        protected void SinglePlayerForm_Load(object sender, EventArgs e)
        {
            this.RestartRound();
        }

        private void RestartRound()
        {
            foreach (var item in this.userCardsControls)
            {
                item.PictureBox.Image = null;
                item.Empty = true;
                item.PictureBox.Location = item.DefaultLocation;
            }

            foreach (var item in this.computerCardsControls)
            {
                item.PictureBox.Image = null;
                item.Empty = true;
                item.PictureBox.Location = item.DefaultLocation;
                item.Card = null;
            }

            this.trumpCardPictureBox.Image = null;

            this.deckPictureBox.BringToFront();
            this.cardsLeftLabel.BringToFront();

            this.cardsLeftToolStrip.Text = this.trumpCardToolStrip.Text = "";

            this.userPlayerMessageCloud.Visible = false;
            this.userPlayerMessageLabel.Visible = false;
            this.computerPlayerMessageCloud.Visible = false;
            this.computerPlayerMessageLabel.Visible = false;

            cardsInDeck = 24;
            this.cardsLeftLabel.Text = "24";

            this.CloseGame = false;
        }

        #region Methods for constructor

        private void MappClassicCards()
        {
            cardsImages = new Dictionary<Card, Bitmap>();

            cardsImages.Add(new Card(CardType.Ace, CardSuit.Club), Resources.AceClub1);
            cardsImages.Add(new Card(CardType.Ace, CardSuit.Diamond), Resources.AceDiamond1);
            cardsImages.Add(new Card(CardType.Ace, CardSuit.Heart), Resources.AceHeart1);
            cardsImages.Add(new Card(CardType.Ace, CardSuit.Spade), Resources.AceSpade1);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Club), Resources.JackClub1);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Diamond), Resources.JackDiamond1);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Heart), Resources.JackHeart1);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Spade), Resources.JackSpade1);
            cardsImages.Add(new Card(CardType.King, CardSuit.Club), Resources.KingClub1);
            cardsImages.Add(new Card(CardType.King, CardSuit.Diamond), Resources.KingDiamond1);
            cardsImages.Add(new Card(CardType.King, CardSuit.Heart), Resources.KingHeart1);
            cardsImages.Add(new Card(CardType.King, CardSuit.Spade), Resources.KingSpade1);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Club), Resources.NineClub1);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Diamond), Resources.NineDiamond1);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Heart), Resources.NineHeart1);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Spade), Resources.NineSpade1);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Club), Resources.QueenClub1);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Diamond), Resources.QueenDiamond1);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Heart), Resources.QueenHeart1);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Spade), Resources.QueenSpade1);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Club), Resources.TenClub1);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Diamond), Resources.TenDiamond1);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Heart), Resources.TenHeart1);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Spade), Resources.TenSpade1);
        }

        private void MappSimpleCards()
        {
            this.cardsImages = new Dictionary<Card, Bitmap>();

            cardsImages.Add(new Card(CardType.Ace, CardSuit.Club), Resources.AceClub);
            cardsImages.Add(new Card(CardType.Ace, CardSuit.Diamond), Resources.AceDiamond);
            cardsImages.Add(new Card(CardType.Ace, CardSuit.Heart), Resources.AceHeart);
            cardsImages.Add(new Card(CardType.Ace, CardSuit.Spade), Resources.AceSpade);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Club), Resources.JackClub);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Diamond), Resources.JackDiamond);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Heart), Resources.JackHeart);
            cardsImages.Add(new Card(CardType.Jack, CardSuit.Spade), Resources.JackSpade);
            cardsImages.Add(new Card(CardType.King, CardSuit.Club), Resources.KingClub);
            cardsImages.Add(new Card(CardType.King, CardSuit.Diamond), Resources.KingDiamond);
            cardsImages.Add(new Card(CardType.King, CardSuit.Heart), Resources.KingHeart);
            cardsImages.Add(new Card(CardType.King, CardSuit.Spade), Resources.KingSpade);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Club), Resources.NineClub);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Diamond), Resources.NineDiamond);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Heart), Resources.NineHeart);
            cardsImages.Add(new Card(CardType.Nine, CardSuit.Spade), Resources.NineSpade);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Club), Resources.QueenClub);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Diamond), Resources.QueenDiamond);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Heart), Resources.QueenHeart);
            cardsImages.Add(new Card(CardType.Queen, CardSuit.Spade), Resources.QueenSpade);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Club), Resources.TenClub);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Diamond), Resources.TenDiamond);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Heart), Resources.TenHeart);
            cardsImages.Add(new Card(CardType.Ten, CardSuit.Spade), Resources.TenSpade);
        }

        private void InitializeCardControls()
        {
            userCardsControls = new List<CardControl>();
            computerCardsControls = new List<CardControl>();

            this.userCardsControls.Add(new CardControl(pictureBox1, pictureBox1.Location));
            this.userCardsControls.Add(new CardControl(pictureBox2, pictureBox2.Location));
            this.userCardsControls.Add(new CardControl(pictureBox3, pictureBox3.Location));
            this.userCardsControls.Add(new CardControl(pictureBox4, pictureBox4.Location));
            this.userCardsControls.Add(new CardControl(pictureBox5, pictureBox5.Location));
            this.userCardsControls.Add(new CardControl(pictureBox6, pictureBox6.Location));

            this.computerCardsControls.Add(new CardControl(pictureBox7, pictureBox7.Location));
            this.computerCardsControls.Add(new CardControl(pictureBox8, pictureBox8.Location));
            this.computerCardsControls.Add(new CardControl(pictureBox9, pictureBox9.Location));
            this.computerCardsControls.Add(new CardControl(pictureBox10, pictureBox10.Location));
            this.computerCardsControls.Add(new CardControl(pictureBox11, pictureBox11.Location));
            this.computerCardsControls.Add(new CardControl(pictureBox12, pictureBox12.Location));
        }

        private void ChooseCharactersImages(int characterImage)
        {
            switch (characterImage)
            {
                case 1:
                    userCharacterImage = Resources.Character1;
                    break;
                case 2:
                    userCharacterImage = Resources.Character2;
                    break;
                case 3:
                    userCharacterImage = Resources.Character3;
                    break;
                case 4:
                    userCharacterImage = Resources.Character4;
                    break;
                case 5:
                    userCharacterImage = Resources.Character5;
                    break;
                default:
                    break;
            }
            userCharacterPictureBox.Image = userCharacterImage;

            computerCharacterImage = Resources.ComputerCharacter;
            computerCharacterPictureBox.Image = Resources.ComputerCharacter;

        }

        #endregion

        private void StartGame()
        {
            userPlayer = new HumanUIPlayer(this.Name);

            computerPlayer = new ComputerUIPlayer(this.level);

            userPlayer.DrawCard += HumanPlayer_DrawCard;
            userPlayer.GameClosed += HumanPlayer_GameClosed;
            userPlayer.TrumpChanged += HumanPlayer_TrumpChanged;
            userPlayer.MyTurn += HumanPlayer_MyTurn;
            userPlayer.NoOneTurn += NoOneTurn;
            userPlayer.Announced += HumanPlayer_Announced;
            userPlayer.InvalidMove += HumanPlayer_InvalidMove;

            computerPlayer.DrawCard += ComputerPlayer_DrawCard;
            computerPlayer.MyTurn += ComputerPlayer_MyTurn;
            computerPlayer.NoOneTurn += NoOneTurn;
            computerPlayer.PlayedCard += ComputerPlayer_PlayedCard;
            computerPlayer.TrumpChanged += ComputerPlayer_TrumpChanged;
            computerPlayer.GameClosed += ComputerPlayer_GameClosed;

            game = new UIGame(userPlayer, computerPlayer, PlayerPosition.FirstPlayer);

            game.UpdatePoints += Game_UpdatePoints;
            game.UpdateRoundPoints += Game_UpdateRoundPoints;
            game.RestartRound += Game_RestartRound;
            game.PrintRoundInfo += Game_PrintRoundInfo;

            Task.Run(() => this.game.Start());
        }

        #region Events

        private void HumanPlayer_DrawCard(object sender, Card e)
        {
            Invoke((MethodInvoker)delegate () { DrawUserCard(e); });
        }

        private void ComputerPlayer_PlayedCard(object sender, Tuple<Card, int, int, int> e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.ComputerPlayCardToCenter(e.Item1, e.Item2);

                if (e.Item3 != -1)
                {
                    /*
                    computerCardsControls[e.Item3].Card = (e.Item1.Type == CardType.Queen) ?
                        new Card(CardType.King, e.Item1.Suit) : new Card(CardType.Queen, e.Item1.Suit);
                    computerCardsControls[e.Item3].PictureBox.Image = cardsImages[computerCardsControls[e.Item3].Card];
                    */

                    computerRoundPointsLabel.Text =
                    (Int32.Parse(computerRoundPointsLabel.Text.ToString()) + e.Item4).ToString();

                    this.UpdateNotificationLabel(
                        "Computer\nannounce " + e.Item4.ToString(), this.computerPlayerMessageCloud, this.computerPlayerMessageLabel);
                }
            });
        }

        private void ComputerPlayer_TrumpChanged(object sender, Tuple<Card, int> e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.UpdateNotificationLabel(
                    "Computer changes\nthe trump card", this.computerPlayerMessageCloud, this.computerPlayerMessageLabel);

                this.DrawTrumpCard(new Card(CardType.Nine, e.Item1.Suit));

                computerCardsControls[e.Item2].Empty = true;

                this.cardsInDeck++;
            });
        }

        private void HumanPlayer_InvalidMove(object sender, Card e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                if (e != null)
                {
                    BackCardOnItsPlace(e);
                }

                this.PlaySound(Resources.negative_2);
                this.UpdateNotificationLabel("Invalid move!!!", this.userPlayerMessageCloud, this.userPlayerMessageLabel);
            });
        }

        private void HumanPlayer_Announced(object sender, int e)
        {
            Invoke((MethodInvoker)delegate ()
            {
               this.userRoundPointsLabel.Text = (Int32.Parse(userRoundPointsLabel.Text) + e).ToString();
               this.UpdateNotificationLabel(
                   this.userNameLabel.Text + " announce \n" + e.ToString(), this.userPlayerMessageCloud, this.userPlayerMessageLabel);
            });
        }

        private void ComputerPlayer_MyTurn(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.userGamePointsLabel.BackColor = Color.FromArgb(150, 5, 35, 35);
                this.userNameLabel.BackColor = Color.FromArgb(150, 240, 140, 0);
                this.userGamePointsLabel.ForeColor = Color.FromArgb(150, Color.WhiteSmoke);
                this.userNameLabel.ForeColor = Color.FromArgb(150, Color.WhiteSmoke);
                this.userCharacterPictureBox.Image = ChangeOpacity(userCharacterImage, 0.59F);

                foreach (var control in this.userCardsControls)
                {
                    if (!control.Empty && !control.InCenter)
                    {
                        control.PictureBox.Image = ChangeOpacity(cardsImages[control.Card], 0.5F);
                    }
                }
            });
        }

        private void HumanPlayer_MyTurn(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.isMyTurn = true;
                this.computerGamePointsLabel.BackColor = Color.FromArgb(150, 5, 35, 35);
                this.computerNameLabel.BackColor = Color.FromArgb(150, 240, 140, 0);
                this.computerGamePointsLabel.ForeColor = Color.FromArgb(150, Color.WhiteSmoke);
                this.computerNameLabel.ForeColor = Color.FromArgb(150, Color.WhiteSmoke);
                this.computerCharacterPictureBox.Image = ChangeOpacity(computerCharacterImage, 0.59F);

                foreach (var control in this.computerCardsControls)
                {
                    if (!control.Empty && !control.InCenter)
                    {
                        if (control.Card == null)
                        {
                            control.PictureBox.Image = ChangeOpacity(cardBackImage, 0.5F);
                        }
                        else
                        {
                            control.PictureBox.Image = ChangeOpacity(this.cardsImages[control.Card], 0.5F);
                        }
                    }
                }
            });
        }

        private void NoOneTurn(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.isMyTurn = false;
                this.computerGamePointsLabel.BackColor = Color.FromArgb(5, 35, 35);
                this.computerNameLabel.BackColor = Color.FromArgb(240, 140, 0);
                this.computerGamePointsLabel.ForeColor = Color.WhiteSmoke;
                this.computerNameLabel.ForeColor = Color.WhiteSmoke;
                this.userGamePointsLabel.BackColor = Color.FromArgb(5, 35, 35);
                this.userNameLabel.BackColor = Color.FromArgb(240, 140, 0);
                this.userGamePointsLabel.ForeColor = Color.WhiteSmoke;
                this.userNameLabel.ForeColor = Color.WhiteSmoke;
                this.computerCharacterPictureBox.Image = computerCharacterImage;
                this.userCharacterPictureBox.Image = userCharacterImage;

                foreach (var control in this.computerCardsControls)
                {
                    if (!control.Empty && !control.InCenter)
                    {
                        if (control.Card == null)
                        {
                            control.PictureBox.Image = cardBackImage;
                        }
                        else
                        {
                            control.PictureBox.Image = this.cardsImages[control.Card];
                        }
                    }
                }
                foreach (var control in this.userCardsControls)
                {
                    if (!control.Empty && !control.InCenter)
                    {
                        control.PictureBox.Image = cardsImages[control.Card];
                    }
                }
            });
        }

        private void Game_PrintRoundInfo(object sender, Tuple<Card, bool> e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                if (this.cardsInDeck == 0)
                    this.DrawEmptyDeck();

                if (e.Item2)
                {
                    this.DrawTrumpCard(e.Item1);
                    this.PrintTrumpCardSuit(e.Item1.Suit);
                }
            });
        }

        private void HumanPlayer_TrumpChanged(object sender, Card e)
        {
            Invoke((MethodInvoker)delegate ()
           {
               this.UpdateNotificationLabel(
                   this.userNameLabel.Text + " changes\nthe trump card", this.userPlayerMessageCloud, this.userPlayerMessageLabel);
               this.isMyTurn = false;
               this.DrawTrumpCard(new Card(CardType.Nine, e.Suit));

               int index = userCardsControls.FindIndex(x => x.Card.Equals(new Card(CardType.Nine, e.Suit)));
               userCardsControls[index].Empty = true;

               this.ChangeTrump = false;
               this.cardsInDeck++;
           });
        }

        private void ComputerPlayer_GameClosed(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.UpdateNotificationLabel(
                    "Computer closed\nthe deck", this.computerPlayerMessageCloud, this.computerPlayerMessageLabel);
                this.isMyTurn = false;
                this.cardsLeftLabel.Text = "🔒";
                this.CloseGame = true;
            });
        }

        private void HumanPlayer_GameClosed(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.UpdateNotificationLabel(
                    this.userNameLabel.Text + " closed\nthe deck", this.userPlayerMessageCloud, this.userPlayerMessageLabel);
                this.isMyTurn = false;
                this.cardsLeftLabel.Text = "🔒";
                this.CloseGame = true;
            });
        }

        private void Game_RestartRound(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate () { RestartRound(); });
        }

        private void Game_UpdateRoundPoints(object sender, Tuple<int, int, bool> e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.isMyTurn = false;

                this.userRoundPointsLabel.Text = e.Item1.ToString();

                this.computerRoundPointsLabel.Text = e.Item2.ToString();

                this.ClearCardsFromCenter();

                if (e.Item3)
                {
                    if (Convert.ToInt32(this.userRoundPointsLabel.Text) > Convert.ToInt32(this.computerRoundPointsLabel.Text))
                        this.UpdateNotificationLabel(this.userNameLabel.Text + " wins\n the round", this.userPlayerMessageCloud, this.userPlayerMessageLabel);
                    else
                        this.UpdateNotificationLabel("Computer wins\n the round", this.computerPlayerMessageCloud, this.computerPlayerMessageLabel);
                }
            });
        }

        private void Game_UpdatePoints(object sender, Tuple<int, int, bool> e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.userGamePointsLabel.Text = e.Item1.ToString();
                this.computerGamePointsLabel.Text = e.Item2.ToString();
                if (e.Item3)
                {
                    if (Convert.ToInt32(this.userGamePointsLabel.Text) > Convert.ToInt32(this.computerGamePointsLabel.Text))
                        this.UpdateNotificationLabel(this.userNameLabel.Text + "wins\n the game", this.userPlayerMessageCloud, this.userPlayerMessageLabel);
                    else
                        this.UpdateNotificationLabel("Computer wins\n the game", this.computerPlayerMessageCloud, this.computerPlayerMessageLabel);

                    this.UpdateStats(e.Item1, e.Item2);

                    Thread.Sleep(2000);
                    this.Close();
                    //MainMenuForm mmf = new MainMenuForm();
                    //mmf.Show();
                }
            });
        }

        private void ComputerPlayer_DrawCard(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate () { this.DrawComputerCard(); });
        }

        #endregion

        #region Updating labels

        private void PrintTrumpCardSuit(CardSuit suit)
        {
            char c;
            switch (suit)
            {
                case CardSuit.Club:
                    c = '♣';
                    break;
                case CardSuit.Diamond:
                    c = '♦';
                    break;
                case CardSuit.Spade:
                    c = '♠';
                    break;
                case CardSuit.Heart:
                    c = '♥';
                    break;
                default:
                    c = ' ';
                    break;
            }

            trumpColorLabel.Text = c.ToString();
            trumpCardToolStrip.Text = "Trump color : " + c.ToString();
        }

        private void UpdateNotificationLabel(String message, PictureBox messageCloud, Label messageLabel)
        {
            if (this.enableNotifications)
            {

                ThreadPool.QueueUserWorkItem(delegate
                {
                    Invoke(new Action(delegate
                    {
                        messageCloud.Visible = true;
                        messageLabel.Visible = true;
                        messageLabel.Text = message;
                    }
                            ));
                    Thread.Sleep(1200);
                    Invoke(new Action(delegate
                    {
                        messageCloud.Visible = false;
                        messageLabel.Visible = false;
                    }
                            ));
                });

            }
        }

        #endregion

        #region Mouse moving and clicking events

        protected void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.isMyTurn)
            {
                if (((PictureBox)sender).Image != null)
                {
                    this.location = new Point(e.X, e.Y);
                    this.Controls.SetChildIndex((PictureBox)sender, 0);
                    this.PlaySound(WIndowsFormsUI.Properties.Resources.cardPlace1);

                    foreach (var control in this.userCardsControls)
                    {
                        if (control.PictureBox != (PictureBox)sender && !control.Empty)
                        {
                            control.PictureBox.Image = ChangeOpacity(control.PictureBox.Image, 0.5F);
                        }
                    }
                }
            }
        }

        protected void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (location != Point.Empty && this.isMyTurn)
            {
                this.panel1.BringToFront();

                Point newLocation = ((PictureBox)sender).Location;

                newLocation.X += e.X - location.X;
                newLocation.Y += e.Y - location.Y;

                if (newLocation.X < 230)
                    newLocation.X = 230;
                if (newLocation.Y < 188)
                    newLocation.Y = 188;
                
                ((PictureBox)sender).Location = newLocation;
            }
        }

        protected void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.isMyTurn)
            {
                int index = userCardsControls.FindIndex(x => x.PictureBox.Equals(sender));
                CardControl userControl = userCardsControls[index];
                location = Point.Empty;
                Point mouseLocation = new Point(e.X, e.Y);
                if (userControl.PictureBox.Bounds.IntersectsWith(new Rectangle(humanPlayedCardLocation, new Size(150, 100))))
                {
                    userControl.PictureBox.Location = this.humanPlayedCardLocation;
                    userControl.InCenter = true;
                    userControl.Empty = true;

                    this.userPlayer.Move(
                        new PlayerMove(PlayerMoveType.PlayCard, userCardsControls[index].Card, Announce.None));
                    this.userPlayer.Signal.Set();
                    this.PlaySound(WIndowsFormsUI.Properties.Resources.cardSlide6);
                }
                else
                    userControl.PictureBox.Location = userCardsControls[index].DefaultLocation;

                foreach (var item in userCardsControls)
                {
                    if (item.PictureBox != (PictureBox)sender && !item.Empty)
                    {
                        item.PictureBox.Image = cardsImages[item.Card];
                    }
                }
            }
        }

        protected void trumpCardPictureBox_Click(object sender, EventArgs e)
        {
            if (this.isMyTurn)
            {
                this.userPlayer.Move(new PlayerMove(PlayerMoveType.ChangeTrump, null, Announce.None));
                this.userPlayer.Signal.Set();
            }
        }

        protected void deckPictureBox_Click(object sender, EventArgs e)
        {
            if (this.isMyTurn)
            {
                this.userPlayer.Move(new PlayerMove(PlayerMoveType.CloseGame, null, Announce.None));
                this.userPlayer.Signal.Set();
            }
        }

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (isMyTurn)
            {
                int index = userCardsControls.FindIndex(x => x.PictureBox.Equals(sender));
                ThreadPool.QueueUserWorkItem(delegate
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        Invoke(new Action(delegate
                        {
                            userCardsControls[index].PictureBox.Location = 
                                new Point(userCardsControls[index].DefaultLocation.X, 
                                    userCardsControls[index].DefaultLocation.Y - i * 3);
                        }
                            ));
                        Thread.Sleep(10);
                    }
                });
            }
        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            if (isMyTurn)
            {
                int index = userCardsControls.FindIndex(x => x.PictureBox.Equals(sender));
                ThreadPool.QueueUserWorkItem(delegate
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        Invoke(new Action(delegate
                        {
                            userCardsControls[index].PictureBox.Location = 
                                new Point(userCardsControls[index].DefaultLocation.X, 
                                    userCardsControls[index].DefaultLocation.Y - 30 + i * 3);

                        }
                            ));
                        Thread.Sleep(10);
                    }
                });

            }
        }

        #endregion

        #region Drawing cards

        public static Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();
            return bmp;
        }

        private void DrawDeck(int cardsInDeck)
        {
            Bitmap bitmap = new Bitmap(129, 169);

            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                for (int i = 1; i < cardsInDeck; i++)
                {
                    gr.DrawImage(deckImage, 24 - i + 1, 24 - i + 1, 105, 147);
                }
            }

            deckPictureBox.Image = bitmap;

            if (cardsInDeck == 1)
            {
                this.trumpCardPictureBox.BringToFront();
            }
            else if (cardsInDeck == 0)
            {
                this.trumpCardPictureBox.Image = null;
            }

            if (this.CloseGame)
            {
                cardsLeftLabel.Text = "🔒";
                cardsLeftToolStrip.Text = "Cards left : 🔒";
            }
            else
            {
                cardsLeftLabel.Text = cardsInDeck.ToString();
                cardsLeftToolStrip.Text = "Cards left : " + cardsInDeck.ToString();
            }
        }

        public void DrawUserCard(Card card)
        {
            int index = -1;
            index = userCardsControls.FindIndex(x => x.Empty == true);

            if (index == -1)
                throw new Exception("Index exception");

            cardsInDeck--;
            DrawDeck(cardsInDeck);

            CardControl currentControl = userCardsControls[index];

            ThreadPool.QueueUserWorkItem(delegate
                {
                    for (int i = 1; i <= 15; i++)
                    {
                        Invoke(new Action(delegate
                        {
                            currentControl.PictureBox.Location =
                                new Point(currentControl.DefaultLocation.X, currentControl.DefaultLocation.Y - 30 + 2 * i);
                        }));
                        Thread.Sleep(10);
                    }
                });

            currentControl.PictureBox.Image = this.cardsImages[card];
            currentControl.Card = card;
            currentControl.Empty = false;
            currentControl.InCenter = false;
            this.PlaySound(WIndowsFormsUI.Properties.Resources.deal);
        }

        public void DrawComputerCard()
        {
            int index = -1;
            index = computerCardsControls.FindIndex(x => x.Empty == true);

            if (index == -1)
                throw new Exception("Index exception");


            cardsInDeck--;
            DrawDeck(cardsInDeck);

            CardControl currentControl = computerCardsControls[index];

            ThreadPool.QueueUserWorkItem(delegate
            {
                //lock (cardBackImage)
                //{ computerCardsControls[index].PictureBox.Image = cardBackImage; }
                currentControl.PictureBox.Image = cardBackImage;
                for (int i = 1; i <= 15; i++)
                {
                    float a;
                    Invoke(new Action(delegate
                    {
                        a = ((float)i) / 20.0F;
                        currentControl.PictureBox.Location = 
                            new Point(currentControl.DefaultLocation.X, currentControl.DefaultLocation.Y + 30 - 2 * i);
                    }));
                    Thread.Sleep(10);
                }
            });

            currentControl.PictureBox.Location = computerCardsControls[index].DefaultLocation;
            currentControl.Empty = false;
            currentControl.InCenter = false;
            this.PlaySound(WIndowsFormsUI.Properties.Resources.deal);
        }

        public void DrawTrumpCard(Card card)
        {
            Image image = new Bitmap(this.cardsImages[card]);
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.trumpCardPictureBox.Image = image;
        }

        public void DrawEmptyDeck()
        {
            this.deckPictureBox.Image = null;
            this.trumpCardPictureBox.BringToFront();
            this.cardsLeftLabel.Text = "0";
            this.trumpCardPictureBox.Image = null;
        }

        public void ComputerPlayCardToCenter(Card card, int index)
        {
            int i = 0, j = 0;
            for (i = 0; i < 6; i++)
            {
                if (computerCardsControls[i].Empty == false)
                    j++;
                if (index + 1 == j)
                    break;
            }

            CardControl currentControl = computerCardsControls[i];

            currentControl.PictureBox.Image = cardsImages[card];
            currentControl.PictureBox.Location = computerPlayedCardLocation;
            currentControl.PictureBox.BringToFront();
            currentControl.InCenter = true;
            currentControl.Empty = true;
            PlaySound(WIndowsFormsUI.Properties.Resources.cardSlide6);
        }

        public void BackCardOnItsPlace(Card card)
        {
            foreach (var item in userCardsControls)
            {
                if(!(item.PictureBox.Image==null))
                {
                    if(item.Card.Equals(card))
                    {
                        item.PictureBox.Location = item.DefaultLocation;
                        item.Empty = false;
                        item.InCenter = false;
                        break;
                    }
                }
            }
        }

        #endregion

        public void ClearCardsFromCenter()
        {
            for (int i = 0; i < 6; i++)
            {
                if (userCardsControls[i].InCenter)
                {
                    userCardsControls[i].InCenter = false;
                    userCardsControls[i].PictureBox.Image = null;
                }

                if (computerCardsControls[i].InCenter)
                {
                    computerCardsControls[i].InCenter = false;
                    computerCardsControls[i].PictureBox.Image = null;
                }
            }
        }

        public void PlaySound(UnmanagedMemoryStream sound)
        {
            if (enableSound)
            {
                SoundPlayer soundPlayer = new SoundPlayer();
                soundPlayer.Stream = sound;
                soundPlayer.Play();
            }
        }

        protected void SinglePlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure want to exit from game?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MainMenuForm mmf = new MainMenuForm();
                mmf.Show();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void deckPictureBox_Click_1(object sender, EventArgs e)
        {
            notificationlabel.Visible = false;
            StartGame();
            this.deckPictureBox.Click -= deckPictureBox_Click_1;
        }

        private void UpdateStats(int myRoundPoints, int computerRoundPoints)
        {
            if (this.level == GameLevel.Easy)
            {
                Settings.Default.totalGamesVersusEasy++;
                Settings.Default.totalRoundWinsVsEasy += myRoundPoints;
                Settings.Default.totalRoundLossesVsEasy += computerRoundPoints;

                if (myRoundPoints > computerRoundPoints)
                {
                    Settings.Default.totalWinsVersusEasy++;
                }
            }
            else if (this.level == GameLevel.Normal)
            {
                Settings.Default.totalGamesVersusNormal++;
                Settings.Default.totalRoundWinsVsNormal += myRoundPoints;
                Settings.Default.totalRoundLossesVsNormal += computerRoundPoints;

                if (myRoundPoints > computerRoundPoints)
                {
                    Settings.Default.totalWinsVersusNormal++;
                }
            }
            else if (this.level == GameLevel.Hard)
            {
                Settings.Default.totalGamesVersusHard++;
                Settings.Default.totalRoundWinsVsHard += myRoundPoints;
                Settings.Default.totalRoundLossesVsHard += computerRoundPoints;

                if (myRoundPoints > computerRoundPoints)
                {
                    Settings.Default.totalWinsVersusHard++;
                }
            }

            Settings.Default.lastUpdate = DateTime.Now;

            Settings.Default.Save();
        }
    }
}