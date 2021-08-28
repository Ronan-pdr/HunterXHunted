using System;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.TeteChercheuse
{
    public class BodyGaz : BodyChercheur
    {
        private class Node
        {
            private Node After { get; }
            private Vector3 Key { get; }

            public Node(Node after, Vector3 key)
            {
                After = after;
                Key = key;
            }
        }
        
        enum Etat
        {
            Attente,
            Avancer
        }
        
        // 0 -> l'origine
        // 1 -> maxZ
        // 2 -> maxX
        private Vector3 coordOrigin;

        // Quelle est la distance qu'il va parcourir losqu'il va se déplacer ?
        // Sachant qu'il ne se déplace que tout droit 
        private float bond = 1f;

        // Cette matrice indique s'il a déjà parcouru une zone
        private bool[,] Sonde;
        private int heigth;
        private int width;

        // cette file contient les positions où il doit aller
        private MyFile<Vector3> file;
        
        // à chaque position, le body attend de savoir s'il est rentré dans un obstacle
        private float periodeAttente =  0.02f;
        private float timeEnvoi;
        private bool obstacle;
        
        // les coordonnées de la destination
        private Vector3 coordDest;

        private int GetIndexX()
        {
            return (int)((Tr.position.x - coordOrigin.x) / bond);
        }
        
        private int GetIndexZ()
        {
            return (int)((Tr.position.z - coordOrigin.z) / bond);
        }

        private void CheckPosition()
        {
            obstacle = false;
            timeEnvoi = Time.time;
            Sonde[GetIndexZ(), GetIndexX()] = true;
        }

        private bool IsCoordInvalid()
        {
            int x = GetIndexX();
            int z = GetIndexZ();

            bool inMat = 0 <= x && x < width && 0 <= z && z < heigth;
            
            return !inMat || Sonde[z, x];
        }

        public static void InstancierStatic(GameObject lanceur, GameObject destination)
        {
            // récupérer la préfab
            BodyGaz original = MasterManager.Instance.GetOriginalBodyGaz();

            // l'instancier localement
            BodyGaz body = Instantiate(original, Vector3.zero, Quaternion.identity);
            
            // instancier tous ces attributs
            body.Instancier(lanceur, destination);
        }

        private void Instancier(GameObject lanceur, GameObject destination)
        {
            SetRbTr();
            
            // grossir son capsule colider
            ownCapsuleCollider.height *= 2;
            ownCapsuleCollider.radius *= 2;
            
            // Initialiser Lanceur et Destination
            Lanceur = lanceur;
            Destination = destination;
            
            // Déterminer l'origine de notre matrice ainsi que ses dimensions avec les contours
            (float minZ, float minX, float maxZ, float maxX) contour = MasterManager.Instance.GetContour();
            
            coordOrigin = new Vector3(contour.minX, 0, contour.minZ);
            
            heigth = (int) ((contour.maxZ - contour.minZ) / bond);
            width = (int) ((contour.maxX - contour.minX) / bond);
            Sonde = new bool[heigth, width];

            // le positioner aux mêmes coordonnées que le lanceur mais calé avec bond
            Vector3 p = Lanceur.transform.position;
            Tr.position = p - SimpleMath.Mod(p, bond) + Vector3.up * 0f;
            
            // Initialiser la file and Let's the party started
            file = new MyFile<Vector3>();
            CheckPosition();
        }

        private void Update()
        {
            if (Time.time - timeEnvoi < periodeAttente)
            {
                return; // il faut attendre la réponse
            }

            if (!obstacle) // est ce que la position actuelle est valide et on y est jamais allé
            {
                //GameObject g = Instantiate(MasterManager.Instance.marqueur, Tr.position, Quaternion.identity);
                //g.transform.parent = MasterManager.Instance.GetDossierBodyChercheur();
                
                Vector3 p = Tr.position;
                
                file.Enfiler(p + Vector3.forward * bond); // avant
                file.Enfiler(p + Vector3.back * bond); // arriere
                file.Enfiler(p + Vector3.right * bond); // droite
                file.Enfiler(p + Vector3.left * bond); // gauche
            }
            
            if (file.IsEmpty())
            {
                Debug.Log("Impossible d'arriver à destination");
                enabled = false;
                return;
            }
            
            // nouvelle position
            do
            {
                Tr.position = file.Defiler();
                
            } while (IsCoordInvalid());
            
            CheckPosition();

            // est ce que le gaz est arrivé à destination
            if (Calcul.Distance(Tr.position, Destination.transform.position, Calcul.Coord.Y) < bond)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger");
            OnCollisionAux(other);
        }

        private void OnTriggerStay(Collider other)
        { 
            OnCollisionAux(other);
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("collision");
            OnCollisionAux(other.collider);
        }

        private void OnCollisionStay(Collision other)
        {
            OnCollisionAux(other.collider);
        }

        private void OnCollisionAux(Collider other)
        {
            if (other.GetComponent<Entity>()) // on ne s'arrête pas si c'est une Entity
            {
                return;
            }
            
            Debug.Log(other.name);

            obstacle = true;
        }
    }
}