using System.Collections.Generic;
using Units.Unit;
using Units.UnitTypes;
using UnityEngine;

namespace Units.Services
{
    public class UnitsRepository : MonoBehaviour
    {
        public IEnumerable<UnitFacade> GetObjects()
        {
            var units = FindObjectsOfType<UnitFacade>();
            foreach (var unit in units)
            {
                if (unit.Alive)
                {
                    yield return unit;
                }
            }
        }

        public IEnumerable<UnitFacade> GetObjectsByType(UnitType unitType)
        {
            var units = FindObjectsOfType<UnitFacade>();
            
            foreach (var unit in units)
            {
                if (unit.UnitType == unitType)
                {
                    yield return unit;
                }
            }
        }
    }
}