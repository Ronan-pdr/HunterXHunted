using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Script.Bar;
using Script.EntityPlayer;
using Script.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Menu
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        // ------------ Atributs ------------
        
        public static RoomManager Instance;

        // ------------ Constructeur ------------
        void Awake()
        {
            if (Instance) // checks if another RoomManager exists
            {
                Destroy(gameObject); // there can only be one
                return;
            }
        
            DontDestroyOnLoad(gameObject); // I am the only one...
            Instance = this;
        }
        
        // ------------ De la magie noire ------------

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        // ------------ manager les scènes ------------

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 2) // We are in the bar
            {
                BarManager barManager = BarManager.Instance;
            
                Transform spawn = barManager.GetSpawn();   
            
                Chassé hunted = PhotonNetwork.Instantiate("PhotonPrefabs/Humanoide/Chassé",
                    spawn.position, spawn.rotation).GetComponent<Chassé>();
            
                barManager.NewHunted(hunted);
            }
            else if (scene.buildIndex == 3) // We are in the game scene
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (MasterManager.Instance)
                    {
                        MasterManager.Instance.SendInfo();
                    }
                    
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                }

                PhotonNetwork.Instantiate("PhotonPrefabs/Manager/PlayerManager",
                    Vector3.zero, Quaternion.identity);
            }
        }
    }
}
