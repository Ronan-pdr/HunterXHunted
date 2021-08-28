using System;
using System.Collections.Generic;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class LabyrintheManager : MonoBehaviour
    {
        // ------------ Serialized Field ------------
        
        [Header("Sortie")]
        [SerializeField] private Transform sortie;
        
        // ------------ Attributs ------------
        
        public static LabyrintheManager Instance;
        private RayGaz _sonde;
        private bool _isSondeFinish;
        
        // ------------ Setter ------------

        private void FinSonde()
        {
            _isSondeFinish = true;
        }

        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Sonder la zone une bonne fois pour toute
            _sonde = RayGaz.GetSonde(sortie.position, FinSonde);
        }
        
        // ------------ Méthode ------------
        public List<Vector3> GetBestPath(Vector3 pos)
        {
            if (_isSondeFinish)
            {
                return _sonde.GetBestPath(pos);
            }

            return new List<Vector3>();
        }
    }
}