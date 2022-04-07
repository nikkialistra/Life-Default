using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities.Services.Appearance
{
    [Serializable]
    public class ItemVariants
    {
        public List<Item> Variants;

        [Button]
        private void CalculateRelativeChances()
        {
            if (Variants.Count == 0)
            {
                return;
            }

            var sum = Variants.Sum(variant => variant.Chance);

            foreach (var variant in Variants)
            {
                variant.RelativeChance = (float)variant.Chance / sum;
            }
        }

        public Mesh GetRandom()
        {
            if (Variants.Count == 0)
            {
                return null;
            }
            
            var randomValue = Random.Range(0f, 1f);

            for (var i = 0; i < Variants.Count; i++)
            {
                if (randomValue <= Variants[i].RelativeChance)
                {
                    return Variants[i].Mesh;
                }
                
                randomValue -= Variants[i].Chance;
            }

            return null;
        }
    }
}
