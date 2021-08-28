using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Script.Animation;
using Script.Animation.Personnages.Hunted;
using Script.EntityPlayer;
using Script.Graph;
using Script.Manager;
using Script.Tools;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = System.Random;

namespace Script.Bot
{
    public abstract class BotClass : Humanoide
    {
        // ------------ SerializeField ------------

        [Header("Les différents design")]
        [SerializeField] private GameObject[] designs;
        
        // ------------ Enum ------------
        protected enum Running
        {
            Arret,
            Squatting,
            Marche,
            Course
        }

        protected Running running = Running.Arret;
        
        // ------------ Attributs ------------
        
        protected BotManager BotManager; // instancié lorsque le bot est créé dans son BotManager
        
        //Rotation
        protected float RotationSpeed = 400;
        protected float AmountRotation;
        
        // variables relatives à la caméra artificiel des bots
        private static float AngleCamera = 80; // le degré pour la vision périphérique

        //Le bot va recalculer automatiquement sa trajectoire au bout de 'ecartTime'
        protected float LastCalculRotation; //cette variable contient le dernier moment durant lequel le bot à recalculer sa trajectoire

        // quand il est bloqué
        private (float time, Vector3 position) block;
        protected float PeriodeBlock = 2.5f;
        
        // direction
        private Vector3 _direction;
        
        // design
        private DesignHumanoide _design;

        // ------------ Getters ------------
        
        // cette fonction indique si un bot est contrôlé par ton ordinateur
        public bool IsMyBot()
        {
            return BotManager != null || !master.IsMultijoueur;
        }

        protected float GetSpeed()
        {
            switch (running)
            {
                case Running.Arret:
                    return 0;
                case Running.Marche:
                    return WalkSpeed;
                default:
                    return SprintSpeed;
            }
        }

        public int IndexDesign => _design.Index;

        // ------------ Setter ------------
        public void SetOwnBotManager(BotManager value)
        {
            BotManager = value;
        }

        protected void SetDirection(Vector3 value)
        {
            if (value.y != 0)
            {
                throw new Exception();
            }

            _direction = value;
        }

        // ------------ Constructeurs ------------
        protected abstract void AwakeBot();
        
        protected void Awake()
        {
            Anim = GetComponent<HuntedStateAnim>();

            AwakeHuman();
            AwakeBot();
            
            SetDirection(Vector3.forward);

            // récupérer le design par défaut, le modifier et
            // l'envoyer aux autres
            _design = new DesignHumanoide(Anim, designs, this);

            if (Pv.IsMine)
            {
                int index = new Random().Next(_design.Length);
                _design.Set(index);
                
                SendInfoDesign();
                
                Invoke(nameof(SendInfoDesign), 1);
                Invoke(nameof(SendInfoDesign), 3);
            }
        }

        protected abstract void StartBot();

        private void Start()
        {
            MaxHealth = 100;
            StartHuman();
            StartBot();
            
            // son nom (qui sera unique)
            if (BotManager.Instance)
            {
                name = BotManager.Instance.GetNameBot(this, Pv.Owner);
            }

            // le parenter
            if (BotManager is null) // cela veut dire que c'est pas cet ordinateur qui a créé ces bots ni qui les contrôle
            {
                // le parenter dans le dossier qui contient tous les bots controlés par les autres joueurs
                Tr.parent = MasterManager.Instance.GetDossierOtherBot();
            }
            else
            {
                Tr.parent = BotManager.transform; // le parenter dans ton dossier de botManager
            }
            
            BotManager.Instance.AddBot(this);
        }

        // ------------ Update ------------
        
        // Upadte
        protected abstract void UpdateBot();

        private void Update()
        {
            UpdateMasterOfTheMaster();
            
            if (!IsMyBot())
                return;
            
            UpdateBot();
            UpdateHumanoide();

            SetSpeed();
            ManageBlock();
        }

        protected void FixedUpdate()
        {
            if (IsMyBot())
            {
                MoveEntity();
            }
        }
        
        // ------------ Méthodes ------------

        // Rotation
        protected void GestionRotation(Vector3 dest, float periodeCalculRotation = 0.2f)
        {
            // il recalcule sa rotation tous les 0.5f
            if (Time.time - LastCalculRotation > periodeCalculRotation)
            {
                CalculeRotation(dest);
            }

            Tourner();
        }
        
        protected void CalculeRotation(Vector3 dest)
        {
            AmountRotation = Calcul.Angle(Tr.eulerAngles.y, Tr.position, dest, Calcul.Coord.Y);
            LastCalculRotation = Time.time;
        }

        protected void Tourner()
        {
            if (SimpleMath.Abs(AmountRotation) <= 0)
                return;
            
            int sensRotation;
            if (AmountRotation >= 0)
                sensRotation = 1;
            else
                sensRotation = -1;

            float yRot = sensRotation * GetAmountYRot() * RotationSpeed * Time.deltaTime;

            if (SimpleMath.Abs(AmountRotation) < SimpleMath.Abs(yRot)) // Le cas où on a finis de tourner
            {
                Tr.Rotate(new Vector3(0, AmountRotation, 0));
                AmountRotation = 0;
            }
            else
            {
                Tr.Rotate(new Vector3(0, yRot, 0));
                AmountRotation -= yRot;
            }
        }

        private float GetAmountYRot()
        {
            float absMoveAmount = SimpleMath.Abs(AmountRotation);
            if (absMoveAmount < 5)
                return 0.8f;
            if (absMoveAmount < 30)
                return 0.9f;
            if (absMoveAmount < 60)
                return 1f;
            if (absMoveAmount < 90)
                return 1.3f;
            if (absMoveAmount < 120)
                return 1.4f;
            if (absMoveAmount < 150)
                return 1.6f;
            
            return 1.8f;
        }
        
        // vitesse
        private void SetSpeed()
        {
            if (running == Running.Arret)
            {
                MoveAmount = Vector3.zero;
                Anim.StopContinue();
            }
            else if (AmountRotation > 100)
            {
                // ralenti pour le virage
                SetMoveAmount(_direction, 0);
                Anim.StopContinue();
            }
            else if (AmountRotation > 40)
            {
                // ralenti pour le virage
                SetMoveAmount(_direction, 1f);
                Anim.Set(HumanAnim.Type.Forward);
            }
            else if (running == Running.Squatting)
            {
                SetMoveAmount(_direction, SquatSpeed);
            }
            else if (running == Running.Marche || HaveHighCollision)
            {
                // marche
                SetMoveAmount(_direction, WalkSpeed);
                Anim.Set(HumanAnim.Type.Forward);
            }
            else if (running == Running.Course)
            {
                // court
                SetMoveAmount(_direction, SprintSpeed);
                Anim.Set(HumanAnim.Type.Run);
            }
        }

        // Destination
        protected bool IsArrivé(Vector3 dest) => IsArrivé(dest, 0.4f);

        protected bool IsArrivé(Vector3 dest, float ecart)
        {
            return Calcul.Distance(Tr.position, dest, Calcul.Coord.Y) < ecart;
        }

        // Vision
        protected List<PlayerClass> GetPlayerInMyVision(TypePlayer typePlayer)
        {
            Func<int, PlayerClass> getPlayer;
            int l;

            // récupérer la fonction pour récupérer le bon type de joueur et
            // le nombre de ce type de joueur
            switch (typePlayer)
            {
                case TypePlayer.Player:
                    getPlayer = master.GetPlayer;
                    l = master.GetNbPlayer();
                    break;
                case TypePlayer.Chasseur:
                    getPlayer = master.GetChasseur;
                    l = master.GetNbChasseur();
                    break;
                case TypePlayer.Chassé:
                    getPlayer = master.GetChassé;
                    l = master.GetNbChassé();
                    break;
                default:
                    throw new Exception($"Le cas {typePlayer} n'est pas encore géré");
            }
            
            List<PlayerClass> playersInVision = new List<PlayerClass>();

            for (int i = 0; i < l; i++)
            {
                PlayerClass player = getPlayer(i);
                if (player && IsInMyVision(player))
                {
                    playersInVision.Add(player);
                }
            }

            return playersInVision;
        }

        protected bool IsInMyVision(PlayerClass player)
        {
            Vector3 posYeuxArti = Tr.position + 2.2f * Vector3.up;
            Vector3 posPlayer = player.transform.position;
            
            float angleY = Calcul.Angle(Tr.eulerAngles.y, posYeuxArti,
                posPlayer, Calcul.Coord.Y);

            if (SimpleMath.Abs(angleY) < AngleCamera) // le chasseur est dans le champs de vision du bot ?
            {
                if (Physics.Linecast(posYeuxArti, posPlayer, out RaycastHit hit)) // y'a t'il aucun obstacle entre le chasseur et le bot ?
                {
                    if (hit.collider.GetComponent<PlayerClass>() && hit.distance < 40)
                    {
                        // si l'obstacle est le joueur alors le bot "VOIT" le joueur
                        return hit.collider.GetComponent<PlayerClass>() == player;
                    }
                }
            }

            return false;
        }

        // GamePlay
        protected override void Die()
        {
            enabled = false;

            // détruire l'objet
            PhotonNetwork.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            BotManager.Instance.Die(this);
        }

        // ------------ Event ------------
        
        private void ManageBlock()
        {
            if (running == Running.Arret)
            {
                // s'il est arrêté il ne peut-être "bloqué"
                block.time = Time.time;
                return;
            }

            // vérifier qu'il n'est pas bloqué
            if (SimpleMath.IsEncadré(block.position, Tr.position, 0.2f))
            {
                // s'il semble bloquer à une position
                if (Time.time - block.time > PeriodeBlock)
                {
                    // et que ça fait longtemps
                    WhenBlock();
                    SetBlock();
                }
            }
            else
            {
                SetBlock();
            }
        }

        private void SetBlock()
        {
            block.time = Time.time;
            block.position = Tr.position;
        }

        protected abstract void WhenBlock();

        // ------------ Mulitijoueur ------------
        
        private void SendInfoDesign()
        {
            Hashtable hash = new Hashtable();
            hash.Add("DesignBot", EncodeHash(name, _design.Index));
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        public override void SendInfoAnimToSet(int info)
        {
            SendInfoAnim("AnimationBotToSet", info);
        }

        public override void SendInfoAnimToStop(int info)
        {
            SendInfoAnim("AnimationBotToStop", info);
        }

        private void SendInfoAnim(string nature, int info)
        {
            Hashtable hash = new Hashtable();
            hash.Add(nature, EncodeHash(name, info));
            Pv.Owner.SetCustomProperties(hash);
        }

        // réception des hash
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!Pv.Owner.Equals(targetPlayer) || !this) // si c'est pas toi la target, tu ne changes rien
                return;

            object mes;
        
            // point de vie -> TakeDamage (Humanoide)
            // Tout le monde doit faire ce changement (trop compliqué de retrouvé celui qui l'a déjà fait)
            if (changedProps.TryGetValue("PointDeVieBot", out mes))
            {
                // parce que chaque joueur contrôle plusieurs bot, il faut donc faire une deuxième vérification
                if (DecodeHash((string)mes, out int vie))
                {
                    CurrentHealth = vie;
                }
            }

            if (Pv.IsMine) // déjà fait de ton propre point de vue
                return;

            // SendInfoDesign (BotClass)
            if (changedProps.TryGetValue("DesignBot", out mes))
            {
                if (DecodeHash((string)mes, out int indexDesign))
                {
                    _design.Set(indexDesign);
                }
            }

            // SendInfoAnimToSet (BotClass)
            if (changedProps.TryGetValue("AnimationBotToSet", out mes))
            {
                if (DecodeHash((string)mes, out int anim))
                {
                    Anim.Set((HumanAnim.Type) anim);
                }
            }
            
            // SendInfoAnimToStop (BotClass)
            if (changedProps.TryGetValue("AnimationBotToStop", out mes))
            {
                if (DecodeHash((string)mes, out int anim))
                {
                    Anim.Stop((HumanAnim.Type) anim);
                }
            }
        }

        // le format -> name[n char]Vie[3 char]
        public static string EncodeHash(string nom, int info)
        {
            return $"{nom};{info}";
        }

        private bool DecodeHash(string mes, out int value)
        {
            string[] infos = mes.Split(';');

            string nameTarget = infos[0];

            if (name == nameTarget)
            {
                value = int.Parse(infos[1]);
                return true;
            }
            
            value = -1;
            return false;
        }
    }
}