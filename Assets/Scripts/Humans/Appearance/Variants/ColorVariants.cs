using Humans.Appearance.ItemVariants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Humans.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Color Variants", menuName = "Human Appearance/Color Variants", order = 1)]
    public class ColorVariants : ScriptableObject
    {
        public IItemVariants<Color> HairColors => _hairColors;
        public IItemVariants<Material> SkinColorMaterials => _skinColorMaterials;

        [SerializeField] private ItemValueVariants<Color> _hairColors;
        [SerializeField] private ItemObjectVariants<Material> _skinColorMaterials;

        [Button]
        private void CalculateAllRelativeChances()
        {
            HairColors.CalculateRelativeChancesForVariants();
            SkinColorMaterials.CalculateRelativeChancesForVariants();
        }
    }
}
