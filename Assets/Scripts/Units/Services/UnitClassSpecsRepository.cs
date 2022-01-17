using System;
using Common;
using Units.Unit.UnitType;
using UnityEngine;

namespace Units.Services
{
    public class UnitClassSpecsRepository : MonoBehaviour
    {
        [SerializeField] private UnitClassSpecsDictionary _scoutClassSpecs;
        [SerializeField] private UnitClassSpecsDictionary _lumberjackClassSpecs;
        [SerializeField] private UnitClassSpecsDictionary _masonClassSpecs;
        [SerializeField] private UnitClassSpecsDictionary _meleeClassSpecs;
        [SerializeField] private UnitClassSpecsDictionary _archerClassSpecs;
        
        [Serializable] public class UnitClassSpecsDictionary : SerializableDictionary<UnitTypeLevel, UnitClassSpecs> { }

        public UnitClassSpecs GetFor(UnitType unitType, UnitTypeLevel level)
        {
            return unitType switch
            {
                UnitType.Scout => _scoutClassSpecs[level],
                UnitType.Lumberjack => _lumberjackClassSpecs[level],
                UnitType.Mason => _masonClassSpecs[level],
                UnitType.Melee => _meleeClassSpecs[level],
                UnitType.Archer => _archerClassSpecs[level],
                _ => throw new ArgumentOutOfRangeException(nameof(unitType))
            };
        }
    }
}
