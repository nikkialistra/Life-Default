using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.UI.Game;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitChoosing : MonoBehaviour
    {
        public Action<UnitFacade> UnitChosen;

        private UnitTypesView _unitTypesView;
        private UnitRepository _unitRepository;
        private SelectedUnits _selectedUnits;

        private readonly Dictionary<UnitType, int> _lastSelectedUnitByType = new(); 

        [Inject]
        public void Construct(UnitTypesView unitTypesView, UnitRepository unitRepository, SelectedUnits selectedUnits)
        {
            _selectedUnits = selectedUnits;
            _unitRepository = unitRepository;
            _unitTypesView = unitTypesView;
        }

        private void Awake()
        {
            InitializeUnitByTypeDictionary();
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

        private void InitializeUnitByTypeDictionary()
        {
            foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
            {
                _lastSelectedUnitByType.Add(unitType, 0);
            }
        }

        private void ChooseUnit(UnitType unitType)
        {
            var units = _unitRepository.GetObjectsByType(unitType).ToArray();
            
            if (units.Length == 0)
            {
                return;
            }

            ChooseWithDictionary(unitType, units);
        }

        private void ChooseWithDictionary(UnitType unitType, UnitFacade[] units)
        {
            var indexToTake = _lastSelectedUnitByType[unitType] + 1;
            if (units.Length > indexToTake)
            {
                var unit = units[indexToTake];
                _selectedUnits.Set(unit);
                UnitChosen?.Invoke(unit);

                _lastSelectedUnitByType[unitType] = indexToTake;
            }
            else
            {
                var unit = units[0];
                _selectedUnits.Set(units[0]);
                UnitChosen?.Invoke(unit);

                _lastSelectedUnitByType[unitType] = 0;
            }
        }

        private void ChooseUnits(UnitType unitType)
        {
            var units = _unitRepository.GetObjectsByType(unitType).ToArray();

            if (units.Length != 0)
            {
                _selectedUnits.Set(units);
            }
        }
    }
}