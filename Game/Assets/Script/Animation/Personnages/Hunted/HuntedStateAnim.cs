using System.Collections.Generic;
using UnityEngine;

namespace Script.Animation.Personnages.Hunted
{
    public class HuntedStateAnim : HumanAnim
    {
        // ------------ Setter ------------

        protected override void AddAnim()
        {
            // one touch
            Dict.Add(Type.Sit, Animator.StringToHash("isSitting"));
        }
    }
}
