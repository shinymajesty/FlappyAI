using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Game
{
    public class GameManager
    {
        private readonly Form _form;
        private readonly System.Windows.Forms.Timer _gameTimer;
        private readonly Label _scoreLabel;
        private readonly Label _debugLabel;

        private BirdManager _birdManager;
        private PipeManager _pipeManager;

        private int _score = 0;
        private const int PIPE_SPEED = 10;
        private bool _isGameRunning = false;

        public GameManager(Form form, System.Windows.Forms.Timer gameTimer, Label scoreLabel, Label debugLabel)
        {
            _form = form;
            _gameTimer = gameTimer;
            _scoreLabel = scoreLabel;
            _debugLabel = debugLabel;

            // Connect the timer tick event
            _gameTimer.Tick += GameTick;
        }

        public void InitializeGame(PictureBox birdTemplate, List<(Panel pipeBot, Panel pipeTop)> pipes, int birdCount = 100)
        {
            _birdManager = new BirdManager(_form, birdTemplate, birdCount);
            _pipeManager = new PipeManager(pipes, _form.ClientSize.Width, _form.ClientSize.Height);
        }

        public void StartGame()
        {
            _isGameRunning = true;
            _score = 0;
            UpdateScoreDisplay();

            _birdManager.SetGameRunning(true);
            _gameTimer.Start();
        }

        public void StopGame()
        {
            _isGameRunning = false;
            _gameTimer.Stop();
            _birdManager.SetGameRunning(false);

            // Disable Add Bird button when game stops
            foreach (Control control in _form.Controls)
            {
                if (control is Button button && button.Text.Contains("Add Bird"))
                {
                    button.Enabled = false;
                    break;
                }
            }
        }

        public async void ResetGame()
        {
            StopGame();
            _birdManager.Reset();
            _pipeManager.Reset();
            _score = 0;
            UpdateScoreDisplay();
            StartGame();
        }

        private void GameTick(object sender, EventArgs e)
        {
            // Check for game over
            CheckForGameOver();

            if (!_isGameRunning)
                return;

            // Move the pipes
            _pipeManager.MovePipes(PIPE_SPEED);

            // Get the nearest pipe for AI decisions
            var currentPipe = _pipeManager.GetNearestPipe(_birdManager.Birds[0]);

            // Update all birds
            _birdManager.UpdateBirds(currentPipe, _form.ClientSize.Height);

            // Check for scoring
            HandleScoring(currentPipe);

            

            // Update debug info
            UpdateDebugInfo(currentPipe);
        }

        private void HandleScoring(in (Panel pipeBot, Panel pipeTop) pipe)
        {
            // Only score for the first bird (player or AI)
            if (_birdManager.Birds.Count > 0)
            {
                Bird firstBird = _birdManager.Birds[0];

                // Score a point when bird passes the middle of the pipe
                if (pipe.pipeBot.Left < firstBird.Left && pipe.pipeBot.Right > firstBird.Left)
                {
                    _score++;
                    UpdateScoreDisplay();
                }
            }
        }

        private void UpdateScoreDisplay()
        {
            _scoreLabel.Text = "Score: " + Math.Round(_score / 20.0).ToString();
        }

        private void UpdateDebugInfo(in (Panel pipeBot, Panel pipeTop) currentPipe)
        {
            if (_birdManager.Birds.Count > 0)
            {
                Bird firstBird = _birdManager.Birds[0];

                // Update debug label with information about the first bird
                double[] inputs = new double[] {
                    firstBird.Top,
                    firstBird.VelocityY,
                    currentPipe.pipeBot.Top,
                    currentPipe.pipeTop.Top + currentPipe.pipeTop.Height
                };

                double[] output = _birdManager.Networks[0].Compute(inputs);

                _debugLabel.Text = $"Output: {output[0]}\n" +
                                  $"Birds Alive: {_birdManager.GetAliveBirds(_form.ClientSize.Height).Count}\n" +
                                  $"Hit Ground: {(firstBird.Top > _form.ClientSize.Height - firstBird.Height)}";
            }
        }

        private void CheckForGameOver()
        {
            List<Bird> aliveBirds = _birdManager.GetAliveBirds(_form.ClientSize.Height);

            foreach (Bird bird in aliveBirds)
            {
                // Check if the bird hit a pipe
                if (_pipeManager.CheckCollision(bird))
                {
                    _birdManager.KillBird(bird);
                }
            }

            // Game over if no birds are alive
            if (_birdManager.GetAliveBirds(_form.ClientSize.Height).Count == 0)
            {
                _birdManager.EndGeneration();
                ResetGame();
                
            }
        }

        public void HandleKeyPress(Keys key)
        {
            if (_isGameRunning && key == Keys.Space && _birdManager.PlayerBird != null)
            {
                _ = _birdManager.PlayerBird.Jump();
            }
        }

        public void AddBird(PictureBox templateBird)
        {
            _birdManager.AddBird(templateBird);
        }
    }
}