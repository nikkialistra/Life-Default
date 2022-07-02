using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment.Items
{
    [Serializable]
    public class ConsumableItem : IItem
    {
        public ItemType ItemType => ItemType.Consumable;
        public object Content => _content;

        [SerializeField] private Consumable _content;
    }
}
