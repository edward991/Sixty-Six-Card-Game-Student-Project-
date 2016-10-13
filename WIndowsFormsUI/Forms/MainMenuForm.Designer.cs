namespace WIndowsFormsUI.Forms
{
    partial class MainMenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenuForm));
            this.panel6 = new System.Windows.Forms.Panel();
            this.statsButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.rulesButton = new System.Windows.Forms.Button();
            this.optionsButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Transparent;
            this.panel6.Controls.Add(this.statsButton);
            this.panel6.Controls.Add(this.exitButton);
            this.panel6.Controls.Add(this.pictureBox1);
            this.panel6.Controls.Add(this.rulesButton);
            this.panel6.Controls.Add(this.optionsButton);
            this.panel6.Controls.Add(this.playButton);
            this.panel6.Location = new System.Drawing.Point(283, 15);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(434, 506);
            this.panel6.TabIndex = 13;
            // 
            // statsButton
            // 
            this.statsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.statsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.statsButton.FlatAppearance.BorderSize = 0;
            this.statsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.statsButton.Font = new System.Drawing.Font("Orator Std", 16F);
            this.statsButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.statsButton.Location = new System.Drawing.Point(143, 346);
            this.statsButton.Name = "statsButton";
            this.statsButton.Size = new System.Drawing.Size(162, 45);
            this.statsButton.TabIndex = 18;
            this.statsButton.Text = "Stats";
            this.statsButton.UseVisualStyleBackColor = false;
            this.statsButton.Click += new System.EventHandler(this.statsButton_Click);
            this.statsButton.MouseEnter += new System.EventHandler(this.statsButton_MouseEnter);
            this.statsButton.MouseLeave += new System.EventHandler(this.statsButton_MouseLeave);
            // 
            // exitButton
            // 
            this.exitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.exitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.exitButton.FlatAppearance.BorderSize = 0;
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Font = new System.Drawing.Font("Orator Std", 16F);
            this.exitButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.exitButton.Location = new System.Drawing.Point(143, 448);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(162, 45);
            this.exitButton.TabIndex = 17;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            this.exitButton.MouseEnter += new System.EventHandler(this.exitButton_MouseEnter);
            this.exitButton.MouseLeave += new System.EventHandler(this.exitButton_MouseLeave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::WIndowsFormsUI.Properties.Resources.logoPicture;
            this.pictureBox1.Location = new System.Drawing.Point(18, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(400, 177);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // rulesButton
            // 
            this.rulesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.rulesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rulesButton.FlatAppearance.BorderSize = 0;
            this.rulesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rulesButton.Font = new System.Drawing.Font("Orator Std", 16F);
            this.rulesButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.rulesButton.Location = new System.Drawing.Point(143, 397);
            this.rulesButton.Name = "rulesButton";
            this.rulesButton.Size = new System.Drawing.Size(162, 45);
            this.rulesButton.TabIndex = 16;
            this.rulesButton.Text = "Rules";
            this.rulesButton.UseVisualStyleBackColor = false;
            this.rulesButton.Click += new System.EventHandler(this.rulesButton_Click);
            this.rulesButton.MouseEnter += new System.EventHandler(this.rulesButton_MouseEnter);
            this.rulesButton.MouseLeave += new System.EventHandler(this.rulesButton_MouseLeave);
            // 
            // optionsButton
            // 
            this.optionsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.optionsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.optionsButton.FlatAppearance.BorderSize = 0;
            this.optionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.optionsButton.Font = new System.Drawing.Font("Orator Std", 16F);
            this.optionsButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.optionsButton.Location = new System.Drawing.Point(143, 295);
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Size = new System.Drawing.Size(162, 45);
            this.optionsButton.TabIndex = 15;
            this.optionsButton.Text = "Options";
            this.optionsButton.UseVisualStyleBackColor = false;
            this.optionsButton.Click += new System.EventHandler(this.optionsButton_Click);
            this.optionsButton.MouseEnter += new System.EventHandler(this.optionsButton_MouseEnter);
            this.optionsButton.MouseLeave += new System.EventHandler(this.optionsButton_MouseLeave);
            // 
            // playButton
            // 
            this.playButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.playButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.playButton.FlatAppearance.BorderSize = 0;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Font = new System.Drawing.Font("Orator Std", 16F);
            this.playButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.playButton.Location = new System.Drawing.Point(143, 244);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(162, 45);
            this.playButton.TabIndex = 14;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = false;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            this.playButton.MouseEnter += new System.EventHandler(this.playButton_MouseEnter);
            this.playButton.MouseLeave += new System.EventHandler(this.playButton_MouseLeave);
            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(96)))), ((int)(((byte)(78)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(983, 579);
            this.Controls.Add(this.panel6);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main menu";
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button optionsButton;
        private System.Windows.Forms.Button rulesButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button statsButton;
    }
}