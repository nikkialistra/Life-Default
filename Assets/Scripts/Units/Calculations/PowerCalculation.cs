﻿using System;
using System.Collections.Generic;
using Units.Enums;
using Units.Stats;
using UnityEngine;

namespace Units.Calculations
{
    public class PowerCalculation : MonoBehaviour
    {
        [SerializeField] private float _groupDamageMultiplier = 0.7f;

        public int CalculateForOne(UnitStats unitStats, float multiplier = 1f)
        {
            var averageUptake = CalculateAverageUptake(unitStats);
            var averageDPS = CalculateAverageDps(unitStats);

            var power = averageUptake * (averageDPS * multiplier);

            return (int)Mathf.Round(power);
        }

        public int CalculateForMultiple(List<UnitStats> unitStatsList)
        {
            if (unitStatsList.Count <= 1)
                throw new InvalidOperationException(
                    $"Power calculation for multiple units have invalid length {unitStatsList.Count}");

            float sumPower = 0;

            foreach (var unitStats in unitStatsList)
                sumPower += CalculateForOne(unitStats, _groupDamageMultiplier);

            return (int)Mathf.Round(sumPower);
        }

        private static float CalculateAverageUptake(UnitStats unitStats)
        {
            var averageAbsorption = unitStats.Armor * (1 - unitStats.EvadeChance.Value);
            var averageUptake = unitStats.MaxHealth.Value / (1 - unitStats.EvadeChance.Value + averageAbsorption);
            return averageUptake;
        }

        public static float CalculateAverageDps(UnitStats unitStats)
        {
            return unitStats.WeaponType.IsMelee()
                ? CalculateMeleeAverageDps(unitStats)
                : CalculateRangedAverageDps(unitStats);
        }

        private static float CalculateMeleeAverageDps(UnitStats unitStats)
        {
            var averageDamage = unitStats.MeleeCurrentDamage * (1 + unitStats.MeleeCriticalChance.Value) *
                                unitStats.MeleeAccuracy.Value;
            var averageDps = averageDamage * unitStats.MeleeAttackSpeed.Value;
            return averageDps;
        }

        private static float CalculateRangedAverageDps(UnitStats unitStats)
        {
            var averageDamage = unitStats.RangedCurrentDamage * (1 + unitStats.RangedCriticalChance.Value) *
                                unitStats.RangedAccuracy.Value;
            var averageDps = averageDamage * unitStats.RangedAttackSpeed.Value;
            return averageDps;
        }
    }
}
