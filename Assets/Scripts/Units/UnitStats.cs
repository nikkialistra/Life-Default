using UnityEngine;

namespace Units
{
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private float _meleeDamage = 20f;
        [SerializeField] private float _meleeAttackSpeed = 20f;
        [SerializeField] private float _meleeAttackRange = 6f;

        public float MeleeDamagePerSecond => _meleeDamage * _meleeAttackSpeed;
        public float MeleeAttackRange => _meleeAttackRange;
    }
}
