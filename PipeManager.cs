using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public class PipeManager
    {
        private readonly List<(Panel pipeBot, Panel pipeTop)> pipes;
        private readonly Random random = new();
        private readonly int clientWidth;
        private readonly int clientHeight;
        private readonly int pipeGap = 250; // Gap between top and bottom pipes

        public PipeManager(List<(Panel pipeBot, Panel pipeTop)> pipes, int clientWidth, int clientHeight)
        {
            this.pipes = pipes;
            this.clientWidth = clientWidth;
            this.clientHeight = clientHeight;

            // Initialize pipes position
            float widthFactor = 0.5f;
            foreach (var (pipeBot, pipeTop) in this.pipes)
            {
                pipeBot.Left = (int)Math.Round(this.clientWidth * widthFactor);
                pipeTop.Left = (int)Math.Round(this.clientWidth * widthFactor);

                // Set initial height
                ResetPipePosition(pipeBot, pipeTop);

                widthFactor += 0.5f;
            }
        }

        public void MovePipes(int speed)
        {
            foreach (var (pipeBot, pipeTop) in pipes)
            {
                pipeBot.Left -= speed;
                pipeTop.Left -= speed;

                // If pipe goes off-screen, reset its position
                if (pipeBot.Left < -pipeBot.Width)
                {
                    pipeBot.Left = (int)Math.Round(clientWidth * 1.0);
                    pipeTop.Left = (int)Math.Round(clientWidth * 1.0);

                    ResetPipePosition(pipeBot, pipeTop);
                }
            }
        }

        private void ResetPipePosition(Panel pipeBot, Panel pipeTop)
        {
            int pipeHeightLimit = clientHeight - 300;
            int bottomPipeHeight = random.Next(50, pipeHeightLimit);
            int topPipeHeight = clientHeight - bottomPipeHeight - pipeGap;

            pipeBot.Height = bottomPipeHeight;
            pipeTop.Height = topPipeHeight;
            pipeBot.Top = clientHeight - bottomPipeHeight;
            pipeTop.Top = 0; // Top pipe is always aligned to the top
        }

        public (Panel pipeBot, Panel pipeTop) GetNearestPipe(Bird bird)
        {
            (Panel pipeBot, Panel pipeTop) nearestPipe = pipes[0];
            int minDistance = int.MaxValue;

            foreach (var pipe in pipes)
            {
                // Only consider pipes that are ahead of the bird
                if (pipe.pipeBot.Left + pipe.pipeBot.Width > bird.Left)
                {
                    int distance = pipe.pipeBot.Left - bird.Left;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestPipe = pipe;
                    }
                }
            }

            return nearestPipe;
        }

        public bool CheckCollision(Bird bird)
        {
            foreach (var (pipeBot, pipeTop) in pipes)
            {
                if (bird.Bounds.IntersectsWith(pipeBot.Bounds) ||
                    bird.Bounds.IntersectsWith(pipeTop.Bounds))
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            float widthFactor = 0.5f;
            foreach (var (pipeBot, pipeTop) in pipes)
            {
                pipeBot.Left = (int)Math.Round(clientWidth * widthFactor);
                pipeTop.Left = (int)Math.Round(clientWidth * widthFactor);
                ResetPipePosition(pipeBot, pipeTop);
                widthFactor += 0.5f;
            }
        }
    }
}