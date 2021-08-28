using UnityEngine;


namespace Script.DossierArme
{
    [CreateAssetMenu(menuName = "FPS/New Arme Info")]
    public class ArmeInfo : ScriptableObject
    {
        [SerializeField] private string armeName;
        [SerializeField] private int damage;
        [SerializeField] private float portéeAttaque;
        [SerializeField] private float périodeAttaque; // le nombre de balle possiblement tiré 
    
        //Getter
        public string GetName() => armeName;
        public int GetDamage() => damage;
        public float GetPortéeAttaque() => portéeAttaque;
        public float GetPériodeAttaque() => périodeAttaque;
    }
}