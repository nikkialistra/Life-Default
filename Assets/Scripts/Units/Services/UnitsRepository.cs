using System;
using System.Collections.Generic;
using System.Linq;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;

namespace Units.Services
{
    public class UnitsRepository : MonoBehaviour
    {
        private List<UnitFacade> _units = new();

        public event Action<UnitFacade> Add;
        public event Action<UnitFacade> Remove;

        private void Start()
        {
            _units = FindObjectsOfType<UnitFacade>().ToList();
        }

        public IEnumerable<UnitFacade> GetUnits()
        {
            foreach (var unit in _units)
            {
                if (unit.Alive)
                {
                    yield return unit;
                }
            }
        }

        public IEnumerable<UnitFacade> GetUnitsByType(UnitType unitType)
        {
            foreach (var unit in _units)
            {
                if (unit.Alive && unit.UnitType == unitType)
                {
                    yield return unit;
                }
            }
        }

        public void AddUnit(UnitFacade unit)
        {
            _units.Add(unit);
            Add?.Invoke(unit);
        }

        public void RemoveUnit(UnitFacade unit)
        {
            _units.Remove(unit);
            Remove?.Invoke(unit);
        }
    }
}