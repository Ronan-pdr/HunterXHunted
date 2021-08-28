using System;
using System.Runtime.Serialization;

namespace Script.Tools
{
    public static class ManString
    {
        // ex : Format("t", 3) -> "  t"
        public static string Format(string s, int taille)
        {
            for (int i = taille - s.Length; i > 0; i--)
            {
                s = " " + s;
            }

            return s;
        }

        public static string Cut(string s, int begin, int end)
        {
            if (begin > end || begin < -1 || end > s.Length)
            {
                throw new Exception();
            }

            string res = "";

            for (int i = begin; i < end; i++)
            {
                res += s[i];
            }

            return res;
        }
    }
}