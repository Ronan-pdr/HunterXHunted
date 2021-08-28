using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Script.EntityPlayer;
using Script.Manager;
using TMPro;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Script.InterfaceInGame
{
    public class InterfaceInGameManager : MonoBehaviourPunCallbacks
    {
        // ------------ SerializedField ------------
        
        [Header("Texte")]
        [SerializeField] private TMP_Text textVie;
        [SerializeField] private TextMeshProUGUI textTime;

        [Header("Image vie")]
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Transform spriteContent;

        [Header("Reaction Degat")]
        [SerializeField] private GameObject teinteDegat;
        
        [Header("For Spectateur")]
        [SerializeField] private GameObject objNbSpect;
        [SerializeField] private TextMeshProUGUI textNbSpect;
        [SerializeField] private TextMeshProUGUI textNameToSpect;
        
        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI debug;

        // ------------ Attributs ------------
        
        public static InterfaceInGameManager Instance;
        
        private PlayerClass _player;
        private Image _imageVie;
        private float _timeEnd;
        private float _timeTeinteFalse;

        private int _nbSpect;
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            objNbSpect.SetActive(false);
            _imageVie = spriteContent.GetComponent<Image>();

            // pour spect
            _nbSpect = 0;
            textNbSpect.text = "0";
            textNameToSpect.text = "";
            
            teinteDegat.SetActive(false);
        }

        public void SetUp(PlayerClass player, float timeEnd)
        {
            _player = player;
            _timeEnd = timeEnd;
        }
        
        // ------------ Update ------------
        
        private void Update()
        {
            if (_player is null)
                return;
            
            int vie = _player.GetCurrentHealth();
            
            UpdateImageVie(vie);
            UpdateTextVie(vie);
            UpdateTime();
        }
        
        // ------------ Public Methodes ------------

        // pour les humanoides
        public void TakeDamage()
        {
            teinteDegat.SetActive(true);

            float periode = 0.6f;

            _timeTeinteFalse = Time.time + periode * 0.9f;
            Invoke(nameof(DesactiverTeinteDegat), periode);
        }

        // pour les scpectateurs
        public void ChangeNbSpect(bool incremente)
        {
            _nbSpect += incremente ? 1 : -1;
            textNbSpect.text = _nbSpect.ToString();
            
            objNbSpect.SetActive(_nbSpect > 0);

            if (_nbSpect < 0)
            {
                debug.text += $"NbSpect = {_nbSpect}";
            }
        }

        public void SetNameForSpect(string namePorteur)
        {
            textNameToSpect.name = namePorteur;
        }

        // pour debug
        public void Print(string mes)
        {
            debug.text += mes + Environment.NewLine;
        }
        
        // ------------ Private Methodes ------------

        private void DesactiverTeinteDegat()
        {
            if (Time.time >= _timeTeinteFalse)
            {
                teinteDegat.SetActive(false);
            }
        }

        private void UpdateImageVie(int v)
        {
            if (v <= 0)
            {
                // plus de vie
                _imageVie.sprite = sprites[0];
            }
            else
            {
                // encore de la vie
                int maxV = _player.GetMaxHealth();
                int len = sprites.Length;
                
                _imageVie.sprite = sprites[v * (len - 1) / maxV];
            }
        }
        
        private void UpdateTextVie(int vie)
        {
            textVie.text = vie <= 0 ? "Dead" : vie.ToString();
        }

        private void UpdateTime()
        {
            int tempsRestant = (int) (_timeEnd - PhotonNetwork.Time);
            int minutes = tempsRestant / 60;
            int secondes = tempsRestant % 60;

            string s = (secondes >= 10 ? "" : "0") + secondes;
            
            textTime.text = $"{minutes}:{s}";
        }
    }
}