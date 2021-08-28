using System.Collections.Generic;
using Script.TeteChercheuse;
using UnityEngine;

namespace Script.Tools
{
    public class Calcul
    {
        public enum Coord
        {
            X,
            Y,
            Z,
            None
        }
        
        // distance entre deux points à deux coordonnées
        public static float Distance(Vector3 a, Vector3 b)
        {
            float x = SimpleMath.Abs(a.x - b.x);
            float y = SimpleMath.Abs(a.y - b.y);
            float z = SimpleMath.Abs(a.z - b.z);

            return Norme(x, y, z);
        }

        // fait la différence entre deux positions avec seulement deux coordonnées sans la "coordWithout",
        // si elle est égale à None, c'est avec les trois coordonnées
        public static float Distance(Vector3 a, Vector3 b, Coord coordWithout)
        {
            (float x, float y, float z) = (0, 0, 0);
            if (coordWithout != Coord.X)
                x = SimpleMath.Abs(a.x - b.x);
            if (coordWithout != Coord.Y)
                y = SimpleMath.Abs(a.y - b.y);
            if (coordWithout != Coord.Z)
                z = SimpleMath.Abs(a.z - b.z);
            
            return Norme(x, y, z);
        }

        // distance entre deux points à une coordonnée
        public static float Distance(float a, float b)
        {
            return SimpleMath.Abs(a - b);
        }
        
        // la distance en prenant en compte les obstacles
        // IMPORTANT : exclut y
        /*public static float GetRealDistance(Vector3 a, Vector3 b)
        {
            List<Vector3> path = RayGaz.GetPath(a, b);
            int len = path.Count;

            float res = 0;
            for (int i = 1; i < len; i++)
            {
                res += Distance(path[i - 1], path[i]);
            }

            return res;
        }*/

        public static float Norme(float x, float y, float z)
        {
            float a = SimpleMath.Pow(x, 2);
            float b = SimpleMath.Pow(y, 2);
            float c = SimpleMath.Pow(z, 2);

            return SimpleMath.Sqrt(a + b + c);
        }

        public static Vector3 Diff(Vector3 destination, Vector3 depart)
        {
            float diffX = destination.x - depart.x;
            float diffY = destination.y - depart.y;
            float diffZ = destination.z - depart.z;
            
            return new Vector3(diffX, diffY, diffZ);
        }

        public static Vector3 FromAngleToDirection(float angleY)
        {
            float x = SimpleMath.Sin(angleY);
            float z = SimpleMath.Cos(angleY);

            return new Vector3(x, 0, z);
        }

        public static float GiveAmoutRotation(float angle, float angleEntity)
        {
            // On doit ajouter sa rotation initiale à la rotation qu'il devait faire s'il était à 0 degré
            angle -= angleEntity;

            if (angle > 180) // Le degré est déjè valide, seulement, il est préférable de tourner de -150° que de 210° (par exemple)
                angle -= 360;
            else if (angle < -180)
                angle += 360;

            return angle;
        }

        public static Vector3 Direction(float degre)
        {
            Vector3 res = Vector3.zero;

            res.z = SimpleMath.Sin(degre);
            res.x = SimpleMath.Cos(degre);

            return res;
        }

        // Calcul l'angle le plus faible pour qu'un objet à la position 'depart'
        // puisse s'orienter face à 'destination'. A noter que ce sera un angle cohérent seulement sur 'coord'.
        // 'rotationInitiale' DOIT être en degré et correspond à la rotation sur 'coord' de l'objet aux
        // coordonnées 'départ'
        public static float Angle(float rotationInitiale, Vector3 depart, Vector3 destination, Coord coord)
        {
            Vector3 diff = Diff(destination, depart);

            float adjacent, opposé;
            if (coord == Coord.X)
            {
                opposé = diff.y;
                adjacent = diff.z;
            }
            else if (coord == Coord.Y)
            {
                opposé = diff.x;
                adjacent = diff.z;
            }
            else if (coord == Coord.Z)
            {
                opposé = diff.x;
                adjacent = diff.y;
            }
            else // pas defini
            {
                opposé = diff.x;
                adjacent = diff.y;
            }

            return BetterArctan(opposé, adjacent, rotationInitiale);
        }

        public static float BetterArctan(float opposé, float adjacent, float rotationInitiale = 0)
        {
            float amountRotation;
            if (opposé == 0)
            {
                if (adjacent > 0)
                    amountRotation = 0;
                else
                    amountRotation = 180;
            }
            else if (adjacent == 0) // on ne peut pas diviser par 0 donc je suis obligé de faire ce cas (dans le ArcTan)
            {
                if (opposé < 0)
                    amountRotation = -90;
                else
                    amountRotation = 90;
            }
            else
            {
                amountRotation = SimpleMath.ArcTan(opposé, adjacent); // amountRotation : Df = ]-90, 90[
            
                if (adjacent < 0) // Fait quatre schémas avec les différentes configurations pour comprendre
                {
                    if (opposé < 0)
                        amountRotation -= 180; // amountRotation était positif
                    else
                        amountRotation += 180; // amountRotation était négatif
                }
            }

            return GiveAmoutRotation(amountRotation, rotationInitiale);
        }
    }
}