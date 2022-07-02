using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    [Serializable]
    public class WeaponSlot
    {
        public WeaponType WeaponType => _weaponType;
        public Weapon Weapon => _weapon;

        public bool HasWeapon => _weapon != null;

        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private Weapon _weapon;
    }
}
