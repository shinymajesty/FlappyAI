using Accord.Neuro;
using Accord.Neuro.Networks;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;

internal class GeneticAlgorithm
{
    private readonly int populationSize;
    private readonly Random random = new();

    public int Generation { get; private set; } = 0;
    public List<(double[] genome, double fitness)> Population { get; private set; } = [];

    public GeneticAlgorithm(int populationSize)
    {
        this.populationSize = populationSize;
    }

    public void Initialize (List<GenomeEntry> initialPopulation)
    {
        Population = [.. initialPopulation.Select(entry => (entry.Genome, entry.Fitness))];
    }
    public void Initialize(List<(double[] genome, double fitness)> initialPopulation)
    {
        Population = initialPopulation;
    }

    public List<double[]> EvolveNextGeneration(double mutationRate = 0.05, double mutationAmount = 0.3)
    {
        var sorted = Population.OrderByDescending(p => p.fitness).ToList();
        var parents = sorted.Take(populationSize / 2).ToList(); // top 20% survive

        List<double[]> nextGeneration = new();
        foreach (var (genome, fitness) in parents)
        {
            nextGeneration.Add(genome);
        }

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

    private double[] UniformCrossover(double[] parent1, double[] parent2)
    {
        var child = new double[parent1.Length];
        for (int i = 0; i < parent1.Length; i++)
        {
            child[i] = random.NextDouble() < 0.5 ? parent1[i] : parent2[i];
        }
        return child;
    }

    private void Mutate(double[] genome, double rate, double amount)
    {
        for (int i = 0; i < genome.Length; i++)
        {
            if (random.NextDouble() < rate)
            {
                genome[i] += (random.NextDouble() * 2 - 1) * amount;
            }
        }
    }

    public void SetFitness(List<Bird> birds, List<ActivationNetwork> networks)
    {
        Population.Clear();
        for (int i = 0; i < birds.Count; i++)
        {
            var genome = ExtractGenome(networks[i]);
            Population.Add((genome, birds[i].Fitness));
        }
    }

    public void AssignGenomesToNetworks(List<double[]> newGenomes, List<ActivationNetwork> networks)
    {
        for (int i = 0; i < newGenomes.Count; i++)
        {
            SetNetworkWeights(networks[i], newGenomes[i]);
        }
    }

    private double[] ExtractGenome(ActivationNetwork network)
    {
        List<double> weights = new();
        foreach (var layer in network.Layers)
        {
            foreach (ActivationNeuron neuron in layer.Neurons)
            {
                weights.AddRange(neuron.Weights);
                weights.Add(neuron.Threshold);
            }
        }
        return weights.ToArray();
    }

    public void SetNetworkWeights(ActivationNetwork network, double[] genome)
    {
        int index = 0;
        foreach (ActivationLayer layer in network.Layers)
        {
            foreach (ActivationNeuron neuron in layer.Neurons)
            {
                for (int i = 0; i < neuron.Weights.Length; i++)
                {
                    neuron.Weights[i] = genome[index++];
                }
                neuron.Threshold = genome[index++];
            }
        }
    }
    

}
