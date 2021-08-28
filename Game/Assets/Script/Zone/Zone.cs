using System.Collections.Generic;
using Script.DossierPoint;
using UnityEngine;

namespace Script.Zone
{
    public class Zone
    {
        // ------------ Attributs ------------

        private GameObject _spawn;
        private GameObject _deadZone;
        private SousCrossManager[] _scms;

        // ------------ Constructeur ------------

        public Zone(GameObject spawn, GameObject deadZone, SousCrossManager[] scms)
        {
            _spawn = spawn;
            _deadZone = deadZone;
            _scms = scms;
        }
        
        // ------------ Method(s) ------------

        public void SetActive(bool active, ref List<SousCrossManager> scmToActive)
        {
            // les spawner sont actifs en même temps que la zone
            _spawn.SetActive(active);
            
            // si on active cette zone, les dead zones ne le sont pas
            _deadZone.SetActive(!active);

            // les zones de spawn ne seront actifs que si la zone est active
            if (active)
            {
                foreach (SousCrossManager scm in _scms)
                {
                    scmToActive.Add(scm);
                }
            }
        }
    }
}