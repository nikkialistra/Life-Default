using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment.Items
{
    [Serializable]
    public class WeaponItem : IItem
    {
        [SerializeField] private Weapon _content;

        public ItemType ItemType => ItemType.Weapon;
        public object Content => _content;
    }
}
