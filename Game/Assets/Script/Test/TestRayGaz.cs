using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Test
{
    public class TestRayGaz : Entity
    {
        // ------------ SerializeField ------------
        
        [SerializeField] private Transform depart;
        [SerializeField] private Transform destination;

        // ------------ Attributs ------------
        
        public enum Couleur
        {
            Red,
            Yellow,
            Brown
        }

        public static TestRayGaz Instance; 

        private List<Vector3> _path;
        private List<GameObject> _gameObjects;
        
        // ------------ Setter ------------

        public void SetGameOject(List<GameObject> value)
        {
            _gameObjects = value;
            Invoke(nameof(Effacer), 1.9f);
        }
        
        // ------------ Constructeur ------------

        private void Start()
        {
            Instance = this;
            
            CreateMarqueur(depart.position, Couleur.Brown);
            CreateMarqueur(destination.position, Couleur.Yellow);
            
            Invoke(nameof(Begin), 1);
        }

        private void Begin()
        {
            enabled = false;
            RayGaz.GetPath(depart.position, destination.position, RecepRayGaz);
        }

        // ------------ Méthodes ------------
        
        private void RecepRayGaz(List<Vector3> path)
        {
            _path = path;
            Invoke(nameof(Print), 2);
        }

        private void Print()
        {
            for (int i = _path.Count - 2; i > 0; i--)
            {
                CreateMarqueur(_path[i], Couleur.Red);
            }
        }
        
        public void Effacer()
        {
            foreach (var g in _gameObjects)
            {
                Destroy(g);
            }
        }

        public static GameObject CreateMarqueur(Vector3 position, Couleur couleur, float scale = 1)
        {
            GameObject prefab;
            switch (couleur)
            {
                case Couleur.Yellow:
                    prefab = MasterManager.Instance.marqueurYellow;
                    break;
                case Couleur.Red:
                    prefab = MasterManager.Instance.marqueurRed;
                    break;
                case Couleur.Brown:
                    prefab = MasterManager.Instance.marqueurBrown;
                    break;
                default:
                    throw new Exception();
            }
            
            GameObject g = Instantiate(prefab, position, Quaternion.identity);
            g.transform.parent = MasterManager.Instance.GetDossierRayGaz();
            g.transform.localScale = new Vector3(scale, scale, scale);

            return g;
        }
    }
}