using System;
using System.Collections.Generic;
using System.Linq;
using Script.Bot;
using Script.EntityPlayer;
using Script.Tools;
using UnityEngine;

namespace Script.Animation
{
    public abstract class HumanAnim : MonoBehaviour
    {
        // ------------ Attributs ------------
        public enum Type
        {
            Idle,
            Forward,
            Backward,
            Right,
            Left,
            DiagR,
            DiagL,
            Run,
            Sit,
            Squat,
            Jump,
            Hit,
            Aiming,
            Shoot
        }

        protected Dictionary<Type, int> Dict;
        private Type[] _animContinue;
        
        // pour les triggers
        private (float time, Type anim) _trigger;
        
        private Animator _animator;
        
        // Synchronisation
        private Humanoide _porteur;
        private int _nbAnimSendInTheLastSeconds;
        
        // ------------ Setter ------------

        public void Constructeur(Animator animator, Humanoide player)
        {
            _animator = animator;
            _porteur = player;
        }
        
        protected abstract void AddAnim();
        
        public void Set(Type newAnim)
        {
            if (!_animator)
                return;

            //if (_porteur is BotClass || !_porteur.photonView.IsMine)
                //return;

            // potentiel erreur
            CheckError(newAnim);

            StopContinue();

            bool isState = IsState(newAnim);
            bool isTrigger = IsTrigger(newAnim, out float timeAnim);
            
            if (isState) // state
            {
                // rien de plus
            }
            else if (isTrigger) // trigger
            {
                // enlever la précédente
                Stop(_trigger.anim);
                
                _trigger.time = Time.time + timeAnim;
                _trigger.anim = newAnim;
            }
            else // continue
            {
                // elle sont stocké dans une liste
                
                if (_porteur is BotClass)
                {
                    return;
                }
            }

            if (_animator.GetBool(Dict[newAnim]))
                return;

            _animator.SetBool(Dict[newAnim], true);
            
            // Synchronisation
            if (!(_porteur is Chasseur) && (isTrigger || isState) && _nbAnimSendInTheLastSeconds < 3)
            {
                _porteur.SendInfoAnimToSet((int)newAnim);
                _nbAnimSendInTheLastSeconds += 1;
            }
        }

        // ------------ Constructeur ------------

        private void Awake()
        {
            Dict = new Dictionary<Type, int>();
            
            // deplacement (anim continue)
            _animContinue = new []
            {
                Type.Forward, Type.Backward, Type.Right, Type.Left,
                Type.DiagR, Type.DiagL, Type.Run
            };

            Dict.Add(Type.Forward, Animator.StringToHash("isWalking"));
            Dict.Add(Type.Backward, Animator.StringToHash("isWalkingBack"));
            Dict.Add(Type.Right, Animator.StringToHash("isSideWalkingR"));
            Dict.Add(Type.Left, Animator.StringToHash("isSideWalkingL"));
            Dict.Add(Type.DiagR, Animator.StringToHash("isRF"));
            Dict.Add(Type.DiagL, Animator.StringToHash("isLF"));
            Dict.Add(Type.Run, Animator.StringToHash("isRunning"));
            
            // one touch
            Dict.Add(Type.Squat, Animator.StringToHash("isSquatting"));
            Dict.Add(Type.Jump, Animator.StringToHash("isJumping"));
            
            // ajouter des anims spécifique
            AddAnim();
            
            // synchro
            _nbAnimSendInTheLastSeconds = 0;
            InvokeRepeating(nameof(DecrementeNbAnimSend), 0.5f, 1);
        }
        
        // ------------ Update ------------

        private void Update()
        {
            // gérer les triggers
            if (SimpleMath.IsEncadré(Time.time, _trigger.time, 0.1f))
            {
                Stop(_trigger.anim);
                _trigger.anim = Type.Idle;
            }
        }

        // ------------ Publique Méthodes ------------

        public void Stop(Type animToStop)
        {
            if (!_animator)
                return;
            
            // potentiel erreur
            CheckError(animToStop);

            if (animToStop == Type.Idle || !_animator.GetBool(Dict[animToStop]))
                return;

            bool isContinue = IsContinue(animToStop);

            if (isContinue && _porteur is BotClass)
                return;

            _animator.SetBool(Dict[animToStop], false);
            
            // Synchronisation
            if (!(_porteur is Chasseur) && !isContinue && _nbAnimSendInTheLastSeconds < 3)
            {
                _porteur.SendInfoAnimToStop((int)animToStop);
                _nbAnimSendInTheLastSeconds += 1;
            }
        }

        public void StopContinue()
        {
            foreach (Type anim in _animContinue)
            {
                Stop(anim);
            }
        }

        // ------------ Private Méthodes ------------
        
        private bool IsState(Type type)
        {
            return type == Type.Squat ||
                   type == Type.Sit ||
                   type == Type.Aiming ||
                   type == Type.Shoot;
        }

        private bool IsTrigger(Type type, out float time)
        {
            switch (type)
            {
                case Type.Hit:
                    time = 0.25f;
                    return true;
                default:
                    time = 0;
                    return false;
            }
        }

        private bool IsContinue(Type type)
        {
            return _animContinue.Contains(type);
        }

        private void DecrementeNbAnimSend()
        {
            if (_nbAnimSendInTheLastSeconds > 0)
            {
                _nbAnimSendInTheLastSeconds--;
            }
        }

        private void CheckError(Type type)
        {
            if (type != Type.Idle && !Dict.ContainsKey(type))
            {
                throw new Exception($"La class '{this}' ne contient pas l'animation {type}");
            }
        }
    }
}