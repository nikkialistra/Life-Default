using System;
using System.Collections.Generic;
using ResourceManagement;
using Units.Equipment;
using UnityEngine;

namespace Units
{
    public class UnitInventory : MonoBehaviour
    {
        [SerializeField] private List<InventorySlot> _slots = new(NumberOfSlots);

        private const int NumberOfSlots = 4;
        
        public bool HasInstrumentFor(ResourceType resourceType)
        {
            return true;
        }

        public Instrument ChooseInstrumentFor(ResourceType wood)
        {
            throw new NotImplementedException();
        }

        private bool HasItemAt(int index)
        {
            ValidateIndex(index);

            return _slots[index].HasItem;
        }

        public Item GetItemAt(int index)
        {
            return HasItemAt(index) ? _slots[index].Item : null;
        }

        private static void ValidateIndex(int index)
        {
            if (index is < 0 or > NumberOfSlots - 1)
            {
                throw new ArgumentException($"Cannot take item at invalid slot index {index}");
            }
        }
    }
}
