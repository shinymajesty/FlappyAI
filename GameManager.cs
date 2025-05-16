using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Game
{
    public class GameManager
    {
        private readonly Form form;
        private readonly System.Windows.Forms.Timer gameTimer;
        private readonly Label scoreLabel;
        private readonly Label debugLabel;

        private BirdManager birdManager;
        private PipeManager pipeManager;

        private int score = 0;
        private const int PIPE_SPEED = 10;
        private bool isGameRunning = false;

        // Fixed serialization to handle double[][] properly
        public List<GenomeEntry> SerializablePopulation =>
            [.. birdManager.Population.Select(p => new GenomeEntry { Genome = p.genome, Fitness = p.fitness })];

        public void Initialize(List<GenomeEntry> initialPopulation) => birdManager.Initialize(initialPopulation);
        public GameManager(Form form, System.Windows.Forms.Timer gameTimer, Label scoreLabel, Label debugLabel)
        {
            this.form = form;
            this.gameTimer = gameTimer;
            this.scoreLabel = scoreLabel;
            this.debugLabel = debugLabel;

            // Connect the timer tick event
            this.gameTimer.Tick += GameTick;
        }

        public void InitializeGame(PictureBox birdTemplate, List<(Panel pipeBot, Panel pipeTop)> pipes, int birdCount = 100)
        {
            birdManager = new BirdManager(form, birdTemplate, birdCount);
            pipeManager = new PipeManager(pipes, form.ClientSize.Width, form.ClientSize.Height);
            form.MaximumSize = form.MinimumSize = form.ClientSize;
        }

        public void StartGame()
        {
            isGameRunning = true;
            score = 0;
            UpdateScoreDisplay();

            birdManager.SetGameRunning(true);
            gameTimer.Start();
        }

        public void StopGame()
        {
            isGameRunning = false;
            gameTimer.Stop();
            birdManager.SetGameRunning(false);

            
        }

        public async void ResetGame()
        {
            StopGame();
            birdManager.Reset();
            pipeManager.Reset();
            score = 0;
            UpdateScoreDisplay();
            StartGame();
        }

        private void GameTick(object sender, EventArgs e)
        {
            // Check for game over
            CheckForGameOver();

            if (!isGameRunning)
                return;

            // Move the pipes
            pipeManager.MovePipes(PIPE_SPEED);

            // Get the nearest pipe for AI decisions
            var currentPipe = pipeManager.GetNearestPipe(birdManager.Birds[0]);

            // Update all birds
            birdManager.UpdateBirds(currentPipe, form.ClientSize.Height);

            // Check for scoring
            HandleScoring(currentPipe);



            // Update debug info
            UpdateDebugInfo(currentPipe);
        }

        private void HandleScoring(in (Panel pipeBot, Panel pipeTop) pipe)
        {
            // Only score for the first bird (player or AI)
            if (birdManager.Birds.Count > 0)
            {
                Bird firstBird = birdManager.Birds[0];

                // Score a point when bird passes the middle of the pipe
                if (pipe.pipeBot.Left < firstBird.Left && pipe.pipeBot.Right > firstBird.Left)
                {
                    score++;
                    UpdateScoreDisplay();
                }
            }
        }

        private void UpdateScoreDisplay()
        {
            scoreLabel.Text = "Score: " + Math.Round(score / 20.0).ToString();
        }

        private void UpdateDebugInfo(in (Panel pipeBot, Panel pipeTop) currentPipe)
        {
            if (birdManager.Birds.Count > 0)
            {
                Bird firstBird = birdManager.Birds[0];

                // Update debug label with information about the first bird
                double[] inputs = new double[] {
                    firstBird.Top,
                    firstBird.VelocityY,
                    currentPipe.pipeBot.Top,
                    currentPipe.pipeTop.Top + currentPipe.pipeTop.Height
                };


                // With the following code to run for each Network and collect all results in a List:
                List<double> outputs = new(birdManager.Networks.Count);
                Parallel.ForEach(birdManager.Networks, network =>
                {
                    double output = network.CalculateWeights(inputs)[0];
                    lock (outputs)
                    {
                        outputs.Add(output);
                    }
                });

                debugLabel.Text = $"Average Output: {outputs.Average()}\n" +
                                  $"Birds Alive: {birdManager.GetAliveBirds(form.ClientSize.Height).Count}\n" +
                                  $"Hit Ground: {(firstBird.Top > form.ClientSize.Height - firstBird.Height)}\n" +
                                  $"Birds Dead: {birdManager.Birds.Count(b => b.IsDead)}\n" +
                                  $"Generation: {birdManager.Generation}";
            }
        }

        private void CheckForGameOver()
        {
            List<Bird> aliveBirds = birdManager.GetAliveBirds(form.ClientSize.Height);

            foreach (Bird bird in aliveBirds)
            {
                // Check if the bird hit a pipe
                if (pipeManager.CheckCollision(bird))
                {
                    birdManager.KillBird(bird, pipeManager.GetNearestPipe(bird));
                }
            }

            var x = aliveBirds.Count == 0;
            if (x)
            {
                birdManager.EndGeneration();
                ResetGame();

            }
        }

        public void AddBird(PictureBox templateBird)
        {
            birdManager.AddBird(templateBird);
        }
    }
}