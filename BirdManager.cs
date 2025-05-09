﻿using System;
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
        public List<(double[] genome, double fitness)> Population => ga.Population;
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
            for (int i = 0; i < birdCount; i++)
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

        public void Initialize(List<GenomeEntry> initialPopulation)
        {
            // Step 1: Pass genomes to GA (so it's in sync internally)
            ga.Initialize(initialPopulation);

            // Step 2: Clear old birds and networks
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

            // Step 3: Recreate birds and load networks from genomes
            foreach (var genome in initialPopulation)
            {
                // Visual
                PictureBox newBirdPicture = new()
                {
                    Image = new Bitmap(templateBird.Image),
                    SizeMode = templateBird.SizeMode,
                    BackColor = Color.Transparent,
                    Size = templateBird.Size,
                    Location = new Point(templateBird.Left, templateBird.Top)
                };

                parentForm.Controls.Add(newBirdPicture);
                newBirdPicture.BringToFront();

                Bird newBird = new(newBirdPicture, false);
                Birds.Add(newBird);

                // Network
                var network = CreateNetwork();
                Networks.Add(network);
                ga.SetNetworkWeights(network, genome.Genome); // Load ge
            }
        }
        public void UpdateBirds((Panel pipeBot, Panel pipeTop) currentPipe, int clientHeight)
        {
            //CleanupDeadBirds();
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
                    Image = new Bitmap(template.Image),
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
                ActivationNetwork net = CreateNetwork();
                ga.SetNetworkWeights(net, newGenomes[i]);
                Networks.Add(net);
            }
        }


    }
}