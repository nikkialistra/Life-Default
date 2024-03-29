﻿using System;
using System.Collections.Generic;
using ResourceManagement;
using Units.Equipment;
using Units.Equipment.Items;
using UnityEngine;

namespace Units
{
    public class UnitInventory : MonoBehaviour
    {
        private const int NumberOfSlots = 4;

        [SerializeField] private List<InventorySlot> _slots = new(NumberOfSlots);

        public bool HasToolFor(ResourceType resourceType)
        {
            foreach (var item in GetItems())
                if (item.TryGetTool(out var tool) && tool.CanExtract(resourceType))
                    return true;

            return false;
        }

        public Tool ChooseToolFor(ResourceType resourceType)
        {
            foreach (var item in GetItems())
                if (item.TryGetTool(out var tool) && tool.CanExtract(resourceType))
                    return tool;

            throw new InvalidOperationException(
                $"Cannot find suitable item, {nameof(HasToolFor)} should be called first");
        }

        public IItem GetItemAt(int index)
        {
            return HasItemAt(index) ? _slots[index].Item : null;
        }

        private IEnumerable<IItem> GetItems()
        {
            for (int i = 0; i < NumberOfSlots; i++)
                if (_slots[i].HasItem)
                    yield return _slots[i].Item;
        }

        private bool HasItemAt(int index)
        {
            ValidateIndex(index);

            return _slots[index].HasItem;
        }

        private static void ValidateIndex(int index)
        {
            if (index is < 0 or > NumberOfSlots - 1)
                throw new ArgumentException($"Cannot take item at invalid slot index {index}");
        }
    }
}
