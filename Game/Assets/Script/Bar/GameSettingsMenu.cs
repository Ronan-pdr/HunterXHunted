using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Script.Manager;
using Script.Zone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Bar
{
    public class GameSettingsMenu : MonoBehaviourPunCallbacks
    {
        // ------------ SerializeField ------------
        
        [Header("Toggle")]
        [SerializeField] private Toggle all;
        [SerializeField] private Toggle inside;
        [SerializeField] private Toggle outside;
        [SerializeField] private Toggle bouffe;
        [SerializeField] private Toggle cours;
    
        [Header("Slider")]
        [SerializeField] private Slider nbChasseurNumber;
        [SerializeField] private Slider nbTimeNumber;
    
        [Header("Texte")]
        [SerializeField] private TMP_Text nbChasseurText;
        [SerializeField] private TMP_Text nbTimeText;
        
        // ------------ Attributs ------------
    
        private Dictionary<ZoneManager.EnumZone, Toggle> _dictToggle;
        private SettingsGame _settingsGame;

        private bool _noWantChange;
        
        // ------------ Constructeur ------------
        
        private void Start()
        {
            // récupérer les options par defaut
            _settingsGame = MasterManager.Instance.SettingsGame;
            ZoneManager.EnumZone zone = _settingsGame.Zone;
            
            // pour les sliders
            SetSliderNbChasseur();
            SetSliderTime();

            // pour les toggles
            _dictToggle = new Dictionary<ZoneManager.EnumZone, Toggle>();
            _dictToggle.Add(ZoneManager.EnumZone.All, all);
            _dictToggle.Add(ZoneManager.EnumZone.Inside, inside);
            _dictToggle.Add(ZoneManager.EnumZone.Outside, outside);
            _dictToggle.Add(ZoneManager.EnumZone.Bouffe, bouffe);
            _dictToggle.Add(ZoneManager.EnumZone.Cours, cours);

            // sécuriser
            _noWantChange = true;

            foreach (var kvp in _dictToggle)
            {
                kvp.Value.isOn = zone == kvp.Key;
            }
            
            // laisser couler
            _noWantChange = false;
        }
        
        // ------------ Public Methodes ------------

        public void SetNbChasseur(float value)
        {
            Set(value, nbChasseurText, _settingsGame.SetNbChasseur);
        }

        public void SetTime(float value)
        {
            Set(value, nbTimeText, _settingsGame.SetTimeMax);
        }

        public void OnChangedValue(int intZone)
        {
            if (_noWantChange)
                return;
            
            ZoneManager.EnumZone newZone = (ZoneManager.EnumZone) intZone;
            Toggle toggle = _dictToggle[newZone];
            
            // sécuriser
            _noWantChange = true;

            if (toggle.isOn)
            {
                // un toggle a été activé

                // désactiver l'ancien
                _dictToggle[_settingsGame.Zone].isOn = false;

                // changer les settings
                _settingsGame.SetZone(newZone);
                
                // activer le nouveau
                _dictToggle[newZone].isOn = true;
            }
            else
            {
                 // un toggle a été désactivé

                 // réactivation de ce toggle
                _dictToggle[newZone].isOn = true;
            }
            
            // laisser couler
            _noWantChange = false;
        }
        
        // ------------ Private Methodes ------------
        
        private void Set(float f, TMP_Text text, Action<int> func)
        {
            int nb = (int) f;
            func(nb);
            text.text = nb.ToString();
        }

        private void SetSliderNbChasseur()
        {
            SetSlider(nbChasseurNumber, 0, PhotonNetwork.CurrentRoom.PlayerCount, _settingsGame.NChasseur, nbChasseurText);
        }

        private void SetSliderTime()
        {
            SetSlider(nbTimeNumber, 1, 20, _settingsGame.TimeMax, nbTimeText);
        }
        
        private void SetSlider(Slider slider, int min, int max, int defaultValue, TMP_Text text)
        {
            slider.minValue = min;
            slider.maxValue = max;

            int value;

            if (min <= defaultValue && defaultValue <= max)
            {
                value = defaultValue;
            }
            else
            {
                value = (max - min) / 2;
            }

            text.text = value.ToString();
            slider.value = value;
        }
        
        // ------------ Event ------------

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            SetSliderNbChasseur();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            SetSliderNbChasseur();
        }
    }
}
