using System;
using System.Collections.Generic;
using Script.Bar;
using Script.Tools;
using UnityEngine;
using Launcher = Script.Menu.Launcher;

namespace Script.Test
{
    public class TestStaticFunc : MonoBehaviour
    {

        private void Start()
        {
            TestChangeName();
        }

        private void Test1()
        {
            int[] arr =
            {
                1, 2, 3, 4, 5, 6, 7
            };

            int[] rnd = ManList<int>.Shuffle(arr);
            Debug.Log(ManList<int>.ToString(rnd));
        }

        private void TestChangeName()
        {
            Aux("1", new []{"Sam1", "Sam"});
            Aux("2", new []{"Sam", "Sam1"});
            Aux("3", new []{"Sam"});
            Aux("4", new []{"Sam", "Sam3", "Sam2"});
            Aux("5", new []{"Sam2", "Sam4", "Sam"});

            void Aux(string expected, string[] arr)
            {
                Assert.AreEqual(expected, BarManager.ChangeName("Sam", arr));
            }
        }
    }
}