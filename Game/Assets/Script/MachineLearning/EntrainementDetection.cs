using Script.Tools;
using Script.Brain;

namespace Script.MachineLearning
{
    public class EntrainementDetection : Entrainement
    {
        // ------------ Attributs ------------

        private const int CoefScore = 10;
        
        // ------------ Getter ------------
        public override string GetNameDirectory() => BrainWall.NameDirectory;
        
        // ------------ Public Methods ------------

        protected override void GetScore()
        {
            // bonus
            float dist = Calcul.Distance(Student.transform.position, begin.position);
            Score += (int)(dist * CoefScore);
        }

        public override void Bonus()
        {
            throw new System.NotImplementedException();
        }

        public override void Malus()
        {
            throw new System.NotImplementedException();
        }

        // ------------ Protected Methods ------------

        protected override Student GetPrefab() => Master.GetOriginalDetecteur();

        protected override void StartTraining()
        {}
    }
}