using Units.Appearance.ItemVariants;
using UnityEngine;

namespace Units.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Garment Set Variants", menuName = "Human Appearance/Garment Set Variants",
        order = 2)]
    public class GarmentSetVariants : ScriptableObject
    {
        public IItemVariants<GarmentSet> GarmentSets => _garmentSets;

        [SerializeField] private ItemObjectVariants<GarmentSet> _garmentSets;
    }
}
