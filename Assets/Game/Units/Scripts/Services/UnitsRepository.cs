using System.Collections.Generic;
using Game.Units.Unit;
using Game.Units.UnitTypes;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitsRepository : MonoBehaviour
    {
        public IEnumerable<UnitFacade> GetObjects()
        {
            IEnumerable<UnitFacade> units = FindObjectsOfType<UnitFacade>();
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