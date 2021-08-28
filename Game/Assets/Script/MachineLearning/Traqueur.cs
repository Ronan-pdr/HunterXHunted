using System;
using Script.Brain;
using UnityEngine;

namespace Script.MachineLearning
{
    public class Traqueur : Student
    {
        // ------------ Attributs ------------
        
        private EntrainementDeplacement _entrainementDeplacement;
        private Vector3 _destination;

        // brains
        private BrainDeplacement _brainDeplacement;
        private BrainJump _brainJump;
        
        // constante
        private const int ValueTurn = 5;
        
        // ------------ Getter ------------
        
        protected override BrainClass GetBrainToTrain(int numero)
        {
            _brainDeplacement = new BrainDeplacement(numero);
            return _brainDeplacement;
        }

        protected override BrainClass GetBrainToTrain()
        {
            _brainDeplacement = new BrainDeplacement();
            return _brainDeplacement;
        }
        
        // ------------ Setter ------------

        public override void SetToTest()
        {
            _destination = _entrainementDeplacement.Arrive;
        }

        public void SetGoal(Vector3 value)
        {
            _destination = value;
        }

        // ------------ Constructeur ------------

        protected override void AwakeStudent()
        {
            // brains
            _brainJump = new BrainJump(0);
        }
        
        // ------------ Methods ------------

        protected override void ErrorEntrainement()
        {
            if (Entrainement is EntrainementDeplacement)
            {
                _entrainementDeplacement = (EntrainementDeplacement)Entrainement;
            }
            else
            {
                throw new Exception();
            }
        }

        // ------------ Brain ------------

        protected override void UseBrain()
        {
            switch (_brainDeplacement.WhatDeplacementShouldDo(Tr, _destination))
            {
                case BrainDeplacement.Output.Avancer:
                    Avancer();
                    break;
                case BrainDeplacement.Output.TournerHoraire: 
                    TournerHoraire();
                    break;
                case BrainDeplacement.Output.TournerTrigo:
                    TournerTrigo();
                    break;
            }
        }

        private void Avancer()
        {
            running = Running.Marche;
            
            if (_brainJump.JumpNeeded(Tr, GetSpeed(), SprintSpeed))
            {
                Jump();
            }
        }

        private void TournerHoraire()
        {
            running = Running.Arret;
            AmountRotation = ValueTurn;
        }
        
        private void TournerTrigo()
        {
            running = Running.Arret;
            AmountRotation = -ValueTurn;
        }

        // ------------ Event ------------
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                // il est arrivé
                _entrainementDeplacement.NextField(true);
            }
        }

        protected override void OnHighCollision(Collision _)
        {
            // il a merdé
            _entrainementDeplacement.NextField(false);
        }
    }
}