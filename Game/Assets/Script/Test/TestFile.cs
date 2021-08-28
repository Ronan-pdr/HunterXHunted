using System;
using Script.Tools;
using UnityEngine;

namespace Script.Test
{
    public class TestFile : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("--------------------------------------------------------");
            Debug.Log("---------------------- TestMyFile ----------------------");
            Debug.Log("--------------------------------------------------------");
            
            Debug.Log("-------- Test1 --------");
            Test1();
            
            Debug.Log("-------- Test2 --------");
            Test2();
        }

        public void Test1()
        {
            MyFile<int> file = new MyFile<int>();
            
            Assert.IsTrue(file.IsEmpty());
            
            file.Enfiler(0);
            file.Enfiler(1);
            file.Enfiler(2);
            
            Assert.AreEqual(0, file.Defiler());
            Assert.AreEqual(1, file.Defiler());
            Assert.AreEqual(2, file.Defiler());
            
            Assert.IsTrue(file.IsEmpty());
        }

        public void Test2()
        {
            MyFile<int> file = new MyFile<int>();
            
            file.Enfiler(0);
            Assert.AreEqual(0, file.Defiler());
            Assert.IsTrue(file.IsEmpty());
            
            file.Enfiler(1);
            Assert.AreEqual(1, file.Defiler());
            Assert.IsTrue(file.IsEmpty());
            
            file.Enfiler(2);
            Assert.AreEqual(2, file.Defiler());
            Assert.IsTrue(file.IsEmpty());
        }
    }
}