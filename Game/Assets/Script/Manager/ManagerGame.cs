using System;
using Script.Bot;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Tools;

namespace Script.Manager
{

    public abstract class ManagerGame
    {
        // ------------ Struct ------------
        protected struct NtypeJoueur
        {
            public int Chasseur;
            public int Chassé;
            public int Blocard;
            public int None;
            
            public (TypePlayer, int)[] GetList()
            {
                return new[]
                {
                    (TypePlayer.Chasseur, Chasseur),
                    (TypePlayer.Chassé, Chassé),
                    (TypePlayer.Blocard, Blocard),
                    (TypePlayer.None, None),
                };
            }

            public int Sum() => Chasseur + Chassé + Blocard + None;
        }
        
        protected struct NtypeBot
        {
            public int Rectiligne;
            public int Fuyard;
            public int Guide;
            public int Suiveur;
            public int Hirondelle;
            
            public (TypeBot, int)[] GetList()
            {
                return new[]
                {
                    (TypeBot.Rectiligne, Rectiligne),
                    (TypeBot.Fuyard, Fuyard),
                    (TypeBot.Guide, Guide),
                    (TypeBot.Suiveur, Suiveur),
                    (TypeBot.Hirondelle, Hirondelle)
                };
            }
            
            public int Sum() => Rectiligne + Fuyard + Guide + Suiveur + Hirondelle;
        }
        
        // ------------ Attribut ------------
        
        protected int NJoueur;
        public bool IsMultijoueur { get; protected set;}

        // ------------ Getters ------------
        protected abstract NtypeJoueur GetNJoueur();
        protected abstract NtypeBot GetNBot();

        // Donne une liste indiquant le type de chaque joueur en fonction 
        public TypePlayer[] GetTypePlayer()
        {
            // récupérer les taux en fonction du type de la partie
            NtypeJoueur n = GetNJoueur();

            CasEreur();

            TypePlayer[] listTrié = GetListTrié(n.GetList(), NJoueur);

            return listTrié;
            //return ManList<TypePlayer>.Shuffle(listTrié);

            void CasEreur()
            {
                if (NJoueur == 0)
                {
                    throw new Exception("Il ne peut y avoir 0 joueur");
                }
                
                // le total doît toujours être égal au nombre de joueur (logique hehe)
                if (n.Sum() != NJoueur)
                {
                    throw new Exception($"{n.Chasseur} + {n.Chassé} + {n.None} + {n.Blocard} != {NJoueur}");
                }
                
                // avec le nombre de spawn
                SpawnManager spawnManager = SpawnManager.Instance;
                if (n.Chasseur > spawnManager.GetNbSpawnChasseur())
                {
                    throw new Exception("Pas assez de spawn pour les chasseurs");
                }

                if (n.Chassé > spawnManager.GetNbSpawnChassé())
                {
                    throw new Exception("Pas assez de spawn pour les chassés");
                }
            }
        }

        public TypeBot[] GetTypeBot()
        {
            // récupérer les taux en fonction du type de la partie
            NtypeBot n = GetNBot();
            
            CasErreur();

            // attribution des types (pour l'instant c'est pas random)
            TypeBot[] listTrié = GetListTrié(n.GetList(), n.Sum());

            return listTrié;

            void CasErreur()
            {
                CrossManager cm = CrossManager.Instance;
                if (cm && n.Rectiligne > cm.GetNumberPoint())
                    throw new Exception($"Y'a seulement {CrossManager.Instance.GetNumberPoint()}" +
                                        $"crossPoint pour {n.Rectiligne} botRectiligne");

                if (!(SpawnManager.Instance is null) &&
                    n.Fuyard + n.Guide + n.Suiveur + n.Hirondelle > SpawnManager.Instance.GetNbSpawnBot())
                    throw new Exception("Pas assez de spawn bot");
            }
        }

        // Regarde les exemples si tu comprends pas
        private T[] GetListTrié<T>((T, int)[] list, int l)
        {
            T[] listTrié = new T[l];
            
            int i = 0;
            foreach ((T type, int nBot) in list)
            {
                for (int j = 0; j < nBot; i++, j++)
                {
                    listTrié[i] = type;
                }
            }

            return listTrié;
        }
    }
}