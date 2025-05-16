using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Pipelines;
using System.Windows.Forms;
using FlappyBrain.Library;

namespace Game
{
    public class BirdManager
    {
        public List<Bird> Birds { get; private set; } = new();
        public List<FlappyBrainNetwork> Networks { get; private set; } = [];

        // Fix the return type to match Genetic Algorithm
        public List<(double[][] genome, double fitness)> Population => ga.Population;
        public int Generation => ga.Generation;

        private readonly Form parentForm;
        private GeneticAlgorithm ga;
        private int populationSize;
        private PictureBox templateBird;
        private float mutationRate = 0.1f;


        public BirdManager(Form parentForm, PictureBox templateBird, int birdCount = 1)
        {
            this.parentForm = parentForm;
            populationSize = birdCount;
            ga = new GeneticAlgorithm(populationSize);
            this.templateBird = templateBird;
            this.templateBird.Visible = false; // Hide the template bird
            Networks.Add(CreateNetwork());

            // Create AI birds
            // Parallelize the creation of AI birds
            Parallel.For(0, birdCount, i =>
            {
                // All UI operations must be invoked on the UI thread
                parentForm.Invoke(new Action(() =>
                {
                    CreateAIBird(templateBird, i);
                }));
            });
        }

        private void CreateAIBird(PictureBox templateBird, int index)
        {
            // Create a new PictureBox based on the template
            PictureBox newBirdPicture = new()
            {
                Image = new Bitmap(templateBird.Image!),
                SizeMode = templateBird.SizeMode,
                BackColor = Color.Transparent,
                Size = templateBird.Size,
                Location = new Point(
                    templateBird.Left,
                    templateBird.Top
                )
            };

            // Add the new PictureBox to the form
            parentForm.Controls.Add(newBirdPicture);
            newBirdPicture.BringToFront();

            // Create and add a new bird
            Bird newBird = new Bird(newBirdPicture, false);
            Birds.Add(newBird);

            // Create a neural network for the AI bird
            var network = CreateNetwork();
            Networks.Add(network);
        }

        private FlappyBrainNetwork CreateNetwork()
        {
            return new FlappyBrainNetwork(
                4,                     // inputs
                6,                     // hidden neurons
                1,                      // output
                FlappyBrain.Library.ActivationFunction.Sigmoid // activation function
            );
        }

        // Parallelize the creation of birds and networks in Initialize
        public void Initialize(List<GenomeEntry> initialPopulation)
        {
            ga.Initialize(initialPopulation);

            foreach (var bird in Birds)
            {
                if (bird.PictureBox != null && !bird.PictureBox.IsDisposed)
                {
                    parentForm.Controls.Remove(bird.PictureBox);
                    bird.Dispose();
                }
            }

            Birds.Clear();
            Networks.Clear();

            // Prepare storage for results
            var birdResults = new (PictureBox pictureBox, Bird bird, FlappyBrainNetwork network)[initialPopulation.Count];

            // Parallel creation of birds and networks (thread-safe, but UI controls must be created on UI thread)
            Parallel.For(0, initialPopulation.Count, i =>
            {
                var genome = initialPopulation[i];

                // Create network in parallel
                var network = CreateNetwork();

                // Prepare bird and picturebox on UI thread
                parentForm.Invoke(new Action(() =>
                {
                    PictureBox newBirdPicture = new()
                    {
                        Image = new Bitmap(templateBird.Image!),
                        SizeMode = templateBird.SizeMode,
                        BackColor = Color.Transparent,
                        Size = templateBird.Size,
                        Location = new Point(templateBird.Left, templateBird.Top)
                    };

                    parentForm.Controls.Add(newBirdPicture);
                    newBirdPicture.BringToFront();

                    Bird newBird = new(newBirdPicture, false);

                    birdResults[i] = (newBirdPicture, newBird, network);
                }));

                // Set network weights (can be done in parallel)
                ga.SetNetworkWeights(network, genome.Genome);
            });

            // Add results to lists (must be done on UI thread)
            foreach (var (pictureBox, bird, network) in birdResults)
            {
                Birds.Add(bird);
                Networks.Add(network);
            }
        }
        public void UpdateBirds((Panel pipeBot, Panel pipeTop) currentPipe, int clientHeight)
        {
            //CleanupDeadBirds();
            HandleDeadBirds(clientHeight, currentPipe.pipeBot, currentPipe.pipeTop);
            for (int i = 0; i < Birds.Count; i++)
            {
                Bird bird = Birds[i];

                // Skip dead birds
                if (bird.IsDead)
                    continue;
                else
                    bird.TicksAlive++;
                // AI decision
                double[] inputs = [
                    bird.Top,
                    bird.VelocityY,
                    currentPipe.pipeBot.Top,
                    currentPipe.pipeTop.Top + currentPipe.pipeTop.Height
                
                ];

                double[] output = Networks[i].CalculateWeights(inputs);

                if (output[0] > 0.5)
                {
                    _ = bird.Jump();
                }
                else
                {
                    bird.Fall();
                }
            }

        }

        public void SetGameRunning(bool isRunning)
        {
            foreach (var bird in Birds)
            {
                bird.SetGameRunning(isRunning);
            }
        }

        public List<Bird> GetAliveBirds(int clientHeight)
        {
            return Birds.FindAll(bird => !bird.IsDead && bird.Top <= clientHeight - bird.Height);
        }

        public void HandleDeadBirds(int clientHeight, Panel pipeBot, Panel pipeTop)
        {
            for (int i = Birds.Count - 1; i >= 0; i--)
            {
                Bird bird = Birds[i];

                // Check if bird hit the ground or is marked as dead
                if (bird.Top > clientHeight - bird.Height || bird.IsDead)
                {
                    bird.Die(pipeTop, pipeBot);

                    // Hide the bird instead of removing it completely
                    // This allows us to reset it later if needed
                    bird.PictureBox.Visible = false;
                }
            }
        }

        public void KillBird(Bird bird, (Panel pipeBot, Panel pipeTop) currentPipe)
        {
            bird.Die(currentPipe.pipeTop, currentPipe.pipeBot);
            bird.PictureBox.Visible = false;
        }

        public void CleanupDeadBirds()
        {
            // Actually remove and dispose of dead birds' picture boxes
            for (int i = Birds.Count - 1; i >= 0; i--)
            {
                if (Birds[i].IsDead)
                {
                    if (Birds[i].PictureBox != null && !Birds[i].PictureBox.IsDisposed)
                    {
                        parentForm.Controls.Remove(Birds[i].PictureBox);
                        Birds[i].Dispose();
                    }
                    //Birds.RemoveAt(i);
                    //Networks.RemoveAt(i);
                }
            }
        }

        public void Reset()
        {
            // Reset existing birds
            foreach (var bird in Birds)
            {
                bird.Reset();
            }

            // Create new birds if some were completely removed
            int currentBirdCount = Birds.Count;
            if (currentBirdCount == 0 && parentForm.Controls.OfType<PictureBox>().Any(pb => pb.Tag?.ToString() == "BirdTemplate"))
            {
                // Find the template bird
                PictureBox templateBird = parentForm.Controls.OfType<PictureBox>()
                    .First(pb => pb.Tag?.ToString() == "BirdTemplate");

                // Create the player bird
                Birds.Add(new Bird(templateBird, false));
                Networks.Add(CreateNetwork());
            }
        }

        // Method to add a new bird during runtime
        public void AddBird(PictureBox templateBird)
        {
            CreateAIBird(templateBird, Birds.Count);
        }

        public void EndGeneration()
        {
            EvolveNewGeneration();
        }

        private void EvolveNewGeneration()
        {
            // Step 1: Send fitness data to GA
            ga.SetFitness(Birds, Networks);

            


            // Step 2: Evolve next generation of genomes
            var newGenomes = ga.EvolveNextGeneration(mutationRate);

            // Step 3: Clear old visuals and memory
            foreach (var bird in Birds)
            {
                if (bird.PictureBox != null && !bird.PictureBox.IsDisposed)
                {
                    parentForm.Controls.Remove(bird.PictureBox);
                    bird.Dispose();
                }
            }

            Birds.Clear();
            Networks.Clear();

            // Step 4: Recreate birds with evolved brains
            var template = this.templateBird;

            for (int i = 0; i < populationSize; i++)
            {
                // Create new bird visual
                PictureBox birdPicture = new()
                {
                    Image = new Bitmap(template.Image!),
                    SizeMode = template.SizeMode,
                    BackColor = Color.Transparent,
                    Size = template.Size,
                    Location = new Point(template.Left, template.Top)
                };

                parentForm.Controls.Add(birdPicture);
                birdPicture.BringToFront();

                Bird bird = new(birdPicture, false);
                Birds.Add(bird);

                // Load weights into a new network
                FlappyBrainNetwork net = CreateNetwork();
                ga.SetNetworkWeights(net, newGenomes[i]);
                Networks.Add(net);
            }
        }
    }
}