using System;
using System.Collections.Generic;
using Photon.Pun;
using Script.EntityPlayer;
using Script.Graph;
using Script.Manager;
using Script.Test;
using Script.TeteChercheuse;
using Script.Tools;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Script.DossierPoint
{
    public class CrossPoint : Point
    {
        private class Node
        {
            // ------------ Attibuts ------------

            private float _bestDist;
            private Node _previous;
            //private Line _bridge;
            private Vector3 _pos;
            
            // ------------ Getter ------------
            public float BestDist => _bestDist;
            public Node Previous => _previous;
            public Vector3 Pos => _pos;
            
            // ------------ Setter ------------

            public void BetterDist(float dist)
            {
                if (dist > _bestDist)
                {
                    throw new Exception();
                }

                _bestDist = dist;
                //_bridge.SetColor(DistToCouleur(_bestDist));
            }

            public bool NewPath(Node node)
            {
                float dist = node._bestDist + Calcul.Distance(_pos, node._pos);
                
                if (dist < _bestDist)
                {
                    _bestDist = dist;
                    _previous = node;
                    //_bridge.SetNode(_pos, _previous._pos);
                    //_bridge.SetColor(DistToCouleur(_bestDist));
                    return true;
                }
                
                return false;
            }

            // ------------ Constructeur ------------

            public Node(Vector3 pos)
            {
                // l'origine d'un graph
                
                _bestDist = 0;
                _previous = null;
                //_bridge = null;
                _pos = pos;
            }
            
            public Node(Node previous, Vector3 pos)
            {
                _previous = previous;
                _pos = pos;
                
                _bestDist = previous._bestDist + Calcul.Distance(_pos, previous._pos);
                
                //_bridge = Line.Create(_pos, previous._pos, DistToCouleur(_bestDist));
            }
            
            // ------------ Method(s) ------------

            private float DistToCouleur(float dist)
            {
                return dist * 4;
            }
        }
        
        // ------------ SerializeField ------------
        
        [Header("Ajustement")]
        [SerializeField] private List<CrossPoint> neighboors;

        // ------------ Attributs ------------
        
        // Les instances (uniques)
        private CrossManager _crossManager;
        private CrossMaintenance _crossMaintenance;
        
        // pour la maintenance
        private MyFile<CrossPoint> _potentialNeighboors;
        private int _indexFile;
        
        // pour le path finding
        private Dictionary<string, Node> _nodes;

        // ------------ Getters ------------
        public CrossPoint GetNeighboor(int index) => neighboors[index];
        public int GetNbNeighboor() => neighboors.Count;
        public int IndexFile => _indexFile;
        private bool Actif => gameObject.activeInHierarchy;

        // ------------ Setter ------------
        public void AddNeighboor(CrossPoint value)
        {
            neighboors.Add(value);

            if (!PhotonNetwork.IsConnected && CrossManager.Instance.MustPrintGraph)
            {
                Line.Create(transform.position, value.transform.position, 200);
            }
        }

        public void ResetPathFinding(string key)
        {
            _nodes.Remove(key);
        }
        
        // ------------ Constructeur ------------
        private void Awake()
        {
            neighboors = new List<CrossPoint>();
            _nodes = new Dictionary<string, Node>();
        }

        private void Start()
        {
            _crossManager = CrossManager.Instance;

            if (!CrossManager.Instance.IsMaintenance)
            {
                // On veut pas les voir
                Invisible();
            }
            
            gameObject.SetActive(true);
        }
        
        // ------------ Méthodes ------------

        public static int NameToIndex(string namePoint)
        {
            string s = "";
            for (int i = namePoint.Length - 2; i >= 0 && namePoint[i] != '('; i--)
            {
                s = namePoint[i] + s;
            }

            if (int.TryParse(s, out int index))
            {
                return index;
            }
            
            throw new Exception($"Le nom '{namePoint}' n'est pas homologé (index = {s})");
        }
        
        public void RemoveNotGoodNeighboor()
        {
            for (int i = neighboors.Count - 1; i >= 0; i--)
            {
                if (!neighboors[i].Actif)
                {
                    // le cp est désactivé donc c'est un crosspoint
                    // que l'on NE DOIT PAS utiliser de la partie
                    neighboors.RemoveAt(i);
                }
            }
        }

        public void PrintNeightboors()
        {
            foreach (CrossPoint neighboor in neighboors)
            {
                Line.Create(transform.position, neighboor.transform.position, 150);
            }
        }
        
        // ------------ PathFinding ------------

        public void Origin(string key)
        {
            if (_nodes.ContainsKey(key))
            {
                throw new Exception("Impossible que l'origine d'un graph soit déjà parcouru durant cette recherche puiqu'elle est censée commencer");
            }
            
            _nodes.Add(key, new Node(transform.position));
        }

        public void SearchPath(GraphPathFinding graph)
        {
            string key = graph.Key;
            
            if (!_nodes.ContainsKey(key))
            {
                throw new Exception("Le node doit déjà être créé pour faitre les recherches sur ces voisins");
            }

            Node node = _nodes[key];
            
            foreach (CrossPoint crossPoint in neighboors)
            {
                crossPoint.Parcourir(graph, node);
            }
        }

        private void Parcourir(GraphPathFinding graph, Node previous)
        {
            string key = graph.Key;
            
            if (_nodes.ContainsKey(key))
            {
                // ce cross point a déjà été parcouru par cette recherche
                if (_nodes[key].NewPath(previous))
                {
                    // le nouveau chemin trouvé est plus court --> ajustement
                    Ajustement(key);
                }
            }
            else
            {
                // ce cross point n'a jamais été parcouru par cette recherche
                _nodes.Add(key, new Node(previous, transform.position));
                if (this != graph.Destination)
                {
                    // si c'est la destination, inutile de relancer la recherche sur celui-ci
                    graph.AddCrossPoint(this);
                }
            }
        }

        private void Ajustement(string key)
        {
            Node node = _nodes[key];
            
            foreach (CrossPoint crossPoint in neighboors)
            {
                if (crossPoint._nodes.ContainsKey(key))
                {
                    // ce point a déjà été parcouru
                    
                    if (crossPoint._nodes[key].Previous == node)
                    {
                        // ce point avait pour chemin ce cross point --> il faut update

                        crossPoint._nodes[key].BetterDist(node.BestDist + Calcul.Distance(transform.position, crossPoint.transform.position));
                        crossPoint.Ajustement(key);
                    }
                }
            }
        }

        public List<Vector3> EndResearchPath(string key)
        {
            if (!_nodes.ContainsKey(key))
            {
                // recherche négative
                Debug.Log("recherche négative");
                return null;
            }
            
            Node node = _nodes[key];
            List<Vector3> path = new List<Vector3>();

            // c'est la position de la destination
            path.Add(transform.position);
            HumanCapsule capsule = MasterManager.Instance.GetHumanCapsule();

            while (node.Previous != null)
            {
                Node nextNode = node.Previous;
                
                for (int i = 0; i >= 0 && nextNode.Previous != null && capsule.CanIPass(node.Pos,
                    Calcul.Diff(nextNode.Previous.Pos, node.Pos),
                    Calcul.Distance(nextNode.Previous.Pos, node.Pos)); i++)
                {
                    nextNode = nextNode.Previous;
                }

                node = nextNode;
                path.Add(node.Pos);
            }
            
            _crossManager.ResetPathFinding(key);

            return path;
        }

        // ------------ Maintenance ------------

        private void Error(string nameFunc)
        {
            if (!CrossManager.Instance.IsMaintenance)
            {
                throw new Exception($"Impossible que la fonction '{nameFunc}' soit appelé s'il n'y a pas en maintenance");
            }
        }
        
        public void EndResearchBody(CrossPoint neighboor) // est appelé dans la class 'BodyChercheur', dans la fonction 'Update'
        {
            Error("EndResearchBody");
            
            if (_potentialNeighboors is null) // tout reçu
            {
                throw new Exception("trop de résultat de body chercheur ont été reçu");
            }

            // est-ce un voisin valide
            if (!(neighboor is null))
            {
                // N'était il pas déjà dedans ?
                if (neighboors.Contains(neighboor))
                {
                    throw new Exception($"Ca devrait être impossible (Lanceur -> {name}, Dest -> {neighboor})");
                }
                
                AddNeighboor(neighboor);
                _crossMaintenance.OneNewNeighboorFind(); // incrémente un indicateur
            }

            if (_potentialNeighboors.IsEmpty()) // tout reçu
            {
                _potentialNeighboors = null;
                _crossMaintenance.EndOfPointResearch(this, neighboors);
            }
            else // ça continue d'en envoyer
            {
                NextResearch();
            }
        }

        public void SearchNeighboors(CrossMaintenance crossMaintenance, int indexFile)
        {
            _indexFile = indexFile;
            _crossMaintenance = crossMaintenance;
            
            if (!_crossManager)
            {
                _crossManager = CrossManager.Instance;
            }
            
            Error("SearchNeighboors");

            _potentialNeighboors = GetPotentialNeigboors();

            if (_potentialNeighboors.IsEmpty()) // aucun voisin potentiel donc c'est la fin de la recherche
            {
                _crossMaintenance.EndOfPointResearch(this, neighboors);
            }
            else // ça commence
            {
                NextResearch();
            }
        }

        private void NextResearch()
        {
            Error("NextResearch");
            
            GameObject destination = _potentialNeighboors.Defiler().gameObject;
            BodyRectilgne.InstancierStatic(gameObject, destination);
        }
        
        private MyFile<CrossPoint> GetPotentialNeigboors()
        {
            Error("GetPotentialNeigboors");
            
            MyFile<CrossPoint> potentialNeighboors = new MyFile<CrossPoint>();
            Vector3 ownCoord = transform.position;
            float distanceMax = 22;
            
            //Debug.Log(name);

            foreach (CrossPoint crossPoint in _crossManager.GetAllCrossPoints())
            {
                // déjà repertorié
                if (!crossPoint ||neighboors.Contains(crossPoint))
                {
                    //Debug.Log($"Déja trouvé -> {potentialNeighboors[i].name}");
                    continue;
                }

                Vector3 pos = crossPoint.transform.position;
                float distanceThisWithDest = Calcul.Distance(ownCoord, pos);
                
                // trop proche ou trop loin
                if (!(0.4f < distanceThisWithDest && distanceThisWithDest < distanceMax))
                { 
                    //Debug.Log(potentialNeighboors[i].name);
                    continue;
                }

                float diffAlt = Calcul.Distance(ownCoord.y, pos.y);
                    
                // trop d'altitude pour la potentiel distance de montée
                if (Calcul.Distance(ownCoord, pos, Calcul.Coord.Y)*0.7f <= diffAlt)
                {
                    //Debug.Log(potentialNeighboors[i].name);
                    continue; 
                }
                
                // plus de deux étages de différences...
                if (diffAlt > 9)
                {
                    continue;
                }
                
                potentialNeighboors.Enfiler(crossPoint);
            }
            
            //Debug.Log($"{name} va envoyé {n} bodyChercheur(s)");

            return potentialNeighboors;
        }
    }
}