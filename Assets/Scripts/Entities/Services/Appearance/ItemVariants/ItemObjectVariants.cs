using System;
using System.Collections.Generic;
using Entities.Services.Appearance.ItemVariants.Item;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Services.Appearance.ItemVariants
{
    [Serializable]
    public class ItemObjectVariants<T> : IItemVariants<T> where T : UnityEngine.Object
    {
        [SerializeField] private List<ItemObject<T>> _variants;
        public IEnumerable<IItem<T>> Variants => _variants;

        [Button]
        private void CalculateRelativeChances()
        {
            ((IItemVariants<T>)this).CalculateRelativeChancesForVariants();
        }
    }
}
