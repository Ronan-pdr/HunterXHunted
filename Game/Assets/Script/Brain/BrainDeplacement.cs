using UnityEngine;

namespace Script.Brain
{
    public class BrainDeplacement : BrainClass
    {
        // ------------ Attributs ------------

        public enum Output
        {
            Avancer = 0,
            TournerHoraire = 1,
            TournerTrigo = 2
        }
        
        // sauvegarde
        public const string NameDirectory = "SauvegardeNeuroneDeplacement";
        
        // brain
        private BrainWall _brainWall;

        // ------------ Getter ------------

        protected override string GetNameDirectory() => NameDirectory;
        
        // ------------ Constructeur ------------

        public BrainDeplacement()
        {
            Set();
            _brainWall = new BrainWall(0);
        }

        public BrainDeplacement(int numero)
        {
            Set(numero);
            _brainWall = new BrainWall(0);
        }
        
        protected override void NewNeuralNetwork()
        {
            // 4 entrées :
            // - la différence de position entre la destination et sa position (en z)
            // - la différence de position entre la destination et sa position (en x)
            // - sa rotation (en z)
            // - obstacle à 12h (booléen)

            // 3 sorties :
            // - avancer
            // - tourner sens horaire
            // - tourner sens trigo

            Neurones = new NeuralNetwork(new []{4, 3});
        }
        
        // ------------ Methods ------------

        public Output WhatDeplacementShouldDo(Transform tr, Vector3 destination)
        {
            bool thereIsObstacle = _brainWall.IsThereWall(tr, 4); 
            
            return WhatDeplacementShouldDo(tr, destination, thereIsObstacle);
        }
        
        public Output WhatDeplacementShouldDo(Transform tr, Vector3 destination, bool obstacle)
        {
            Vector3 ownPos = tr.position;
            
            // former l'input
            double[] input =
            {
                DiffToInput(destination.z, ownPos.z),
                DiffToInput(destination.x, ownPos.x),
                tr.rotation.z,
                BoolToInt(obstacle)
            };
            
            // vérifier que toutes les valeurs sont entre -1 et 1
            ErrorInput(input);
            
            // récupérer l'output
            double[] output = GetResult(Neurones, input);
            
            //Debug.Log($"{output[0]} ; {output[1]} ; {output[2]}");
            
            // interpréter l'output
            return (Output)Max(output);
        }

        private double DiffToInput(float a, float b)
        {
            float diff = a - b;
            
            return diff > 0 ? 1 : diff == 0 ? 0.5d : 0;
        }
    }
}