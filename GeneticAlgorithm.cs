using FlappyBrain.Library;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;

internal class GeneticAlgorithm
{
    private readonly int populationSize;
    private readonly Random random = new();
    public int Generation { get; private set; } = 0;
    public List<(double[][] genome, double fitness)> Population { get; private set; } = [];

    public GeneticAlgorithm(int populationSize)
    {
        this.populationSize = populationSize;
    }

    public void Initialize(List<GenomeEntry> initialPopulation)
    {
        // GenomeEntry already uses double[][] for the Genome property
        Population = [.. initialPopulation.Select(entry => (entry.Genome, entry.Fitness))];
    }

    public List<double[][]> EvolveNextGeneration(double mutationRate = 0.5, double mutationAmount = 0.5)
    {
        var sorted = Population.OrderByDescending(p => p.fitness).ToList();
        var parents = sorted.Take(populationSize / 5).ToList(); // top 50% survive
        List<double[][]> nextGeneration = new();

        // Add the parents to the new generation (elitism)
        foreach (var (genome, fitness) in parents)
        {
            nextGeneration.Add(CloneGenome(genome));
        }

        // Create children until we reach population size
        while (nextGeneration.Count < populationSize)
        {
            var parent1 = parents[random.Next(parents.Count)].genome;
            var parent2 = parents[random.Next(parents.Count)].genome;
            var child = UniformCrossover(parent1, parent2);
            Mutate(child, mutationRate, mutationAmount);
            nextGeneration.Add(child);
        }

        Generation++;
        return nextGeneration;
    }

    private double[][] CloneGenome(double[][] source)
    {
        double[][] clone = new double[source.Length][];
        for (int i = 0; i < source.Length; i++)
        {
            clone[i] = new double[source[i].Length];
            Array.Copy(source[i], clone[i], source[i].Length);
        }
        return clone;
    }

    private double[][] UniformCrossover(double[][] parent1, double[][] parent2)
    {
        var child = new double[parent1.Length][];

        for (int layer = 0; layer < parent1.Length; layer++)
        {
            child[layer] = new double[parent1[layer].Length];
            for (int i = 0; i < parent1[layer].Length; i++)
            {
                child[layer][i] = random.NextDouble() < 0.5 ? parent1[layer][i] : parent2[layer][i];
            }
        }

        return child;
    }

    private void Mutate(double[][] genome, double rate, double amount)
    {
        for (int layer = 0; layer < genome.Length; layer++)
        {
            for (int i = 0; i < genome[layer].Length; i++)
            {
                if (random.NextDouble() < rate)
                {
                    genome[layer][i] += (random.NextDouble() * 2 - 1) * amount;
                }
            }
        }
    }

    public void SetFitness(List<Bird> birds, List<FlappyBrainNetwork> networks)
    {
        Population.Clear();
        for (int i = 0; i < birds.Count; i++)
        {
            var genome = ExtractGenome(networks[i]);
            birds[i].GetVerticalDistanceToPipe();
            Population.Add((genome, birds[i].Fitness));
        }
    }

    public void AssignGenomesToNetworks(List<double[][]> newGenomes, List<FlappyBrainNetwork> networks)
    {
        for (int i = 0; i < Math.Min(newGenomes.Count, networks.Count); i++)
        {
            SetNetworkWeights(networks[i], newGenomes[i]);
        }
    }

    private double[][] ExtractGenome(FlappyBrainNetwork network)
    {
        var layers = network.Layers;
        double[][] genome = new double[layers.Length][];

        for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
        {
            var layer = layers[layerIndex];
            int rows = layer.GetLength(0);
            int cols = layer.GetLength(1);

            // Flatten the 2D matrix into a 1D array
            genome[layerIndex] = new double[rows * cols];
            int index = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    genome[layerIndex][index++] = layer[i, j];
                }
            }
        }

        return genome;
    }

    public void SetNetworkWeights(FlappyBrainNetwork network, double[][] genome)
    {
        var layers = network.Layers;

        for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
        {
            var layer = layers[layerIndex];
            int rows = layer.GetLength(0);
            int cols = layer.GetLength(1);
            int index = 0;

            // Reconstruct the 2D matrix from the flattened array
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    layer[i, j] = genome[layerIndex][index++];
                }
            }
        }
    }
}