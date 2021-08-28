using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Script.Menu;
using Script.EntityPlayer;
using Script.Manager;
using PlayerManager = Script.EntityPlayer.PlayerManager;

namespace Script.InterfaceInGame
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Precision")]
        [SerializeField] private Menu.Menu menuToOpenWhenResume;
        
        // ------------ Attributs ------------
        
        public static PauseMenu Instance;
        
        // Etat
        private bool _isPaused;

        // ------------ Getters ------------
        public bool GetIsPaused() => _isPaused;

        // ------------ Constructeur ------------
        public void Awake()
        {
            _isPaused = false;
        }

        // ------------ Public Méthodes ------------

        public void Resume()
        {
            MenuManager.Instance.OpenMenu(menuToOpenWhenResume);
            _isPaused = false;
        }

        public void Pause()
        {
            MenuManager.Instance.OpenMenu("pause");
            _isPaused = true;
        }
    }
}