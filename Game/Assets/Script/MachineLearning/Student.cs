using System;
using Script.Animation;
using Script.Bot;
using UnityEngine;
using Script.Brain;

namespace Script.MachineLearning
{
    public abstract class Student : BotClass
    {
        // ------------ Attributs ------------
        
        protected Entrainement Entrainement;
        
        protected const float MaxDistJump = 3;

        protected BrainClass BrainToTrain;
        
        // ------------ Getter ------------
        
        public BrainClass Brain => BrainToTrain;

        protected abstract BrainClass GetBrainToTrain(int numero);
        
        protected abstract BrainClass GetBrainToTrain();

        // ------------ Setter ------------

        public BrainClass SetBrain(int numero)
        {
            BrainToTrain = GetBrainToTrain(numero);
            return BrainToTrain;
        }

        public BrainClass SetBrain()
        {
            BrainToTrain = GetBrainToTrain();
            return BrainToTrain;
        }

        public void SetEntrainement(Entrainement value)
        {
            Entrainement = value;
            
            ErrorEntrainement();
        }
        
        // ------------ Constructeur ------------

        protected abstract void AwakeStudent();

        protected override void AwakeBot()
        {
            AwakeStudent();
        }
        
        protected override void StartBot()
        {
            SetToTest();
            InvokeRepeating(nameof(UseBrain), 0.1f, 0.1f);
            //Invoke(nameof(PotentielJump), 0.5f);
        }
        
        // ------------ Update ------------

        protected override void UpdateBot()
        {}
        
        // ------------ Abstact Methods ------------
        
        protected abstract void ErrorEntrainement();
        protected abstract void UseBrain();

        public abstract void SetToTest();
        
        
        // ------------ Brain ------------

        protected double[] InputJump(double minDist, double height)
        {
            return new [] {minDist / MaxDistJump, height / capsule.Height, GetSpeed() / SprintSpeed};
        }
        
        // ------------ Event ------------

        protected override void WhenBlock()
        {
            Entrainement.EndTest();
        }

        private void OnCollisionEnter(Collision other)
        {
            OnCollision(other);
        }

        private void OnCollisionStay(Collision other)
        {
            OnCollision(other);
        }

        protected abstract void OnHighCollision(Collision other);

        private void OnCollision(Collision other)
        {
            foreach (ContactPoint contact in other.contacts)
            {
                if (contact.point.y - Tr.position.y > capsule.Rayon)
                {
                    OnHighCollision(other);
                    return;
                }
            }
        }
    }
}