using System.Collections.Generic;
using Game.Units.Unit;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitsRepository : MonoBehaviour
    {
        private IEnumerable<UnitFacade> _units;

        private void Start()
        {
            SetUpRepository();
        }

        public IEnumerable<UnitFacade> GetObjects()
        {
            if (_units == null)
            {
                SetUpRepository();
            }

            return _units;
        }

        public void ResetObjects()
        {
            _units = null;
        }

        public IEnumerable<UnitFacade> GetObjectsByType(UnitType unitType)
        {
            if (_units == null)
            {
                SetUpRepository();
            }
            
            foreach (var unit in _units)
            {
                if (unit.UnitType == unitType)
                {
                    yield return unit;
                }
            }
        }

        private void SetUpRepository()
        {
            _units = FindObjectsOfType<UnitFacade>();
        }
    }
}