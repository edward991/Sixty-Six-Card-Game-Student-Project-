using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Windows.Forms;
using WIndowsFormsUI.Properties;

namespace WIndowsFormsUI.Forms
{

    public partial class MainMenuForm : Form
    {
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

        private void PlaySound(UnmanagedMemoryStream sound)
        {
            if (Settings.Default.sound)
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

        private void statsButton_MouseEnter(object sender, EventArgs e)
        {
            statsButton.BackColor = Color.FromArgb(200, 5, 35, 35);
            this.PlaySound(WIndowsFormsUI.Properties.Resources.misc_menu1);
        }

        private void statsButton_MouseLeave(object sender, EventArgs e)
        {
            statsButton.BackColor = Color.FromArgb(5, 35, 35);
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
                new SinglePlayerForm(Settings.Default.playerName, Settings.Default.level, Settings.Default.deckType, 
                    Settings.Default.cardBack, Settings.Default.sound, Settings.Default.notifications, Settings.Default.character);
            singlePlayerForm.Show();
            this.Hide();
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            OptionsForm of = new OptionsForm();
            if (of.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.playerName = of.PlayerName;

                Settings.Default.level = of.Level;

                Settings.Default.deckType = of.DeckType;

                Settings.Default.cardBack = of.CardsBack;

                Settings.Default.sound = of.Sound;

                Settings.Default.notifications = of.Notifications;

                Settings.Default.character = of.Character;

                Settings.Default.Save();
            }
        }

        private void rulesButton_Click(object sender, EventArgs e)
        {
            GameRulesForm grf = new GameRulesForm();
            grf.ShowDialog();
        }

        private void statsButton_Click(object sender, EventArgs e)
        {
            StatsForm sf = new StatsForm();
            sf.ShowDialog();
        }
    }
}
