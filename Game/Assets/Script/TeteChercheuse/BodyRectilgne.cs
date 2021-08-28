using System;
using System.Collections.Generic;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.TeteChercheuse
{
    public class BodyRectilgne : BodyChercheur
    {
        // ------------ Attributs ------------
        
        private float Vitesse = 4f;

        private Dictionary<GameObject, float> dictTimeCollision;
        
        // ------------ Constructeurs ------------
        public static void InstancierStatic(GameObject lanceur, GameObject destination)
        {
            BodyRectilgne original = MasterManager.Instance.GetOriginalBodyRectilgne(); // récupérer la préfab

            Vector3 posisionLanceur = lanceur.transform.position;
            float rotation = Calcul.Angle(0, posisionLanceur, destination.transform.position, Calcul.Coord.Y);
            BodyRectilgne body = Instantiate(original, posisionLanceur, lanceur.transform.rotation);
            
            body.Instancier(lanceur, destination, rotation);
        }
        
        // l'instancier de manière non-static (est appelé dans 'InstatncierStatic')
        private void Instancier(GameObject lanceur, GameObject destination, float rotation)
        {
            SetRbTr();
            
            Lanceur = lanceur;
            Destination = destination;

            Tr.Rotate(new Vector3(0, rotation, 0));

            /*// je fais ça pour qu'ils se décalent un peu
            MoveAmount = new Vector3(0, 0, 30*(3-SimpleMath.Mod((int)rotation/6, 2)));
            Tr.position += Tr.TransformDirection(MoveAmount) * Time.fixedDeltaTime;*/
            
            // instancier le dico
            dictTimeCollision = new Dictionary<GameObject, float>();
        }
        
        // ------------ Update ------------
        private void Update()
        {
            MoveAmount = new Vector3(0, 0, Vitesse);

            float dist = Calcul.Distance(Tr.position, Destination.transform.position, Calcul.Coord.Y);

            if (dist > 100) // s'il il est trop loin de sa destination (il s'est perdu) c'est fini
            {
                Debug.Log($"WARNING : la distance entre un body chercheur et sa destination était de {dist}");

                FinDeCourse(null);
                return;
            }

            if (Tr.position.y < -10) // s'il est tombé
            {
                Vector3 pos = Tr.position;
                Debug.Log($"WARNING : Un body Chercher est tombé aux coordonnées ({pos.x}, {pos.y}, {pos.z})");

                FinDeCourse(null);
                return;
            }
            
            // est-il bloqué
            foreach (KeyValuePair<GameObject, float> e in dictTimeCollision)
            {
                if (Time.time - e.Value > 2)
                {
                    // ça fait trop longtemps qu'il butte sur un obstacle
                    FinDeCourse(null);
                }
            }

            if (dist < EcartDistance) // est-il arrivé à destination
            {
                if (Calcul.Distance(Tr.position.y, Destination.transform.position.y) < ownCapsuleCollider.height / 2) // est ce qu'il est à la bonne altitude ?)
                {
                    FinDeCourse(Destination.GetComponent<CrossPoint>()); // c'est que c'est une destination valide
                }
                else // sur la bonne position en x et z mais pas sur y, ça veut dire qu'il dépasserait la destination s'il n'y avait pas ce cas
                {
                    FinDeCourse(null);
                }
            }

            // else il est trop loin de sa destination, donc il continue
        }

        private void FixedUpdate()
        {
            MoveEntity();
        }
        
        // ------------ Event ------------
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.GetComponent<Entity>())
            {
                ownCapsuleCollider.isTrigger = false;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            OnCollisionAux(other);
            
            // mémoriser le moment où le body a commencé à se faire bloquer
            if (!dictTimeCollision.ContainsKey(other.gameObject) && IsACollisionUpper(other, ownCapsuleCollider.radius * 0.1f))
            {
                dictTimeCollision.Add(other.gameObject, Time.time);
            }
        }
        
        private void OnCollisionStay(Collision other)
        {
            OnCollisionAux(other);
        }
        
        private void OnCollisionExit(Collision other)
        {
            // il n'est plus bloqué par 'other'
            if (dictTimeCollision.ContainsKey(other.gameObject))
            {
                dictTimeCollision.Remove(other.gameObject);
            }
        }

        private void OnCollisionAux(Collision other)
        {
            if (other.gameObject.GetComponent<Entity>()) // Si ça a touché une 'Entity', ça ne s'arrête pas
            {
                ownCapsuleCollider.isTrigger = true;
                return;
            }

            if (IsACollisionUpper(other, ownCapsuleCollider.radius * 1f)) 
            {
                // cela signifie qu'un objet (qui n'est pas une entity) l'a touché à une hauteur supérieur au rayon
                FinDeCourse(null);
            }
        }

        private bool IsACollisionUpper(Collision other, float dist)
        {
            ContactPoint[] listContact = other.contacts;
            int len = listContact.Length;
            
            int i;
            for (i = 0; i < len && Calcul.Distance(listContact[i].point.y, GetYSol()) < dist; i++)
            {}

            return i < len;
        }

        // ------------ Méthodes ------------
        private void FinDeCourse(CrossPoint reponse)
        {
            // rendre vraiment impossible d'avoir deux résultats d'un seul bodyRectiligne
            if (!gameObject || !enabled)
                return;
            
            enabled = false;
            Destroy(gameObject);
            Lanceur.GetComponent<CrossPoint>().EndResearchBody(reponse);
        }
        
        private float GetYSol()
        {
            float y = ownCapsuleCollider.transform.position.y;

            return y;
        }
    }
}