using Script.Animation;
using Script.EntityPlayer;
using UnityEngine;
using Script.TeteChercheuse;

namespace Script.DossierArme
{
    public class Gun : Arme
    {
        // ------------ Serialize Field ------------
        
        [Header("Porteur")]
        [SerializeField] protected Chasseur porteur;
        [SerializeField] protected Transform cam;
        
        // ------------ Méthode ------------
        public override void UtiliserArme()
        {
            Anim.Set(HumanAnim.Type.Shoot);

            float rotCam = cameraHolder.eulerAngles.x;
            float rotChasseur = porteur.transform.eulerAngles.y;

            Vector3 rotation = new Vector3(rotCam, rotChasseur, 0);
        
            BalleFusil.Tirer(cameraHolder.position, cam,
                porteur, rotation, armeInfo);
        }
    }
}

