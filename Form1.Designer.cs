namespace Game
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            bird = new PictureBox();
            pipeBot = new Panel();
            pipeTop = new Panel();
            gameTimer = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)bird).BeginInit();
            SuspendLayout();
            // 
            // bird
            // 
            bird.BackColor = Color.Transparent;
            bird.Image = Properties.Resources.bird_flat;
            bird.Location = new Point(128, 235);
            bird.Name = "bird";
            bird.Size = new Size(152, 129);
            bird.SizeMode = PictureBoxSizeMode.StretchImage;
            bird.TabIndex = 0;
            bird.TabStop = false;
            // 
            // pipeBot
            // 
            pipeBot.BackColor = Color.ForestGreen;
            pipeBot.Location = new Point(1016, 490);
            pipeBot.Name = "pipeBot";
            pipeBot.Size = new Size(200, 443);
            pipeBot.TabIndex = 5;
            // 
            // pipeTop
            // 
            pipeTop.BackColor = Color.ForestGreen;
            pipeTop.Location = new Point(1016, -3);
            pipeTop.Name = "pipeTop";
            pipeTop.Size = new Size(200, 251);
            pipeTop.TabIndex = 6;
            // 
            // gameTimer
            // 
            gameTimer.Interval = 10;
            gameTimer.Tick += gameTimer_Tick;
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(107, 48);
            button1.TabIndex = 7;
            button1.TabStop = false;
            button1.Text = "Start";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonStart_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Highlight;
            ClientSize = new Size(1549, 769);
            Controls.Add(button1);
            Controls.Add(pipeTop);
            Controls.Add(pipeBot);
            Controls.Add(bird);
            Name = "Form1";
            Text = "Form1";
            KeyDown += Form1_KeyDown;
            ((System.ComponentModel.ISupportInitialize)bird).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox bird;
        private Panel pipeBot;
        private Panel pipeTop;
        private System.Windows.Forms.Timer gameTimer;
        private Button button1;
    }
}
