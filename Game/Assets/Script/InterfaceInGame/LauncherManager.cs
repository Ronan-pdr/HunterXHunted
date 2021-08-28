using System;
using System.Collections;
using Photon.Pun;
using Script.EntityPlayer;
using Script.Manager;
using Script.Menu;
using UnityEngine;

namespace Script.InterfaceInGame
{
    public class LauncherManager : MonoBehaviour
    {
        // ------------ SerializedField ------------

        [Header("Menus")]
        [SerializeField] private InterfaceInGameManager interfaceInGame;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private TabMenu tabMenu;
        [SerializeField] private EndGameMenu endGameMenu;
        
        // ------------ Attributs ------------

        public static LauncherManager Instance;
        
        private MenuManager _menuManager;
        private bool _loading;
        
        // ------------ Setter ------------

        public void EndLoading()
        {
            _loading = false;
            MenuManager.Instance.OpenMenu("InterfaceInGame");
        }
        
        // ------------ Constructeur ------------
        private void Awake()
        {
            // s'instancier
            Instance = this;
            
            // instancier les autres
            InterfaceInGameManager.Instance = interfaceInGame;
            PauseMenu.Instance = pauseMenu;
            TabMenu.Instance = tabMenu;
            EndGameMenu.Instance = endGameMenu;
        }

        private void Start()
        {
            _menuManager = MenuManager.Instance;

            if (PhotonNetwork.IsConnected)
            {
                _menuManager.OpenMenu("loading");
                _loading = true;
            }
        }

        // ------------ Update ------------
        void Update()
        {
            if (MasterManager.Instance.IsDisconnecting || _loading)
                return;

            // la souris
            if (PlayerClass.MustArret())
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
            if (MasterManager.Instance.IsGameEnded())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                MenuManager.Instance.OpenMenu("EndGame");
                GestionGameEnded();
            }
            else
            {
                GestionInGame();
            }
        }

        // ------------ Private Méthodes ------------
        
        private void GestionInGame()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // géré le menu pause
                if (pauseMenu.GetIsPaused())
                {
                    pauseMenu.Resume();
                }
                else
                {
                    pauseMenu.Pause();
                }
            }
            else if (!pauseMenu.GetIsPaused())
            {
                // gérer le menu tab
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    // ouvrir menu tab
                    _menuManager.OpenMenu("tab");
                }
                else if (Input.GetKeyUp(KeyCode.Tab))
                {
                    // fermé menu tab -> ouvrir interfaInGame
                    _menuManager.OpenMenu("InterfaceInGame");
                }
            }
        }
        
        private void GestionGameEnded()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // ouvrir de force menu tab pour
                // pas que l'écran de win s'efface
                _menuManager.ForceOpenMenu("tab");
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                // fermé menu tab
                _menuManager.CloseMenu("tab");
            }
        }
    }
}