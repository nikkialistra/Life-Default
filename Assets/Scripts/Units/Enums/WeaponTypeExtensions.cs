using System;

namespace Units.Enums
{
    public static class WeaponTypeExtensions
    {
        public static bool IsMelee(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.MeleeOneHand => true,
                WeaponType.MeleeTwoHand => true,
                WeaponType.RangedBow => false,
                WeaponType.RangedGun => false,
                
                _ => throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null)
            };
        }
        
        public static bool IsRanged(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.MeleeOneHand => false,
                WeaponType.MeleeTwoHand => false,
                WeaponType.RangedBow => true,
                WeaponType.RangedGun => true,
                
                _ => throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null)
            };
        }
    }
}
