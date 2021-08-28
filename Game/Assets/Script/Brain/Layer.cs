using System;

namespace Script.Brain
{
    public class Layer
    {
        public Neurone[] Neurones { get; }

        /// <summary>
        /// Create new layer of neurones
        /// </summary>
        /// <param name="size"> Dimension of the current layer </param>
        /// <param name="prevSize"> Dimension of the previous layer </param>
        public Layer(int size, int prevSize)
        {
            Neurones = new Neurone[size];

            for (int i = 0; i < size; i++)
            {
                Neurones[i] = new Neurone(prevSize);
            }
        }

        /// <summary>
        /// Mutate a layer
        /// </summary>
        /// <param name="layer"> Layer to mutate </param>
        /// <param name="mutate"> Apply mutation </param>
        public Layer(Layer layer, bool mutate)
        {
            int size = layer.Neurones.Length;
            Neurones = new Neurone[size];
            
            for (int i = 0; i < size; i++)
            {
                Neurones[i] = new Neurone(layer.Neurones[i], mutate);
            }
        }

        /// <summary>
        /// Mutate the current layer
        /// </summary>
        public void Mutate()
        {
            foreach (Neurone neurone in Neurones)
            {
                neurone.Mutate();
            }
        }

        /// <summary>
        /// Mix the current layer with a partner layer
        /// </summary>
        /// <param name="partner"></param>
        public void Crossover(Layer partner)
        {
            int l = Neurones.Length;
            if (l != partner.Neurones.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < l; i++)
            {
                Neurones[i].Crossover(partner.Neurones[i]);
            }
        }

        /// <summary>
        /// Apply the propagation function to the layer
        /// </summary>
        /// <param name="prevLayer"> Dimension of the previous layer</param>
        public void FrontProp(Layer prevLayer)
        {
            foreach (Neurone neurone in Neurones)
            {
                neurone.FrontProp(prevLayer);
            }
        }
        
        // ------------ Surchargeur ------------

        public override string ToString()
        {
            string res = "";
            res += Neurones[0].ToString();
            
            int l = Neurones.Length;
            for (int i = 1; i < l; i++)
            {
                res += Environment.NewLine + Neurones[i];
            }

            return res;
        }
    }
}