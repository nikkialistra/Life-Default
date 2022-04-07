using Entities.Services.Appearance.ItemVariants;
using UnityEngine;

namespace Entities.Services.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Head Variants", menuName = "Human Appearance/Head Variants", order = 0)]
    public class HeadVariants : ScriptableObject
    {
        [SerializeField] private ItemObjectVariants<Mesh> _head;
        [SerializeField] private ItemObjectVariants<Mesh> _hair;
        [SerializeField] private ItemObjectVariants<Mesh> _ears;
        [SerializeField] private ItemObjectVariants<Mesh> _eyebrows;
        [SerializeField] private ItemObjectVariants<Mesh> _facialHair;

        public IItemVariants<Mesh> Head => _head;
        public IItemVariants<Mesh> Hair => _hair;
        public IItemVariants<Mesh> Ears => _ears;
        public IItemVariants<Mesh> Eyebrows => _eyebrows;
        public IItemVariants<Mesh> FacialHair => _facialHair;
    }
}
