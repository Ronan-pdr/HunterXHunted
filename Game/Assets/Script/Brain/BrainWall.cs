using UnityEngine;

namespace Script.Brain
{
    public class BrainWall : BrainClass
    {
        // ------------ Attributs ------------
        
        // sauvegarde
        public const string NameDirectory = "SauvegardeNeuroneDetectionMur";

        // ------------ Getter ------------

        protected override string GetNameDirectory() => NameDirectory;
        
        // ------------ Constructeur ------------

        public BrainWall()
        {
            Set();
        }

        public BrainWall(int numero)
        {
            Set(numero);
        }
        
        protected override void NewNeuralNetwork()
        {
            // 1 entrée :
            // - la hauteur du premier obstacle
            
            // 1 sortie :
            // - est-ce un obstacle infranchissable ?
            
            Neurones = new NeuralNetwork(new[] {1, 1});
        }
        
        // ------------ Methods ------------

        // à partir de raycast
        public bool IsThereWall(Transform tr, float distMaxDetection)
        {
            // recupérer les infos par rapport aux obstacles
            (_, double height) = GetDistHeightFirstObstacle(tr, distMaxDetection);

            return IsThereWall(height);
        }
        
        // à partir de linecast
        public bool IsThereWall(Vector3 depart, Vector3 fin)
        {
            // recupérer les infos par rapport aux obstacles
            (_, double height) = GetDistHeightFirstObstacle(depart, fin);

            return IsThereWall(height);
        }
        
        public bool IsThereWall(double height)
        {
            // former l'input
            double[] input = {height};
            
            // vérifier que toutes les valeurs sont entre -1 et 1
            ErrorInput(input);
            
            // récupérer l'output
            double[] output = GetResult(Neurones, input);
            
            // interpréter l'output
            return output[0] > 0.6d;
        }
    }
}