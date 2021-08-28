using System;
using System.Collections.Generic;
using System.Diagnostics;
using Script.EntityPlayer;
using Script.Graph;
using Script.Manager;
using Script.Test;
using Script.Tools;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Script.TeteChercheuse
{
    public class RayGaz : MonoBehaviour
    {
        // ------------ Attributs ------------
        private class Node
        {
            public Node After { get; }
            public Vector3 Position { get; }

            public Node(Node after, Vector3 p)
            {
                After = after;
                Position = p;
            }
        }

        enum TypeRecherche
        {
            Sonde,
            Path
        }

        private TypeRecherche type;

        // fonctionde renvoi (lorsque l'algo se termine pour envoyer la réponse)
        private Action<List<Vector3>> RecepGetPath;
        private Action FinGetSonde;

        // les coordonnées origines
        private Vector3 coordOrigin;

        // Quelle est la distance qu'il va parcourir losqu'il va se déplacer ?
        // Sachant qu'il ne se déplace que tout droit 
        private float bond = 1f;

        // Cette matrice indique s'il a déjà parcouru une zone
        private Node[,] Sonde;
        private int heigth;
        private int width;

        // calculer la complexité
        private Stopwatch time;

        // cette file contient des positions valides (mais il y est peut-être déjà allé)
        // où il devra se répendre autour
        private MyFile<Node> file;

        // variable relative à sa capsule
        private HumanCapsule capsule;

        // position
        private Vector3 _depart;
        private Vector3 destination;
        
        // ------------ Getters ------------
        
        // ce qu'on l'on est amené à appeler dans les autres classes
        // s'il n'y a aucun chemin possible, renvoie une liste vide
        public static void GetPath(Vector3 depart, Vector3 destination, Action<List<Vector3>> f)
        {
            RayGaz rayGaz = Instantiate(MasterManager.Instance.GetOriginalRayGaz(), Vector3.up * 50, Quaternion.identity);

            rayGaz.SetRecepGetPath(f, destination);
            rayGaz.Instancier(depart, TypeRecherche.Path);
        }

        public static RayGaz GetSonde(Vector3 depart, Action f)
        {
            RayGaz rayGaz = Instantiate(MasterManager.Instance.GetOriginalRayGaz(), Vector3.up * 50, Quaternion.identity);

            rayGaz.SetFinGetSonde(f);
            rayGaz.Instancier(depart, TypeRecherche.Sonde);

            return rayGaz;
        }

        // ------------ Setters ------------
        private void SetRecepGetPath(Action<List<Vector3>> f, Vector3 dest)
        {
            RecepGetPath = f;
            destination = dest;
        }

        private void SetFinGetSonde(Action f)
        {
            FinGetSonde = f;
        }

        // ------------ Constructeur ------------
        private void Instancier(Vector3 posInitiale, TypeRecherche t)
        {
            type = t;
            _depart = posInitiale;
            
            // légèrement modifier la position initiale en fonction du bond
            // c'est pour que les positions soit toujours bien calé avec la matrice
            posInitiale -= SimpleMath.Mod(posInitiale, bond) + Vector3.up * 0f;
            
            // Déterminer l'origine de notre matrice ainsi que ses dimensions grâce aux contours
            (float minZ, float minX, float maxZ, float maxX) contour = MasterManager.Instance.GetContour();
            
            coordOrigin = new Vector3(contour.minX, 0, contour.minZ);
            
            heigth = (int) ((contour.maxZ - contour.minZ) / bond);
            width = (int) ((contour.maxX - contour.minX) / bond);
            Sonde = new Node[heigth, width];
            
            // vérifiez que la position ininiale est bien comprise dans les bornes
            if (!IsValidPosition(posInitiale))
            {
                throw new Exception($"Un RayGaz ne peut-être lancé en dehors des contours ({posInitiale.x}, {posInitiale.y}, {posInitiale.z})");
            }
            
            // récupérer les côtes des bots pour les ray ------------
            capsule = MasterManager.Instance.GetHumanCapsule();
            
            // enregistrer temps au début de la recherche
            time = new Stopwatch();
            time.Start();

            // enfiler la première position and Let's this party started
            Node first = new Node(null, posInitiale);
            file = new MyFile<Node>();
            file.Enfiler(first);
            CheckPosition(first);
        }
        
        // ------------ Update ------------

        // la fonction principale du rayGaz
        // inspiré du parcours largeur
        private void Update()
        {
            // Impossible que la file soit empty ici
            Node node = file.Defiler();
            
            for (int i = 0; i < 100 && (!file.IsEmpty() || i == 0) && !Arrivé(node.Position); i++)
            {
                if (i > 0)
                    node = file.Defiler();
                
                // temporaire
                //TestRayGaz.CreateMarqueur(node.Position + Vector3.up * 1f, TestRayGaz.Couleur.Brown);

                // devant
                NewPosition(node, Vector3.forward, bond);
                // derriere
                NewPosition(node, Vector3.back, bond);
                // droite
                NewPosition(node, Vector3.right, bond);
                // gauche
                NewPosition(node, Vector3.left, bond);

                float maxDist = bond * SimpleMath.Sqrt(2.1f);
                
                // devant - droite
                //NewPosition(node, new Vector3(1, 0, 1), maxDist);
                // devant - gauche
                //NewPosition(node, new Vector3(-1, 0, 1), maxDist);
                // derrière - droite
                //NewPosition(node, new Vector3(1, 0, -1), maxDist);
                // derrière - gauche
                //NewPosition(node, new Vector3(-1, 0, -1), maxDist);
            }

            if (file.IsEmpty())
            {
                FinForcé();
            }
            else if (Arrivé(node.Position)) // bien arrivé
            {
                time.Stop();
                Debug.Log($"Le gaz s'est répendu en {time.ElapsedMilliseconds/60000} minute(s) et {time.ElapsedMilliseconds/1000%60} seconde(s)");
                
                RecepGetPath(GetBestPath(node)); // on est obligé d'être dans 'GetPath'
                Destroy(gameObject); // c'est fini donc il se détruit
            }
        }
        
        // ------------ Méthodes ------------

        private void FinForcé()
        {
            switch (type)
            {
                case TypeRecherche.Path:
                    //Debug.Log($"Il existe aucun chemin pour y accéder ({destination.x}, {destination.y}, {destination.z})");
                    RecepGetPath(new List<Vector3>());
                    Destroy(gameObject);
                    break;
                case TypeRecherche.Sonde:
                    Debug.Log($"Le gaz s'est répendu en {time.ElapsedMilliseconds} milisecondes");
                    FinGetSonde();
                    enabled = false;
                    break;
                default:
                    throw new Exception($"Le cas de {type} n'a pas encore été géré");
            }
        }

        private void CheckPosition(Node node)
        {
            Vector3 p = node.Position;
            Sonde[GetIndexZ(p), GetIndexX(p)] = node;
        }

        // cette fonction vérifie d'une part si la position "p"
        // se situe sur le terrain (dans les bornes de la matrice)
        // et si l'on ne l'a pas déjà traitée
        private bool IsValidPosition(Vector3 p)
        {
            int x = GetIndexX(p);
            int z = GetIndexZ(p);

            bool inBorne = 0 <= x && x < width && 0 <= z && z < heigth;
            
            return inBorne && Sonde[z, x] == null;
        }

        private void NewPosition(Node after, Vector3 direction, float maxDistance)
        {
            Vector3 position = after.Position;
            
            //Debug.Log($"Can I pass ? {CanIPass(position, direction)}");
            //Debug.Log($"Est ce valide ? {IsValidPosition(position)}");

            Vector3 newPos = position + direction * bond;

            if (capsule.CanIPass(position, direction, maxDistance) && IsValidPosition(newPos))
            {
                // nous avons trouvé une position où le gaz va se répendre...
                Node node = new Node(after, newPos);
                
                // ...plus qu'à l'enfiler et...
                file.Enfiler(node);
                
                // ...indiquer que cette position va être traiter,
                // ainsi, il sera inutile de la refaire
                CheckPosition(node);
                
                // affichage
                Line.Create(position, newPos, Calcul.Distance(_depart, newPos) * 4);
            }
        }

        private bool Arrivé(Vector3 p)
        {
            if (type == TypeRecherche.Sonde)
            {
                return false;
            }
            
            return Calcul.Distance(p, destination, Calcul.Coord.Y) < bond;
        }

        // La position donnée en argument sera le premier de la liste
        public List<Vector3> GetBestPath(Vector3 v)
        {
            int x = GetIndexX(v);
            int z = GetIndexZ(v);

            return GetBestPath(Sonde[z, x]);
        }

        private List<GameObject> _gameObjects;

        private List<Vector3> GetBestPath(Node node)
        {
            List<Vector3> path = new List<Vector3>();
            
            // c'est la position de la destination
            path.Add(node.Position);
            
            _gameObjects = new List<GameObject>();

            Node nextNode;
            while (node.After != null)
            {
                nextNode = node.After;
                for (int i = 0; i >= 0 && nextNode.After != null && capsule.CanIPass(node.Position, Calcul.Diff(nextNode.After.Position, node.Position),
                    Calcul.Distance(nextNode.After.Position, node.Position)); i++)
                {
                    nextNode = nextNode.After;
                }

                node = nextNode;
                path.Add(node.Position);
            }

            return path;
        }
        
        // sert à récupérer l'index x de la matrice correspond à la position "p"
        private int GetIndexX(Vector3 p)
        {
            return (int)((p.x - coordOrigin.x) / bond);
        }
        
        // sert à récupérer l'index z de la matrice correspond à la position "p"
        private int GetIndexZ(Vector3 p)
        {
            return (int)((p.z - coordOrigin.z) / bond);
        }
    }
}