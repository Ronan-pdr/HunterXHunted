using System;
using Script.Animation;
using UnityEngine;


namespace Script.EntityPlayer
{
    public class DesignHumanoide
    {
        // ------------ Attributs ------------

        private HumanAnim _anim;
        private GameObject[] _designs;
        private int _indexDesign;
        private Humanoide _porteur;
        
        // ------------ Getter ------------

        public int Index => _indexDesign;

        public int Length => _designs.Length;

        // ------------ Setter ------------

        public void Set(int index)
        {
            if (index >= _designs.Length)
            {
                throw new Exception($"Il n'y a que {_designs.Length} designs ; index = {index}");
            }

            _designs[_indexDesign].SetActive(false);
            _indexDesign = index;
            _designs[_indexDesign].SetActive(true);

            SetAnim();
        }

        // ------------ Constructeur ------------

        public DesignHumanoide(HumanAnim anim, GameObject[] designs, Humanoide porteur)
        {
            _anim = anim;
            _designs = designs;
            _indexDesign = 0;
            _porteur = porteur;
            
            for (int i = designs.Length - 1; i >= 0; i--)
            {
                designs[i].SetActive(i == _indexDesign);
            }
            
            SetAnim();
        }
        
        // ------------ Private Methode ------------
        
        private void SetAnim()
        {
            _anim.Constructeur(_designs[_indexDesign].GetComponent<Animator>(), _porteur);
        }
    }
}