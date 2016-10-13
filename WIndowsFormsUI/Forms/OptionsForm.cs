using ArtificialIntelligence;
using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using WIndowsFormsUI.Properties;


namespace WIndowsFormsUI.Forms
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        public string PlayerName
        {
            get
            { return this.nameTextBox.Text; }
        }

        public GameLevel Level
        {
            get
            {
                if (this.easyRadioButton.Checked)
                    return GameLevel.Easy;
                else if (this.normalRadioButton.Checked)
                    return GameLevel.Normal;
                else
                    return GameLevel.Hard;
            }
        }

        public CardDeckType DeckType
        {
            get
            {
                if (this.classicRadioButton.Checked)
                    return CardDeckType.Classic;
                else
                    return CardDeckType.Simple;
            }
        }

        public CardBackType CardsBack
        {
            get
            {
                if (this.redRadioButton.Checked)
                    return CardBackType.Red;
                else
                    return CardBackType.Blue;
            }
        }

        public bool Sound
        {
            get
            {
                if (this.soundCheckBox.Checked)
                    return true;
                else
                    return false;
            }
        }

        public bool Notifications
        {
            get
            {
                if (this.notificationCheckBox.Checked)
                    return true;
                else
                    return false;
            }
        }

        public int Character
        {
            get
            {
                if (this.radioButton1.Checked)
                    return 1;
                else if (this.radioButton2.Checked)
                    return 2;
                else if (this.radioButton3.Checked)
                    return 3;
                else if (this.radioButton4.Checked)
                    return 4;
                else
                    return 5;
            }
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            this.nameTextBox.Text = WIndowsFormsUI.Properties.Settings.Default.playerName;

            if (Settings.Default.level == GameLevel.Easy)
            {
                this.easyRadioButton.Checked = true;
            }
            else if (Settings.Default.level == GameLevel.Normal)
            {
                this.normalRadioButton.Checked = true;
            }
            else if (Settings.Default.level == GameLevel.Hard)
            {
                this.hardRadioButton.Checked = true;
            }

            if (Settings.Default.deckType == CardDeckType.Classic)
            {
                this.classicRadioButton.Checked = true;
            }
            else if (Settings.Default.deckType == CardDeckType.Simple)
            {
                this.simpleRadioButton.Checked = true;
            }

            if (Settings.Default.cardBack == CardBackType.Blue)
            {
                this.blueRadioButton.Checked = true;
            }
            else if (Settings.Default.cardBack == CardBackType.Red)
            {
                this.redRadioButton.Checked = true;
            }

            if (Settings.Default.sound == true)
            {
                this.soundCheckBox.Checked = true;
            }
            else
            {
                this.soundCheckBox.Checked = false;
            }

            if (Settings.Default.notifications == true)
            {
                this.notificationCheckBox.Checked = true;
            }
            else
            {
                this.notificationCheckBox.Checked = false;
            }

            switch (WIndowsFormsUI.Properties.Settings.Default.character)
            {
                case 1:
                    this.radioButton1.Checked = true;
                    break;
                case 2:
                    this.radioButton2.Checked = true;
                    break;
                case 3:
                    this.radioButton3.Checked = true;
                    break;
                case 4:
                    this.radioButton4.Checked = true;
                    break;
                case 5:
                    this.radioButton5.Checked = true;
                    break;
                default:
                    break;
            }
        }

        private void control_Click(object sender, EventArgs e)
        {
            this.PlaySound(WIndowsFormsUI.Properties.Resources.click_2);
         }

        private void PlaySound(UnmanagedMemoryStream sound)
        {
            if (this.Sound)
            {
                SoundPlayer soundPlayer = new SoundPlayer();
                soundPlayer.Stream = sound;
                soundPlayer.Play();
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect, // x-coordinate of upper-left corner
           int nTopRect, // y-coordinate of upper-left corner
           int nRightRect, // x-coordinate of lower-right corner
           int nBottomRect, // y-coordinate of lower-right corner
           int nWidthEllipse, // height of ellipse
           int nHeightEllipse // width of ellipse
        );
    }
}
