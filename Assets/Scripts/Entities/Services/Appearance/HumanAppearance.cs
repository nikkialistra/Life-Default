using Entities.Types;
using UnityEngine;

namespace Entities.Services.Appearance
{
    public class HumanAppearance : MonoBehaviour
    {
        [SerializeField] private HeadVariants _maleHeadVariants;
        [SerializeField] private HeadVariants _femaleHeadVariants;

        [SerializeField] private ColorVariants _colorVariants;

        public HeadVariants HeadVariantsFor(Gender gender)
        {
            return gender == Gender.Male ? _maleHeadVariants : _femaleHeadVariants;
        }
        
        public ColorVariants ColorVariants => _colorVariants;
    }
}