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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            bird = new PictureBox();
            pipeBot1 = new Panel();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            pipeTop1 = new Panel();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            gameTimer = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            label1 = new Label();
            panel1 = new Panel();
            pipeBot2 = new Panel();
            pictureBox7 = new PictureBox();
            pictureBox8 = new PictureBox();
            pipeTop2 = new Panel();
            pictureBox5 = new PictureBox();
            pictureBox6 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)bird).BeginInit();
            pipeBot1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            pipeTop1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel1.SuspendLayout();
            pipeBot2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            pipeTop2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            SuspendLayout();
            // 
            // bird
            // 
            bird.BackColor = Color.Transparent;
            bird.Image = (Image)resources.GetObject("bird.Image");
            bird.Location = new Point(128, 270);
            bird.Name = "bird";
            bird.Size = new Size(101, 70);
            bird.SizeMode = PictureBoxSizeMode.StretchImage;
            bird.TabIndex = 0;
            bird.TabStop = false;
            // 
            // pipeBot1
            // 
            pipeBot1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pipeBot1.BackColor = Color.ForestGreen;
            pipeBot1.Controls.Add(pictureBox4);
            pipeBot1.Controls.Add(pictureBox3);
            pipeBot1.Location = new Point(435, 516);
            pipeBot1.Name = "pipeBot1";
            pipeBot1.Size = new Size(200, 308);
            pipeBot1.TabIndex = 5;
            // 
            // pictureBox4
            // 
            pictureBox4.BackgroundImage = Properties.Resources.pipeBot;
            pictureBox4.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox4.Dock = DockStyle.Fill;
            pictureBox4.Location = new Point(0, 70);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(200, 238);
            pictureBox4.TabIndex = 3;
            pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = Properties.Resources.pipeBot_Top;
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Dock = DockStyle.Top;
            pictureBox3.Location = new Point(0, 0);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(200, 70);
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // pipeTop1
            // 
            pipeTop1.BackColor = Color.ForestGreen;
            pipeTop1.Controls.Add(pictureBox1);
            pipeTop1.Controls.Add(pictureBox2);
            pipeTop1.Location = new Point(435, 1);
            pipeTop1.Name = "pipeTop1";
            pipeTop1.Size = new Size(200, 251);
            pipeTop1.TabIndex = 6;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.pipeBot_Top;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Dock = DockStyle.Bottom;
            pictureBox1.Location = new Point(0, 181);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(200, 70);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox2.BackgroundImage = Properties.Resources.pipeBot;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(200, 251);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // gameTimer
            // 
            gameTimer.Interval = 10;
            gameTimer.Tick += GameTimer_Tick;
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
            button1.Click += ButtonStart_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Black;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 27F);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(4, 4);
            label1.Name = "label1";
            label1.Padding = new Padding(3);
            label1.Size = new Size(192, 55);
            label1.TabIndex = 8;
            label1.Text = "Score: ";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top;
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(label1);
            panel1.Location = new Point(865, -3);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(4);
            panel1.Size = new Size(200, 63);
            panel1.TabIndex = 9;
            // 
            // pipeBot2
            // 
            pipeBot2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pipeBot2.BackColor = Color.ForestGreen;
            pipeBot2.Controls.Add(pictureBox7);
            pipeBot2.Controls.Add(pictureBox8);
            pipeBot2.Location = new Point(1400, 600);
            pipeBot2.Name = "pipeBot2";
            pipeBot2.Size = new Size(200, 229);
            pipeBot2.TabIndex = 7;
            // 
            // pictureBox7
            // 
            pictureBox7.BackgroundImage = Properties.Resources.pipeBot;
            pictureBox7.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox7.Dock = DockStyle.Fill;
            pictureBox7.Location = new Point(0, 70);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(200, 159);
            pictureBox7.TabIndex = 5;
            pictureBox7.TabStop = false;
            // 
            // pictureBox8
            // 
            pictureBox8.BackgroundImage = Properties.Resources.pipeBot_Top;
            pictureBox8.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox8.Dock = DockStyle.Top;
            pictureBox8.Location = new Point(0, 0);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(200, 70);
            pictureBox8.TabIndex = 4;
            pictureBox8.TabStop = false;
            // 
            // pipeTop2
            // 
            pipeTop2.BackColor = Color.ForestGreen;
            pipeTop2.Controls.Add(pictureBox5);
            pipeTop2.Controls.Add(pictureBox6);
            pipeTop2.Location = new Point(1400, 0);
            pipeTop2.Name = "pipeTop2";
            pipeTop2.Size = new Size(200, 111);
            pipeTop2.TabIndex = 8;
            // 
            // pictureBox5
            // 
            pictureBox5.BackgroundImage = Properties.Resources.pipeBot_Top;
            pictureBox5.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox5.Dock = DockStyle.Bottom;
            pictureBox5.Location = new Point(0, 41);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(200, 70);
            pictureBox5.TabIndex = 5;
            pictureBox5.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.BackgroundImage = Properties.Resources.pipeBot;
            pictureBox6.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox6.Dock = DockStyle.Fill;
            pictureBox6.Location = new Point(0, 0);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(200, 111);
            pictureBox6.TabIndex = 4;
            pictureBox6.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Highlight;
            ClientSize = new Size(1814, 761);
            Controls.Add(panel1);
            Controls.Add(pipeBot2);
            Controls.Add(pipeTop2);
            Controls.Add(bird);
            Controls.Add(pipeBot1);
            Controls.Add(pipeTop1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            ((System.ComponentModel.ISupportInitialize)bird).EndInit();
            pipeBot1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            pipeTop1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel1.ResumeLayout(false);
            pipeBot2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            pipeTop2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox bird;
        private Panel pipeBot1;
        private Panel pipeTop1;
        private System.Windows.Forms.Timer gameTimer;
        private Button button1;
        private Label label1;
        private Panel panel1;
        private Panel pipeBot2;
        private Panel pipeTop2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private PictureBox pictureBox4;
        private PictureBox pictureBox1;
        private PictureBox pictureBox7;
        private PictureBox pictureBox8;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;
    }
}
