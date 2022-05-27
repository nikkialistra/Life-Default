using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment.Items
{
    [Serializable]
    public class ConsumableItem : IItem
    {
        [SerializeField] private Consumable _content;

        public ItemType ItemType => ItemType.Consumable;
        public object Content => _content;
    }
}
