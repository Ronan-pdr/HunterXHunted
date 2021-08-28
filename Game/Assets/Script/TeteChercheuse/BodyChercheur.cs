using System;
using UnityEngine;
using Script.Tools;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;

namespace Script.TeteChercheuse
{
    public abstract class BodyChercheur : TeteChercheuse
    {
        // Nous n'avons pas envie que tous les ordinateurs des joueurs suivent ce script.
        // Ainsi, les body checheur s'instancie localement (pas avec photon),
        
        // ------------ Attributs ------------
        
        // les capsules colliders
        [SerializeField] protected CapsuleCollider botCapsuleCollider;
        protected CapsuleCollider ownCapsuleCollider;

        // Destination
        protected GameObject Destination;
        
        //Ecart maximum entre sa destination et sa position pour qu'il soit considéré comme arrivé
        protected float EcartDistance;

        // ------------ Constructeurs ------------
        private void Awake()
        {
            ownCapsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            // parenter
            Tr.parent = MasterManager.Instance.GetDossierBodyChercheur();

            // on récupère toutes les caractéristiques du CapsuleCollider du bot
            ownCapsuleCollider.center = botCapsuleCollider.center;
            ownCapsuleCollider.height = botCapsuleCollider.height;
            ownCapsuleCollider.radius = botCapsuleCollider.radius * 1f; // 0,8 pour laisser une petite marge

            float rayon = ownCapsuleCollider.radius;

            EcartDistance = rayon*2;
        }
    }
}