using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment.Items
{
    [Serializable]
    public class WeaponItem : IItem
    {
        public ItemType ItemType => ItemType.Weapon;
        public object Content => _content;

        [SerializeField] private Weapon _content;
    }
}
