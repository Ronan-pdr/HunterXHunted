using System.Collections.Generic;
using Photon.Realtime;
using Script.Animation;
using Script.Brain;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.Bot
{
    public class Suiveur : BotClass
    {
        // ------------ Etat ------------
        private enum Etat
        {
            Statique,
            Follow,
            Escape,
            Searching,
            Looking
        }

        private Etat etat;
        
        // ------------ Attributs ------------
        
        private (Chasseur chasseur, Vector3 position) Vu;

        private const float RayonPerimetre = 30;
        
        // destination
        private Vector3 _whereToLookAt;
        
        // cerveaux
        private BrainWall _brainWall;
        private BrainJump _brainJump;
        
        // ------------ Setter ------------

        private void SetEscape()
        {
            StandUp();
            running = Running.Course;
            _whereToLookAt = FindEscapePosition(Vu.position);
            etat = Etat.Escape;
        }

        private void SetStatique()
        {
            StandUp();
            Anim.Set(HumanAnim.Type.Sit);
            running = Running.Arret;
            _whereToLookAt = Vu.position;
            etat = Etat.Statique;
        }

        private void SetLooking()
        {
            StandUp();
            running = Running.Arret;
            _whereToLookAt = Vu.position;
            Vu.chasseur = null;
        }

        private void SetSearching()
        {
            StandUp();
            running = Running.Marche;
            _whereToLookAt = Vu.position;
            etat = Etat.Searching;
        }

        private void SetFollow()
        {
            StandUp();
            Anim.Set(HumanAnim.Type.Squat);
            running = Running.Squatting;
            _whereToLookAt = Vu.position;
            etat = Etat.Follow;
        }

        private void StandUp()
        {
            Anim.Stop(HumanAnim.Type.Squat);
            Anim.Stop(HumanAnim.Type.Sit);
        }


        // ------------ Constructeurs ------------

        protected override void AwakeBot()
        {
            _brainWall = new BrainWall(0);
            _brainJump = new BrainJump(0);

            PeriodeBlock = 1;
        }

        protected override void StartBot()
        {
            running = Running.Arret;
            etat = Etat.Looking;
            
            InvokeRepeating(nameof(UpdateSuiveur), 0, 0.3f);
        }
    
        // ------------ Update ------------

        protected override void UpdateBot()
        {
            Tourner();

            if (etat == Etat.Escape && SimpleMath.IsEncadré(_whereToLookAt, Tr.position, 1.5f))
            {
                // il a fini de fuir
                SetLooking();
                CalculeRotation(_whereToLookAt);
            }

            if (running != Running.Arret && _brainJump.JumpNeeded(Tr, GetSpeed(), SprintSpeed))
            {
                Jump();
            }
        }
        
        private void UpdateSuiveur()
        {
            GestionRotation(_whereToLookAt, 0);
            
            if (!Vu.chasseur)
            {
                // il n'a encore vu personne
                etat = Etat.Looking;
                SearchChasseurWithVision();
            }
            else if (IsInMyVision(Vu.chasseur))
            {
                // un chasseur est dan sma vision
                UpdateWhenChasseurInMyVision();
            }
            else
            {
                // a perdu de vue le chasseur
                if (etat == Etat.Escape)
                {
                    // s'il fuit c'est normal
                }
                else if (!SearchChasseurWithVision())
                {
                    // il faut tenter de le retrouver
                    // étant donné qu'il n'y a aucun autre chasseur
                    SetSearching();
                }
            }
        }

        private void UpdateWhenChasseurInMyVision()
        {
            // ...donc j'update sa position
            Vu.position = Vu.chasseur.transform.position;
            
            float dist = Calcul.Distance(Tr.position, Vu.position, Calcul.Coord.Y);
                    
            if (SimpleMath.IsEncadré(dist, RayonPerimetre, 1.5f))
            {
                // est pile à la bonne position
                SetStatique();
            }
            else if (dist < RayonPerimetre)
            {
                // trop proche
                SetEscape();
            }
            else
            {
                // est trop loin
                SetFollow();
            }
            
            CalculeRotation(_whereToLookAt);
        }

        // ------------ Méthodes ------------
        private bool SearchChasseurWithVision()
        {
            List<PlayerClass> vus = GetPlayerInMyVision(TypePlayer.Chasseur);

            if (vus.Count > 0)
            {
                // Il a désormais un chasseur dans sa vision
                PlayerClass player = vus[0];
                
                Vu.chasseur = (Chasseur) player;
                Vu.position = player.transform.position;
                UpdateWhenChasseurInMyVision();

                return true;
            }

            return false;
        }

        // Prend la position la plus proche du bot parmi toutes
        // celles au périmètre (le cercle) du "Vu"
        private Vector3 FindEscapePosition(Vector3 centre)
        {
            (Vector3 bestDest, float minDist) res = (Vector3.zero, RayonPerimetre * 2.1f);
            
            for (int degre = 0; degre < 360; degre += 10)
            {
                Vector3 pos = centre + RayonPerimetre * Calcul.Direction(degre);
                float dist = Calcul.Distance(Tr.position, pos);

                if (dist < res.minDist && !_brainWall.IsThereWall(Tr.position, pos))
                {
                    // nouvelle meilleure destination
                    res = (pos, dist);
                }
            }

            return res.bestDest;
        }
        
        // ------------ Event ------------
        
        // bloqué
        protected override void WhenBlock()
        {
            SetLooking();
        }
    }
}
