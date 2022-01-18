using System.Collections.Generic;
using System.Linq;
using Entities.Entity;
using UnityEngine;

namespace Units.Unit.UnitType
{
    [CreateAssetMenu(fileName = "UnitClassSpecs", menuName = "Data/Unit Class Specs")]
    public class UnitClassSpecs : ScriptableObject
    {
        [SerializeField] private UnitType _unitType;
        [SerializeField] private UnitTypeLevel _unitTypeLevel;

        [Space]
        [SerializeField] private List<SpecsPerEntityType> _allSpecs;

        public bool ContainsSpecsFor(EntityType entityType)
        {
            return _allSpecs.Any(specsPerEntityType => specsPerEntityType.EntityType == entityType);
        }
    }
}
