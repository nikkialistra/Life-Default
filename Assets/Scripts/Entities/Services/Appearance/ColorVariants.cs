using System.Collections.Generic;
using UnityEngine;

namespace Entities.Services.Appearance
{
    [CreateAssetMenu(fileName = "Human Color Variants", menuName = "Human Appearance/Color Variants", order = 1)]
    public class ColorVariants : ScriptableObject
    {
        public List<Color> HairColors;
        public List<Color> SkinColors;
    }
}
