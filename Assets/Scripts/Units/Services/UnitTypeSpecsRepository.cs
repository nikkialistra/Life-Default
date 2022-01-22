using System;
using Common;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace Units.Services
{
    public class UnitTypeSpecsRepository : MonoBehaviour
    {
        [SerializeField] private UnitTypeSpecsDictionary _scoutTypeSpecs;
        [SerializeField] private UnitTypeSpecsDictionary _lumberjackTypeSpecs;
        [SerializeField] private UnitTypeSpecsDictionary _masonTypeSpecs;
        [SerializeField] private UnitTypeSpecsDictionary _meleeTypeSpecs;
        [SerializeField] private UnitTypeSpecsDictionary _archerTypeSpecs;

        [Serializable] public class UnitTypeSpecsDictionary : SerializableDictionary<UnitTypeLevel, UnitTypeSpecs> { }

        public UnitTypeSpecs GetFor(UnitType unitType, UnitTypeLevel level)
        {
            return unitType switch
            {
                UnitType.Scout => _scoutTypeSpecs[level],
                UnitType.Lumberjack => _lumberjackTypeSpecs[level],
                UnitType.Mason => _masonTypeSpecs[level],
                UnitType.Melee => _meleeTypeSpecs[level],
                UnitType.Archer => _archerTypeSpecs[level],
                _ => throw new ArgumentOutOfRangeException(nameof(unitType))
            };
        }
    }
}
