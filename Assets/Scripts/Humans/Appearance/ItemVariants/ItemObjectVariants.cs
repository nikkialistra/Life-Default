using System;
using System.Collections.Generic;
using Humans.Appearance.ItemVariants.Item;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Humans.Appearance.ItemVariants
{
    [Serializable]
    public class ItemObjectVariants<T> : IItemVariants<T> where T : Object
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
