using System;
using System.IO;

namespace Script.Brain
{
    public class NeuralNetwork
    {
        // ------------ Attributs ------------
        private Layer[] Layers { get; }
        
        // ------------ Getter ------------

        public double[] GetResult()
        {
            Neurone[] neurones = Layers[Layers.Length - 1].Neurones;
            int l = neurones.Length;
            double[] values = new double[l];

            for (int i = 0; i < l; i++)
            {
                values[i] = neurones[i].Value;
            }

            return values;
        }

        public string[] ToStrings()
        {
            int l = Layers.Length;
            string[] res = new string[l];

            for (int i = 0; i < l; i++)
            {
                res[i] = Layers[i].ToString();
            }

            return res;
        }
        
        // ------------ Constructeur ------------
       
        /// <summary>
        /// Create a neural network
        /// </summary>
        /// <param name="layerDimension"> Dimension of the neural network </param>
        public NeuralNetwork(int[] layerDimension)
        {
            int l = layerDimension.Length;
            if (l < 2)
            {
                throw new Exception();
            }
            
            Layers = new Layer[l];
            Layers[0] = new Layer(layerDimension[0], 0);
            for (int i = 1; i < l; i++)
            {
                Layers[i] = new Layer(layerDimension[i], layerDimension[i-1]);
            }
        }

        /// <summary>
        /// Create a new neural network with mutation
        /// </summary>
        /// <param name="neuralNetwork"> old neural network </param>
        /// <param name="mutate"> apply mutation </param>
        public NeuralNetwork(NeuralNetwork neuralNetwork, bool mutate)
        {
            int l = neuralNetwork.Layers.Length;
            Layers = new Layer[l];
            
            for (int i = 0; i < l; i++)
            {
                Layers[i] = new Layer(neuralNetwork.Layers[i], mutate);
            }
        }

        /// <summary>
        /// Feed the neural network with the current bird's state
        /// </summary>
        /// <param name="input"></param>
        public void Feed(double[] input)
        {
            Neurone[] neurones = Layers[0].Neurones;
            int l = neurones.Length;
            
            if (input.Length != l)
            {
                throw new Exception($"lenght = {input.Length} ; l = {l}");
            }

            for (int i = 0; i < l; i++)
            {
                neurones[i].Value = input[i];
            }
        }

        /// <summary>
        /// Apply the front propagation to the neural network
        /// </summary>
        public void FrontProp()
        {
            int l = Layers.Length;
            for (int i = 1; i < l; i++)
            {
                Layers[i].FrontProp(Layers[i-1]);
            }
        }

        /// <summary>
        /// Mix the neural network with a partner
        /// </summary>
        /// <param name="partner"> the partner to be mixed with </param>
        public void Crossover(NeuralNetwork partner)
        {
            int l = Layers.Length;
            for (int i = 0; i < l; i++)
            {
                if (Layers[i].Neurones.Length != partner.Layers[i].Neurones.Length)
                {
                    throw new Exception();
                }

                Layers[i].Crossover(partner.Layers[i]);
            }
        }

        /// <summary>
        /// Mutate the current neural network
        /// </summary>
        public void Mutate()
        {
            foreach (Layer layer in Layers)
            {
                layer.Mutate();
            }
        }

        /// <summary>
        /// Save the neural network in a file
        /// </summary>
        /// <param name="path"> path of the file </param>
        public void Save(string path)
        {
            var format = "";
            foreach (var layer in Layers)
            {
                format = layer.Neurones.Length + " " + format;
                format += '\n';
                foreach (var neurone in layer.Neurones)
                {
                    format = format + '\n' + neurone.Bias + " ";
                    foreach (var weight in neurone.Weights) 
                        format = format + weight + " ";
                }
            }

            format += "\n\n\n";
            File.WriteAllText(path, format);
        }

        /// <summary>
        /// Get information of the neural network from a file
        /// </summary>
        /// <param name="format"> string of the while file </param>
        /// <param name="i"> current position in the file </param>
        /// <returns></returns>
        private static int[] GetSizes(string format, ref int i)
        {
            var nbLayers = 0;
            for (var j = 0; format[j] != 0 && format[j] != '\n'; j++)
                if (format[j] == ' ')
                    nbLayers++;

            var sizes = new int[nbLayers];
            nbLayers--;

            while (format[i] != 0 && format[i] != '\n') //Sizes
            {
                var end = i;
                while (format[end] != ' ') //Layer's Size
                    end++;

                sizes[nbLayers] = int.Parse(format.Substring(i, end - i));

                nbLayers--;
                i = end + 1;
            }

            i++;
            return sizes;
        }

        /// <summary>
        /// create a neural network from a file
        /// </summary>
        /// <param name="path"> path of the file </param>
        /// <returns> fully working brain </returns>
        public static NeuralNetwork Restore(string path)
        {
            var format = File.ReadAllText(path);
            var i = 0;
            var sizes = GetSizes(format, ref i);
            var network = new NeuralNetwork(sizes);
            i++;

            var layer = 0;
            while (format[i] != 0 && format[i] != '\n') // Layers
            {
                var neurone = 0;
                while (format[i] != 0 && format[i] != '\n') // Neurones
                {
                    var j = i;
                    while (format[j] != ' ') // Biais
                        j++;
                    
                    network.Layers[layer].Neurones[neurone].Bias = double.Parse(format.Substring(i, j - i));
                    i = j + 1;

                    var weight = 0;
                    while (format[i] != 0 && format[i] != '\n') //Weights
                    {
                        j = i;
                        while (format[j] != ' ')
                            j++;
                        network.Layers[layer].Neurones[neurone].Weights[weight] =
                            double.Parse(format.Substring(i, j - i));
                        i = j + 1;
                        weight++;
                    }

                    neurone++;
                    i++;
                }

                layer++;
                i++;
            }

            return network;
        }
    }
}