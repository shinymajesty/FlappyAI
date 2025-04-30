using System.Drawing.Drawing2D;

namespace Game
{
    public partial class Form1 : Form
    {

        public bool birdIsJumping = false;
        public Form1()
        {
            InitializeComponent();
        }


        private void gameTimer_Tick(object sender, EventArgs e)
        {
            pipeBot.Left -= 10;
            pipeTop.Left -= 10;

            if (pipeBot.Left < -pipeBot.Width)
            {
                pipeBot.Left = this.ClientSize.Width;
                pipeTop.Left = this.ClientSize.Width;
            }
            if(!birdIsJumping)
                bird.Top += 5;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            gameTimer.Start();
            button1.Enabled = false;
            this.Focus();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
         {
            if(e.KeyCode == Keys.Space)
            {
                IncreaseBirdHeight();
                
            }
        }
        async Task IncreaseBirdHeight()
        {
            birdIsJumping = true;
            
            bird.Image = RotateImage(new Bitmap(bird.Image), -30);



            foreach (var i in Enumerable.Range(0, 10))
            {
                bird.Top -= 10;
                await Task.Delay(1);
            }
            birdIsJumping = false;
            await Task.Delay(100);
            bird.Image = RotateImage(new Bitmap(bird.Image), 30);
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
            return result;
        }

    }
}
