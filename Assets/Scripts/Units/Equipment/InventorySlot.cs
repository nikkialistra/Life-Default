using System;
using Units.Enums;
using Units.Equipment.Items;
using UnityEngine;

namespace Units.Equipment
{
    [Serializable]
    public class InventorySlot
    {
        public ItemType ItemType => _itemType;
        public IItem Item => _item;

        public bool HasItem => _item != null;

        [SerializeField] private ItemType _itemType;
        [SerializeReference] private IItem _item;
    }
}
