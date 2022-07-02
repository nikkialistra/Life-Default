using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
    public class Weapon : ScriptableObject
    {
        public WeaponType WeaponType => _weaponType;
        public GameObject WeaponGameObject => _weapon;

        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private GameObject _weapon;
    }
}
