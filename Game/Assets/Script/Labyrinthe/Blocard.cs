using System.Collections.Generic;
using ExitGames.Client.Photon;
using Script.EntityPlayer;
using Script.Test;
using Script.Tools;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class Blocard : PlayerClass
    {
        // ------------ Attributs ------------
        
        private int nCaillouMax = 50;
        private int nCaillou;
        private List<GameObject> caillous = new List<GameObject>();

        // ------------ Constructeurs ------------
        
        protected override void AwakePlayer()
        {}

        protected override void StartPlayer()
        {
            MaxHealth = 100;
            nCaillou = 0;
        }

        // ------------ Update ------------
        protected override void UpdatePlayer()
        {
            if (Input.GetKeyDown("f"))
            {
                PoserCaillou();
            }

            if (Input.GetKey("r"))
            {
                RetirerCaillou();
            }
        }

        // ------------ Méthodes ------------

        private void PoserCaillou()
        {
            caillous.Add(TestRayGaz.CreateMarqueur(Tr.position, TestRayGaz.Couleur.Brown));
            nCaillou += 1;
                
            if (nCaillou == nCaillouMax)
            {
                SupprimerCaillou(0);
            }
        }

        private void RetirerCaillou()
        {
            for (int i = caillous.Count - 1; i >= 0; i--)
            {
                if (SimpleMath.IsEncadré(caillous[i].transform.position + Vector3.down * 0.8f, Tr.position))
                {
                    SupprimerCaillou(i);
                }
            }
        }

        private void SupprimerCaillou(int i)
        {
            Destroy(caillous[i]);
            caillous.RemoveAt(i);
            nCaillou -= 1;
        }
        
        // ------------ Multijoueur ------------

        protected override void PropertiesUpdate(Hashtable _)
        {}
    }
}