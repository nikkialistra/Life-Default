using System;
using System.Collections.Generic;
using Humans.Appearance.ItemVariants.Item;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Humans.Appearance.ItemVariants
{
    [Serializable]
    public class ItemValueVariants<T> : IItemVariants<T>
    {
        public IEnumerable<IItem<T>> Variants => _variants;

        [SerializeField] private List<ItemValue<T>> _variants;

        [Button]
        private void CalculateRelativeChances()
        {
            ((IItemVariants<T>)this).CalculateRelativeChancesForVariants();
        }
    }
}
