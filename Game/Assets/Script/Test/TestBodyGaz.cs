using System;
using Script.EntityPlayer;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Test
{
    public class TestBodyGaz : Entity
    {
        [SerializeField] private GameObject destination;

        private void Start()
        {
            //BodyGaz.InstancierStatic(gameObject, destination);
        }
    }
}