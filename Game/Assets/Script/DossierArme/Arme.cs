using System;
using Script.Animation;
using Script.EntityPlayer;
using UnityEngine;

namespace Script.DossierArme
{
    public abstract class Arme : MonoBehaviour
    {
        // ------------ Serialize Field ------------
        
        // les gameObject de la caméra et du joueur qui porte le flingue
        [Header("Controller")]
        [SerializeField] protected Transform cameraHolder;

        // Variables relatives à l'arme en elle-même
        [Header("Info")]
        [SerializeField] protected ArmeInfo armeInfo;
    
        // ------------ Attributs ------------
        
        // pour la fréquence de tir
        private float lastUse = -1;
        
        // animation
        private HumanAnim _anim;


        // ------------ Getter ------------
        public HumanAnim Anim => _anim;

        // ------------ Constructeur ------------

        private void Awake()
        { 
            PlayerClass porteur = GetComponentInParent<PlayerClass>();
            
            _anim = GetComponent<HumanAnim>();
            _anim.Constructeur(GetComponent<Animator>(), porteur);
        }

        // ------------ Méthodes ------------

        public void Use()
        {
            if (Time.time - lastUse > armeInfo.GetPériodeAttaque())
            {
                UtiliserArme();
                lastUse = Time.time;
            }
        }
    
        public abstract void UtiliserArme();
    }
}

