using System.IO;
using UnityEngine;

namespace Script.Brain
{
    public class BrainJump : BrainClass
    {
        // ------------ Attributs ------------
        
        // sauvegarde
        public const string NameDirectory = "SauvegardeNeuroneSaut";

        private const double MaxDistJump = 3;
        
        // ------------ Getter ------------

        protected override string GetNameDirectory() => NameDirectory;

        // ------------ Constructeur ------------

        public BrainJump()
        {
            Set();
        }

        public BrainJump(int numero)
        {
            Set(numero);
        }
        
        protected override void NewNeuralNetwork()
        {
            // 3 entrées :
            // - la distance entre le bot et l'obstacle
            // - la hauteur du premier obstacle
            // - la vitesse du bot
            
            // 1 sortie :
            // - should jump
            
            Neurones = new NeuralNetwork(new[] {3, 1});
        }
        
        // ------------ Methods ------------

        public bool JumpNeeded(Transform tr, float speed, float maxSpeed)
        {
            // recupérer les infos par rapport aux obstacles
            (double minDist, double height) = GetDistHeightFirstObstacle(tr, MaxDistJump);

            return JumpNeeded(minDist, height, speed, maxSpeed);
        }

        public bool JumpNeeded(double minDist, double height, float speed, float maxSpeed)
        {
            // former l'input
            double[] input = {minDist / MaxDistJump, height / Capsule.Height, speed / maxSpeed};

            // vérifier que toutes les valeurs sont entre -1 et 1
            ErrorInput(input);
            
            // récupérer l'output
            double[] output = GetResult(Neurones, input);

            // interpréter l'output
            return output[0] > 0.5d;
        }
    }
}