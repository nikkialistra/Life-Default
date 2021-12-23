using System.Collections.Generic;
using System.Linq;
using Kernel.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Units.Services
{
    public class UnitTypeCounts : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;
        [Required] 
        [SerializeField] private UnitRepository _unitRepository;

        private IEnumerable<UnitFacade> _units;

        private void Start()
        {
            _units = _unitRepository.GetObjects();
            
            ShowUnitTypeCount(UnitType.Traveler);
        }

        private void ShowUnitTypeCount(UnitType unitType)
        {
            var count = _units.Count(unit => unit.UnitType == unitType);

            _unitTypesView.ChangeUnitTypeCount(unitType, count);
        }
    }
}