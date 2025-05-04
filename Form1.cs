using System.Drawing.Drawing2D;

namespace Game
{
    public partial class Form1 : Form
    {
        int vY = 1;
        int ticksFallen = 0;
        public bool birdIsJumping = false;
        int Score = 0;
        bool hasPassedPipe = false;
        Random rand = new Random();
        Image birdStraight;
        Image birdUp;

        /// <summary>
        /// List of pipes to be used in the game.
        /// (pipeBot, pipeTop)
        /// </summary>
        List<(Panel pipeBot, Panel pipeTop)> pipes = [];
        public Form1()
        {
            InitializeComponent();
            pipes = [(pipeBot1, pipeTop1), (pipeBot2, pipeTop2)];
            birdStraight = new Bitmap(bird.Image);
            birdUp = RotateImage(new Bitmap(bird.Image), -30);
            float widthFactor = 0.5f;
            foreach(var pipe in pipes)
            {
                pipe.pipeBot.Left =  (int)Math.Round(this.ClientSize.Width * widthFactor);
                pipe.pipeTop.Left =  (int)Math.Round(this.ClientSize.Width * widthFactor);
                widthFactor += 0.5f;            
            }
        }


        private void gameTimer_Tick(object sender, EventArgs e)
        {
            foreach (var pipe in pipes)
            {
                pipe.pipeBot.Left -= 10;
                pipe.pipeTop.Left -= 10;

                if (pipe.pipeBot.Left < -pipe.pipeBot.Width)
                {
                    pipe.pipeBot.Left = (int)Math.Round(this.ClientSize.Width * 1.0);
                    pipe.pipeTop.Left = (int)Math.Round(this.ClientSize.Width * 1.0);


                    int pipeHeightLimit = this.ClientSize.Height - 300;
                    int bottomPipeHeight = rand.Next(50, pipeHeightLimit);
                    int topPipeHeight = this.ClientSize.Height - bottomPipeHeight - 250;
                    (pipe.pipeBot.Height, pipe.pipeTop.Height) = (bottomPipeHeight, topPipeHeight);
                    pipe.pipeBot.Top = this.ClientSize.Height - bottomPipeHeight;
                }
            }
            int fallSpeed = vY * ((ticksFallen++ * ticksFallen++) / 350);

            if (!birdIsJumping)
                bird.Top += fallSpeed;
            foreach (var pipe in pipes)
            {
                if (pipe.pipeBot.Left < bird.Left && pipe.pipeBot.Right > bird.Left)
                {
                    hasPassedPipe = true;
                    Score++;
                    label1.Text = "Score: " + Math.Round(Score / 20.0).ToString();
                }
                else hasPassedPipe = false;
            }

            CheckForGameOver();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            gameTimer.Start();
            button1.Enabled = false;
            this.Focus();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                Jump();
            }
        }
        private bool CanJump(int jumpHeight, int miniHops)
        {
            if (gameTimer.Enabled &&
                !(birdIsJumping || bird.Top < jumpHeight * miniHops))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        async Task Jump()
        {
            int jumpHeight = 10;
            int miniHops = 10;
            if (!CanJump(jumpHeight, miniHops))
                return;

            birdIsJumping = true;
            ticksFallen = 0;
            bird.Image = birdUp;




            foreach (var i in Enumerable.Range(0, miniHops))
            {
                bird.Top -= jumpHeight;
                await Task.Delay(1);
            }
            birdIsJumping = false;
            await Task.Delay(100);
            bird.Image = birdStraight;
        }
        public Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap result = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                g.DrawImage(bmp, new Point(0, 0));
            }
            bmp.Dispose();
            return result;
        }
        private void CheckForGameOver()
        {
            foreach (var pipe in pipes)
            {
                if (bird.Bounds.IntersectsWith(pipe.pipeBot.Bounds) || bird.Bounds.IntersectsWith(pipe.pipeTop.Bounds))
                {
                    gameTimer.Stop();
                    button1.Enabled = true;
                    MessageBox.Show("Game Over");
                    Application.Restart();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
