using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment.Items
{
    [Serializable]
    public class PropertyItem : IItem
    {
        public ItemType ItemType => ItemType.Property;
        public object Content => _content;

        [SerializeField] private Property _content;
    }
}
