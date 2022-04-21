using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants.Item;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Appearance.ItemVariants
{
    public interface IItemVariants<T>
    {
        public IEnumerable<IItem<T>> Variants { get; }

        public bool IsEmpty => !Variants.Any();

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
                
                randomValue -= variant.RelativeChance;
            }

            return default;
        }

        public int GetRandomIndex()
        {
            var index = 0;
            var randomValue = Random.Range(0f, 1f);

            foreach (var variant in Variants)
            {
                if (randomValue <= variant.RelativeChance)
                {
                    break;
                }
                
                randomValue -= variant.RelativeChance;
                index++;
            }

            return index;
        }

        public T GetAtIndex(int index)
        {
            if (index >= Variants.Count())
            {
                throw new InvalidOperationException("Taking item element at not existing index");
            }

            return Variants.ElementAt(index).Value;
        }
    }
}
