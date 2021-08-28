using UnityEngine;

namespace Script.EntityPlayer
{
    public class HumanCapsule
    {
        // ------------ Attributs ------------
        
        private float _hauteur;
        private float _rayon;
        
        // ------------ Getter ------------

        public float Height => _hauteur;
        public float Rayon => _rayon;
        
        // ------------ Constructeur ------------
        public HumanCapsule(CapsuleCollider cap)
        {
            float scale = cap.transform.localScale.y;
            _hauteur = cap.height * scale;
            _rayon = cap.radius * scale;
        }
        
        // ------------ Méthode ------------

        public bool CanIPass(Vector3 position, Vector3 direction, float maxDistance)
        {
            // haut du corps (vers les yeux)
            Vector3 hautDuCorps = position + Vector3.up * (_hauteur - _rayon);
            
            // bas du corps (vers le haut des pieds)
            Vector3 basDuCorps = position + Vector3.up * _rayon;

            if (Physics.CapsuleCast(hautDuCorps, basDuCorps, _rayon, direction, out RaycastHit hit, maxDistance))
            {
                return hit.collider.GetComponent<Entity>();
            }

            return true;
        }
    }
}