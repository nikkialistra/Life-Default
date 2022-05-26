using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    [Serializable]
    public class WeaponSlot
    {
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private Weapon _weapon;
        
        public WeaponType WeaponType => _weaponType;
        public Weapon Weapon => _weapon;

        public bool HasWeapon => _weapon != null;
    }
}
