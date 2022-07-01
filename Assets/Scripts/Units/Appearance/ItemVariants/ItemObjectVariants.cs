using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants.Item;
using UnityEngine;

namespace Units.Appearance.ItemVariants
{
    [Serializable]
    public class ItemObjectVariants<T> : IItemVariants<T> where T : UnityEngine.Object
    {
        public IEnumerable<IItem<T>> Variants => _variants;

        [SerializeField] private List<ItemObject<T>> _variants;

        [Button]
        private void CalculateRelativeChances()
        {
            ((IItemVariants<T>)this).CalculateRelativeChancesForVariants();
        }
    }
}
