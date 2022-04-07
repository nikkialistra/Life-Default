using UnityEngine;

namespace Entities.Services.Appearance
{
    [CreateAssetMenu(fileName = "Human Head Variants", menuName = "Human Appearance/Head Variants", order = 0)]
    public class HeadVariants : ScriptableObject
    {
        public ItemVariants Head;
        public ItemVariants Hair;
        public ItemVariants Ears;
        public ItemVariants Eyebrows;
        public ItemVariants FacialHair;
    }
}
