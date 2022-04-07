using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Colonists.Colonist.Appearance
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
    }
}
