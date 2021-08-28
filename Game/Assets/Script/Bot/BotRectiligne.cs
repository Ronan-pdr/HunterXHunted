using UnityEngine;
using Random = UnityEngine.Random;
using Script.DossierPoint;

namespace Script.Bot
{
    public class BotRectiligne : BotClass
    {
        // ------------ Etat ------------
        private enum Etat
        {
            EnChemin,
            Attend // il attend seulement lorsqu'il est sur un point qui possède 0 voisin
        }
        
        private Etat _etat = Etat.Attend;

        private CrossPoint _pointDestination;
        
        // pour quand il est bloqué
        private CrossPoint _previousPoint;

        // ------------ Setter ------------
        public void SetCrossPoint(CrossPoint value)
        {
            _pointDestination = value;
            FindNewDestination();
        }

        // ------------ Constructeurs ------------
        protected override void AwakeBot()
        {
            RotationSpeed = 600;
        }

        protected override void StartBot()
        {}

        // ------------ Update ------------
        protected override void UpdateBot()
        {
            if (_etat == Etat.Attend) // s'il est en train d'attendre,...
                return; // ...il ne fait rien

            GestionRotation(_pointDestination.transform.position);

            if (_etat == Etat.EnChemin)
            {
                if (IsArrivé(_pointDestination.transform.position, 0.3f)) // arrivé
                {
                    FindNewDestination();
                    //AnimationStop();
                }
            }
        }

        // ------------ Méthodes ------------
        private void FindNewDestination()
        {
            int nNeighboor = _pointDestination.GetNbNeighboor();

            if (nNeighboor > 0)
            {
                // sauvegarde de sa précédente destination
                _previousPoint = _pointDestination;
                
                // il repart
                _pointDestination = _pointDestination.GetNeighboor(Random.Range(0, nNeighboor));
                CalculeRotation(_pointDestination.transform.position);
                _etat = Etat.EnChemin;
                running = Running.Marche;
            }
            else
            {
                _etat = Etat.Attend;
                running = Running.Arret;
            }
        }
        
        // ------------ Event ------------
        
        // Bloqué
        protected override void WhenBlock()
        {
            _pointDestination = _previousPoint;
        }
    }
}