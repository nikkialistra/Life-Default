using Sirenix.OdinInspector;
using Units.Enums;
using UnityEngine;

namespace Units.Stats
{
    [RequireComponent(typeof(UnitEquipmentStats))]
    public class UnitStats : MonoBehaviour
    {
        [SerializeField] private float _attackRangeMultiplierToStartFight = 0.75f;
        
        [Title("General")]
        [SerializeField] private Stat _maxHealth;
        [SerializeField] private Stat _maxRecoverySpeed;
        [Space]
        [SerializeField] private Stat _dodgeChance;

        [Title("Melee")]
        [SerializeField] private Stat _meleeDamageMultiplier;
        [SerializeField] private Stat _meleeAttackSpeed;
        [SerializeField] private Stat _meleeCriticalChance;
        [SerializeField] private Stat _meleeAttackRange;
        [SerializeField] private Stat _meleeAccuracy;
        
        [Title("Ranged")]
        [SerializeField] private Stat _rangedDamageMultiplier;
        [SerializeField] private Stat _rangedAttackSpeed;
        [SerializeField] private Stat _rangedCriticalChance;
        [SerializeField] private Stat _rangedAttackRange;
        [SerializeField] private Stat _rangedAccuracy;

        private UnitEquipmentStats _unitEquipmentStats;

        public WeaponType WeaponType => _unitEquipmentStats.WeaponType;
        
        public float MeleeCurrentDamage => _unitEquipmentStats.MeleeDamage * _meleeDamageMultiplier.Value;
        public float RangedCurrentDamage => _unitEquipmentStats.RangedDamage * _rangedDamageMultiplier.Value;

        public float Armor => _unitEquipmentStats.Armor;

        public float MaxHealth => _maxHealth.Value;
        public float MaxRecoverySpeed => _maxRecoverySpeed.Value;
        
        public float DodgeChance => _dodgeChance.Value;

        public float MeleeDamageMultiplier => _meleeDamageMultiplier.Value;
        public float MeleeAttackSpeed => _meleeAttackSpeed.Value;
        public float MeleeCriticalChance => _meleeCriticalChance.Value;
        public float MeleeAttackRange => _meleeAttackRange.Value;
        public float MeleeAttackDistance => _meleeAttackRange.Value * _attackRangeMultiplierToStartFight;
        public float MeleeAccuracy => _meleeAccuracy.Value;

        public float RangedDamageMultiplier => _rangedDamageMultiplier.Value;
        public float RangedAttackSpeed => _rangedAttackSpeed.Value;
        public float RangedCriticalChance => _rangedCriticalChance.Value;
        public float RangedAttackRange => _rangedAttackRange.Value;
        public float RangedAttackDistance => _rangedAttackRange.Value * _attackRangeMultiplierToStartFight;
        public float RangedAccuracy => _rangedAccuracy.Value;

        private void Awake()
        {
            _unitEquipmentStats = GetComponent<UnitEquipmentStats>();

            InitializeStats();
        }

        private void InitializeStats()
        {
            _maxHealth.Initialize();
            _maxRecoverySpeed.Initialize();
            
            _dodgeChance.Initialize();

            _meleeDamageMultiplier.Initialize();
            _meleeAttackSpeed.Initialize();
            _meleeCriticalChance.Initialize();
            _meleeAttackRange.Initialize();
            _meleeAccuracy.Initialize();

            _rangedDamageMultiplier.Initialize();
            _rangedAttackSpeed.Initialize();
            _rangedCriticalChance.Initialize();
            _rangedAttackRange.Initialize();
            _rangedAccuracy.Initialize();
        }
    }
}
