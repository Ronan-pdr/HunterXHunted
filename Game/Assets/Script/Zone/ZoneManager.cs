using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Script.DossierPoint;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.Zone
{
    public class ZoneManager : MonoBehaviour
    {
        // ------------ SerializeField ------------
        
        [Header("Dossier Spawn")]
        [SerializeField] private GameObject spawnInside;
        [SerializeField] private GameObject spawnBouffe;
        [SerializeField] private GameObject spawnCours;

        [Header("Dead Zone")]
        [SerializeField] private GameObject deadZoneInside;
        [SerializeField] private GameObject deadZoneBouffe;
        [SerializeField] private GameObject deadZoneCours;
        
        [Header("Sous Cross Manager")]
        [SerializeField] private SousCrossManager[] scmInside;
        [SerializeField] private SousCrossManager[] scmBouffe;
        [SerializeField] private SousCrossManager[] scmCours;
        
        [Header("Choose zone")]
        [SerializeField] private SettingsGame settings;
        
        [Header("Téléportation")]
        [SerializeField] private Transform[] tps;
        
        // ------------ Attributs ------------

        public static ZoneManager Instance;

        public enum EnumZone
        {
            All = 0,
            Inside = 1,
            Outside = 2,
            Bouffe = 3,
            Cours = 4
        }
        
        // Zones
        private Zone _inside;
        private Zone _bouffe;
        private Zone _cours;
        
        // téléportation
        private int _indexTp;
        
        // ------------ Getter ------------

        public Transform GetTp()
        {
            _indexTp += 1;
            if (_indexTp < tps.Length)
            {
                return tps[_indexTp];
            }

            return null;
        }
        
        // ------------ Constructeur ------------

        private void Awake()
        {
            Instance = this;
            
            if (!PhotonNetwork.IsConnected)
            {
                SetZone();
            }

            _indexTp = -1;
        }

        public void SetZone()
        {
            // set les zones
            InitialiserZones();
            
            // les aciver ceux qu'on active
            bool[] zonesToActive = GetZonesToAcitve();

            // les activer ou non
            List<SousCrossManager> scmToActive = new List<SousCrossManager>();
            
            _inside.SetActive(zonesToActive[0], ref scmToActive);
            _bouffe.SetActive(zonesToActive[1], ref scmToActive);
            _cours.SetActive(zonesToActive[2], ref scmToActive);
            
            CrossManager cm = CrossManager.Instance;

            if (!cm.IsMaintenance)
            {
                cm.ActiveSousCrossManager(ManList<SousCrossManager>.Copy(scmToActive));
            }
        }
        
        // ------------ Methods ------------

        private void InitialiserZones()
        {
            // inside
            _inside = new Zone(spawnInside, deadZoneInside, scmInside);
            
            // bouffe
            _bouffe = new Zone(spawnBouffe, deadZoneBouffe, scmBouffe);
            
            // cours
            _cours = new Zone(spawnCours, deadZoneCours, scmCours);
        }

        private bool[] GetZonesToAcitve()
        {
            // inside, bouffe, cours
            
            switch (settings.Zone)
            {
                case EnumZone.All:
                    return new[] {true, true, true};
                case EnumZone.Inside:
                    return new[] {true, false, false};
                case EnumZone.Outside:
                    return new[] {false, true, true};
                case EnumZone.Bouffe:
                    return new[] {false, true, false};
                case EnumZone.Cours:
                    return new[] {false, false, true};
                default:
                    throw new Exception();
            }
        }
    }
}
