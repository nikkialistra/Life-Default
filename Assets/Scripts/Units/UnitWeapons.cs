using System;
using Units.Enums;
using Units.Equipment;
using UnityEngine;

namespace Units
{
    public class UnitWeapons : MonoBehaviour
    {
        [SerializeField] private WeaponSlot _meleeWeapon;
        [SerializeField] private WeaponSlot _rangedWeapon;
        
        public bool HasWeaponOf(WeaponSlotType weaponSlotType)
        {
            return weaponSlotType == WeaponSlotType.Melee ? _meleeWeapon.HasWeapon : _rangedWeapon.HasWeapon;
        }
        
        public Weapon ChooseWeapon(WeaponSlotType weaponSlotType)
        {
            return weaponSlotType == WeaponSlotType.Melee ? TakeMeleeWeapon() : TakeRangedWeapon();
        }

        private Weapon TakeMeleeWeapon()
        {
            if (!_meleeWeapon.HasWeapon)
            {
                throw new InvalidOperationException("Trying to choose melee weapon of empty melee weapon slot");
            }

            return _meleeWeapon.Weapon;
        }

        private Weapon TakeRangedWeapon()
        {
            if (!_rangedWeapon.HasWeapon)
            {
                throw new InvalidOperationException("Trying to choose ranged weapon of empty ranged weapon slot");
            }

            return _rangedWeapon.Weapon;
        }
    }
}
