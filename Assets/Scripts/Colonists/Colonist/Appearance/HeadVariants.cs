using System.Collections.Generic;
using UnityEngine;

namespace Colonists.Colonist.Appearance
{
    [CreateAssetMenu(fileName = "Colonist Head", menuName = "Colonist Appearance", order = 0)]
    public class HeadVariants : ScriptableObject
    {
        [SerializeField] private ItemVariants _head;
        [SerializeField] private ItemVariants _hair;
        [SerializeField] private ItemVariants _ears;
        [SerializeField] private ItemVariants _eyebrows;
        [SerializeField] private ItemVariants _facialHair;

        public List<Item> Head => _head.Variants;
        public List<Item> Hair => _hair.Variants;
        public List<Item> Ears => _ears.Variants;
        public List<Item> Eyebrows => _eyebrows.Variants;
        public List<Item> FacialHair => _facialHair.Variants;
    }
}
