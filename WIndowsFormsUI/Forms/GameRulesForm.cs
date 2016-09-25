using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WIndowsFormsUI.Forms
{
    public partial class GameRulesForm : Form
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

        public GameRulesForm()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void DrawBackgroud(Object sender, PaintEventArgs args)
        {
            /*
            Rectangle rectangle1 = new Rectangle(0, 0, this.Width, this.Height / 2 + 1);
            Rectangle rectangle2 = new Rectangle(0, this.Height / 2, this.Width, this.Height - this.Height / 2);
            Color color1 = Color.FromArgb(0, 0, 0);
            Color color2 = Color.FromArgb(25, 25, 25);
            LinearGradientBrush brush1 = new LinearGradientBrush(rectangle1, color1, color2, LinearGradientMode.Vertical);
            LinearGradientBrush brush2 = new LinearGradientBrush(rectangle2, color2, color1, LinearGradientMode.Vertical);

            args.Graphics.FillRectangle(brush2, rectangle2);
            args.Graphics.FillRectangle(brush1, rectangle1);
            */
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton2.Checked)
            {
                radioButton1.Checked = false;
                gameRulesPanel.Visible = false;
                gameInstructionsPanel.Visible = true;
                gameInstructionsPanel.Location = gameRulesPanel.Location;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                radioButton2.Checked = false;
                gameInstructionsPanel.Visible = false;
                gameRulesPanel.Visible = true;
            }
        }
    }
}
