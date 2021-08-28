using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Script.Menu
{
    public class RoomListItem : MonoBehaviour
    {
        // ------------ SerializeField ------------
    
        [SerializeField] private TMP_Text text;

        // ------------ Attributs ------------
        
        private RoomInfo _info;
    
        // ------------ Constructeur ------------
        
        public void SetUp(RoomInfo info)
        {
            _info = info;
            text.text = _info.Name;
        }
        
        // ------------ Methodes ------------

        public void OnClick()
        {
            Launcher.Instance.JoinRoom(_info);
        }
    }
}
