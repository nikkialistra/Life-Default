using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment
{
    [Serializable]
    public class Item<T>
    {
        [SerializeField] private ItemType _itemType;
        [SerializeField] private T _content;

        public ItemType ItemType => _itemType;
        public T Content => _content;
    }
}
