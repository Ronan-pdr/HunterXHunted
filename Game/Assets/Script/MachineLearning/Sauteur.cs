using System;
using Script.Animation;
using Script.Bot;
using Script.Brain;
using Script.Graph;
using Script.Test;
using UnityEngine;
using Random = System.Random;

namespace Script.MachineLearning
{
    public class Sauteur : Student
    {
        // ------------ Attributs ------------

        private BrainJump _brainSaut;
        
        // ------------ Setter ------------
        public override void SetToTest()
        {
            running = Running.Marche;
        }

        protected override BrainClass GetBrainToTrain(int numero)
        {
            _brainSaut = new BrainJump(numero);
            return _brainSaut;
        }
        
        protected override BrainClass GetBrainToTrain()
        {
            _brainSaut = new BrainJump();
            return _brainSaut;
        }

        // ------------ Methods ------------

        protected override void ErrorEntrainement()
        {
            if (!(Entrainement is EntrainementSaut))
            {
                throw new Exception();
            }
        }
        
        // ------------ Constructeur ------------
        
        protected override void AwakeStudent()
        {}

        // ------------ Brain ------------

        protected override void UseBrain()
        {
            if (!Grounded)
                return;
            
            if (_brainSaut.JumpNeeded(Tr, GetSpeed(), SprintSpeed))
            {
                Jump();
                Entrainement.Malus();
            }
        }
        
        // ------------ Event ------------
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("SetRun"))
            {
                running = Running.Course;
            }
        }

        protected override void OnHighCollision(Collision _)
        {
            Entrainement.EndTest();
        }
    }
}