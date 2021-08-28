using System;
using System.Collections.Generic;
using Script.Bot;
using Script.EntityPlayer;
using Script.Test;
using Script.Tools;
using UnityEngine;

namespace Script.Labyrinthe
{
    public class Guide : BotClass
    {
        // ------------ Etat ------------
        private enum Etat
        {
            Attend,
            Guidage,
            Arrivé
        }

        private Etat etat;
        
        // ------------ Attributs ------------
        
        private float timeLastRegard;
        private List<Vector3> path;
        
        // ------------ Constructeurs ------------
        
        protected override void AwakeBot()
        {}

        protected override void StartBot()
        {
            etat = Etat.Attend;
        }
        
        // ------------ Update ------------
        protected override void UpdateBot()
        {
            switch (etat)
            {
                case Etat.Attend:
                    if (IsDepart())
                    {
                        Depart();
                    }
                    break;
                case Etat.Guidage:
                    Guidage();
                    break;
                case Etat.Arrivé:
                    // il ne fait plus rien
                    break;
                default:
                    throw new Exception($"Le cas de {etat} n'est pas encore géré");
            }
        }
        
        // ------------ Méthodes ------------
        
        private bool IsDepart()
        {
            if (Time.time - timeLastRegard < 1)
                return false;
            
            timeLastRegard = Time.time;
            
            return GetPlayerInMyVision(TypePlayer.Player).Count > 0;
        }

        private void Depart()
        {
            path = LabyrintheManager.Instance.GetBestPath(Tr.position);

            if (path.Count > 0)
            {
                etat = Etat.Guidage;
                running = Running.Course;
            }
        }

        private void Guidage()
        {
            // s'il a finit une étape de son plan
            if (IsArrivé(path[0]))
            {
                path.RemoveAt(0);
                
                // Est - il arrivé ?
                if (path.Count == 0)
                {
                    // Il ne fait plus jamais rien
                    etat = Etat.Arrivé;
                    enabled = false;
                    return;
                }
                
                CalculeRotation(path[0]);
            }
            
            GestionRotation(path[0]);
        }
        
        // bloqué
        protected override void WhenBlock()
        {
            
        }
    }
}