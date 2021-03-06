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
        
        // ------------ M??thodes ------------

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
            
            throw new Exception($"Le nom '{namePoint}' n'est pas homolog?? (index = {s})");
        }
        
        public void RemoveNotGoodNeighboor()
        {
            for (int i = neighboors.Count - 1; i >= 0; i--)
            {
                if (!neighboors[i].Actif)
                {
                    // le cp est d??sactiv?? donc c'est un crosspoint
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
                throw new Exception("Impossible que l'origine d'un graph soit d??j?? parcouru durant cette recherche puiqu'elle est cens??e commencer");
            }
            
            _nodes.Add(key, new Node(transform.position));
        }

        public void SearchPath(GraphPathFinding graph)
        {
            string key = graph.Key;
            
            if (!_nodes.ContainsKey(key))
            {
                throw new Exception("Le node doit d??j?? ??tre cr???? pour faitre les recherches sur ces voisins");
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
                // ce cross point a d??j?? ??t?? parcouru par cette recherche
                if (_nodes[key].NewPath(previous))
                {
                    // le nouveau chemin trouv?? est plus court --> ajustement
                    Ajustement(key);
                }
            }
            else
            {
                // ce cross point n'a jamais ??t?? parcouru par cette recherche
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
                    // ce point a d??j?? ??t?? parcouru
                    
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
                // recherche n??gative
                Debug.Log("recherche n??gative");
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
                throw new Exception($"Impossible que la fonction '{nameFunc}' soit appel?? s'il n'y a pas en maintenance");
            }
        }
        
        public void EndResearchBody(CrossPoint neighboor) // est appel?? dans la class 'BodyChercheur', dans la fonction 'Update'
        {
            Error("EndResearchBody");
            
            if (_potentialNeighboors is null) // tout re??u
            {
                throw new Exception("trop de r??sultat de body chercheur ont ??t?? re??u");
            }

            // est-ce un voisin valide
            if (!(neighboor is null))
            {
                // N'??tait il pas d??j?? dedans ?
                if (neighboors.Contains(neighboor))
                {
                    throw new Exception($"Ca devrait ??tre impossible (Lanceur -> {name}, Dest -> {neighboor})");
                }
                
                AddNeighboor(neighboor);
                _crossMaintenance.OneNewNeighboorFind(); // incr??mente un indicateur
            }

            if (_potentialNeighboors.IsEmpty()) // tout re??u
            {
                _potentialNeighboors = null;
                _crossMaintenance.EndOfPointResearch(this, neighboors);
            }
            else // ??a continue d'en envoyer
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
            else // ??a commence
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
                // d??j?? repertori??
                if (!crossPoint ||neighboors.Contains(crossPoint))
                {
                    //Debug.Log($"D??ja trouv?? -> {potentialNeighboors[i].name}");
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
                    
                // trop d'altitude pour la potentiel distance de mont??e
                if (Calcul.Distance(ownCoord, pos, Calcul.Coord.Y)*0.7f <= diffAlt)
                {
                    //Debug.Log(potentialNeighboors[i].name);
                    continue; 
                }
                
                // plus de deux ??tages de diff??rences...
                if (diffAlt > 9)
                {
                    continue;
                }
                
                potentialNeighboors.Enfiler(crossPoint);
            }
            
            //Debug.Log($"{name} va envoy?? {n} bodyChercheur(s)");

            return potentialNeighboors;
        }
    }
}