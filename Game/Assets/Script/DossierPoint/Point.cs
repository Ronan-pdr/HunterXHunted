using Script.EntityPlayer;
using Script.Manager;
using UnityEngine;

namespace Script.DossierPoint
{
    public class Point : MonoBehaviour
    {
        // ------------ Serialized Field ------------
        
        [Header("Graphics")]
        [SerializeField] private GameObject[] graphics;
    
        // ------------ Constructeur ------------
        private void Start()
        {
            if (!(this is CrossPoint))
            {
                Invisible();
            }
        }
        
        // ------------ Méthodes ------------
        protected void Invisible()
        {
            foreach (GameObject e in graphics)
            {
                e.SetActive(false);
            }
        }
    }
}

