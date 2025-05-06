using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Accord.Neuro;
using Accord.Neuro.Learning;

namespace Game
{
    public class BirdManager
    {
        public List<Bird> Birds { get; private set; } = new();
        public List<ActivationNetwork> Networks { get; private set; } = [];


        private readonly Form _parentForm;
        private readonly int _birdOffsetY = 40; // Vertical distance between birds
        private GeneticAlgorithm _ga;
        private int _populationSize;


        public Bird PlayerBird => Birds.Count > 0 ? Birds[0] : null;

        public BirdManager(Form parentForm, PictureBox templateBird, int birdCount = 1)
        {
            _parentForm = parentForm;
            _populationSize = birdCount;
            _ga = new GeneticAlgorithm(_populationSize);

            // Create the player's bird first using the existing template
            Birds.Add(new Bird(templateBird, false));
            Networks.Add(CreateNetwork());

            // Create AI birds
            for (int i = 1; i < birdCount; i++)
            {
                CreateAIBird(templateBird, i);
            }
        }

        private void CreateAIBird(PictureBox templateBird, int index)
        {
            // Create a new PictureBox based on the template
            PictureBox newBirdPicture = new()
            {
                Image = new Bitmap(templateBird.Image),
                SizeMode = templateBird.SizeMode,
                BackColor = Color.Transparent,
                Size = templateBird.Size,
                Location = new Point(
                    templateBird.Left,
                    templateBird.Top + (_birdOffsetY * index)
                )
            };

            // Add the new PictureBox to the form
            _parentForm.Controls.Add(newBirdPicture);
            newBirdPicture.BringToFront();

            // Create and add a new bird
            Bird newBird = new Bird(newBirdPicture, false);
            Birds.Add(newBird);

            // Create a neural network for the AI bird
            var network = CreateNetwork();
            new NguyenWidrow(network).Randomize(); // Different initial weights for each bird
            Networks.Add(network);
        }

        private ActivationNetwork CreateNetwork()
        {
            return new ActivationNetwork(
                new SigmoidFunction(), // activation
                4,                     // inputs
                6,                     // hidden neurons
                1                      // output
            );
        }

        public void UpdateBirds((Panel pipeBot, Panel pipeTop) currentPipe, int clientHeight)
        {
            CleanupDeadBirds();
            HandleDeadBirds(clientHeight);
            for (int i = 0; i < Birds.Count; i++)
            {
                Bird bird = Birds[i];

                // Skip dead birds
                if (bird.IsDead)
                    continue;
                else
                    bird.TicksAlive++;
                // AI decision
                if (i > 0 || i == 0 && PlayerBird != null) // For AI birds or if first bird is AI-controlled
                {
                    double[] inputs = [
                        bird.Top,
                        bird.VelocityY,
                        currentPipe.pipeBot.Top,
                        currentPipe.pipeTop.Top + currentPipe.pipeTop.Height
                    ];

                    double[] output = Networks[i].Compute(inputs);

                    if (output[0] > 0.5)
                    {
                        _ = bird.Jump();
                    }
                    else
                    {
                        bird.Fall();
                    }
                }
                else
                {
                    bird.Fall(); // Player bird falls naturally, jumps controlled by key press
                }
            }
            if (Birds.All(b => b.IsDead))
            {
                EvolveNewGeneration();
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

        public void HandleDeadBirds(int clientHeight)
        {
            for (int i = Birds.Count - 1; i >= 0; i--)
            {
                Bird bird = Birds[i];

                // Check if bird hit the ground or is marked as dead
                if (bird.Top > clientHeight - bird.Height || bird.IsDead)
                {
                    bird.Die();

                    // Hide the bird instead of removing it completely
                    // This allows us to reset it later if needed
                    bird.PictureBox.Visible = false;
                }
            }
        }

        public void KillBird(Bird bird)
        {
            bird.Die();
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
                        _parentForm.Controls.Remove(Birds[i].PictureBox);
                        Birds[i].Dispose();
                    }
                    Birds.RemoveAt(i);
                    Networks.RemoveAt(i);
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
            if (currentBirdCount == 0 && _parentForm.Controls.OfType<PictureBox>().Any(pb => pb.Tag?.ToString() == "BirdTemplate"))
            {
                // Find the template bird
                PictureBox templateBird = _parentForm.Controls.OfType<PictureBox>()
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

            for (int i = 0; i < Birds.Count; i++)
            {
                // Store the network and fitness of each bird
            }
        }

        double[] GetNetworkWeights(ActivationNetwork network)
        {
            var weights = new List<double>();

            foreach (ActivationLayer layer in network.Layers)
                foreach (ActivationNeuron neuron in layer.Neurons)
                    weights.AddRange(neuron.Weights.Append(neuron.Threshold)); // include weights + bias

            return weights.ToArray();
        }
        private void EvolveNewGeneration()
        {
            // Step 1: Send fitness data to GA
            _ga.SetFitness(Birds, Networks);

            // Step 2: Evolve next generation of genomes
            var newGenomes = _ga.EvolveNextGeneration();

            // Step 3: Clear old visuals and memory
            foreach (var bird in Birds)
            {
                if (bird.PictureBox != null && !bird.PictureBox.IsDisposed)
                {
                    _parentForm.Controls.Remove(bird.PictureBox);
                    bird.Dispose();
                }
            }

            Birds.Clear();
            Networks.Clear();

            // Step 4: Recreate birds with evolved brains
            var template = _parentForm.Controls.OfType<PictureBox>()
                             .First(pb => pb.Tag?.ToString() == "BirdTemplate");

            for (int i = 0; i < _populationSize; i++)
            {
                // Create new bird visual
                PictureBox birdPicture = new()
                {
                    Image = new Bitmap(template.Image),
                    SizeMode = template.SizeMode,
                    BackColor = Color.Transparent,
                    Size = template.Size,
                    Location = new Point(template.Left, template.Top + (_birdOffsetY * i))
                };

                _parentForm.Controls.Add(birdPicture);
                birdPicture.BringToFront();

                Bird bird = new(birdPicture, false);
                Birds.Add(bird);

                // Load weights into a new network
                ActivationNetwork net = CreateNetwork();
                _ga.SetNetworkWeights(net, newGenomes[i]);
                Networks.Add(net);
            }
        }


    }
}