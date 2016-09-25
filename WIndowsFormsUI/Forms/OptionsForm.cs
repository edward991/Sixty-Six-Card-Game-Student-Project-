using ArtificialIntelligence;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;


namespace WIndowsFormsUI.Forms
{
    public partial class OptionsForm : Form
    {
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

        XmlDocument settingsXml;

        public OptionsForm(XmlDocument settingsXml)
        {
            InitializeComponent();
            this.settingsXml = settingsXml;
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

        private void DrawBackgroud(Object sender, PaintEventArgs args)
        {
            /*   
            Rectangle rectangle1=new Rectangle(0,0,this.Width,this.Height/2+1);
            Rectangle rectangle2 = new Rectangle(0, this.Height / 2, this.Width, this.Height-this.Height/2);
            Color color1= Color.FromArgb(2, 40, 40);
            Color color2= Color.FromArgb(2, 40, 40);
            //Color color1 = Color.FromArgb(0, 0, 0);
            //Color color2 = Color.FromArgb(25, 25, 25);
            LinearGradientBrush brush1 = new LinearGradientBrush(rectangle1, color1, color2, LinearGradientMode.Vertical);
            LinearGradientBrush brush2 = new LinearGradientBrush(rectangle2, color2, color1, LinearGradientMode.Vertical);

            args.Graphics.FillRectangle(brush2, rectangle2);
            args.Graphics.FillRectangle(brush1, rectangle1);
            */
             
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {

            this.nameTextBox.Text = settingsXml.DocumentElement.ChildNodes[0].InnerText;
            if (settingsXml.DocumentElement.ChildNodes[1].InnerText == GameLevel.Easy.ToString())
                this.easyRadioButton.Checked = true;
            else if (settingsXml.DocumentElement.ChildNodes[1].InnerText == GameLevel.Normal.ToString())
                this.normalRadioButton.Checked = true;
            else if (settingsXml.DocumentElement.ChildNodes[1].InnerText == GameLevel.Hard.ToString())
                this.hardRadioButton.Checked = true;

            if (settingsXml.DocumentElement.ChildNodes[2].Attributes[0].Value == CardDeckType.Classic.ToString())
                this.classicRadioButton.Checked = true;
            else if (settingsXml.DocumentElement.ChildNodes[2].Attributes[0].Value == CardDeckType.Simple.ToString())
                this.simpleRadioButton.Checked = true;

            if (settingsXml.DocumentElement.ChildNodes[2].Attributes[1].Value == CardBackType.Red.ToString())
                this.redRadioButton.Checked = true;
            else if (settingsXml.DocumentElement.ChildNodes[2].Attributes[1].Value == CardBackType.Blue.ToString())
                this.blueRadioButton.Checked = true;

            if (settingsXml.DocumentElement.ChildNodes[3].Attributes[0].Value == "true")
                this.soundCheckBox.Checked = true;
            else
                this.soundCheckBox.Checked = false;

            if (settingsXml.DocumentElement.ChildNodes[4].Attributes[0].Value == "true")
                this.notificationCheckBox.Checked = true;
            else
                this.notificationCheckBox.Checked = false;

            switch (settingsXml.DocumentElement.ChildNodes[5].Attributes[0].Value)
            {
                case "1":
                    this.radioButton1.Checked = true;
                    break;
                case "2":
                    this.radioButton2.Checked = true;
                    break;
                case "3":
                    this.radioButton3.Checked = true;
                    break;
                case "4":
                    this.radioButton4.Checked = true;
                    break;
                case "5":
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
    }
}
