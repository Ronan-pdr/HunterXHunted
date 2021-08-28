using System;
using System.Collections.Generic;
using Photon.Realtime;
using Script.EntityPlayer;
using Script.Manager;
using TMPro;
using UnityEngine;

namespace Script.InterfaceInGame
{
    public class TabMenu : MonoBehaviour
    {
        // ------------ SerializedField ------------

        [Header("Texte Hunter")]
        [SerializeField] private TextMeshProUGUI hunterNames;
        [SerializeField] private TextMeshProUGUI hunterLifes;
        
        [Header("Texte Hunted")]
        [SerializeField] private TextMeshProUGUI huntedNames;
        [SerializeField] private TextMeshProUGUI huntedLifes;
        
    
        // ------------ Attributs ------------
    
        public static TabMenu Instance;

        // les listes
        private List<PlayerInfoTab> _infosChasseurs = new List<PlayerInfoTab>();
        private List<PlayerInfoTab> _infosChassés = new List<PlayerInfoTab>();
        
        // ------------ Setter ------------

        public void NewChasseur(Chasseur value)
        {
            _infosChasseurs.Add(new PlayerInfoTab(value));
        }
        
        public void NewChassé(Chassé value)
        {
            _infosChassés.Add(new PlayerInfoTab(value));
        }

        // ------------ Constructeur ------------

        private void Start()
        {
            Updateinfos();
        }
        
        // ------------ Event ------------

        private void OnEnable()
        {
            Updateinfos();
        }

        // ------------ Publique méthodes ------------

        public void Updateinfos()
        {
            // hunter
            Write(_infosChasseurs, hunterNames, hunterLifes);
            
            // hunted
            Write(_infosChassés, huntedNames, huntedLifes);
            
            // fonction auxiliaire

            void Write(List<PlayerInfoTab> listInfos, TextMeshProUGUI nameP, TextMeshProUGUI life)
            {
                nameP.text = "";
                life.text = "";
                
                foreach (PlayerInfoTab infos in listInfos)
                {
                    infos.UpdatedInfos(nameP, life);
                }
            }
        }
    }
} 
