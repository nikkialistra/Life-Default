using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemType _itemType;
        [SerializeField] private Item _item;

        public ItemType ItemType => _itemType;
        public Item Item => _item;

        public bool HasItem => _item != null;
    }
}
