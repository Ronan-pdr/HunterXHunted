using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Photon.Pun;
using Script.EntityPlayer;
using Script.InterfaceInGame;
using Script.Manager;
using Script.Tools;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Script.DossierPoint
{
    public class CrossManager : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Maintenance")]
        [SerializeField] private bool inMaintenance;
        [SerializeField] private bool printAllGraph;
        
        [Header("Mainteance et jeu")]
        [SerializeField] private SousCrossManager[] sousCrossManagers;
        
        // ------------ Attributs ------------
        
        public static CrossManager Instance;
        
        private CrossPoint[] allCrossPoints;
        private CrossPoint[] _crossPoints;
        
        private string DossierRangement = "SauvegardeCrossManager/";

        // ------------ Getters ------------
        public bool IsMaintenance => inMaintenance;
        public bool MustPrintGraph => printAllGraph;
        public string GetDossier() => DossierRangement;
        public int GetNumberPoint() => _crossPoints.Length;
        
        public CrossPoint GetPoint(int index) => _crossPoints[index];

        public CrossPoint[] GetAllCrossPoints() => allCrossPoints;
        
        public int[] GetSpawnBot() => ManList.RandomIndex(_crossPoints.Length);
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            Instance = this;

            SetAllCrossPoint();
            for (int i = 0; i < allCrossPoints.Length; i++)
            {
                if (allCrossPoints[i] is null)
                {
                    Debug.Log($"La liste allCrossPoints a mal été créé ou le cross point avec l'index '{i}' n'existe pas");
                }
            }

            if (IsMaintenance)
            {
                foreach (SousCrossManager e in sousCrossManagers)
                {
                    gameObject.AddComponent<CrossMaintenance>().SetSousCrossManager(e);
                }
            }
            else
            {
                // On NE DOIT PAS utiliser cette liste si c'est pas une maintenance
                sousCrossManagers = null;
            }
        }

        private void Start()
        {
            foreach (SousCrossManager e in GetComponentsInChildren<SousCrossManager>())
            {
                LoadNeigboors(e);
            }
        }

        // ------------ Méthodes ------------

        public CrossPoint GetNearestPoint(Vector3 pos)
        {
            int l = _crossPoints.Length;

            CrossPoint point = _crossPoints[0];
            (CrossPoint point, float dist) best = (point, Calcul.Distance(pos, point.transform.position));

            for (int i = 1; i < l; i++)
            {
                point = _crossPoints[i];
                Vector3 posPoint = point.transform.position;
                
                float dist = Calcul.Distance(pos, posPoint);

                if (dist < best.dist && SimpleMath.Abs(pos.y - posPoint.y) < 1)
                {
                    best = (point, dist);
                }
            }

            return best.point;
        }
        
        private void SetAllCrossPoint()
        {
            // Range tous les crossPoints en fonction de leur nom (il y a un numéro dedans)
            // Vérifie si aucun crossPoint n'a le même numéro qu'un autre
            
            CrossPoint[] crossPoints = GetComponentsInChildren<CrossPoint>();
            int l = crossPoints.Length;

            allCrossPoints = new CrossPoint[l];

            int printForError = 0;

            foreach (CrossPoint crossPoint in crossPoints)
            {
                int i = CrossPoint.NameToIndex(crossPoint.name);

                if (printForError > 0)
                {
                    Debug.Log($"{printForError}ème prochain = {crossPoint.name}");
                    printForError--;
                }

                if (i == -1)
                {
                    printForError = 2;
                }

                if (!(allCrossPoints[i] is null))
                {
                    Debug.Log($"Deux cross points ont le même numéro ({i})");
                    printForError = 2;
                }
                    
                allCrossPoints[i] = crossPoint;
            }
        }

        private void SetCrossPoints(SousCrossManager[] scms)
        {
            List<CrossPoint> listCrossPoint = new List<CrossPoint>();

            foreach (SousCrossManager scm in scms)
            {
                foreach (CrossPoint point in scm.GetComponentsInChildren<CrossPoint>())
                {
                    // le rajouter à la liste
                    listCrossPoint.Add(point);
                    
                    // lui enlever ses voisins en trop
                    point.RemoveNotGoodNeighboor();
                }
            }

            _crossPoints = ManList<CrossPoint>.Copy(listCrossPoint);

            if (MustPrintGraph)
            {
                foreach (CrossPoint cp in _crossPoints)
                {
                    cp.PrintNeightboors();
                }
            }
        }
        
        public void ActiveSousCrossManager(SousCrossManager[] scms)
        {
            // desactiver tous les sous cross manager
            foreach (SousCrossManager e in GetComponentsInChildren<SousCrossManager>())
            {
                e.gameObject.SetActive(false);
            }

            // activer ceux avec lesquels on veut jouer
            foreach (SousCrossManager e in scms)
            {
                e.gameObject.SetActive(true);
            }
                
            SetCrossPoints(scms);
        }
        
        // ------------ Graph ------------

        public void ResetPathFinding(string key)
        {
            foreach (CrossPoint point in _crossPoints)
            {
                point.ResetPathFinding(key);
            }
        }
        
        // ------------ Private methods ------------

        private void DebugWithFile()
        {
            string build = "Build";
            Aux("",build);
            
            string d = "dossierDebug";
            Aux(build,d);

            using (File.Create($"{build}/{d}/AAAAAAAAAAAAAAAAAA[{PhotonNetwork.LocalPlayer.NickName}]"))
            {}

            void Aux(string path, string nameDir)
            {
                string p = $"{path}/{nameDir}";
                
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }
        }

        // ------------ Parsing ------------

        private void LoadNeigboors(SousCrossManager sousCrossManager)
        {
            int l = allCrossPoints.Length;
            string fileName = sousCrossManager.name;

            string path = "";
            if (Directory.Exists("Build"))
            {
                path = "Build/";
            }

            path += DossierRangement + fileName;

            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string ligne;
                
                    while ((ligne = sr.ReadLine()) != null)
                    {
                        string[] infos = ligne.Split(',');

                        string nameCrossPoint = infos[0].Substring(0, infos[0].Length - 3);
                        int iCrossPoint = CrossPoint.NameToIndex(nameCrossPoint);

                        if (iCrossPoint >= l)
                        {
                            InterfaceInGameManager.Instance.Print
                            ($"WARNING 1 : Le fichier de sauvegarde '{fileName}' des crossPoints n'est pas compatible --> faire une maintenance");
                        }

                        if (allCrossPoints[iCrossPoint].name != nameCrossPoint)
                        {
                            InterfaceInGameManager.Instance.Print
                                ($"WARNING 2 : Le fichier de sauvegarde '{fileName}' des crossPoints n'est pas compatible --> faire une maintenance");
                            
                        }

                        // set les neighboors du cross point
                        int nInfo = infos.Length;
                        for (int i = 1; i < nInfo; i++)
                        {
                            CrossPoint cp = allCrossPoints[int.Parse(infos[i])];

                            allCrossPoints[iCrossPoint].AddNeighboor(cp);
                        }
                    }
                }
            }
            else if (!IsMaintenance)
            {
                InterfaceInGameManager.Instance.Print
                ($"WARNING : Le fichier de sauvegarde '{sousCrossManager.name}' des crossPoints n'existe pas --> faire une maintenance");
            }
        }

        private void Print(string mes)
        {
            InterfaceInGameManager.Instance.Print(mes);
        }
    }
}