using System.Drawing;
using System.IO.Pipelines;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public class Bird
    {
        // Properties
        public PictureBox PictureBox { get; private set; }
        public bool IsJumping { get; private set; }
        public int Top => PictureBox.Top;
        public int Left => PictureBox.Left;
        public int Height => PictureBox.Height;
        public int Width => PictureBox.Width;
        public Rectangle Bounds => PictureBox.Bounds;
        public int VelocityY => _baseVelocityY + (((_ticksFallen * _ticksFallen) / 250));
        public bool IsDead { get; private set; } = false;
        public int? CurrentPipeTop { get; private set; } = null; // Current top pipe
        public int? CurrentPipeBottom { get; private set; } = null; // Current bottom pipe
        private int? VerticalDistanceToPipe = null;
        /// <summary>
        /// Fitness Value for Bird
        /// </summary>
        public double Fitness => TicksAlive * 10; // Fitness score for the bird
        /// <summary>
        /// Ticks bird has been alive, also used for fitness calculation
        /// </summary>
        public int TicksAlive { get; set; } = 0;

        // Private fields
        private readonly int _jumpHeight = 10;
        private readonly int _miniHops = 10;
        private readonly Image _straightImage;
        private readonly Image _upImage;
        private int _ticksFallen = 0;
        private readonly int _baseVelocityY = 1;
        private bool _isGameRunning;
        private (int X, int Y) _initialPosition; // Initial position of the bird

        // Constructor
        public Bird(PictureBox pictureBox, bool isGameRunning)
        {
            PictureBox = pictureBox;
            _isGameRunning = isGameRunning;
            _straightImage = new Bitmap(pictureBox.Image);
            _upImage = RotateImage(new Bitmap(pictureBox.Image), -30);
            _initialPosition = (pictureBox.Left, pictureBox.Top);
        }

        // Methods
        public void Fall()
        {
            if (!IsJumping)
            {
                PictureBox.Top += VelocityY;
                _ticksFallen++;
            }
        }

        public void ResetFallSpeed()
        {
            _ticksFallen = 0;
        }

        public void SetGameRunning(bool isRunning)
        {
            _isGameRunning = isRunning;
        }

        private bool CanJump()
        {
            return _isGameRunning &&
                  !(IsJumping || PictureBox.Top < _jumpHeight * _miniHops);
        }

        public async Task Jump()
        {
            if (!CanJump())
                return;

            IsJumping = true;
            ResetFallSpeed();
            PictureBox.Image = _upImage;

            for (int i = 0; i < _miniHops; i++)
            {
                PictureBox.Top -= _jumpHeight;
                await Task.Delay(1);
            }

            IsJumping = false;
            await Task.Delay(100);
            PictureBox.Image = _straightImage;
        }

        private static Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap result = new(bmp.Width, bmp.Height);
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

        public void Reset()
        {
            PictureBox.Top = _initialPosition.Y;
            PictureBox.Left = _initialPosition.X;
            ResetFallSpeed();
            PictureBox.Image = _straightImage;
            IsJumping = false;
            IsDead = false;
            this.TicksAlive = 0;
            // Make sure the bird is visible when reset
            PictureBox.Visible = true;
        }

        public void Die(Panel top, Panel bot)
        {
            IsDead = true;
            CurrentPipeTop = top.Top;
            CurrentPipeBottom = bot.Top;
        }

        public void Dispose()
        {
            if (PictureBox != null && !PictureBox.IsDisposed)
            {
                PictureBox.Image?.Dispose();
                PictureBox.Dispose();
            }
        }
        public void GetVerticalDistanceToPipe()
        {
            if (CurrentPipeTop == null || CurrentPipeBottom == null)
            {
                this.VerticalDistanceToPipe = 9999;
                return;
            }

            int pipeTop = (int)CurrentPipeTop;
            int pipeBottom = (int)CurrentPipeBottom;
            // Calculate the distance to the top and bottom of the pipe
            int distanceToTop = Math.Abs(pipeTop - PictureBox.Top);
            int distanceToBottom = Math.Abs(pipeBottom - PictureBox.Top);
            // Return the minimum distance
            this.VerticalDistanceToPipe = Math.Min(distanceToTop, distanceToBottom);
        }
    }
}