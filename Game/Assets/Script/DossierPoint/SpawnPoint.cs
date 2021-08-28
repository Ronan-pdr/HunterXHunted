using System;
using System.Linq;
using UnityEngine;

namespace Script.DossierPoint
{
    public class SpawnPoint : Point
    {
        // ------------ Attributs ------------
        public enum Type
        {
            Chasseur,
            Chassé,
            Bot
        }

        private Type _type;
        public Type Typo => _type;
        
        // ------------ Getters ------------

        public bool IsChasseurSpawn() => name.Contains("Chasseur");

        public bool IsChasséSpawn() => name.Contains("Chassé");

        public bool IsBotSpawn() => name.Contains("Bot");
        

        // ------------ Constructeur ------------
        private void Awake()
        {
            if (IsChasseurSpawn())
            {
                _type = Type.Chasseur;
            }
            else if (IsChasséSpawn())
            {
                _type = Type.Chassé;
            }
            else if (IsBotSpawn())
            {
                _type = Type.Bot;
            }
            else
            {
                Debug.Log($"Le nom '{name}' ne correspond à aucun spawnPoint");
            }
        }
    }
}