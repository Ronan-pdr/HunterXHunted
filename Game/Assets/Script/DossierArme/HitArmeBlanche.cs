using System;
using System.Collections.Generic;
using System.Linq;
using Script.EntityPlayer;
using Script.Tools;
using UnityEngine;

namespace Script.DossierArme
{
    public class HitArmeBlanche : MonoBehaviour
    {
        // ------------ SerializeField ------------ 

        [Header("Recupérer le chasseur")]
        [SerializeField] private Chasseur porteur;

        [Header("InfoArme")]
        [SerializeField] private int degatArme;
        
        // ------------ Attributs ------------

        private bool _isUse;
        private float _timeEndHit;

        private Dictionary<string, bool> _dict;
        
        // ------------ Constructeur

        private void Start()
        {
            _dict = new Dictionary<string, bool>();
        }

        // ------------ Setter ------------

        public void HitBegin()
        {
            _isUse = true;
            _timeEndHit = Time.time + 0.45f;
        }
        
        // ------------ Update ------------

        private void Update()
        {
            if (_isUse && SimpleMath.IsEncadré(_timeEndHit, Time.time, 0.1f))
            {
                //Debug.Log("fin d'utilisation");
                _isUse = false;
                _dict.Clear();
            }
        }

        // ------------ Event ------------
        private void OnTriggerStay(Collider other)
        {
            if (_isUse && !_dict.ContainsKey(other.name))
            {
                //Debug.Log("TOUCHE");
                porteur.WhenWeaponHit(other.gameObject, degatArme);
                
                _dict.Add(other.name, true);
            }
        }
    }
}