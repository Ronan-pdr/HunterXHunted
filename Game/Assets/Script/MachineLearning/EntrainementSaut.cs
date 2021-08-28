using System;
using System.Diagnostics;
using Script.DossierPoint;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using Script.Brain;

namespace Script.MachineLearning
{
    public class EntrainementSaut : Entrainement
    {
        // ------------ Attributs ------------

        private int _nSaut;
        
        private const int CoefScore = 10;
        
        // ------------ Getter ------------
        public override string GetNameDirectory() => BrainJump.NameDirectory;

        // ------------ Public Methods ------------
        
        protected override void GetScore()
        {
            // bonus
            float dist = Calcul.Distance(Student.transform.position, begin.position);
            Score += (int)(dist * CoefScore);
            
            // malus
            if (dist > 5)
            {
                Score -= _nSaut * CoefScore;
            }
        }

        public override void Bonus()
        {
            throw new NotImplementedException();
        }

        public override void Malus()
        {
            _nSaut += 1;
        }

        // ------------ Protected Methods ------------

        protected override Student GetPrefab() => Master.GetOriginalSauteur();

        protected override void StartTraining()
        {
            _nSaut = 0;
        }
    }
}