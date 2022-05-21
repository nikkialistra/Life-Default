using Sirenix.OdinInspector;
using Units.Enums;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitEquipmentStats))]
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _maxRecoverySpeed = 3f;
        
        [Space]
        [Range(0f, 1f)]
        [SerializeField] private float _dodgeChance = 0.1f;

        [Title("Melee")]
        [SerializeField] private float _meleeDamageMultiplier = 1f;
        [SerializeField] private float _meleeAttackSpeed = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _meleeCriticalChance = 0.1f;
        [Space]
        [SerializeField] private float _meleeAttackDistance = 1.7f;
        [SerializeField] private float _meleeAttackRange = 2.2f;
        [Range(0f, 1f)]
        [SerializeField] private float _meleeAccuracy = 0.8f;
        
        [Title("Ranged")]
        [SerializeField] private float _rangedDamageMultiplier = 1f;
        [SerializeField] private float _rangedAttackSpeed = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _rangedCriticalChance = 0.1f;
        [Space]
        [SerializeField] private float _rangedAttackDistance = 1.7f;
        [SerializeField] private float _rangedAttackRange = 2.2f;
        [Range(0f, 1f)]
        [SerializeField] private float _rangedAccuracy = 0.8f;

        private UnitEquipmentStats _unitEquipmentStats;

        public WeaponType WeaponType => _unitEquipmentStats.WeaponType;
        
        public float MeleeCurrentDamage => _unitEquipmentStats.MeleeDamage * _meleeDamageMultiplier;
        public float RangedCurrentDamage => _unitEquipmentStats.RangedDamage * _rangedDamageMultiplier;

        public float Armor => _unitEquipmentStats.Armor;

        public float MaxHealth => _maxHealth;
        public float MaxRecoverySpeed => _maxRecoverySpeed;
        
        public float DodgeChance => _dodgeChance;

        public float MeleeDamageMultiplier => _meleeDamageMultiplier;
        public float MeleeAttackSpeed => _meleeAttackSpeed;
        public float MeleeCriticalChance => _meleeCriticalChance;
        public float MeleeAttackDistance => _meleeAttackDistance;
        public float MeleeAttackRange => _meleeAttackRange;
        public float MeleeAccuracy => _meleeAccuracy;

        public float RangedDamageMultiplier => _rangedDamageMultiplier;
        public float RangedAttackSpeed => _rangedAttackSpeed;
        public float RangedCriticalChance => _rangedCriticalChance;
        public float RangedAttackDistance => _rangedAttackDistance;
        public float RangedAttackRange => _rangedAttackRange;
        public float RangedAccuracy => _rangedAccuracy;

        private void Awake()
        {
            _unitEquipmentStats = GetComponent<UnitEquipmentStats>();
        }
    }
}
