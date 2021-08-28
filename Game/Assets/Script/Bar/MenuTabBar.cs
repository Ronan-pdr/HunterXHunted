using System;
using Photon.Pun;
using Photon.Realtime;
using Script.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Bar
{
    public class MenuTabBar : MonoBehaviourPunCallbacks
    {
        // ------------ SerializeField ------------
        
        [Header("Pour liste joueurs")]
        [SerializeField] TextMeshProUGUI[] playerListContent;

        [Header("Room")]
        [SerializeField] private TMP_Text roomNameText;

        // ------------ Constructeur ------------

        private void Awake()
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            SetListPlayer();
        }
        
        // ------------ Public Methods ------------

        public void Open()
        {
            MenuManager.Instance.OpenMenu(GetComponent<Menu.Menu>());
            SetListPlayer();
        }

        // ------------ Private Methods ------------
        
        private void SetListPlayer()
        {
            // effacer
            foreach (TextMeshProUGUI content in playerListContent)
            {
                content.text = "";
            }
            
            // récup info
            Player[] players = PhotonNetwork.PlayerList;
            int nbPlayer = players.Length;
            int nContent = playerListContent.Length;
            
            // écrire
            for (int i = 0; i < nbPlayer; i++)
            {
                playerListContent[i % nContent].text += players[i].NickName + Environment.NewLine;
            }
        }

        // ------------ Event ------------
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            SetListPlayer();
        }

        public override void OnPlayerLeftRoom(Player _)
        {
            SetListPlayer();
        }
    }
}