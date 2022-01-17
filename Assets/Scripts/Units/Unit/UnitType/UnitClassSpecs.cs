using System.Collections.Generic;
using UnityEngine;

namespace Units.Unit.UnitType
{
    [CreateAssetMenu(fileName = "UnitClassSpecs", menuName = "Data/Unit Class Specs")]
    public class UnitClassSpecs : ScriptableObject
    {
        public UnitType UnitType;
        public UnitTypeLevel UnitTypeLevel;

        [Space]
        public List<SpecsPerEntityType> Specs;
    }
}
