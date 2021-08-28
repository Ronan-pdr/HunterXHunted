using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Menu;
using UnityEngine;

namespace Script.Bar
{
    public class EscapeBarMenu : MonoBehaviourPunCallbacks
    {
        // ------------ SerializeField ------------
    
        [Header("Boutton")]
        [SerializeField] private GameObject gameSettingsButton;
        [SerializeField] private GameObject startGameButton;
        
        // ------------ Attributs ------------

        private bool _isStartingGame;
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            SetPrivilegeMaster();
            _isStartingGame = false;
        }

        // ------------ Public Methodes ------------

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            SetPrivilegeMaster();   
        }
    
        public void StartGame()
        {
            if (_isStartingGame)
                return;

            _isStartingGame = true;
            
            if (PhotonNetwork.MasterClient.NickName == "Labyrinthe")
            {
                PhotonNetwork.LoadLevel(4);
            }
            else
            {
                // c'est parti pour le guess who
                PhotonNetwork.LoadLevel(3);
            }
        }
    
        // ------------ Private Methodes ------------

        private void SetPrivilegeMaster()
        {
            gameSettingsButton.SetActive(PhotonNetwork.IsMasterClient);
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
}
