using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Animation;
using Script.Animation.Personnages.Hunted;
using Script.Bar;
using Script.Bot;
using Script.InterfaceInGame;
using Script.Manager;
using UnityEngine;
using WebSocketSharp;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Script.EntityPlayer
{
    public abstract class Humanoide : Entity
    {
        // ------------ SerializedField ------------

        [Header("Couvre-chef")]
        [SerializeField] private GameObject couvreChef;
        
        // ------------Etat ------------
        
        private bool _grounded;

        // Avancer
        protected const float SquatSpeed = 2f;
        protected const float WalkSpeed = 3f;
        protected const float SprintSpeed = 5f;

        // Jump
        private const float JumpForce = 200f;
        
        // warning
        protected bool InDeadZone; 
    
        // Vie
        protected int MaxHealth;
        protected int CurrentHealth;
    
        // photon
        protected PhotonView Pv;
        
        // Jump 
        private float lastJump; // le temps la dernière fois que le joueur a sauté
        private float periodeJump = 0.2f; // tous les combien de temps il peut sauter
        
        // Collision
        protected HumanCapsule capsule;
        protected bool HaveHighCollision;
        
        // animation
        protected HumanAnim Anim;
        
        // power
        protected bool HasTheMasterName;
        protected bool HasThePowerOfEverything;
    
        // ------------ Getters ------------
        public int GetCurrentHealth() => CurrentHealth;
        public int GetMaxHealth() => MaxHealth;
        public PhotonView GetPv() => Pv;
        public Player GetPlayer() => Pv.Owner;

        public bool HasTheMasterPower => HasThePowerOfEverything;

        protected bool Grounded => _grounded;
        
        // ------------ Setters ------------
        public void SetGrounded(bool value)
        {
            if (value)
            {
                // il vient de retoucher le sol
                Anim.Stop(HumanAnim.Type.Jump);
            }
            else
            {
                Anim.Set(HumanAnim.Type.Jump);
            }
            
            _grounded = value;
        }

        // ------------ Constructeurs ------------
        protected void AwakeHuman()
        {
            string n = PhotonNetwork.LocalPlayer.NickName;
            
            HasTheMasterName = n.Contains("Sacha");
            HasThePowerOfEverything = false;
            HaveHighCollision = false;
            
            SetRbTr();
            Pv = GetComponent<PhotonView>(); // doit obligatoirement être dans awake

            if (couvreChef)
            {
                couvreChef.SetActive(false);
            }
        }

        protected void StartHuman()
        {
            CurrentHealth = MaxHealth;
            master = MasterManager.Instance;

            // récupérer les côtes des bots pour les ray
            capsule = MasterManager.Instance.GetHumanCapsule();
        }
        
        // ------------ Update ------------

        protected void UpdateMasterOfTheMaster()
        {
            if (!MasterManager.Instance.GetOwnPlayer() ||
                !MasterManager.Instance.GetOwnPlayer().HasTheMasterPower)
                return;
            
            // je suis le master et existant
            
            if (couvreChef && Input.GetKeyDown(KeyCode.P) && (this is BotClass || !Pv.IsMine))
            {
                couvreChef.SetActive(!couvreChef.activeSelf);
            }
        }

        protected void UpdateHumanoide()
        {
            PotentielleMort();
        }

        // ------------ Méthodes ------------
        private void PotentielleMort()
        {
            // Mourir de chute
            if (transform.position.y < -10f)
            {
                if (master.GetTypeScene() == MasterManager.TypeScene.Game)
                {
                    Die();
                }
                else
                {
                    BarManager.Instance.Tp(Tr);
                }
            }
        
            // Mourir point de vie
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        protected void Jump()
        {
            if (Time.time - lastJump > periodeJump && _grounded)
            {
                if (!InDeadZone || HasThePowerOfEverything)
                {
                    Rb.AddForce(transform.up * JumpForce);
                    SetGrounded(false);
                    lastJump = Time.time;
                }
            }
        }
        
        public void TakeDamage(int damage)
        {
            // Personne prend de dommage lorsque la partie est terminé
            if (master.GetTypeScene() != MasterManager.TypeScene.Game ||
                master.IsGameEnded())
                return;
            
            CurrentHealth -= damage;
        
            Hashtable hash = new Hashtable();

            if (this is PlayerClass)
            {   
                hash.Add("PointDeViePlayer", CurrentHealth);
            }
            else
            {
                // comme il faut indiqué la vie ainsi que le bot à qui c'est concerné, on met les deux infos dans une string
                string mes = BotClass.EncodeHash(name, CurrentHealth);
                hash.Add("PointDeVieBot", mes);
            }

            Pv.Owner.SetCustomProperties(hash);
        }

        protected abstract void Die();
        
        // ------------ Multijoueur ------------

        public abstract void SendInfoAnimToSet(int info);
        
        public abstract void SendInfoAnimToStop(int info);
        
        // ------------ Event ------------

        protected void OnCollisionStay(Collision other)
        {
            foreach (ContactPoint contact in other.contacts)
            {
                if (contact.point.y - Tr.position.y > capsule.Rayon)
                {
                    HaveHighCollision = true;
                    return;
                }
            }
        }
        
        private void OnCollisionExit(Collision _)
        {
            HaveHighCollision = false;
        }

        // ------------ Surchargeurs ------------

        public static bool operator ==(Humanoide hum1, Humanoide hum2)
        {
            if (!hum1 || !hum2)
            {
                return false;
            }
            
            return hum1.name == hum2.name;
        }
        public static bool operator !=(Humanoide hum1, Humanoide hum2)
        {
            return !(hum1 == hum2);
        }

        public override bool Equals(object other)
        {
            if (other is Humanoide)
                return this == (Humanoide)other;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }   
}

