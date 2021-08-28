using Photon.Pun;
using UnityEngine;
using Script.InterfaceInGame;
using Script.Tools;

namespace Script.EntityPlayer
{
    public class Spectateur : Entity
    {
        // ------------ Attributs ------------
        
        // Celui que l'on va suivre
        private Transform _porteur;
        private int _indexPorteur;
        private bool _thereIsSomeone;
        
        //Photon
        protected PhotonView Pv;
        
        //Variable similaire aux playerClass
        private float _yLookRotation;
        private float _xLookRotation;
        private float mouseSensitivity = 3f;

        // hauteur pour atteindre la tête
        private float hauteur = 1.4f;
        
        // interface
        private InterfaceInGameManager _interfaceInGameManager;
        
        // ------------ Setter ------------
        private void SetPorteur()
        {
            PlayerClass porteur = master.GetPlayer(_indexPorteur);

            if (!porteur)
            {
                _thereIsSomeone = false;
                return;
            }
                

            _porteur = porteur.transform;
            Position();

            if (Pv.IsMine)
            {
                _interfaceInGameManager.SetNameForSpect(porteur.name);
            }
        }
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            // primordial
            SetRbTr();
            Pv = GetComponent<PhotonView>();

            // Le ranger dans MasterClient
                transform.parent = master.transform;

            // interface
            _interfaceInGameManager = InterfaceInGameManager.Instance;
            _interfaceInGameManager.ChangeNbSpect(true);

            // reste
            _thereIsSomeone = true;
            _indexPorteur = 0;
            SetPorteur();
        }

        private void Start()
        {
            if (Pv.IsMine)
            {
                if (!master.IsGameEnded() && LauncherManager.Instance)
                {
                    LauncherManager.Instance.EndLoading();
                }
            }
            else
            {
                // On veut détruire les caméras qui ne sont pas les tiennes
                Destroy(GetComponentInChildren<Camera>().gameObject);
            }
        }

        // ------------ Update ------------
        
        private void Update()
        {
            if (!Pv.IsMine || !_thereIsSomeone)
                return;

            // le cas ou l'ancier porteur est mort ou à quitter la partie
            if (!_porteur)
            {
                _indexPorteur = 0;
                SetPorteur();
            }
            
            Position();

            if (PlayerClass.MustArret())
                return;

            Look();
            ChangerPorteur();
        }

        // ------------ Méthodes ------------
        
        private void Position()
        {
            Tr.position = _porteur.position + Vector3.up * hauteur;
        }

        private void Look()
        {
            _xLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            _xLookRotation = Mathf.Clamp(_xLookRotation, -50f, 30f);
            
            _yLookRotation += Input.GetAxisRaw("Mouse X") * mouseSensitivity;

            Tr.localEulerAngles = new Vector3(-_xLookRotation, _yLookRotation, 0);
        }

        private void ChangerPorteur()
        {
            //changer d'arme avec la molette
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                _indexPorteur = SimpleMath.Mod(_indexPorteur + 1, master.GetNbPlayer());
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                _indexPorteur = SimpleMath.Mod(_indexPorteur - 1, master.GetNbPlayer());
            }
            
            SetPorteur();
        }
        
        // ------------ Event ------------

        private void OnDestroy()
        {
            _interfaceInGameManager.ChangeNbSpect(false);
        }
    }
}