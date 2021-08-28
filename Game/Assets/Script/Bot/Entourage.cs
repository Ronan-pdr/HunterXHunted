using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Script.Bot
{
    public class Entourage : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        private Hirondelle _hirondelle;
        private Func<GameObject, bool> _keep;
        private Dictionary<GameObject, Vector3> _dict;

        // ------------ Getter ------------

        public List<Vector3> GetList()
        {
            List<Vector3> list = new List<Vector3>();

            foreach (KeyValuePair<GameObject, Vector3> e in _dict)
            {
                list.Add(e.Value);
            }

            return list;
        }

        public Dictionary<GameObject, Vector3> GetDict() => _dict;

        public int GetNb() => _dict.Count;
        
        // ------------ Setter ------------

        public void Set(Func<GameObject, bool> keep)
        {
            _keep = keep;
        }
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            _hirondelle = GetComponentInParent<Hirondelle>();
            _dict = new Dictionary<GameObject, Vector3>();
        }

        // ------------ Event ------------

        private void OnTriggerStay(Collider other)
        {
            GameObject obj = other.gameObject;
            
            if (_keep(obj))
            {
                // ajouter ou modifier le point de l'obstacle
                _dict[obj] = other.ClosestPoint(_hirondelle.transform.position);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            GameObject obj = other.gameObject;

            if (_dict.ContainsKey(obj))
            {
                _dict.Remove(obj);
            }
        }
    }
}