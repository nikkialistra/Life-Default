using Units.Enums;
using UnityEngine;

namespace Units.Stats
{
    public class UnitEquipmentStats : MonoBehaviour
    {
        public WeaponType WeaponType { get; set; }

        public float MeleeDamage { get; set; } = 20;
        public float RangedDamage { get; set; } = 10;

        public float Armor => HeadArmor + BodyArmor;

        public float HeadArmor { get; set; } = 0.1f;
        public float BodyArmor { get; set; } = 0.2f;
    }
}
