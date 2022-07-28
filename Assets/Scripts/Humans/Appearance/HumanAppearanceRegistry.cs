using Humans.Appearance.ItemVariants;
using Humans.Appearance.Variants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Humans.Appearance
{
    public class HumanAppearanceRegistry : MonoBehaviour
    {
        public ColorVariants ColorVariants => _colorVariants;

        public enum HumanType
        {
            Colonist,
            Aborigine
        }

        [SerializeField] private HeadVariants _maleHeadVariants;
        [SerializeField] private HeadVariants _femaleHeadVariants;
        [Space]
        [SerializeField] private ColorVariants _colorVariants;

        [Title("Colonist Garment Sets")]
        [SerializeField] private GarmentSetVariants _maleColonistGarmentSetVariants;
        [SerializeField] private GarmentSetVariants _femaleColonistGarmentSetVariants;

        [Title("Aborigine Garment Sets")]
        [SerializeField] private GarmentSetVariants _maleAborigineGarmentSetVariants;
        [SerializeField] private GarmentSetVariants _femaleAborigineGarmentSetVariants;

        public HeadVariants HeadVariantsFor(Gender gender)
        {
            return gender == Gender.Male ? _maleHeadVariants : _femaleHeadVariants;
        }

        public IItemVariants<GarmentSet> GarmentSetFor(Gender gender, HumanType humanType)
        {
            if (humanType == HumanType.Colonist)
                return gender == Gender.Male
                    ? _maleColonistGarmentSetVariants.GarmentSets
                    : _femaleColonistGarmentSetVariants.GarmentSets;
            else
                return gender == Gender.Male
                    ? _maleAborigineGarmentSetVariants.GarmentSets
                    : _femaleAborigineGarmentSetVariants.GarmentSets;
        }
    }
}
