using System.Drawing.Drawing2D;
using AForge;

namespace Game
{
    public partial class Form1 : Form
    {
        readonly int vY = 1;
        int ticksFallen = 0;
        public bool birdIsJumping = false;
        int Score = 0;
        private readonly Random rand = new();
        private readonly Image birdStraight;
        private readonly Image birdUp;
        bool isGameRunning = false;
        /// <summary>
        /// List of pipes to be used in the game.
        /// (pipeBot, pipeTop)
        /// </summary>
        private readonly List<(Panel pipeBot, Panel pipeTop)> pipes = [];
        public Form1()
        {
            InitializeComponent();
            pipes = [(pipeBot1, pipeTop1), (pipeBot2, pipeTop2)];
            birdStraight = new Bitmap(bird.Image);
            birdUp = RotateImage(new Bitmap(bird.Image), -30);
            float widthFactor = 0.5f;
            foreach (var (pipeBot, pipeTop) in pipes)
            {
                pipeBot.Left =  (int)Math.Round(this.ClientSize.Width * widthFactor);
                pipeTop.Left =  (int)Math.Round(this.ClientSize.Width * widthFactor);
                widthFactor += 0.5f;            
            }
        }


        private void GameTimer_Tick(object sender, EventArgs e)
        {
            foreach (var (pipeBot, pipeTop) in pipes)
            {
                pipeBot.Left -= 10;
                pipeTop.Left -= 10;

                if (pipeBot.Left < -pipeBot.Width)
                {
                    pipeBot.Left = (int)Math.Round(this.ClientSize.Width * 1.0);
                    pipeTop.Left = (int)Math.Round(this.ClientSize.Width * 1.0);


                    int pipeHeightLimit = this.ClientSize.Height - 300;
                    int bottomPipeHeight = rand.Next(50, pipeHeightLimit);
                    int topPipeHeight = this.ClientSize.Height - bottomPipeHeight - 250;
                    (pipeBot.Height, pipeTop.Height) = (bottomPipeHeight, topPipeHeight);
                    pipeBot.Top = this.ClientSize.Height - bottomPipeHeight;
                }
            }
            int fallSpeed = vY * ((ticksFallen++ * ticksFallen++) / 350);

            if (!birdIsJumping)
                bird.Top += fallSpeed;
            foreach (var (pipeBot, _) in pipes)
            {
                if (pipeBot.Left < bird.Left && pipeBot.Right > bird.Left)
                {
                    Score++;
                    label1.Text = "Score: " + Math.Round(Score / 20.0).ToString();
                }
            }

            CheckForGameOver();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            gameTimer.Start();
            button1.Enabled = false;
            isGameRunning = true;
            this.Focus();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                _ = Jump();
            }
        }
        private bool CanJump(int jumpHeight, int miniHops)
        {
            if (isGameRunning &&
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




            for(int i = 0; i < miniHops; i++) { 
                bird.Top -= jumpHeight;
                await Task.Delay(1);
            }
            birdIsJumping = false;
            await Task.Delay(100);
            bird.Image = birdStraight;
        }
        public static Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap result = new(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                g.DrawImage(bmp, new System.Drawing.Point(0, 0));
            }
            bmp.Dispose();
            return result;
        }
        private void CheckForGameOver()
        {
            foreach (var (pipeBot, pipeTop) in pipes)
            {
                if (bird.Bounds.IntersectsWith(pipeBot.Bounds) || bird.Bounds.IntersectsWith(pipeTop.Bounds) || bird.Top > this.Height-bird.Height)
                {
                    gameTimer.Stop();
                    button1.Enabled = true;
                    MessageBox.Show("Game Over");
                    isGameRunning = false;
                    Application.Restart();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
