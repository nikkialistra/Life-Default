using Entities.Types;
using UnityEngine;

namespace Entities.Services.Appearance
{
    public class HumanAppearance : MonoBehaviour
    {
        [SerializeField] private HeadVariants _maleHeadVariants;
        [SerializeField] private HeadVariants _femaleHeadVariants;

        public HeadVariants GetVariantsFor(Gender gender)
        {
            return gender == Gender.Male ? _maleHeadVariants : _femaleHeadVariants;
        }
    }
}