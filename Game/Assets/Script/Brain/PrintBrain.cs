using TMPro;
using UnityEngine;

namespace Script.Brain
{
    public class PrintBrain : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [SerializeField] private TextMeshPro[] layers;
        
        // ------------ Attributs ------------

        public static PrintBrain Instance;
        
        // ------------ Methods ------------

        public void Print(NeuralNetwork neuralNetwork)
        {
            
        }
    }
}