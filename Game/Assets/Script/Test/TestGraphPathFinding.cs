using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.Graph;
using UnityEngine;

namespace Script.Test
{
    public class TestGraphPathFinding : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [SerializeField] private CrossPoint depart;
        [SerializeField] private CrossPoint destination;
        
        // ------------ Constructeur ------------

        private void Start()
        {
            TestRayGaz.CreateMarqueur(depart.transform.position, TestRayGaz.Couleur.Brown);
            TestRayGaz.CreateMarqueur(destination.transform.position, TestRayGaz.Couleur.Yellow);
            
            GraphPathFinding.GetPath(depart, destination, "sacha", Recep);
        }
        
        // ------------ Methods ------------

        private void Recep(List<Vector3> path)
        {
            /*for (int i = path.Count - 2; i > 0; i--)
            {
                TestRayGaz.CreateMarqueur(path[i], TestRayGaz.Couleur.Red);
            }*/
        }
    }
}