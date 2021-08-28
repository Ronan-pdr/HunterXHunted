using System.Collections.Generic;
using UnityEngine;

namespace Script.Animation.Personnages.Clef
{
    public class ClefStateAnim : HumanAnim
    {
        // ------------ Setter ------------

        protected override void AddAnim()
        {
            // arme
            Dict.Add(Type.Hit, Animator.StringToHash("Hit"));
        }
    }
}
