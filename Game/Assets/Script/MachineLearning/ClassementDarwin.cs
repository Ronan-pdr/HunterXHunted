using System;
using System.IO;
using TMPro;
using UnityEngine;
using Random = System.Random;
using Script.Brain;

namespace Script.MachineLearning
{
    public class ClassementDarwin : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Canvas")]
        [SerializeField] private GameObject menuTab;
        [SerializeField] private TextMeshProUGUI[] zonesTexte;

        [Header("Sauvegarde")]
        [SerializeField] private bool mustRecoverSave;
        
        // ------------ Attributs ------------

        private Entrainement[] _zoneEntrainement;
        private int _nZone;
        
        private (BrainClass brain, int score)[] _classement;

        private Random _rnd;

        private string _nameDirectory;

        // ------------ Constructeur ------------

        private void Awake()
        {
            _zoneEntrainement = GetComponentsInChildren<Entrainement>();
            _nameDirectory = _zoneEntrainement[0].GetNameDirectory();
            _nZone = _zoneEntrainement.Length;
            
            foreach (Entrainement zone in _zoneEntrainement)
            {
                zone.SetClassement(this);
            }

            _rnd = new Random();
        }

        private void Start()
        {
            _classement = new (BrainClass, int)[_nZone];
            
            if (mustRecoverSave)
            {
                for (int i = 0; i < _nZone; i++)
                {
                    _classement[i].brain = _zoneEntrainement[i].Bot.SetBrain(i);
                }
            }
            else
            {
                for (int i = 0; i < _nZone; i++)
                {
                    _classement[i].brain = _zoneEntrainement[i].Bot.SetBrain();
                }
            }

            menuTab.SetActive(false);
        }
        
        // ------------ Update ------------

        private void Update()
        {
            // le menu tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // afficher le classement
                menuTab.SetActive(true);
                UpdateAffichageClassement();
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                menuTab.SetActive(false);
            }
            
            // la sauvegarde
            if (Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("Sauvegarde !!!!!!");
                
                string path = $"Build/{_nameDirectory}";
                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                
                for (int i = 0; i < _nZone; i++)
                {
                    _classement[i].brain.Save(i);
                }
            }
        }
        
        // ------------ Public Methods ------------

        public void EndEpreuve(BrainClass brain, int score)
        {
            int i = _nZone - 1;
            
            if (score >= _classement[i].score)
            {
                // le réseau neurones est assez bon pour rentrer dans le classement
                
                for (; i > 0 && score >= _classement[i - 1].score; i--)
                {
                    // décaler tous les neurones inférieurs
                    _classement[i] = _classement[i - 1];
                }

                // insérer le nouveau cerveau dans le tableau
                _classement[i] = (brain, score);
                
                UpdateAffichageClassement();
            }
            
            // faire la somme des scores
            int sum = 0;
            for (i = 0; i < _nZone; i++)
            {
                sum += _classement[i].score;
            }

            BrainClass brain1 = SelectNeuralNetwork(sum);
            BrainClass brain2 = SelectNeuralNetwork(sum);

            brain.UpdateNeurones(brain1, brain2);
        }
        
        // ------------ Private Methods ------------

        private void UpdateAffichageClassement()
        {
            int l = zonesTexte.Length;
            int scorePerLine = _nZone / l + 1;

            for ((int i, int j) = (0, 0); i < l; i++)
            {
                zonesTexte[i].text = "";
                
                for (int k = scorePerLine; k > 0 && j < _nZone; k--, j++)
                {
                    zonesTexte[i].text += $"{j}. {_classement[j].score}" + Environment.NewLine;
                }
            }
        }
        
        private BrainClass SelectNeuralNetwork(double fitnessSum)
        {
            if (fitnessSum == 0)
            {
                return _classement[0].brain;
            }
            
            int r = _rnd.Next((int)fitnessSum);
            long s = 0;

            for (int i = 0; i < _nZone; i++)
            {
                s += _classement[i].score;
                
                if (r < s)
                {
                    return _classement[i].brain;
                }
            }

            throw new Exception($"fitnessSum = {fitnessSum} ; r = {r} ; s = {s}");
        }
    }
}