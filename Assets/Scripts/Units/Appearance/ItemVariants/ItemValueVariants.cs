using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants.Item;
using UnityEngine;

namespace Units.Appearance.ItemVariants
{
    [Serializable]
    public class ItemValueVariants<T> : IItemVariants<T>
    {
        [SerializeField] private List<ItemValue<T>> _variants;
        public IEnumerable<IItem<T>> Variants => _variants;
        
        [Button]
        private void CalculateRelativeChances()
        {
            ((IItemVariants<T>)this).CalculateRelativeChancesForVariants();
        }
    }
}
