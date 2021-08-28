using System;
using Script.EntityPlayer;
using Script.Graph;
using UnityEngine;

namespace Script.Test
{
    public class TestLine : Entity
    {
        // ------------ SerializeField ------------

        //[SerializeField] private Transform[] otherPoints;
        
        // ------------ Constructeur ------------
        private void Start()
        {
            SetRbTr();

            foreach (Transform point in GetComponentInChildren<Transform>())
            {
                Line.Create(Tr.position, point.position);
            }
        }
    }
}