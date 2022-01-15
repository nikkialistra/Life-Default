using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Units.Unit.UnitTypes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Units.Services
{
    public class UnitTypeAppearanceRegistry : MonoBehaviour
    {
        [ValidateInput("@_travelers.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<UnitTypeAppearance> _travelers;
        [ValidateInput("@_lumberjacks.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<UnitTypeAppearance> _lumberjacks;
        [ValidateInput("@_masons.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<UnitTypeAppearance> _masons;
        [ValidateInput("@_melees.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<UnitTypeAppearance> _melees;
        [ValidateInput("@_archers.Count >= 1", "List should have at least one element.")]
        [SerializeField] private List<UnitTypeAppearance> _archers;

        public UnitTypeAppearance GetForType(UnitType unitType)
        {
            return unitType switch
            {
                UnitType.Traveler => GetFrom(_travelers),
                UnitType.Lumberjack => GetFrom(_lumberjacks),
                UnitType.Mason => GetFrom(_masons),
                UnitType.Melee => GetFrom(_melees),
                UnitType.Archer => GetFrom(_archers),
                _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };
        }

        private UnitTypeAppearance GetFrom(List<UnitTypeAppearance> unitTypeAppearances)
        {
            return unitTypeAppearances[Random.Range(0, unitTypeAppearances.Count)];
        }
    }
}
