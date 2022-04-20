using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
using UnityEngine;

namespace Units.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Color Variants", menuName = "Human Appearance/Color Variants", order = 1)]
    public class ColorVariants : ScriptableObject
    {
        [SerializeField] private ItemValueVariants<Color> _hairColors;
        [SerializeField] private ItemObjectVariants<Material> _skinColorMaterials;
        
        [Button]
        private void CalculateAllRelativeChances()
        {
            HairColors.CalculateRelativeChancesForVariants();
            SkinColorMaterials.CalculateRelativeChancesForVariants();
        }

        public IItemVariants<Color> HairColors => _hairColors;
        public IItemVariants<Material> SkinColorMaterials => _skinColorMaterials;
    }
}
