using Script.DossierPoint;

namespace Script.Manager
{
    public class InCageOiseaux : ManagerGame
    {
        // ------------ Constructeur ------------
        
        public InCageOiseaux(int nJoueur)
        {
            NJoueur = nJoueur;
            IsMultijoueur = false;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            NtypeJoueur n = new NtypeJoueur();
            n.None = NJoueur;

            return n;
        }
        
        protected override NtypeBot GetNBot()
        {
            NtypeBot n = new NtypeBot();
            n.Hirondelle = SpawnManager.Instance.GetNbSpawnBot();
            
            return n;
        }
    }
}