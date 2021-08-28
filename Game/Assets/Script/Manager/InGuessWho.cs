using System;
using Script.EntityPlayer;
using Script.Zone;

namespace Script.Manager
{
    public class InGuessWho : ManagerGame
    {
        // ------------ Constructeur ------------
        public InGuessWho(int nJoueur)
        {
            NJoueur = nJoueur;
            IsMultijoueur = true;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            NtypeJoueur n = new NtypeJoueur();
            
            n.Chasseur = MasterManager.Instance.SettingsGame.NChasseur;
            n.Chassé = NJoueur - n.Chasseur;

            return n;
        }
        
        protected override NtypeBot GetNBot()
        {
            NtypeBot n = new NtypeBot();
            switch (NJoueur)
            {
                case 1:
                    n.Rectiligne = 10;
                    n.Fuyard = 2;
                    break;
                case 2:
                    n.Rectiligne = 16;
                    n.Fuyard = 3;
                    break;
                case 3:
                    n.Rectiligne = 26;
                    n.Fuyard = 3;
                    break;
                case 4:
                    n.Rectiligne = 34;
                    n.Fuyard = 4;
                    break;
                default:
                    n.Rectiligne = 43;
                    n.Fuyard = 5;
                    break;
            }

            ZoneManager.EnumZone zone = MasterManager.Instance.SettingsGame.Zone;

            if (zone != ZoneManager.EnumZone.All &&
                zone != ZoneManager.EnumZone.Inside)
            {
                n.Suiveur = 1;
            }

            //n.Rectiligne = 0;
            //n.Suiveur = 0;
            //n.Fuyard = 0;
            
            return n;
        }
    }
}