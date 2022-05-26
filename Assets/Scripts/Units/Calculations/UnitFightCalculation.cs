using Units.Enums;
using Units.Stats;
using UnityEngine;

namespace Units.Calculations
{
    [RequireComponent(typeof(UnitStats))]
    public class UnitFightCalculation : MonoBehaviour
    {
        private UnitStats _unitStats;

        private void Awake()
        {
            _unitStats = GetComponent<UnitStats>();
        }

        public float CalculateDamage()
        {
            var weaponType = _unitStats.WeaponType;
            
            var criticalMultiplier = CalculateCriticalMultiplier(weaponType);
            var damage = CalculateNormalDamage(weaponType) * criticalMultiplier;

            return damage;
        }

        public float CalculateHitDamage(float damage)
        {
            return damage * (1 - _unitStats.Armor);
        }

        public bool Missed()
        {
            return Random.Range(0f, 1f) > _unitStats.MeleeAccuracy.Value;
        }

        public bool Dodged()
        {
            return Random.Range(0f, 1f) <= _unitStats.EvadeChance.Value;
        }

        private float CalculateCriticalMultiplier(WeaponType weaponType)
        {
            var criticalChance = weaponType.IsMelee()
                ? _unitStats.MeleeCriticalChance.Value
                : _unitStats.RangedCriticalChance.Value;

            return Random.Range(0f, 1f) <= criticalChance ? 2f : 1f;
        }

        private float CalculateNormalDamage(WeaponType weaponType)
        {
            if (weaponType.IsMelee())
            {
                return _unitStats.MeleeCurrentDamage * _unitStats.MeleeDamageMultiplier.Value;
            }
            else
            {
                return _unitStats.RangedCurrentDamage * _unitStats.RangedDamageMultiplier.Value;
            }
        }
    }
}
