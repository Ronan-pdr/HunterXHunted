using UnityEngine;
using Photon.Pun;
using System.IO;
using Script.Tools;
using Script.EntityPlayer;
using Script.Bot;
using Script.DossierArme;
using Script.Manager;

namespace Script.TeteChercheuse
{
    public class BalleFusil : TeteChercheuse
    {
        // ------------ Attributs ------------
        
        private ArmeInfo armeInfo;
        private PhotonView Pv;

        private Chasseur _lanceur;
    
        // ------------ Constructeurs ------------
        
        private void Start()
        {
            SetRbTr();
    
            Pv = GetComponent<PhotonView>();
    
            // parenter
            Tr.parent = MasterManager.Instance.GetDossierBalleFusil();
            
            MoveAmount = new Vector3(0, 0, 150);
            
            // déplacer un peu
            Tr.position += Tr.TransformDirection(Vector3.forward) * 1;
        }
        
        
        public static void Tirer(Vector3 coordAnim, Transform cam, Chasseur lanceur, Vector3 rotation, ArmeInfo armeInf)
        {
            BalleFusil balleFusil = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "TeteChercheuse", "Balle" + armeInf.GetName()),
                coordAnim, Quaternion.identity).GetComponent<BalleFusil>();
            
            balleFusil.VecteurCollision(cam, lanceur, rotation, armeInf);
        }
    
        private void VecteurCollision(Transform cam, Chasseur lanceur, Vector3 rotation, ArmeInfo armeInf)
        {
            _lanceur = lanceur;
            armeInfo = armeInf;
            
            transform.Rotate(rotation);
            
            // le ray cast pour être sur que ça touche bien
            Ray ray = new Ray(cam.position, cam.TransformDirection(Vector3.forward));

            // est-je touché quelque chose ?
            if (Physics.Raycast(ray, out RaycastHit hit, armeInf.GetPortéeAttaque()))
            {
                //Debug.Log($"J'ai touché {hit.collider.name}");
                
                // est-ce un humain ?
                if (hit.collider.GetComponent<Humanoide>()) // si l'obstacle est le joueur alors le bot "VOIT" le joueur
                {
                    lanceur.WhenWeaponHit(hit.collider.gameObject, armeInf.GetDamage());
                }
            }
        }
    
        // ------------ Update ------------
        public void Update()
        {
            // Seul le créateur de la balle la contrôle
            if (!Pv.IsMine)
                return;
            
            // Si le tireur n'existe plus, la balle se détruit
            if (!_lanceur)
            {
                PhotonNetwork.Destroy(gameObject);
                return;
            }
            
            // si max distance -> il s'arrête
            if (Calcul.Distance(_lanceur.transform.position, Tr.position) > armeInfo.GetPortéeAttaque())
            {
                enabled = false;
                PhotonNetwork.Destroy(gameObject);
            }
        }

        public void FixedUpdate()
        {
            if (!Pv.IsMine) // Seul le créateur de la balle la contrôle
                return;
            
            MoveEntity();
        }
        
        // ------------ Event ------------
        private void OnTriggerEnter(Collider other)
        {
            OnCollisionAux(other);
        }
    
        private void OnTriggerStay(Collider other)
        {
            OnCollisionAux(other);
        }
    
        private void OnCollisionEnter(Collision other)
        {
            OnCollisionAux(other.collider);
        }
    
        private void OnCollisionStay(Collision other)
        {
            OnCollisionAux(other.collider);
        }

        private void OnCollisionAux(Collider other)
        {
            // si le lanceur est détruit sa balle ne fait plus de dégat
            if (!_lanceur)
                return;
            
            // Seul le créateur de la balle gère les collisions
            if (!Pv.IsMine)
                return;
            
            // Le cas où c'est avec notre propre personnage ou que c'est avec une autre tête chercheuse
            if (other.gameObject == _lanceur.gameObject || other.gameObject.GetComponent<TeteChercheuse>())
                return;
            
            //_lanceur.WhenWeaponHit(other.gameObject, armeInfo.GetDamage());

            PhotonNetwork.Destroy(gameObject);
        }
    }
}

