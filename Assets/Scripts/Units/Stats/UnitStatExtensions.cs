using System;

namespace Units.Stats
{
    public static class UnitStatExtensions
    {
        public static string GetUxmlName(this UnitStat unitStat)
        {
            return unitStat switch {
                UnitStat.MovementSpeed => "movement-speed",
                
                UnitStat.MaxHealth => "max-health",
                UnitStat.MaxRecoverySpeed => "max-recovery-speed",
                
                UnitStat.EvadeChance => "evade-chance",
                
                UnitStat.MeleeDamageMultiplier => "melee-damage-multiplier",
                UnitStat.MeleeAttackSpeed => "melee-attack-speed",
                UnitStat.MeleeCriticalChance => "melee-critical-chance",
                UnitStat.MeleeAttackRange => "melee-attack-range",
                UnitStat.MeleeAccuracy => "melee-accuracy",
                
                UnitStat.RangedDamageMultiplier => "ranged-damage-multiplier",
                UnitStat.RangedAttackSpeed => "ranged-attack-speed",
                UnitStat.RangedCriticalChance => "ranged-critical-chance",
                UnitStat.RangedAttackRange => "ranged-attack-range",
                UnitStat.RangedAccuracy => "ranged-accuracy",
                
                _ => throw new ArgumentOutOfRangeException(nameof(unitStat), unitStat, null)
            };
        }
    }
}
