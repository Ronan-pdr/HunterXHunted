using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Script.EntityPlayer;
using Script.InterfaceInGame;
using Script.Menu;
using UnityEngine;
using Random = System.Random;

namespace Script.Bar
{
    public class BarManager : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Menu")]
        [SerializeField] private Menu.Menu menuWaiting;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private MenuTabBar tabMenu;
        [SerializeField] private SettingsMenu settingsMenus;

        [Header("Spawner")]
        [SerializeField] private Transform[] spawns;
        
        // ------------ Attributs ------------

        public static BarManager Instance;
        private Random _rnd;
        
        // ------------ Getter ------------

        public Transform GetSpawn()
        {
            return spawns[_rnd.Next(spawns.Length)];
        }
        
        // ------------ Setter ------------

        public void NewHunted(Chassé value)
        {
            // enlever son coeur
            value.GetComponentInChildren<HumanHeart>().gameObject.SetActive(false);
        }
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            // s'initialiser et les autres
            Instance = this;
            PauseMenu.Instance = pauseMenu;
            
            // initialiser le reste
            _rnd = new Random();
            
            // changer de nom
            PhotonNetwork.NickName = ChangeName(PhotonNetwork.NickName, RecupNameOtherPlayers());
        }

        private void Start()
        {
            // afficher le bon menu
            MenuManager.Instance.OpenMenu(menuWaiting);
            
            settingsMenus.StartVolume();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ------------ Update ------------

        private void Update()
        {
            // la souris
            if (pauseMenu.GetIsPaused())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }

            // manager
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenu.GetIsPaused())
                {
                    // fermer le menu escape
                    pauseMenu.Resume();
                }
                else
                {
                    // ouvrir le menu escape
                    pauseMenu.Pause();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                // ouvrir le menu tab
                tabMenu.Open();
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                // fermer le menu tab
                MenuManager.Instance.OpenMenu(menuWaiting);
            }
        }
        
        // ------------ Publique méthode ------------

        public void Tp(Transform player)
        {
            Transform tp = GetSpawn();
                
            player.position = tp.position;
            player.rotation = tp.rotation;
        }
        
        // ------------ Private méthod ------------
        
        public static string ChangeName(string namePlayer, string[] namesOtherPlayer)
        {
            string res = namePlayer;
            string[] players = namesOtherPlayer;
            int count = 1;
            int l = players.Length;
            
            // arthur2 ; arthur

            for ((int j, int i) = (0, 0); j < l && i != l; j++)
            {
                for (i = 0; i < l && !ChangedName(namesOtherPlayer[i]); i++)
                {}
            }

            bool ChangedName(string nameOtherPlayer)
            {
                if (nameOtherPlayer == res)
                {
                    count += 1;
                    res = namePlayer + count;
                    return true;
                }

                return false;
            }

            return res;
        }
        
        private string[] RecupNameOtherPlayers()
        {
            Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
            int l = otherPlayers.Length;

            string[] namesOtherPlayer = new string[l];

            for (int i = 0; i < l; i++)
            {
                namesOtherPlayer[i] = otherPlayers[i].NickName;
            }

            return namesOtherPlayer;
        }
    }
}