using Script.DossierPoint;

namespace Script.Manager
{
    public class InLabyrinthe : ManagerGame
    {
        // ------------ Constructeur ------------
        public InLabyrinthe(int nJoueur)
        {
            NJoueur = nJoueur;
            IsMultijoueur = true;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            NtypeJoueur n = new NtypeJoueur();
            n.Blocard = NJoueur;

            return n;
        }
        
        protected override NtypeBot GetNBot()
        {
            NtypeBot n = new NtypeBot();
            n.Guide = SpawnManager.Instance.GetNbSpawnBot();
            // zero bot
            n.Guide = 0;
            
            return n;
        }
    }
}