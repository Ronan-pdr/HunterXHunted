using System;
using System.Collections.Generic;
using Script.EntityPlayer;
using Script.Test;
using Script.Tools;
using UnityEngine;

namespace Script.Bot
{
    public class Hirondelle : BotClass
    {
        // ------------ SerializeField ------------

        [Header("Obstacle")]
        [SerializeField] private Entourage obstacles;
        
        [Header("Voisins Droit")]
        [SerializeField] private Entourage voisinsNearRight;
        [SerializeField] private Entourage voisinsPerfectRight;
        [SerializeField] private Entourage voisinsFarRight;
        
        [Header("Voisins Gauche")]
        [SerializeField] private Entourage voisinsNearLeft;
        [SerializeField] private Entourage voisinsPerfectLeft;
        [SerializeField] private Entourage voisinsFarLeft;
        
        // ------------ Attributs ------------

        private float _nextRegard;

        private bool _synchro;
        
        // ------------ Setter ------------

        private void SetSynchro(bool value)
        {
            _synchro = value;
            
            voisinsNearRight.gameObject.SetActive(value);
            voisinsPerfectRight.gameObject.SetActive(value);
            voisinsFarRight.gameObject.SetActive(value);
            
            voisinsNearLeft.gameObject.SetActive(value);
            voisinsPerfectLeft.gameObject.SetActive(value);
            voisinsFarLeft.gameObject.SetActive(value);
        }

        private int Mod(float a, int b) => SimpleMath.Mod((int) a, b);

        // ------------ Constructeur ------------
        protected override void AwakeBot()
        {
            RotationSpeed = 500;
            running = Running.Marche;
            
            SetSynchro(true);
            
            // l'entourage
            
            obstacles.Set(o => o.CompareTag("Respawn"));

            Func<GameObject, bool> f = g => g.GetComponent<Hirondelle>();
            
            voisinsNearRight.Set(f);
            voisinsPerfectRight.Set(f);
            voisinsFarRight.Set(f);
            
            voisinsNearLeft.Set(f);
            voisinsPerfectLeft.Set(f);
            voisinsFarLeft.Set(f);
        }

        protected override void StartBot()
        {
            AmountRotation = UnityEngine.Random.Range(-180, 180);
            //Syncronisation.Instance.AddHirondelle(this);

            InvokeRepeating(nameof(GererObstacles), 1, 0.4f);
            InvokeRepeating(nameof(EcartUpdate), 1, 0.05f);
        }
    
        // ------------ Update ------------
    
        protected override void UpdateBot()
        {
            Tourner();

            if (Input.GetKeyDown(KeyCode.P))
            {
                SetSynchro(!_synchro);

                if (_synchro)
                {
                    Debug.Log("Synchro");
                }
                else
                {
                    Debug.Log("Desynchro");
                }
            }
        }
        
        private void EcartUpdate()
        {
            if (_synchro)
            {
                //Synchronisation();
            }
            else
            {
                // tourner légerement
                AmountRotation += UnityEngine.Random.Range(-10f, 10f);
            }
        }
    
        // ------------ Méthodes ------------

        private void GererObstacles()
        {
            if (Time.time < _nextRegard)
                return;

            _nextRegard = Time.time + 0.4f;
            
            List<Vector3> cage = obstacles.GetList();
            int l = cage.Count;

            if (l == 1)
            {
                SingleObstacle(cage[0]);
            }
            else if (l == 2)
            {
                SingleObstacle(cage[0]);
                SingleObstacle(cage[1]);
            }
            else if (l != 0)
            {
                AmountRotation += Autour(160);
            }
        }

        private void TwoObstacle(Vector3 obstacle1, Vector3 obstacle2)
        {
            float angle1 = GetAngle(obstacle1);
            float angle2 = GetAngle(obstacle2);

            int moy = (int) ((angle1 + angle2) / 2);

            if (SimpleMath.Abs(angle1 - angle2) >= 180)
            {
                Debug.Log("vers la moyenne");
                // aller vers la moyenne
                AmountRotation += Autour(moy);
            }
            else
            {
                
                // aller à l'inverse de la moyenne
                float angle = Calcul.GiveAmoutRotation(moy + 180, Tr.eulerAngles.y);
                Debug.Log($"l'inverse de la moyenne, angle = '{angle}'");
                AmountRotation += angle;
            }
        }

        private void SingleObstacle(Vector3 obstacle)
        {
            float angle = GetAngle(obstacle);
            
            if (SimpleMath.Abs(angle) > 90)
            {
                // on s'ent fout des obstacles dans ton dos ou presque
                return;
            }
            
            if (angle > 50)
            {
                AmountRotation -= PetitVirage();
            }
            else if (angle >= 0)
            {
                AmountRotation -= GrosVirage();
            }
            else if (angle > -50)
            {
                AmountRotation += GrosVirage();
            }
            else
            {
                AmountRotation += PetitVirage();
            }
        }

        private float GetAngle(Vector3 pos)
        {
            return Calcul.Angle(Tr.eulerAngles.y, Tr.position, pos, Calcul.Coord.Y);
        }
        
        // ------------ Synchronisation ------------

        private void Synchronisation()
        {
            // s'éloigner de la droite
            AmountRotation -= 3 * voisinsNearRight.GetNb();

            // se rapprocher de la droite
            AmountRotation += 3 * voisinsFarRight.GetNb();
            
            // s'éloigner de la gauche
            AmountRotation += 3 * voisinsNearLeft.GetNb();

            // se rapprocher de la gauche
            AmountRotation -= 3 * voisinsFarLeft.GetNb();
        }

        private void Eloigner(Vector3 pos)
        {
            float angle = GetAngle(pos);
            float abs = SimpleMath.Abs(angle);
            
            if (10 <= abs && abs <= 170)
            {
                // s'éloigner
                AmountRotation += 4 * (angle > 0 ? -1 : 1);
            }
        }

        private void Aligner(Hirondelle hirondelle)
        {
            float angle = hirondelle.Tr.eulerAngles.y - Tr.eulerAngles.y;
            float abs = SimpleMath.Abs(angle);

            if (5 <= abs && abs <= 180)
            {
                // s'aligner
                AmountRotation += 2 * (angle > 0 ? 1 : -1);
            }
        }
        
        private void Rapprocher(Vector3 pos)
        {
            float angle = GetAngle(pos);
            float abs = SimpleMath.Abs(angle);
            
            if (5 <= abs && abs <= 180)
            {
                // s'éloigner
                AmountRotation += 3 * (angle > 0 ? 1 : -1);
            }
        }
        
        // ------------ Random ------------

        private int PetitVirage()
        {
            return Virage(50, 90);
        }
        
        private int GrosVirage()
        {
            return Virage(100, 140);
        }

        private int Virage(int begin, int end)
        {
            return UnityEngine.Random.Range(begin, end);
        }

        private int Autour(int angle, int ecart = 20)
        {
            return Virage(angle - ecart, angle + ecart);
        }


        // ------------ Event ------------
        
        protected override void WhenBlock()
        {
            AmountRotation += Autour(160);
            Debug.Log("Je suis bloqué chef");
        }
    }
}
