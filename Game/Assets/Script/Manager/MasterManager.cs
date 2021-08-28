using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using Script.Bot;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Graph;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Script.InterfaceInGame;
using Script.Labyrinthe;
using Script.MachineLearning;
using Script.Menu;
using Script.TeteChercheuse;
using Script.Tools;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Script.Manager
{
    public class MasterManager : MonoBehaviourPunCallbacks
    {
        // ------------ SerializedField ------------
        
        [Header("Prefab Bot")]
        [SerializeField] private BodyRectilgne originalBodyRectilgne;
        [SerializeField] private Hirondelle originalHirondelle;
        [SerializeField] private Sauteur originalSauteur;
        [SerializeField] private Detecteur originalDetecteur;
        [SerializeField] private Traqueur originalTraqueur;
        
        [Header("Prefab Autre")]
        [SerializeField] private BodyGaz originalBodyGaz;
        [SerializeField] private RayGaz originalRayGaz;
        [SerializeField] private GraphPathFinding originalGraphPathFinding;
        [SerializeField] private Line originalLine;
        public GameObject marqueurBrown;
        public GameObject marqueurRed;
        public GameObject marqueurYellow;
        [SerializeField] private SettingsGame settingsGame;

        [Header("Dossier")]
        [SerializeField] private Transform dossierBodyChercheur; // ranger les 'BodyChercheur'
        [SerializeField] private Transform dossierBalleFusil; // ranger les 'BalleFusil'
        [SerializeField] private Transform dossierRayGaz; // ranger les marqueurs des 'RayGaz'
        [SerializeField] private Transform dossierOtherBot; // le dossier où les bots que ton ordinateur ne contrôle pas seront rangés
        [SerializeField] private Transform dossierGraph; 
        
        [Header("Bot")]
        [SerializeField] private CapsuleCollider capsuleBot;
        
        [Header("Scene")]
        [SerializeField] private TypeScene scene;
        
        [Header("Contour")]
        [SerializeField] private bool isBordNeeded;

        [Header("InterfaceInGame")]
        [SerializeField] private GameObject visé;

        [Header("Canvas")]
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject photoWarning;
        
        [Header("Main Camera")]
        [SerializeField] private AudioListener audioListener;
        
        // ------------ Attributs ------------
        
        public static MasterManager Instance;
        
        // les contours de la scène (notamment utilisé par le gaz)
        private (float minZ, float minX, float maxZ, float maxX) contour;

        // nombre de participant (sera utilisé pour déterminer le moment lorsque tous les joueurs auront instancié leur playerController)
        private int nParticipant; // participant regroupe les joueurs ainsi que les spectateurs
        
        // Accéder aux différents joueurs, chaque joueur sera donc stocké deux fois, sauf s'il est mort, il sera juste un spectateur
        private List<PlayerClass> players;
        private List<Chasseur> chasseurs;
        private List<Chassé> chassés;
        private List<Spectateur> spectateurs;

        // attribut relatif à ton avatar
        private PlayerClass ownPlayer;

        // pour savoir sur quel scène nous sommes
        public enum TypeScene
        {
            Game,
            Labyrinthe,
            Maintenance,
            CageOiseaux,
            EntrainementSaut,
            Bar
        }

        private ManagerGame typeScene;

        // etat partie / joueur
        private bool _endedGame;
        private bool _disconnecting;

        // time (en minutes)
        private int _timeEnd;
        
        // full hunter
        private bool _modeBattleRoyal;
        
        // pour envoyer les infos
        private bool _infoSent;

        // ------------ Getters ------------
        public int GetNbParticipant() => nParticipant; // les spectateurs sont compris
        public int GetNbPlayer() => players.Count;
        public int GetNbChasseur() => chasseurs.Count;
        public int GetNbChassé() => chassés.Count;
        public PlayerClass GetOwnPlayer() => ownPlayer;
        public List<PlayerClass> GetListPlayer() => players;
        public PlayerClass GetPlayer(int index) => players[index];
        public Chasseur GetChasseur(int index) => chasseurs[index];
        public Chassé GetChassé(int index) => chassés[index];
        public BodyRectilgne GetOriginalBodyRectilgne() => originalBodyRectilgne;
        public Hirondelle GetOriginalHirondelle() => originalHirondelle;
        public Sauteur GetOriginalSauteur() => originalSauteur;
        public Detecteur GetOriginalDetecteur() => originalDetecteur;
        public Traqueur GetOriginalTraqueur() => originalTraqueur;
        public BodyGaz GetOriginalBodyGaz() => originalBodyGaz;
        public RayGaz GetOriginalRayGaz() => originalRayGaz;
        public GraphPathFinding GetOriginalGraphPathFinding() => originalGraphPathFinding;
        public Line GetOriginalLine() => originalLine;
        public Transform GetDossierBodyChercheur() => dossierBodyChercheur;
        public Transform GetDossierBalleFusil() => dossierBalleFusil;
        public Transform GetDossierRayGaz() => dossierRayGaz;
        public Transform GetDossierOtherBot() => dossierOtherBot;
        public Transform GetDossierGraph() => dossierGraph;
        public (float, float, float, float) GetContour() => contour;
        public HumanCapsule GetHumanCapsule() => new HumanCapsule(capsuleBot);
        public TypeScene GetTypeScene() => scene;
        public SettingsGame SettingsGame => settingsGame;
        public bool IsGameEnded() => _endedGame;
        public bool IsMultijoueur => typeScene.IsMultijoueur;

        public bool IsInMaintenance() => typeScene is InMaintenance;
        public bool IsDisconnecting => _disconnecting;
        public bool IsBatleRoyal => _modeBattleRoyal;

        // ------------ Setters ------------
        
        public void SetVisée(bool value)
        {
            if (visé)
            {
                visé.SetActive(value);
            }
        }
        
        public void SetOwnPlayer(PlayerClass value)
        {
            if (ownPlayer is null)
            {
                ownPlayer = value;

                if (typeScene is InGuessWho)
                {
                    InterfaceInGameManager.Instance.SetUp(ownPlayer, _timeEnd);
                }
            }
            else
            {
                throw new Exception("Un script a tenté de réinitialiser la variable ownPLayer");
            }
        }
        
        public void AjoutPlayer(PlayerClass player)
        {
            players.Add(player);
            
            // ce if s'active lorsque tous les joueurs ont créé leur avatar et l'ont ajouté à la liste 'players'
            if (players.Count == nParticipant && typeScene is InGuessWho)
            {
                TabMenu.Instance.Updateinfos();
            }
        }
        
        public void AjoutChasseur(Chasseur value)
        {
            chasseurs.Add(value);

            if (typeScene is InGuessWho)
            {
                TabMenu.Instance.NewChasseur(value);
            }
        }
        public void AjoutChassé(Chassé value)
        {
            chassés.Add(value);
            
            if (typeScene is InGuessWho)
            {
                TabMenu.Instance.NewChassé(value);
            }
        }

        public void SetTimeEnd(int value)
        {
            _timeEnd = value;
        }

        public void SetActiveWarning(bool value)
        {
            photoWarning.SetActive(value);
        }
        
        public void ClignotementWarning()
        {
            photoWarning.SetActive(!photoWarning.activeSelf);
        }

        public void LetsGetReadyToRambo()
        {
            _modeBattleRoyal = true;
        }

        public void DestroyAudioListener()
        {
            if (audioListener)
            {
                Destroy(audioListener);
            }
        }

        public void SendInfo()
        {
            if (!_infoSent && PhotonNetwork.IsMasterClient)
            {
                settingsGame.Send();
                _infoSent = true;
            }
        }

        // ------------ Constructeurs ------------
        
        private void Awake()
        {
            // On peut faire ça puisqu'il y en aura qu'un seul
            Instance = this;
            
            if (canvas)
            {
                // le cas où on a oublié de le remettre
                canvas.gameObject.SetActive(true);
            }

            if (photoWarning)
            {
                // le cas où on a oublié de le désactiver
                SetActiveWarning(false);
            }
            
            // instancier le nombre de joueur
            nParticipant = PhotonNetwork.PlayerList.Length;

            // instancier les listes
            players = new List<PlayerClass>();
            chasseurs = new List<Chasseur>();
            chassés = new List<Chassé>();
            spectateurs = new List<Spectateur>();
            
            if (isBordNeeded)
            {
                // récupérer les contours de la map
                RecupContour();
            }
            
            // determiner le typeScene
            if (CrossManager.Instance && CrossManager.Instance.IsMaintenance) // maintenance des crossPoints
            {
                Debug.Log("Début Maintenance des CrossPoints");
                typeScene = new InMaintenance(nParticipant);
            }
            else
            {
                typeScene = RecupManagerGame();
            }
            
            // pour pas que ça se termine instantanément
            _timeEnd = (int)PhotonNetwork.Time + 60;
            
            // etat
            _disconnecting = false;
            
            // infos
            if (PhotonNetwork.IsMasterClient)
            {
                SendInfo();
            }
        }

        public void Start()
        {
            if (typeScene.IsMultijoueur)
            {
                // 
            }
            else
            {
                int i = 0;
                foreach (TypeBot typeBot in typeScene.GetTypeBot())
                {
                    BotManager.Instance.CreateBot(typeBot, i++);
                }
            }
        }
        
        // ------------ Update ------------

        private void Update()
        {
            if (typeScene is InGuessWho && PhotonNetwork.Time >= _timeEnd)
            {
                if (IsBatleRoyal)
                {
                    EndGame(TypePlayer.Chasseur, "Everybody won");
                }
                else
                {
                    EndGame(TypePlayer.Chassé, "End by time");
                }
            }
        }

        // ------------ Private Méthodes ------------
        private void RecupContour()
        {
            Point[] contourPoint = GetComponentsInChildren<Point>();
            int l = contourPoint.Length;
            Vector3[] list = new Vector3[l];
            
            for (int i = 0; i < l; i++)
            {
                list[i] = contourPoint[i].transform.position;
            }

            // min
            contour.minZ = ManList.GetMin(list, ManList.Coord.Z);
            contour.minX = ManList.GetMin(list, ManList.Coord.X);
            // max
            contour.maxZ = ManList.GetMax(list, ManList.Coord.Z);
            contour.maxX = ManList.GetMax(list, ManList.Coord.X);
        }

        private ManagerGame RecupManagerGame()
        {
            switch (scene)
            {
                case TypeScene.Game:
                    return new InGuessWho(nParticipant);
                case TypeScene.Labyrinthe:
                    return new InLabyrinthe(nParticipant);
                case TypeScene.CageOiseaux:
                    return new InCageOiseaux(nParticipant);
                case TypeScene.EntrainementSaut:
                    return new InEntrainementSaut(nParticipant);
                case TypeScene.Bar:
                    return new InBar(nParticipant);
                default:
                    throw new Exception();
            }
        }
        
        // ------------ Multijoueur ------------

        public void SendInfoPlayer()
        {
            if (settingsGame.NChasseur == nParticipant)
            {
                // battle royale !!!!!!!!!!!!!!!!
                LetsGetReadyToRambo();
            }
            
            if (!PhotonNetwork.IsMasterClient)
                return;

            // les spawns
            int[] indexSpawnChasseur = SpawnManager.Instance.GetSpawnPlayer(TypePlayer.Chasseur);
            int iChasseur = 0;
            int[] indexSpawnChassé = SpawnManager.Instance.GetSpawnPlayer(TypePlayer.Chassé);
            int iChassé = 0;

            // les types en fonction du type de la partie
            TypePlayer[] types = typeScene.GetTypePlayer();
            int indexSpawn;

            for (int i = 0; i < nParticipant; i++)
            {
                if (types[i] != TypePlayer.None) // on va devoir envoyer quelque chose au PlayerManager
                {
                    if (types[i] == TypePlayer.Chasseur) // chasseur
                    {
                        indexSpawn = indexSpawnChasseur[iChasseur];
                        iChasseur++;
                    }
                    else if (types[i] == TypePlayer.Chassé) // chassé
                    {
                        indexSpawn = indexSpawnChassé[iChassé];
                        iChassé++;
                    }
                    else if (types[i] == TypePlayer.Blocard) // blocard
                    {
                        indexSpawn = indexSpawnChassé[iChassé];
                        iChassé++;
                    }
                    else
                    {
                        throw new Exception($"Pas encore géré le cas du {types[i]}");
                    }
                    
                    string infoJoueur = PlayerManager.EncodeFormatInfoJoueur(indexSpawn, types[i]);
                    
                    // envoi des infos au concerné(e)
                    Hashtable hash = new Hashtable();
                    hash.Add("InfoCréationJoueur", infoJoueur);
                    PhotonNetwork.PlayerList[i].SetCustomProperties(hash);
                }
            }
        }

        public void SendInfoBot()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            // les spawns
            int[] indexSpawnBotRectiligne = CrossManager.Instance.GetSpawnBot();
            
            int iRectiligne = 0;
            int[] indexSpawnReste = SpawnManager.Instance.GetSpawnBot();
            int iReste = 0;

            // les types en fonction du type de la partie et du nombre de joueur
            TypeBot[] types = typeScene.GetTypeBot();
            int[] nBotParJoueur = ManList.SplitResponsabilité(types.Length, nParticipant);

            int iType = 0;
            for (int iPlayer = 0; iPlayer < nParticipant; iPlayer++)
            {
                // rassembler toutes les infos des bots qu'est responsable le joueur
                int nBot = nBotParJoueur[iPlayer];
                (int indexSpawn, TypeBot type)[] infosBot = new (int, TypeBot)[nBot];

                for (int iBot = 0; iBot < nBot; iBot++, iType++)
                {
                    if (types[iType] == TypeBot.Rectiligne) // rectiligne
                    {
                        infosBot[iBot].indexSpawn = indexSpawnBotRectiligne[iRectiligne];
                        iRectiligne++;
                    }
                    else  // le reste
                    {
                        infosBot[iBot].indexSpawn = indexSpawnReste[iReste];
                        iReste++;
                    }

                    infosBot[iBot].type = types[iType];
                }
                
                if (infosBot.Length == 0)
                    continue; // rien à envoyer

                // envoi des infos au concerné(e)
                string mes = BotManager.EncodeFormatInfoBot(infosBot);
                
                Hashtable hash = new Hashtable();
                hash.Add("InfoCréationBots", mes);
                PhotonNetwork.PlayerList[iPlayer].SetCustomProperties(hash);
            }
        }
        
        // ------------ Event ------------
        
        // cette fontion seulement aux personnes rentrant dans la room alors que le jeu a déjà commencé
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (typeScene is InGuessWho && PhotonNetwork.IsMasterClient)
            {
                Hashtable hash = new Hashtable();
                
                // pour lui indiquer que ce sera un spectateur et le "time end"
                hash.Add("Retardataire", _timeEnd);

                // lui envoyer
                newPlayer.SetCustomProperties(hash);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            SupprList(players);
            SupprList(chasseurs);
            SupprList(chassés);
            SupprList(spectateurs);
            
            bool oneHuntedLeft = chassés.Count == 1;
            bool oneHunterLeft = chasseurs.Count == 1;

            void SupprList<T>(List<T> list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i] is null)
                    {
                        list.RemoveAt(i);
                    }
                }
            }
            
            string mes = "End by dead";
            
            if (oneHuntedLeft && chassés.Count == 0)
            {
                EndGame(TypePlayer.Chasseur, mes);
            }
            
            if (oneHunterLeft && chasseurs.Count == 0)
            {
                EndGame(TypePlayer.Chassé, mes);
            }
        }

        // ------------ GamePlay ------------

        public void Die(PlayerClass playerClass)
        {
            if (!ownPlayer || IsDisconnecting)
            {
                // cela veut dire qu'on est sur un joueur qui a quitté la partie
                // ou qui n'existe déjà plus, donc on ne fait rien de plus
                return;
            }

            if (!players.Contains(playerClass))
            {
                throw new Exception("Un script tente de supprimer un joueur de la liste qui n'y est plus");
            }

            players.Remove(playerClass); // remove de la liste players
            string mes = "End by dead";

            if (playerClass is Chassé)
            {
                // remove de la liste chassés
                chassés.Remove((Chassé) playerClass);

                if (chassés.Count == 0)
                {
                    EndGame(TypePlayer.Chasseur, mes);
                }
            }
            else if (playerClass is Chasseur)
            {
                // remove de la liste chasseurs
                chasseurs.Remove((Chasseur) playerClass);

                if (chasseurs.Count == 0)
                {
                    EndGame(TypePlayer.Chassé, mes);
                }
            }
            else if (playerClass is Blocard)
            {
                // rien, il n'est pas stocké dans une liste spécifique
            }
            else
            {
                throw new Exception($"La mort du type de {playerClass} n'est pas encore implémenté");
            }

            PhotonView pv = playerClass.GetPv(); // on récupère le point de vue du mourant

            if (!pv.IsMine || IsDisconnecting) // Seul le mourant créé un spectateur (s'il reste dans la partie)
                return;

            if (players.Count == 0)
                return;
            
            // création du spectateur
            Spectateur spectateur = PlayerManager.CreateSpectateur(pv);

            // ajout à la liste 'spectateurs'
            spectateurs.Add(spectateur);
        }

        private void EndGame(TypePlayer typeWinner, string mes)
        {
            /*PhotonNetwork.Destroy(ownPlayer.gameObject);
            
            players.Clear();
            chasseurs.Clear();
            chassés.Clear();
            spectateurs.Clear();*/

            _endedGame = true;
            MenuManager.Instance.OpenMenu("EndGame");
            EndGameMenu.Instance.SetUp(typeWinner, mes);
        }
        
        // ------------ Quitter ------------
        
        public void StartQuit()
        {
            _disconnecting = true;
            StartCoroutine(Quit());
        }
        
        private IEnumerator Quit()
        {
            PhotonNetwork.Disconnect();
            Destroy(RoomManager.Instance.gameObject);

            while(PhotonNetwork.IsConnected)
            {
                yield return null;
            }
            
            // c'est reparti pour le menu
            SceneManager.LoadScene(1);
        }
    }
}