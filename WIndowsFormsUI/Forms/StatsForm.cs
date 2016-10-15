using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WIndowsFormsUI.Properties;

namespace WIndowsFormsUI.Forms
{
    public partial class StatsForm : Form
    {
        public StatsForm()
        {
            InitializeComponent();

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            this.ShowStats();
        }

        private void ShowStats()
        {
            this.EasyGamesLabel.Text = Settings.Default.totalGamesVersusEasy.ToString();
            this.NormalGamesLabel.Text = Settings.Default.totalGamesVersusNormal.ToString();
            this.HardGamesLabel.Text = Settings.Default.totalGamesVersusHard.ToString();

            this.EasyWinsLabel.Text = Settings.Default.totalWinsVersusEasy.ToString();
            this.NormalWinsLabel.Text = Settings.Default.totalWinsVersusNormal.ToString();
            this.HardWinsLabel.Text = Settings.Default.totalWinsVersusHard.ToString();

            this.EasyWinRatio.Text = (Settings.Default.totalGamesVersusEasy==0)?
                "0":((double)Settings.Default.totalWinsVersusEasy / (double)Settings.Default.totalGamesVersusEasy).ToString();

            this.NormalWinRatio.Text = (Settings.Default.totalGamesVersusNormal==0)?
                "0":((double)Settings.Default.totalWinsVersusNormal / (double)Settings.Default.totalGamesVersusNormal).ToString();

            this.HardWinRatio.Text = (Settings.Default.totalGamesVersusHard==0)?
                "0":((double)Settings.Default.totalWinsVersusHard / (double)Settings.Default.totalGamesVersusHard).ToString();

            this.EasyTotalRoundsLabel.Text = Settings.Default.totalRoundWinsVsEasy.ToString();
            this.NormalTotalRoundsLabel.Text = Settings.Default.totalRoundWinsVsNormal.ToString();
            this.HardTotalRoundsLabel.Text = Settings.Default.totalRoundWinsVsHard.ToString();

            this.EasyTotalRoundLossesLabel.Text = Settings.Default.totalRoundLossesVsEasy.ToString();
            this.NormalTotalRoundLossesLabel.Text = Settings.Default.totalRoundLossesVsNormal.ToString();
            this.HardTotalRoundLossesLabel.Text = Settings.Default.totalRoundLossesVsHard.ToString();

            this.updateLabel.Text = "Lastly updated on: " + Settings.Default.lastUpdate.ToShortDateString();
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
