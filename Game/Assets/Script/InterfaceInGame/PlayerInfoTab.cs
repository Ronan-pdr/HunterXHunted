using System;
using Photon.Realtime;
using Script.EntityPlayer;
using TMPro;

namespace Script.InterfaceInGame
{
    public class PlayerInfoTab
    {
        // ------------ Attributs ------------

        private PlayerClass _player;
        private string _namePlayer;

        // ------------ Constructeur ------------

        public PlayerInfoTab(PlayerClass player)
        {
            _player = player;
            
            _namePlayer = player.name;
            int lenName = _namePlayer.Length;
            if (lenName > 9)
            {
                if (IsAlpha(_namePlayer[lenName - 1]))
                {
                    // prendre le numéro et couper le reste
                    _namePlayer = player.name.Substring(0, 8) + _namePlayer[lenName - 1];
                }
                else
                {
                    // couper le reste
                    _namePlayer = player.name.Substring(0, 9);
                }
            }
            
            bool IsAlpha(char c) => '0' <= c && c <= '9';
        }
        
        // ------------ Update ------------

        public void UpdatedInfos(TextMeshProUGUI nameP, TextMeshProUGUI life)
        {
            nameP.text += _namePlayer + Environment.NewLine;

            if (_player)
            {
                int vie = _player.GetCurrentHealth();
                life.text += vie <= 0 ? "Dead" : $"{vie}/{_player.GetMaxHealth()}" + Environment.NewLine;
            }
            else
            {
                life.text += "Dead" + Environment.NewLine;
            }
        }
    }
}