using System.Collections.Generic;
using UnityEngine;

namespace Units.Calculations
{
    public class PowerCalculation
    {
        public static int CalculateForOne(UnitStats unitStats)
        {
            var averageDPS = unitStats.MeleeCurrentDamage * (1 + unitStats.MeleeCriticalChance) * unitStats.MeleeAttackSpeed;
            
            var result = unitStats.MaxHealth * averageDPS;

            return (int)Mathf.Round(result);
        }

        public static int CalculateForMultiple(List<UnitStats> unitStats)
        {
            return 1;
        }
    }
}
