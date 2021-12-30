using System;
using System.Collections.Generic;
using System.Linq;
using Units.Unit;
using Units.UnitTypes;
using UnityEngine;

namespace Units.Services
{
    public class UnitsRepository : MonoBehaviour
    {
        public event Action Update;
        
        private List<UnitFacade> _units = new();

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

        public void Add(UnitFacade unit)
        {
            _units.Add(unit);
            Update?.Invoke();
        }

        public void Remove(UnitFacade unit)
        {
            var result = _units.Remove(unit);
            Update?.Invoke();
        }
    }
}