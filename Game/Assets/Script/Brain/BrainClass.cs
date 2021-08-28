using System;
using System.IO;
using Script.EntityPlayer;
using Script.Graph;
using Script.MachineLearning;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using Random = System.Random;

namespace Script.Brain
{
    public abstract class BrainClass
    {
        // ------------ Attributs ------------

        protected HumanCapsule Capsule;
        protected NeuralNetwork Neurones;
        private Random _rnd;
        
        // ------------ Constructeur ------------

        private void SetAttributs()
        {
            Capsule = MasterManager.Instance.GetHumanCapsule();
            _rnd = new Random();
        }

        protected void Set()
        {
            NewNeuralNetwork();
            SetAttributs();
        }
        
        protected void Set(int numero)
        {
            string path = GetPath(numero);

            if (File.Exists(path))
            {
                Neurones = NeuralNetwork.Restore(path);
            }
            else
            {
                Debug.Log($"Le numéro {numero} du {GetNameDirectory()} du saut n'existe pas");
                NewNeuralNetwork();
            }

            SetAttributs();
        }

        protected abstract void NewNeuralNetwork();


        // ------------ Public Methods ------------
        
        public void UpdateNeurones(BrainClass brain1, BrainClass brain2)
        {
            switch (_rnd.Next(2))
            {
                case 0:
                    // le muter
                    Neurones = new NeuralNetwork(brain1.Neurones, true);
                    break;
                default:
                    // enfanter
                    Neurones = new NeuralNetwork(brain1.Neurones, false);
                    Neurones.Crossover(brain2.Neurones);
                    break;
            }
        }

        public void Save(int numero)
        {
            Neurones.Save(GetPath(numero));
        }
        
        // ------------ Detection (hauteur d'un obstacle) ------------
        
        // avec des line cast
        
        protected (double _, double height) GetDistHeightFirstObstacle(Vector3 depart, Vector3 fin)
        {
            return GetStaticDistHeightFirstObstacle(depart, fin, Capsule);
        }

        public static (double dist, double height) GetStaticDistHeightFirstObstacle(Vector3 depart, Vector3 fin, HumanCapsule capsule)
        {
            float decoupage = 10;
            float minDist = Calcul.Distance(depart, fin);
            float height = 0;
            float ecart = capsule.Height / decoupage;
            Vector3 pos1 = 1 * depart;
            Vector3 pos2 = 1 * fin;
            pos2.y = pos1.y;

            // trouver la hauteur et la distance du premier obstacle
            for (int i = 2; i < decoupage; i++)
            {
                pos1 += Vector3.up * ecart;
                pos2 += Vector3.up * ecart;
                
                if (Physics.Linecast(pos1, pos2, out RaycastHit hit) && hit.distance <= minDist + 0.1f)
                {
                    //Line.Create(pos1, hit.point, 250);
                    
                    minDist = hit.distance;
                    height = pos1.y - depart.y;
                }
            }

            return (minDist, height);
        }
        
        // avec des raycast
        
        protected (double dist, double height) GetDistHeightFirstObstacle(Transform tr, double distMax)
        {
            return GetStaticDistHeightFirstObstacle(tr, distMax, Capsule);
        }
        
        public static (double dist, double height) GetStaticDistHeightFirstObstacle(Transform tr, double distMax, HumanCapsule capsule)
        {
            float decoupage = 10;
            float minDist = (float)distMax;
            float height = 0;
            float ecart = capsule.Height / decoupage;
            Vector3 pos = tr.position;
            
            Ray ray = new Ray(pos + Vector3.forward * 0.1f, tr.TransformDirection(Vector3.forward));
            
            // trouver la hauteur et la distance du premier obstacle
            for (int i = 2; i < decoupage; i++)
            {
                ray.origin += Vector3.up * ecart;

                if (Physics.Raycast(ray, out RaycastHit hit, minDist))
                {
                    //Line.Create(ray.origin, hit.point, 250);
                    minDist = hit.distance;
                    height = ray.origin.y - pos.y;
                }
            }

            return (minDist, height);
        }

        // ------------ Protected Methods ------------

        protected string GetPath(int numero) => $"Build/{GetNameDirectory()}/{numero}";

        protected abstract string GetNameDirectory();

        protected int BoolToInt(bool value) => value ? 1 : 0;

        protected double[] GetResult(NeuralNetwork neuralNetwork, double[] input)
        {
            // feed et enclencher les neurones
            neuralNetwork.Feed(input);
            neuralNetwork.FrontProp();
            
            // retourner le résultat
            return neuralNetwork.GetResult();
        }
        
        protected void ErrorInput(double[] input)
        {
            // vérifier qu'il n'a pas de problème avec les valeurs de l'input
            int l = input.Length;
            for (int i = 0; i < l; i++)
            {
                if (input[i] < -0.1 || input[i] > 1.1)
                {
                    //Debug.Log($"input[{i}] = {input[i]}");
                }
            }
        }

        protected int Max(double[] output)
        {
            int l = output.Length;

            if (l == 0)
            {
                throw new ArgumentException("L'output ne peut être vide");
            }
            
            (int index, double value) max = (0, output[0]);

            for (int i = 1; i < l; i++)
            {
                if (output[i] > max.value)
                {
                    max = (i, output[i]);
                }
            }

            return max.index;
        }
    }
}