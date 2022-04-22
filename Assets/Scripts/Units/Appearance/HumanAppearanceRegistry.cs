using Sirenix.OdinInspector;
using Units.Appearance.ItemVariants;
using Units.Appearance.Variants;
using Units.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Appearance
{
    public class HumanAppearanceRegistry : MonoBehaviour
    {
        [SerializeField] private HeadVariants _maleHeadVariants;
        [SerializeField] private HeadVariants _femaleHeadVariants;
        [Space]
        [SerializeField] private ColorVariants _colorVariants;
        
        [Title("Colonist Garment Sets")]
        [SerializeField] private GarmentSetVariants _maleColonistGarmentSetVariants;
        [SerializeField] private GarmentSetVariants _femaleColonistGarmentSetVariants;
        
        [Title("Enemy Garment Sets")]
        [SerializeField] private GarmentSetVariants _maleEnemyGarmentSetVariants;
        [SerializeField] private GarmentSetVariants _femaleEnemyGarmentSetVariants;

        public ColorVariants ColorVariants => _colorVariants;

        public HeadVariants HeadVariantsFor(Gender gender)
        {
            return gender == Gender.Male ? _maleHeadVariants : _femaleHeadVariants;
        }

        public IItemVariants<GarmentSet> GarmentSetFor(Gender gender, HumanType humanType)
        {
            if (humanType == HumanType.Colonist)
            {
                return gender == Gender.Male ? _maleColonistGarmentSetVariants.GarmentSets : _femaleColonistGarmentSetVariants.GarmentSets;
            }
            else
            {
                return gender == Gender.Male ? _maleEnemyGarmentSetVariants.GarmentSets : _femaleEnemyGarmentSetVariants.GarmentSets;
            }
        }
        
        public enum HumanType
        {
            Colonist,
            Enemy
        }
    }
}