using Units.Appearance.Variants;
using UnityEngine;

namespace Units.Appearance
{
    public class HumanAppearance : MonoBehaviour
    {
        [SerializeField] private HeadVariants _maleHeadVariants;
        [SerializeField] private HeadVariants _femaleHeadVariants;
        [Space]
        [SerializeField] private GarmentSetVariants _maleGarmentSetVariants;
        [SerializeField] private GarmentSetVariants _femaleGarmentSetVariants;
        [Space]
        [SerializeField] private ColorVariants _colorVariants;

        public ColorVariants ColorVariants => _colorVariants;

        public HeadVariants HeadVariantsFor(Gender gender)
        {
            return gender == Gender.Male ? _maleHeadVariants : _femaleHeadVariants;
        }

        public GarmentSetVariants GarmentSetFor(Gender gender)
        {
            return gender == Gender.Male ? _maleGarmentSetVariants : _femaleGarmentSetVariants;
        }
    }
}