using UnityEngine;
using Script.EntityPlayer;

namespace Script.TeteChercheuse
{
    public abstract class TeteChercheuse : Entity
    {
        // Cette classe mère va regrouper tous les objets lancés depuis une entité
        // pour vérifier les collisions et, pottentiellement récupérer les objets touchés
        
        // ------------ Attributs ------------
        protected GameObject Lanceur;
    }
}