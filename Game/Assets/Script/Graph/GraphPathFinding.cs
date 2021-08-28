using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Script.DossierPoint;
using Script.Manager;
using Script.Test;
using Script.Tools;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Script.Graph
{
    public class GraphPathFinding : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        // static
        private static List<string> _ensembleKey = new List<string>();

        // clé de la recherche (sert pour les cp)
        private string _key;
        
        // positions
        private Vector3 _start;
        private CrossPoint _destination;
        
        // autre
        private MyFile<CrossPoint> _file;
        private Action<List<Vector3>> _renvoi;
        private Stopwatch _chrono;
        
        // ------------ Getter ------------

        public string Key => _key;

        public CrossPoint Destination => _destination;
        
        // ------------ Constructeur ------------

        private void Constructeur(CrossPoint start, CrossPoint destination, string key, Action<List<Vector3>> renvoi)
        {
            // vérification
            if (_ensembleKey.Contains(key))
            {
                throw new Exception($"Il ne peut avoir deux clés identiques (key = {key})");
            }
            
            // clef
            _ensembleKey.Add(key);
            _key = key;

            // positions
            _start = start.transform.position;
            _destination = destination;
            
            start.Origin(key);
            
            // file
            _file = new MyFile<CrossPoint>();
            _file.Enfiler(start);
            
            // autre
            _renvoi = renvoi;
            _chrono = Stopwatch.StartNew();
        }
        
        // ------------ Static Method(s) ------------

        public static void GetPath(CrossPoint start, CrossPoint destination, string key, Action<List<Vector3>> renvoi)
        {
            GraphPathFinding graph = Instantiate(MasterManager.Instance.GetOriginalGraphPathFinding(),
                Vector3.zero, Quaternion.identity).GetComponent<GraphPathFinding>();
            
            graph.Constructeur(start, destination, key, renvoi);
            
            // c'est parti
            
            graph.InvokeRepeating(nameof(Research), 0, 0.05f);
        }
        
        // ------------ Public Method(s) ------------

        public void AddCrossPoint(CrossPoint crossPoint)
        {
            _file.Enfiler(crossPoint);
        }
        
        // ------------ Private Method(s) ------------

        private void Research()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 300 && !_file.IsEmpty() && stopwatch.ElapsedMilliseconds < 20; i++)
            {
                _file.Defiler().SearchPath(this);
            }
            
            stopwatch.Stop();

            if (_file.IsEmpty())
            {
                // fin de la recherche
                _ensembleKey.Remove(_key);
                List<Vector3> path = Destination.EndResearchPath(_key);
                
                Destroy(gameObject);

                if (path is null)
                {
                    // recherche négative
                    //TestRayGaz.CreateMarqueur(_destination.transform.position, TestRayGaz.Couleur.Red);
                    
                    //throw new Exception("Recherche négative");
                    return;
                }
                
                if (SimpleMath.IsEncadré(path[path.Count - 1], _start) && 
                    SimpleMath.IsEncadré(path[0], _destination.transform.position))
                {
                    // tout va bien
                    _chrono.Stop();
                    //Debug.Log($"Recherche positive en {_chrono.ElapsedMilliseconds / 1000f} secondes");
                    
                    _renvoi(path);
                }
                else
                {
                    // erreur
                    TestRayGaz.CreateMarqueur(path[path.Count - 1], TestRayGaz.Couleur.Red);
                    throw new Exception();
                }
                
                Destroy(gameObject);
            }
        }
    }
}