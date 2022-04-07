using Entities.Types;
using UnityEngine;

namespace Colonists.Colonist.Appearance
{
    public class ColonistAppearance : MonoBehaviour
    {
        [SerializeField] private HeadVariants _maleHeadVariants;
        [SerializeField] private HeadVariants _femaleHeadVariants;

        public void GenerateAppearanceFor(ColonistFacade colonist)
        {
            if (colonist.Gender == Gender.Male)
            {
                colonist.RandomizeAppearanceWith(_maleHeadVariants);
            }
            else
            {
                colonist.RandomizeAppearanceWith(_femaleHeadVariants);
            }
        }
    }
}