using System;
using System.Collections.Generic;
using Units.Unit.UnitType;
using UnityEngine;

namespace Units.Services
{
    public class UnitClassSpecsRepository : MonoBehaviour
    {
        [SerializeField] private Dictionary<UnitTypeLevel, UnitClassSpecs> _scoutClassSpecs;
        [SerializeField] private Dictionary<UnitTypeLevel, UnitClassSpecs> _lumberjackClassSpecs;
        [SerializeField] private Dictionary<UnitTypeLevel, UnitClassSpecs> _masonClassSpecs;
        [SerializeField] private Dictionary<UnitTypeLevel, UnitClassSpecs> _meleeClassSpecs;
        [SerializeField] private Dictionary<UnitTypeLevel, UnitClassSpecs> _archerClassSpecs;

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
