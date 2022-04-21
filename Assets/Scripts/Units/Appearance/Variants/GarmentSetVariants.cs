using Units.Appearance.ItemVariants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Appearance.Variants
{
    [CreateAssetMenu(fileName = "Human Garment Set Variants", menuName = "Human Appearance/Garment Set Variants", order = 2)]
    public class GarmentSetVariants : ScriptableObject
    {
        [SerializeField] private ItemObjectVariants<GarmentSet> _garmentSets;
        
        public IItemVariants<GarmentSet> GarmentSets => _garmentSets;
    }
}
