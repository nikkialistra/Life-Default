using Units.Enums;
using UnityEngine;

namespace Units
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Fraction _fraction;
        
        private bool _died;

        public bool Alive => !_died;
        
        public Fraction Fraction => _fraction;

        public void TakeDamage(float damage)
        {
            
        }
    }
}
