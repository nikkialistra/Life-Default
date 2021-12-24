using Kernel.UI.GameViews;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitChoosing : MonoBehaviour
    {
        private UnitTypesView _unitTypesView;
        private UnitRepository _unitRepository;
        private SelectedUnits _selectedUnits;

        [Inject]
        public void Construct(UnitTypesView unitTypesView, UnitRepository unitRepository, SelectedUnits selectedUnits)
        {
            _selectedUnits = selectedUnits;
            _unitRepository = unitRepository;
            _unitTypesView = unitTypesView;
        }

        private void OnEnable()
        {
            _unitTypesView.LeftClick += ChooseUnit;
            _unitTypesView.RightClick += ChooseUnits;
        }

        private void OnDisable()
        {
            _unitTypesView.LeftClick -= ChooseUnit;
            _unitTypesView.RightClick -= ChooseUnits;
        }

        private void ChooseUnit(UnitType unitType)
        {
            Debug.Log("choosing " + unitType);
        }

        private void ChooseUnits(UnitType unitType)
        {
            var units = _unitRepository.GetObjectsByType(unitType);
            _selectedUnits.Set(units);
        }
    }
}