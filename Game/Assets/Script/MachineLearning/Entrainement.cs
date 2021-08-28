using Script.Brain;
using Script.Manager;
using UnityEngine;

namespace Script.MachineLearning
{
    public abstract class Entrainement : MonoBehaviour
    {
        // ------------ SerializeField ------------

        [Header("Depart")]
        [SerializeField] protected Transform begin;
        
        // ------------ Attributs ------------

        protected MasterManager Master;
        protected Student Student;
        protected int Score;
        protected ClassementDarwin Classement;

        // ------------ Getter ------------

        public Student Bot => Student;
        
        // ------------ Setter ------------

        public void SetClassement(ClassementDarwin value)
        {
            Classement = value;
        }
        
        // ------------ Constructeur ------------

        private void Start()
        {
            Master = MasterManager.Instance;
            
            Student = Instantiate(GetPrefab(), Vector3.zero, begin.rotation);

            Student.SetEntrainement(this);
            StartEntrainement();
        }
        
        // ------------ Public Methods ------------

        public void EndTest()
        {
            GetScore();
            
            // donner le cerveau au classement avec son score
            // et récupérer un nouveau cerveau
            Classement.EndEpreuve(Student.Brain, Score);

            // recommencer
            StartEntrainement();
        }
        
        // ------------ Abstract Methods ------------

        public abstract void Bonus();
        
        public abstract void Malus();
        
        public abstract string GetNameDirectory();

        protected abstract Student GetPrefab();
        
        protected abstract void StartTraining();
        
        protected abstract void GetScore();

        // ------------ Private Methods ------------
        
        private void StartEntrainement()
        {
            // téléportation
            Student.transform.position = begin.position;
            
            // reset score et indicateurs
            Score = 0;
            StartTraining();
            
            // set le bot
            Student.SetToTest();
        }
    }
}