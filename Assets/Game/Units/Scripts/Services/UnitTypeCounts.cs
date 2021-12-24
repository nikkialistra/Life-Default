using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitTypeCounts : MonoBehaviour
    {
        public event Action<UnitType, float> UnitTypeCountChange; 
        
        [Required] 
        [SerializeField] private UnitRepository _unitRepository;

        private IEnumerable<UnitFacade> _units;

        private void Start()
        {
            _units = _unitRepository.GetObjects();
            
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