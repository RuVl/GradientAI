namespace AI;

public class NeuralNetwork
{
    /// <summary>
    /// Weights for neural network
    /// [output x input] x layer
    /// </summary>
    public double[][,] Weights = null!;

    /// <summary>
    /// Calculation stages (input, hidden layers, output)
    /// [neuron] x layer
    /// </summary>
    protected double[][] Neurons = null!;

    /// <summary>
    /// Function activation
    /// </summary>
    protected readonly Func<double, double> Activation;

    /// <summary>
    /// Derivative of the function activation
    /// </summary>
    protected readonly Func<double, double> Derivative;

    public NeuralNetwork(Func<double, double> activation, Func<double, double> derivative, IReadOnlyList<(int y, int x)> layersWeightsSize)
    {
        Activation = activation;
        Derivative = derivative;

        GenerateWeights(layersWeightsSize);
    }

    public NeuralNetwork(IReadOnlyList<(int y, int x)> layersWeightsSize) : this(x => 1 / (1 + Math.Exp(-x)), x => x * (1 - x), layersWeightsSize) { }
    
    /// <summary>
    /// Generate weights for each layer
    /// </summary>
    /// <param name="layersWeightsSize">(output, input) x layers</param>
    protected void GenerateWeights(IReadOnlyList<(int output, int input)> layersWeightsSize)
    {
        int layers = layersWeightsSize.Count;
        var random = new Random();

        Weights = new double[layers][,];
        Neurons = new double[layers + 1][];

        for (var layer = 0; layer < layers; layer++)
        {
            int inputNeurons = layersWeightsSize[layer].input;
            int outputNeurons = layersWeightsSize[layer].output;

            if (layer > 0 && inputNeurons < layersWeightsSize[layer - 1].output)
            {
                throw new ArgumentException($"Input of {layer} layer can't be lower output of {layer - 1} layer.");
            }
            
            Weights[layer] = new double[outputNeurons, inputNeurons];
            for (var y = 0; y < outputNeurons; y++)
            {
                for (var x = 0; x < inputNeurons; x++)
                {
                    Weights[layer][y, x] = random.NextDouble();
                }
            }
        }
    }

    /// <summary>
    /// Pass AI with input and return last stage (output)
    /// </summary>
    /// <param name="input">Input for neural network</param>
    /// <returns>Output of neural network depends of current weights</returns>
    /// <exception cref="ArgumentException">Input lenght is not great than weights input lenght.</exception>
    public virtual double[] FeedForward(double[] input)
    {
        if (input.Length > Weights[0].GetLength(1))
            throw new ArgumentException("Input lenght isn't expected");

        Neurons[0] = input;
        
        for (var layer = 0; layer < Weights.Length; layer++)
        {
            Neurons[layer + 1] = new double[Weights[layer].GetLength(0)];
            for (var y = 0; y < Weights[layer].GetLength(0); y++)
            {
                for (var x = 0; x < Weights[layer].GetLength(1); x++)
                {
                    if (x < Neurons[layer].Length) // Input from previous layer
                    {
                        Neurons[layer + 1][y] += Neurons[layer][x] * Weights[layer][y, x];
                    }
                    else // Bias input
                    {
                        Neurons[layer + 1][y] += Weights[layer][y, x];
                    }
                }

                Neurons[layer + 1][y] = Activation(Neurons[layer + 1][y]); // Activating neuron
            }
        }

        return Neurons[^1];
    }

    /// <summary>
    /// Train neural network based on last FeedForward() results and expected output
    /// </summary>
    /// <param name="target">Expected output</param>
    /// <param name="epsilon">Epsilon</param>
    /// <exception cref="ArgumentException">If target lenght isn't equals to output lenght</exception>
    public virtual void BackPropagation(double[] target, double epsilon = 0.7)
    {
        if (target.Length != Neurons[^1].Length)
            throw new ArgumentException("Target lenght must be equals to output lenght.");
        
        //? Calculating deltas
        var deltas = new double[Weights.Length][]; // Calculating deltas for each neuron except input layer

        // Neurons without output weights
        deltas[^1] = new double[target.Length];
        for (var x = 0; x < target.Length; x++)
        {
            deltas[^1][x] = (target[x] - Neurons[^1][x]) * Derivative(Neurons[^1][x]);
        }
        
        // Neurons with output weights
        for (int layer = Weights.Length - 1; layer >= 1; layer--)
        {
            deltas[layer - 1] = new double[Weights[layer - 1].GetLength(0)]; // Don't calculate for bias
            for (var x = 0; x < deltas[layer-1].Length; x++)
            {
                for (var y = 0; y < Weights[layer].GetLength(0); y++)
                {
                    deltas[layer - 1][x] += deltas[layer][y] * Weights[layer][y, x];
                }

                deltas[layer - 1][x] *= Derivative(Neurons[layer][x]);
            }
        }
        
        //? Calculating gradients and change weights
        for (var layer = 0; layer < Weights.Length; layer++)
        {
            for (var y = 0; y < Weights[layer].GetLength(0); y++)
            {
                for (var x = 0; x < Weights[layer].GetLength(1); x++)
                {
                    double gradient = x < Neurons[layer].Length ? Neurons[layer][x] * deltas[layer][y] : deltas[layer][y];
                    
                    Weights[layer][y, x] += epsilon * gradient; //* Without last deltaWeights
                }
            }
        }
    }
}