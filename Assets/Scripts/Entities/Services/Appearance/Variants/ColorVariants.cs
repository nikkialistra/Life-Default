using Entities.Services.Appearance.ItemVariants;
using UnityEngine;

namespace Entities.Services.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Color Variants", menuName = "Human Appearance/Color Variants", order = 1)]
    public class ColorVariants : ScriptableObject
    {
        [SerializeField] private ItemValueVariants<Color> _hairColors;
        [SerializeField] private ItemObjectVariants<Texture2D> _skinColorTextures;

        public IItemVariants<Color> HairColors => _hairColors;
        public IItemVariants<Texture2D> SkinColorTextures => _skinColorTextures;
    }
}
