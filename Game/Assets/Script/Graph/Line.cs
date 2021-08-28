using System;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.Graph
{
    public class Line: Entity
    {
        // ------------ SerializeField ------------
        
        //[SerializeField] private Transform graphics;
        [SerializeField] private Renderer render;
        
        // ------------ Setter ------------

        public void SetColor(float couleur)
        {
            float val = 255;
            float[] rgb = new float[3];

            /*int i;
            for (i = 0; i < 3 && couleur > val; i++)
            {
                couleur -= val;
            }*/

            int i = (int)couleur / (int)val;
            if (i < 3)
            {
                rgb[i] = couleur % val / val;
            }

            render.material.color = new Color(rgb[0], rgb[1], rgb[2]);
        }
        
        // ------------ Static Constructeur ------------

        public static Line Create(Vector3 point1, Vector3 point2, float couleur = 0)
        {
            Line line = Instantiate(MasterManager.Instance.GetOriginalLine(), point1, Quaternion.identity);
            
            line.Constructeur(point1, point2, couleur);

            return line;
        }
        
        // ------------ Private Constructeur ------------

        private void Constructeur(Vector3 point1, Vector3 point2, float couleur)
        {
            SetRbTr();

            SetNode(point1, point2);
            
            // ranger dans la hiérarchie
            transform.parent = master.GetDossierGraph();
            
            // materiel
            SetColor(couleur);
        }
        
        // ------------ Public Method ------------

        public void SetNode(Vector3 point1, Vector3 point2)
        {
            // position
            Tr.position = point1;
            
            // rotation
            Vector3 diff = Calcul.Diff(point2, point1);

            float rotX = -Calcul.BetterArctan(diff.y, Calcul.Distance(point1, point2, Calcul.Coord.Y));
            float rotY = Calcul.Angle(0, point1, point2, Calcul.Coord.Y);
            
            Tr.rotation = Quaternion.identity;
            Tr.Rotate(rotX, rotY, 0);

            float sizeZ = Calcul.Distance(point1, point2);
            
            Tr.localScale = new Vector3(1, 1, sizeZ);
            
            //Tr.position += Tr.TransformDirection(Vector3.forward) * sizeZ / 2;
        }
    }
}