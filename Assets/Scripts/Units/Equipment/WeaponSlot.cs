using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    public class WeaponSlot : MonoBehaviour
    {
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private Weapon _weapon;
        
        public WeaponType WeaponType => _weaponType;
        public Weapon Weapon => _weapon;

        public bool NotEmpty => _weapon != null;
    }
}
