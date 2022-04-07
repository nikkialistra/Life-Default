using System.Collections.Generic;
using System.Linq;
using Entities.Services.Appearance.ItemVariants.Item;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Services.Appearance.ItemVariants
{
    public interface IItemVariants<T>
    {
        public IEnumerable<IItem<T>> Variants { get; }
        
        [Button]
        public void CalculateRelativeChancesForVariants()
        {
            if (!Variants.Any())
            {
                return;
            }

            var sum = Variants.Sum(variant => variant.Chance);

            foreach (var variant in Variants)
            {
                variant.RelativeChance = (float)variant.Chance / sum;
            }
        }

        public T GetRandom()
        {
            if (!Variants.Any())
            {
                return default;
            }
            
            var randomValue = Random.Range(0f, 1f);

            foreach (var variant in Variants)
            {
                if (randomValue <= variant.RelativeChance)
                {
                    return variant.Value;
                }
                
                randomValue -= variant.Chance;
            }

            return default;
        }
    }
}
