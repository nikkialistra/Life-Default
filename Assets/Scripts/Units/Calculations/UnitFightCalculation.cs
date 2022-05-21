using Units.Enums;
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
            return Random.Range(0f, 1f) > _unitStats.MeleeAccuracy;
        }

        public bool Dodged()
        {
            return Random.Range(0f, 1f) <= _unitStats.DodgeChance;
        }

        private float CalculateCriticalMultiplier(WeaponType weaponType)
        {
            var criticalChance = weaponType == WeaponType.Melee
                ? _unitStats.MeleeCriticalChance
                : _unitStats.RangedCriticalChance;

            return Random.Range(0f, 1f) <= criticalChance ? 2f : 1f;
        }

        private float CalculateNormalDamage(WeaponType weaponType)
        {
            if (weaponType == WeaponType.Melee)
            {
                return _unitStats.MeleeCurrentDamage * _unitStats.MeleeDamageMultiplier;
            }
            else
            {
                return _unitStats.RangedCurrentDamage * _unitStats.RangedDamageMultiplier;
            }
        }
    }
}
