using System;
using UnityEngine;
using static System.Math;

namespace Script.Tools
{
    public class SimpleMath
    {
        public static int Mod(int a, int b)
        {
            int r = a % b;
            if (r < 0)
                return b + r;
            return r;
        }

        public static Vector3 Mod(Vector3 v, float f)
        {
            return new Vector3(v.x % f, v.y % f, v.z % f);
        }
    
        public static float Abs(float a)
        {
            if (a < 0)
                return -a;
            return a;
        }
    
        public static float Pow(float x, int n)
        {
            float res = 1;
            for (int i = 0; i < n; i++)
            {
                res *= x;
            }
    
            return res;
        }
    
        public static float Sqrt(float a)
        {
            return (float)Math.Sqrt(a);
        }
    
        public static float Cos(float angle)
        {
            float rad = DegreToRadian(angle);
            return (float)Math.Cos(rad);
        }
        
        public static float Sin(float angle)
        {
            float rad = DegreToRadian(angle);
            return (float)Math.Sin(rad);
        }
    
        public static float ArcTan(float opposé, float adjacent) // l'angle obtenu sera toujours positif
        {
            if (adjacent == 0)
            {
                return opposé >= 0 ? 90 : -90;
            }
                
                
            float r = (float) Atan(opposé / adjacent); //r est l'angle en radian
            return RadianToDegre(r);
        }
    
        public static float RadianToDegre(float angle)
        {
            return (float) (angle * 360 / (2 * PI));
        }

        public static float DegreToRadian(float angle)
        {
            return (float) (angle * 2 * PI / 360);
        }
        
        // isEncadré
        public static bool IsEncadré(Vector3 v, Vector3 e, float ecart)
        {
            return e.x - ecart < v.x && v.x < e.x + ecart &&
                   e.y - ecart < v.y && v.y < e.y + ecart &&
                   e.z - ecart < v.z && v.z < e.z + ecart;
        }

        public static bool IsEncadré(Vector3 v, Vector3 e)
        {
            return IsEncadré(v, e, 0.5f);
        }
        
        public static bool IsEncadré(float a, float b)
        {
            return IsEncadré(a, b, 0.5f);
        }
        
        public static bool IsEncadré(float a, float b, float ecart)
        {
            return b - ecart < a && a < b + ecart;
        }
    }    
}