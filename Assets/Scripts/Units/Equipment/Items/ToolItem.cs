using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment.Items
{
    [Serializable]
    public class ToolItem : IItem
    {
        [SerializeField] private Tool _content;

        public ItemType ItemType => ItemType.Tool;
        public object Content => _content;
    }
}
