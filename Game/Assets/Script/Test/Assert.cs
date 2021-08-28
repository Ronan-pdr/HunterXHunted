using UnityEngine;

namespace Script.Test
{
    public class Assert
    {
        public static void AreEqual<T>(T expected, T res)
        {
            if (!expected.Equals(res))
            {
                Debug.Log("-> AreEqual");
                Debug.Log($"expected = {expected} ; res = {res}");
            }
                
        }

        public static void IsTrue(bool res)
        {
            if (!res)
            {
                Debug.Log("-> IsTrue");
                Debug.Log("res is false");
            }
        }
    }
}