using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Script.Bar
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        // ------------ SerializeField ------------
    
        [Header("Affichage")]
        [SerializeField] private TMP_Text text;
    
        // ------------ Attributs ------------
    
        private Player _player;
    
        // ------------ Setter ------------
    
        public void SetUp(Player player)
        {
            _player = player;
            text.text = player.NickName;
        }
    
        // ------------ Methodes ------------

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (_player.Equals(otherPlayer))
            {
                Destroy(gameObject);
            }
        }

        public override void OnLeftRoom()
        {
            Destroy(gameObject);
        }
    }
}
