namespace Script.Manager
{
    public class InEntrainementSaut : ManagerGame
    {
        // ------------ Constructeur ------------
        
        public InEntrainementSaut(int nJoueur)
        {
            NJoueur = nJoueur;
            IsMultijoueur = false;
        }
        
        // ------------ Méthodes ------------
        protected override NtypeJoueur GetNJoueur()
        {
            return new NtypeJoueur();
        }
        
        protected override NtypeBot GetNBot()
        {
            return new NtypeBot();
        }
    }
}