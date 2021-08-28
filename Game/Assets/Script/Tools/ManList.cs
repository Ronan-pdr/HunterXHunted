using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Script.Tools
{
    public static class ManList
    {
        // constructeurs
        
        // ex : CreateListRange(4) -> [0, 1, 2, 3]
        private static List<int> CreateListRange(int iFin)
        {
            return new List<int>(CreateListRange(0, iFin));
        }

        private static int[] CreateListRange(int iDebut, int iFin)
        {
            if (iDebut > iFin)
            {
                throw new Exception($"iDebut = {iDebut} et iFin = {iFin}");
            }
            
            int[] res = new int[iFin - iDebut];
            
            for (int i = iDebut; i < iFin; i++)
            {
                res[i - iDebut] = i;
            }

            return res;
        }

        public static int[] CreateArrRange(int l)
        {
            return ManList<int>.Copy(CreateListRange(l));
        }

        // manipulation

        public enum Coord
        {
            X,
            Y,
            Z
        }

        private static float GetCoord(Vector3 v, Coord coord)
        {
            if (coord == Coord.X)
                return v.x;
            if (coord == Coord.Y)
                return v.y;
            return v.z;
        }

        public static float GetMax(Vector3[] positions, Coord coord)
        {
            int l = positions.Length;
            if (l == 0)
            {
                throw new Exception("Pas de max dans une array vide");
            }
            
            float max = GetCoord(positions[0], coord);
            for (int i = 1; i < l; i++)
            {
                if (GetCoord(positions[i], coord) > max)
                {
                    max = GetCoord(positions[i], coord);
                }
            }

            return max;
        }
        
        public static float GetMin(Vector3[] positions, Coord coord)
        {
            int l = positions.Length;
            if (l == 0)
            {
                throw new Exception("Pas de min dans une array vide");
            }
            
            float min = GetCoord(positions[0], coord);
            for (int i = 1; i < l; i++)
            {
                if (GetCoord(positions[i], coord) < min)
                {
                    min = GetCoord(positions[i], coord);
                }
            }

            return min;
        }
        
        public static int[] RandomIndex(int length)
        {
            int[] arr = new int[length];
            List<int> list = CreateListRange(length);

            for (int iArr = 0; iArr < length; iArr++)
            {
                int iList = new Random().Next(list.Count);
                arr[iArr] = list[iList];
                list.RemoveAt(iList);
            }

            return arr;
        }
        
        // Exemple :
        // - (7, 3) --> [3, 2, 2]
        // - (8, 5) --> [2, 2, 2, 1, 1]
        public static int[] SplitResponsabilité(int n, int l)
        {
            int[] res = new int[l];
            int mod = n % l;

            for (int i = 0; i < l; i++)
            {
                res[i] = n / l + (mod > i ? 1 : 0);
            }

            if (n != Sum(res))
                throw new Exception($"({n}, {l}) donne {ManList<int>.ToString(res)}");

            return res;
        }

        private static int Sum(int[] arr)
        {
            int sum = 0;
            foreach (int v in arr)
            {
                sum += v;
            }

            return sum;
        }
    }
    
    public static class ManList<T>
    {
        // contructeur
        public static T[] Copy(List<T> list)
        {
            int l = list.Count;
            T[] arr = new T[l];

            for (int i = 0; i < l; i++)
            {
                arr[i] = list[i];
            }

            return arr;
        }

        public static string ToString(T[] arr)
        {
            return ToString(new List<T>(arr));
        }

        public static string ToString(List<T> list)
        {
            int l = list.Count;
            if (l == 0)
            {
                return "[]";
            }
            
            string res = "[" + list[0];

            for (int i = 1; i < l; i++)
            {
                res += $", {list[i]}";  
            }

            return res + "]";
        }

        public static T[] Shuffle(T[] arr)
        {
            int l = arr.Length;
            int[] randomIndex = ManList.RandomIndex(l);
            T[] res = new T[l];

            for (int i = 0; i < l; i++)
            {
                res[i] = arr[randomIndex[i]];
            }

            return res;
        }
        
        // recherche
        public static int GetIndex(T[] arr, T e)
        {
            int l = arr.Length;
            for (int i = 0; i < l; i++)
            {
                if (arr[i].Equals(e))
                {
                    return i;
                }
            }

            throw new Exception($"{e} n'est pas contenu dans {arr}");
        }
    }
}