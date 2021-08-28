namespace Script.Manager
{
    public class InBar : ManagerGame
    {
        // ------------ Constructeur ------------
        
        public InBar(int nJoueur)
        {
            NJoueur = nJoueur;
            IsMultijoueur = true;
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
            return new NtypeBot();
        }
    }
}