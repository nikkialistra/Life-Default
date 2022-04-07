using System;
using System.Collections.Generic;
using Entities.Services.Appearance.ItemVariants.Item;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Services.Appearance.ItemVariants
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
