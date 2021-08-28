using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Script.Audio;
using Script.EntityPlayer;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Script.Menu
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        // ------------ SerializeField ------------
        
        [Header("Menu Principale")]
        [SerializeField] TMP_InputField nameInputField;

        [Header("Jouer")]
        [SerializeField] private Button buttonPlay;
        
        [Header("Room")]
        [SerializeField] Transform roomListContent;
        [SerializeField] GameObject roomListItemPrefab;
        [SerializeField] TMP_InputField roomNameInputField;
        [SerializeField] TMP_Text errorText;

        [Header("Son")]
        [SerializeField] private SettingsMenu settingsMenu;
        
        [Header("Crédit")]
        [SerializeField] private Image background;
        
        // ------------ Attributs ------------
        
        public static Launcher Instance;
        private const string PlayerPrefsNameKey = "PlayerName";

        // ------------ Constructeur ------------
        
        private void Awake()
        {
            new TouchesClass();
            Instance = this;
        }
        
        //Se connecte au serveur que l'on retrouve dans Assets/Photon/Photon/UnityNetworking/Ressources/PhotonSer...
        private void Start()
        {
            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
            SetUpInputField();
            
            settingsMenu.StartVolume();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    
        // ------------ Connexion ------------
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("title");
            Debug.Log("Joined Lobby");
            SavePlayerName();
        }
        
        // ------------ Créer/Rejoindre ------------
        
        // Est appelé par un boutton
        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
                return;

            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu("loading");
            SavePlayerName();
        }
        
        // Est appelé par un boutton
        public void JoinRoom(RoomInfo info)
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("loading");
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(2);
            }
        }

        // ------------ Error ------------
    
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Room Creation Failed" + message;
            MenuManager.Instance.OpenMenu("error");
        }

        // ------------ Liste des chambres ------------

        //Est appelé automatiquement dés que y'a un changement dans la liste des rooms
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (Transform trans in roomListContent)
            {
                Destroy(trans.gameObject);
            }

            foreach (RoomInfo room in roomList)
            {
                //Unity ne supprime pas une room vide ou pleine ou caché, elle met juste l'attribut RemovedF... = true
                if (room.RemovedFromList)
                    continue;
                
                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
            }
        }
        
        // ------------ Names ------------

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsNameKey))
                return;
        
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

            nameInputField.text = string.IsNullOrEmpty(defaultName) ? "Somebody" : defaultName;
        }

        public void SetPlayerName()
        {
            buttonPlay.interactable = !string.IsNullOrEmpty(nameInputField.text);
        }

        private void SavePlayerName()
        {
            string playerName = nameInputField.text;
            PhotonNetwork.NickName = playerName;
            PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
        }

        //--------------Pour le crédit------------
        
        public void Jouerlavideo(VideoPlayer input)
        {
            input.Play();
            background.enabled = false;
        }

        public void Arreterlavideo(VideoPlayer input)
        {
            input.Stop();
            background.enabled = true;
        }
        
        // ------------ Quitter ------------

        public void QuitGame()
        {
            Application.Quit();
        }
        
        // ------------ Disconnect----------
        
         public void DisconnectUser()
        {
            PhotonNetwork.Disconnect();
             Destroy(RoomManager.Instance.gameObject);
             SceneManager.LoadScene(0);
        }
    }
}
