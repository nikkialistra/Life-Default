using System.Collections.Generic;
using Game.Units.Unit;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitsRepository : MonoBehaviour
    {
        public IEnumerable<UnitFacade> GetObjects()
        {
            return FindObjectsOfType<UnitFacade>();
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