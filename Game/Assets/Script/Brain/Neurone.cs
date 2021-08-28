using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Script.Brain
{
    public class Neurone
    {
        // ------------ Attributs ------------
        
        /// <summary>
        /// Value of the neurone after the propagation
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Weights of the neurone
        /// </summary>
        public double[] Weights { get; }

        /// <summary>
        /// Bias of the neurones
        /// </summary>
        public double Bias { get; set; }

        private Random _rnd;
        
        // ------------ Constructeur ------------
        
        /// <summary>
        /// Create new neurone
        /// </summary>
        /// <param name="prevSize"> size of the last layer </param>
        public Neurone(int prevSize)
        {
            _rnd = new Random();
            
            Weights = new double[prevSize];
            for (int i = 0; i < prevSize; i++)
            {
                Weights[i] = Aux();
            }

            Bias = Aux();

            double Aux()
            {
                return _rnd.NextDouble() * 2 - 1;
            }
        }

        /// <summary>
        /// Mutate a neurone
        /// </summary>
        /// <param name="neurone"> old neurone </param>
        /// <param name="mutate"> apply mutation </param>
        public Neurone(Neurone neurone, bool mutate)
        {
            _rnd = new Random();
            
            Value = 0;

            int l = neurone.Weights.Length;
            Weights = new double[l];
            for (int i = 0; i < l; i++)
            {
                Weights[i] = neurone.Weights[i];
            }

            Bias = neurone.Bias;

            if (mutate)
            {
                Mutate();
            }
        }

        // ------------ Methodes ------------
        
        /// <summary>
        /// Random function using Box-Muller transform
        /// </summary>
        /// <returns></returns>
        private double RandomGaussian()
        {
            var u1 = -2.0 * Math.Log(_rnd.NextDouble());
            var u2 = 2.0 * Math.PI * _rnd.NextDouble();
            return Math.Sqrt(u1) * Math.Cos(u2);
        }

        /// <summary>
        /// Apply the mutation depending on a chosen algorithm
        /// </summary>
        /// <param name="probability"> probability of applying the mutation </param>
        /// <returns> true to mutate, false otherwise </returns>
        private bool ShouldMutate(double probability) => _rnd.Next(1000) < probability * 1000;

        /// <summary>
        /// Mutate the neurone
        /// </summary>
        public void Mutate()
        {
            int l = Weights.Length;
            for (int i = 0; i < l; i++)
            {
                if (ShouldMutate(0.4))
                {
                    Weights[i] += RandomGaussian();
                }
            }

            if (ShouldMutate(0.2))
            {
                Bias += RandomGaussian();
            }
        }

        /// <summary>
        /// Mix the neurone with its partner
        /// </summary>
        /// <param name="partner"> the partner to be mixed with </param>
        public void Crossover(Neurone partner)
        {
            int l = Weights.Length;
            
            List<int> random = new List<int>();
            for (int i = 0; i < l; i++)
            {
                random.Add(i);
            }

            // 50 % des gênes de l'autre
            for (int i = l / 2; i >= 0 && random.Count > 0; i--)
            {
                int indexToSuppr = _rnd.Next(random.Count);
                
                int rndIndex = random[indexToSuppr];

                Weights[rndIndex] = partner.Weights[rndIndex];
                
                random.RemoveAt(indexToSuppr);
            }
        }

        /// <summary>
        /// Apply the front propagation to the current neurone and update its value
        /// </summary>
        /// <param name="prevLayer"></param>
        public void FrontProp(Layer prevLayer)
        {
            int l = Weights.Length;
            Neurone[] neurones = prevLayer.Neurones;
            if (l != neurones.Length)
            {
                throw new Exception();
            }

            Value = Bias;
            for (int i = 0; i < l; i++)
            {
                Value += neurones[i].Value * Weights[i];
            }

            Value = Activation(Value);
        }

        /// <summary>
        /// Activation function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double Activation(double x)
        {
            //return 1 / (1 + Math.Exp(-x));
            return Math.Log(1 + Math.Exp(x));
        }
        
        // ------------ Surchargeur ------------

        public override string ToString()
        {
            int entier = (int)Value;
            int relatif = (int)((Value - entier) * 100);

            return $"{entier},{relatif}";
        }
    }
}