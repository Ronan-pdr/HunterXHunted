using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Script.MachineLearning
{
    public class TerrainDeplacement : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [SerializeField] private Transform depart;
        [SerializeField] private Transform arrive;
        
        // ------------ Attributs ------------
        
        // indicateur
        private const int TimeMax = 30;
        private Stopwatch _chrono;

        // entrainement
        private EntrainementDeplacement _entrainementDeplacement;

        // ------------ Getter ------------

        public int TimeToScore(bool achieve)
        {
            _chrono.Stop();

            // - s'il a réussi, moins il met de temps mieux c'est
            if (achieve)
            {
                return (TimeMax - (int) (_chrono.ElapsedMilliseconds / 1000)) * 3;
            }
            
            // - s'il n'a pas réussi, plus il reste en vie mieux c'est
            return (int)(_chrono.ElapsedMilliseconds / 1000);
        }

        public Vector3 Arrive => arrive.position;
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            _chrono = new Stopwatch();
        }

        // ------------ Methods ------------
        
        public void BeginTraining(EntrainementDeplacement entrainement)
        {
            _entrainementDeplacement = entrainement;
            _chrono.Restart();

            InvokeRepeating(nameof(ForceEnd), 0, 1);
        }
        
        public void ForceEnd()
        {
            if (_chrono.IsRunning && _chrono.ElapsedMilliseconds / 1000 > TimeMax)
            {
                _entrainementDeplacement.NextField(false);
            }
        }

        public void Teleportation(Transform tr)
        {
            tr.position = depart.position;
            tr.rotation = arrive.rotation;
        }
    }
}