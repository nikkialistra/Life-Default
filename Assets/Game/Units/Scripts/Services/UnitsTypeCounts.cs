using System;
using System.Collections.Generic;
using System.Linq;
using Game.Units.Unit;
using Game.Units.UnitTypes;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitsTypeCounts : MonoBehaviour
    {
        public event Action<UnitType, float> UnitTypeCountChange; 
        
        private UnitsRepository _unitsRepository;

        private IEnumerable<UnitFacade> _units;

        [Inject]
        public void Construct(UnitsRepository unitsRepository)
        {
            _unitsRepository = unitsRepository;
        }

        private void Start()
        {
            _units = _unitsRepository.GetObjects();
            
            foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
            {
                ShowUnitTypeCount(unitType);
            }
        }

        private void ShowUnitTypeCount(UnitType unitType)
        {
            var count = _units.Count(unit => unit.UnitType == unitType);

            UnitTypeCountChange?.Invoke(unitType, count);
        }
    }
}