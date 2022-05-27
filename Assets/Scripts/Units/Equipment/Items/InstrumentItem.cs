using System;
using Units.Enums;
using UnityEngine;

namespace Units.Equipment.Items
{
    [Serializable]
    public class InstrumentItem : IItem
    {
        [SerializeField] private Instrument _content;

        public ItemType ItemType => ItemType.Instrument;
        public object Content => _content;
    }
}
