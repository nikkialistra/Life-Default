using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    public class Weapon : ScriptableObject
    {
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private GameObject _weapon;

        public WeaponType WeaponType => _weaponType;
        public GameObject WeaponGameObject => _weapon;
    }
}
