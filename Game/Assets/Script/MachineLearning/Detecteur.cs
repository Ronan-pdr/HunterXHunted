using System;
using System.IO;
using Script.Animation;
using Script.Brain;
using Script.Tools;
using UnityEngine;

namespace Script.MachineLearning
{
    public class Detecteur : Student
    {
        // ------------ Attributs ------------

        // brains
        private BrainJump _brainJump;
        private BrainWall _brainWall;
        
        // etat
        private bool _sitting;
        
        // traverser mur
        private MyFile<Collider> _fileWall;
        private bool _isWaitingResult;
        
        // ------------ Getter ------------
        
        protected override BrainClass GetBrainToTrain(int numero)
        {
            _brainWall = new BrainWall(numero);
            return _brainWall;
        }

        protected override BrainClass GetBrainToTrain()
        {
            _brainWall = new BrainWall();
            return _brainWall;
        }

        // ------------ Setter ------------

        public override void SetToTest()
        {
            SetStandUp();
            running = Running.Marche;
        }

        private void SetSit()
        {
            if (!_sitting)
            {
                Anim.Set(HumanAnim.Type.Sit);
            }
            
            _sitting = true;
        }

        private void SetStandUp()
        {
            if (_sitting)
            {
                Anim.Stop(HumanAnim.Type.Sit);
            }
            
            _sitting = false;
        }

        // ------------ Constructeur ------------

        protected override void AwakeStudent()
        {
            _brainJump = new BrainJump(0);
            
            _fileWall = new MyFile<Collider>();
            _isWaitingResult = false;
        }

        // ------------ Methods ------------

        protected override void ErrorEntrainement()
        {
            if (!(Entrainement is EntrainementDetection))
            {
                throw new Exception();
            }
        }
        
        // ------------ Brain ------------
        
        protected override void UseBrain()
        {
            if (!Grounded || _isWaitingResult)
                return;
            
            // recupérer les infos par rapport aux obstacles
            (double dist, double height) = BrainClass.GetStaticDistHeightFirstObstacle(Tr, MaxDistJump, capsule);

            if (_brainJump.JumpNeeded(dist, height, GetSpeed(), SprintSpeed))
            {
                // il FAUT sauter (on part du principe que notre cerveau a raison)
                Jump();
                _isWaitingResult = true;
                Invoke(nameof(Result), 1f);

                if (_brainWall.IsThereWall(height))
                {
                    // pense que l'on peut pas passer
                    SetSit();
                }
                else
                {
                    SetStandUp();
                }
            }
        }

        private void Result()
        {
            if (!_isWaitingResult)
                return;
            
            // il n'a rencontré aucun obstacle
            // puiqu'il attend toujours un résultat

            if (_sitting)
            {
                // a mal anticipé
                Entrainement.EndTest();
            }

            _isWaitingResult = false;
        }
        
        private void Rematerialiser()
        {
            while (!_fileWall.IsEmpty())
            {
                _fileWall.Defiler().isTrigger = false;
            }
        }
        
        // ------------ Event ------------
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                // fin de l'épreuve (tout réussi)
                Entrainement.EndTest();
            }
        }

        protected override void OnHighCollision(Collision other)
        {
            if (_sitting)
            {
                // bien anticipé --> continue l'épreuve
                
                // il faut traverser le mur
                other.collider.isTrigger = true;
                _fileWall.Enfiler(other.collider);
                Invoke(nameof(Rematerialiser), 0.5f);
            }
            else
            {
                // perdu
                Entrainement.EndTest();
            }

            // on a déja le résultat
            _isWaitingResult = false;
        }
    }
}