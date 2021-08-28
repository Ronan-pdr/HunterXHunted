using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Script.EntityPlayer;
using Script.Manager;
using Script.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.InterfaceInGame
{
    public class EndGameMenu : MonoBehaviourPunCallbacks
    {
        // ------------ Serialize Field ------------

        [Header("Restart")]
        [SerializeField] private GameObject restartGameButton;

        [Header("Ecran D'affichage")]
        [SerializeField] private TextMeshProUGUI textWinner;
        [SerializeField] private TextMeshProUGUI textMes;
        
        // ------------ Attributs ------------

        public static EndGameMenu Instance;

        private TypePlayer _winner;
        
        // ------------ Setters ------------

        public void SetUp(TypePlayer winner, string mes)
        {
            _winner = winner;

            // texte
            textMes.text = mes;
            textWinner.text = winner == TypePlayer.Chasseur ? "Hunter Won" : "Hunted Won";
            
            // Win ou lose
            textWinner.color = PlayerManager.Own.Type == _winner ? Color.green : Color.red;
            
            Start();
        }
        
        // ------------ Constructeurs ------------

        private void Start()
        {
            // Le masterClient a le pouvoir de restart
            restartGameButton.SetActive(PhotonNetwork.IsMasterClient);
            
            // Gérer la souris
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ------------ On Button Click ------------

        public void Restart()
        {
            // c'est parti pour le bar
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(2);
        }
        
        // ------------ Event ------------
        
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            restartGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }
}
