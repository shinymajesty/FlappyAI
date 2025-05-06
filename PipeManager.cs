using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public class PipeManager
    {
        private readonly List<(Panel pipeBot, Panel pipeTop)> _pipes;
        private readonly Random _random = new();
        private readonly int _clientWidth;
        private readonly int _clientHeight;
        private readonly int _pipeGap = 250; // Gap between top and bottom pipes

        public PipeManager(List<(Panel pipeBot, Panel pipeTop)> pipes, int clientWidth, int clientHeight)
        {
            _pipes = pipes;
            _clientWidth = clientWidth;
            _clientHeight = clientHeight;

            // Initialize pipes position
            float widthFactor = 0.5f;
            foreach (var (pipeBot, pipeTop) in _pipes)
            {
                pipeBot.Left = (int)Math.Round(_clientWidth * widthFactor);
                pipeTop.Left = (int)Math.Round(_clientWidth * widthFactor);

                // Set initial height
                ResetPipePosition(pipeBot, pipeTop);

                widthFactor += 0.5f;
            }
        }

        public void MovePipes(int speed)
        {
            foreach (var (pipeBot, pipeTop) in _pipes)
            {
                pipeBot.Left -= speed;
                pipeTop.Left -= speed;

                // If pipe goes off-screen, reset its position
                if (pipeBot.Left < -pipeBot.Width)
                {
                    pipeBot.Left = (int)Math.Round(_clientWidth * 1.0);
                    pipeTop.Left = (int)Math.Round(_clientWidth * 1.0);

                    ResetPipePosition(pipeBot, pipeTop);
                }
            }
        }

        private void ResetPipePosition(Panel pipeBot, Panel pipeTop)
        {
            int pipeHeightLimit = _clientHeight - 300;
            int bottomPipeHeight = _random.Next(50, pipeHeightLimit);
            int topPipeHeight = _clientHeight - bottomPipeHeight - _pipeGap;

            pipeBot.Height = bottomPipeHeight;
            pipeTop.Height = topPipeHeight;
            pipeBot.Top = _clientHeight - bottomPipeHeight;
            pipeTop.Top = 0; // Top pipe is always aligned to the top
        }

        public (Panel pipeBot, Panel pipeTop) GetNearestPipe(Bird bird)
        {
            (Panel pipeBot, Panel pipeTop) nearestPipe = _pipes[0];
            int minDistance = int.MaxValue;

            foreach (var pipe in _pipes)
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
            foreach (var (pipeBot, pipeTop) in _pipes)
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
            foreach (var (pipeBot, pipeTop) in _pipes)
            {
                pipeBot.Left = (int)Math.Round(_clientWidth * widthFactor);
                pipeTop.Left = (int)Math.Round(_clientWidth * widthFactor);
                ResetPipePosition(pipeBot, pipeTop);
                widthFactor += 0.5f;
            }
        }
    }
}