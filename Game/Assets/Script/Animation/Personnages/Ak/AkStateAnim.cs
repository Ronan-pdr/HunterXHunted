using UnityEngine;

namespace Script.Animation.Personnages.Ak
{
    public class AkStateAnim : HumanAnim
    {
        // ------------ Setter ------------

        protected override void AddAnim()
        {
            // arme
            Dict.Add(Type.Aiming, Animator.StringToHash("isAiming"));
            Dict.Add(Type.Shoot, Animator.StringToHash("isShooting"));
        }
    }
}
