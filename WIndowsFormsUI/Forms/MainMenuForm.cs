using ArtificialIntelligence;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Windows.Forms;
using System.Xml;

namespace WIndowsFormsUI.Forms
{

    public partial class MainMenuForm : Form
    {
        private string settings = "settings.xml";
        private XmlDocument settingsXml = new XmlDocument();

        private string playerName;
        private CardDeckType deckType;
        private CardBackType cardsBack;
        private GameLevel level;
        private bool enableSound;
        private bool enableNotifications;
        private int character;

        private SoundPlayer soundPlayer;

        public MainMenuForm()
        {
            InitializeComponent();

            this.soundPlayer = new SoundPlayer();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

            Rectangle rectangle1 = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height / 2 + 1);
            Rectangle rectangle2 = new Rectangle(0, ClientRectangle.Height / 2, ClientRectangle.Width, ClientRectangle.Height - ClientRectangle.Height / 2);

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
        }

        private void MainMenuForm_Load(Object sender, EventArgs e)
        {
            if (!File.Exists(settings))
            {

                XmlElement element = settingsXml.CreateElement("settings");
                settingsXml.AppendChild(element);

                XmlElement tempChild;

                tempChild = settingsXml.CreateElement("playerName");
                tempChild.InnerText = "Default name";
                element.AppendChild(tempChild);

                tempChild = settingsXml.CreateElement("level");
                tempChild.InnerText = GameLevel.Easy.ToString();
                element.AppendChild(tempChild);

                tempChild = settingsXml.CreateElement("deck");
                tempChild.SetAttribute("deckType", CardDeckType.Classic.ToString());
                tempChild.SetAttribute("cardsBack", CardBackType.Red.ToString());
                element.AppendChild(tempChild);

                tempChild = settingsXml.CreateElement("sound");
                tempChild.SetAttribute("enable", "true");
                element.AppendChild(tempChild);

                tempChild = settingsXml.CreateElement("notifications");
                tempChild.SetAttribute("enable", "true");
                element.AppendChild(tempChild);

                tempChild = settingsXml.CreateElement("character");
                tempChild.SetAttribute("image", "1");
                element.AppendChild(tempChild);

            }
            else
            {
                this.settingsXml.Load(this.settings);

                XmlElement element = settingsXml.DocumentElement;

                this.playerName = element.ChildNodes[0].InnerText;

                this.level = (GameLevel)Enum.Parse(typeof(GameLevel), element.ChildNodes[1].InnerText);

                this.deckType = (CardDeckType)Enum.Parse(typeof(CardDeckType), element.ChildNodes[2].Attributes[0].Value);

                this.cardsBack = (CardBackType)Enum.Parse(typeof(CardBackType), element.ChildNodes[2].Attributes[1].Value);

                if (element.ChildNodes[3].Attributes[0].Value == "true")
                    this.enableSound = true;
                else
                    this.enableSound = false;

                if (element.ChildNodes[4].Attributes[0].Value == "true")
                    this.enableNotifications = true;
                else
                    this.enableNotifications = false;

                this.character = int.Parse(element.ChildNodes[5].Attributes[0].Value);
            }
        }

        private void PlaySound(UnmanagedMemoryStream sound)
        {
            if (this.enableSound)
            {
                soundPlayer.Stream = sound;
                soundPlayer.Play();
            }
        }

        private void playButton_MouseEnter(object sender, EventArgs e)
        {
            playButton.BackColor = Color.FromArgb(200, 5, 35, 35);
            this.PlaySound(WIndowsFormsUI.Properties.Resources.misc_menu1);
        }

        private void playButton_MouseLeave(object sender, EventArgs e)
        {
            playButton.BackColor = Color.FromArgb(5, 35, 35);
        }

        private void optionsButton_MouseEnter(object sender, EventArgs e)
        {
            optionsButton.BackColor = Color.FromArgb(200, 5, 35, 35);
            this.PlaySound(WIndowsFormsUI.Properties.Resources.misc_menu1);
        }

        private void optionsButton_MouseLeave(object sender, EventArgs e)
        {
            optionsButton.BackColor = Color.FromArgb(5, 35, 35);
        }

        private void rulesButton_MouseEnter(object sender, EventArgs e)
        {
            rulesButton.BackColor = Color.FromArgb(200, 5, 35, 35);
            this.PlaySound(WIndowsFormsUI.Properties.Resources.misc_menu1);
        }

        private void rulesButton_MouseLeave(object sender, EventArgs e)
        {
            rulesButton.BackColor = Color.FromArgb(5, 35, 35);
        }

        private void exitButton_MouseEnter(object sender, EventArgs e)
        {
            exitButton.BackColor = Color.FromArgb(200, 5, 35, 35);
            this.PlaySound(WIndowsFormsUI.Properties.Resources.misc_menu1);
        }

        private void exitButton_MouseLeave(object sender, EventArgs e)
        {
            exitButton.BackColor = Color.FromArgb(5, 35, 35);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure want to exit from game?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            SinglePlayerForm singlePlayerForm =
                new SinglePlayerForm(this.playerName, this.level, this.deckType, this.cardsBack, this.enableSound, this.enableNotifications, this.character);
            singlePlayerForm.Show();
            this.Hide();
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            OptionsForm of = new OptionsForm(settingsXml);
            if (of.ShowDialog() == DialogResult.OK)
            {
                this.playerName = of.PlayerName;
                this.settingsXml.DocumentElement.ChildNodes[0].InnerText = of.PlayerName;

                this.level = of.Level;
                this.settingsXml.DocumentElement.ChildNodes[1].InnerText = of.Level.ToString();

                this.deckType = of.DeckType;
                this.settingsXml.DocumentElement.ChildNodes[2].Attributes[0].Value = of.DeckType.ToString();

                this.cardsBack = of.CardsBack;
                this.settingsXml.DocumentElement.ChildNodes[2].Attributes[1].Value = of.CardsBack.ToString();

                if (of.Sound)
                {
                    this.enableSound = true;
                    this.settingsXml.DocumentElement.ChildNodes[3].Attributes[0].Value = "true";
                }
                else
                {
                    this.enableSound = false;
                    this.settingsXml.DocumentElement.ChildNodes[3].Attributes[0].Value = "false";
                }

                if (of.Notifications)
                {
                    this.enableNotifications = true;
                    this.settingsXml.DocumentElement.ChildNodes[4].Attributes[0].Value = "true";
                }
                else
                {
                    this.enableNotifications = false;
                    this.settingsXml.DocumentElement.ChildNodes[4].Attributes[0].Value = "false";
                }

                this.character = of.Character;
                this.settingsXml.DocumentElement.ChildNodes[5].Attributes[0].Value = of.Character.ToString();

                this.settingsXml.Save(settings);
            }
        }

        private void rulesButton_Click(object sender, EventArgs e)
        {
            GameRulesForm grf = new GameRulesForm();
            grf.ShowDialog();
        }
    }
}
