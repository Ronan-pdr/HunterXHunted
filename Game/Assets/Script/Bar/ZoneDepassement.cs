using System;
using Script.EntityPlayer;
using UnityEngine;

namespace Script.Bar
{
    public class ZoneDepassement : MonoBehaviour
    {
        // ------------ Event ------------

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerClass>())
            {
                BarManager.Instance.Tp(other.transform);
            }
        }
    }
}