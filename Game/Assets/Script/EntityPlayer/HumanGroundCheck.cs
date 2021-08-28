using System;
using System.Collections;
using System.Collections.Generic;
using Script.Bot;
using UnityEngine;

namespace Script.EntityPlayer
{
    public class HumanGroundCheck : Entity
    {
        // ------------ Attribut
        
        private Humanoide human;

        // ------------ Constructeur ------------
        private void Awake()
        {
            human = GetComponentInParent<Humanoide>();
        }

        // ------------ Event ------------
        private void OnTriggerEnter(Collider other)
        {
            Aux(other, true);
        }
        
        private void OnTriggerStay(Collider other)
        {
            Aux(other, true);
        }

        private void OnTriggerExit(Collider other)
        {
            Aux(other, false);
        }

        private void Aux(Collider other, bool res)
        {
            if (other.gameObject == human.gameObject) // Le cas où c'est avec notre propre personnage
                return;
        
            human.SetGrounded(res);
        }
    }
}

