using UnityEngine;

namespace Units
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private float _meleeDamage = 20f;
        [SerializeField] private float _meleeAttackSpeed = 1f;
        [Space]
        [SerializeField] private float _meleeAttackDistance = 1.7f;
        [SerializeField] private float _meleeAttackRange = 2.2f;
        [Range(0f, 1f)]
        [SerializeField] private float _meleeAccuracy = 0.8f;
        
        [Space]
        [Range(0f, 1f)]
        [SerializeField] private float _attackDodgeChance = 0.1f;

        public float MeleeDamagePerSecond => _meleeDamage * _meleeAttackSpeed;
        
        public float MeleeAttackSpeed => _meleeAttackSpeed;
        
        public float MeleeAttackRange => _meleeAttackRange;
        public float MeleeAttackDistance => _meleeAttackDistance;
        
        public float MeleeAccuracy => _meleeAccuracy;


        public float AttackDodgeChance => _attackDodgeChance;
    }
}
