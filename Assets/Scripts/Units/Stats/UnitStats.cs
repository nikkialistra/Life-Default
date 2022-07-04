using System;
using Sirenix.OdinInspector;
using Units.Enums;
using UnityEngine;

namespace Units.Stats
{
    [RequireComponent(typeof(UnitEquipmentStats))]
    public class UnitStats : MonoBehaviour
    {
        public WeaponType WeaponType => _unitEquipmentStats.WeaponType;

        public float MeleeCurrentDamage => _unitEquipmentStats.MeleeDamage * _meleeDamageMultiplier.Value;
        public float RangedCurrentDamage => _unitEquipmentStats.RangedDamage * _rangedDamageMultiplier.Value;

        public float Armor => _unitEquipmentStats.Armor;

        public Stat<UnitStat> MaxHealth => _maxHealth;
        public Stat<UnitStat> MaxRecoverySpeed => _maxRecoverySpeed;

        public Stat<UnitStat> MovementSpeed => _movementSpeed;

        public Stat<UnitStat> EvadeChance => _evadeChance;

        public Stat<UnitStat> MeleeDamageMultiplier => _meleeDamageMultiplier;
        public Stat<UnitStat> MeleeAttackSpeed => _meleeAttackSpeed;
        public Stat<UnitStat> MeleeCriticalChance => _meleeCriticalChance;
        public Stat<UnitStat> MeleeAttackRange => _meleeAttackRange;
        public Stat<UnitStat> MeleeAccuracy => _meleeAccuracy;

        public Stat<UnitStat> RangedDamageMultiplier => _rangedDamageMultiplier;
        public Stat<UnitStat> RangedAttackSpeed => _rangedAttackSpeed;
        public Stat<UnitStat> RangedCriticalChance => _rangedCriticalChance;
        public Stat<UnitStat> RangedAttackRange => _rangedAttackRange;
        public Stat<UnitStat> RangedAccuracy => _rangedAccuracy;

        [Title("General")]
        [SerializeField] private Stat<UnitStat> _movementSpeed;
        [Space]
        [SerializeField] private Stat<UnitStat> _maxHealth;
        [SerializeField] private Stat<UnitStat> _maxRecoverySpeed;
        [Space]
        [SerializeField] private Stat<UnitStat> _evadeChance;

        [Title("Melee")]
        [SerializeField] private Stat<UnitStat> _meleeDamageMultiplier;
        [SerializeField] private Stat<UnitStat> _meleeAttackSpeed;
        [SerializeField] private Stat<UnitStat> _meleeCriticalChance;
        [SerializeField] private Stat<UnitStat> _meleeAttackRange;
        [SerializeField] private Stat<UnitStat> _meleeAccuracy;

        [Title("Ranged")]
        [SerializeField] private Stat<UnitStat> _rangedDamageMultiplier;
        [SerializeField] private Stat<UnitStat> _rangedAttackSpeed;
        [SerializeField] private Stat<UnitStat> _rangedCriticalChance;
        [SerializeField] private Stat<UnitStat> _rangedAttackRange;
        [SerializeField] private Stat<UnitStat> _rangedAccuracy;

        private UnitEquipmentStats _unitEquipmentStats;

        private void Awake()
        {
            _unitEquipmentStats = GetComponent<UnitEquipmentStats>();

            InitializeStats();
        }

        public void AddStatModifier(StatModifier<UnitStat> statModifier)
        {
            var stat = ChooseStat(statModifier);

            stat.AddModifier(statModifier);
        }

        public void RemoveStatModifier(StatModifier<UnitStat> statModifier)
        {
            var stat = ChooseStat(statModifier);

            stat.RemoveModifier(statModifier);
        }

        private void InitializeStats()
        {
            _movementSpeed.Initialize();

            _maxHealth.Initialize();
            _maxRecoverySpeed.Initialize();

            _evadeChance.Initialize();

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

        private Stat<UnitStat> ChooseStat(StatModifier<UnitStat> statModifier)
        {
            var stat = statModifier.StatType switch
            {
                UnitStat.MovementSpeed => _movementSpeed,

                UnitStat.MaxHealth => _maxHealth,
                UnitStat.MaxRecoverySpeed => _maxRecoverySpeed,

                UnitStat.EvadeChance => _evadeChance,

                UnitStat.MeleeDamageMultiplier => _meleeDamageMultiplier,
                UnitStat.MeleeAttackSpeed => _meleeAttackSpeed,
                UnitStat.MeleeCriticalChance => _meleeCriticalChance,
                UnitStat.MeleeAttackRange => _meleeAttackRange,
                UnitStat.MeleeAccuracy => _meleeAccuracy,

                UnitStat.RangedDamageMultiplier => _rangedDamageMultiplier,
                UnitStat.RangedAttackSpeed => _rangedAttackSpeed,
                UnitStat.RangedCriticalChance => _rangedCriticalChance,
                UnitStat.RangedAttackRange => _rangedAttackRange,
                UnitStat.RangedAccuracy => _rangedAccuracy,

                _ => throw new ArgumentOutOfRangeException()
            };
            return stat;
        }
    }
}
