using System;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Game;
using Game.Units.Services;
using Game.Units.Unit;
using UnityEngine;
using Zenject;

namespace Game.Units.Selecting
{
    public class UnitsChoosing : MonoBehaviour
    {
        public Action<UnitFacade> UnitChosen;

        private UnitTypesView _unitTypesView;
        private UnitsInfoView _unitsInfoView;
        private UnitsRepository _unitsRepository;
        private SelectedUnits _selectedUnits;

        private readonly Dictionary<UnitType, int> _lastSelectedUnitByType = new();

        [Inject]
        public void Construct(UnitTypesView unitTypesView, UnitsInfoView unitsInfoView, UnitsRepository unitsRepository, SelectedUnits selectedUnits)
        {
            _unitTypesView = unitTypesView;
            _unitsInfoView = unitsInfoView;
            _unitsRepository = unitsRepository;
            _selectedUnits = selectedUnits;
        }

        private void Awake()
        {
            InitializeUnitByTypeDictionary();
        }

        private void OnEnable()
        {
            _unitsInfoView.UnitIconClick += ChooseUnit;
            
            _unitTypesView.LeftClick += ChooseUnitByType;
            _unitTypesView.RightClick += ChooseUnits;
        }

        private void OnDisable()
        {
            _unitsInfoView.UnitIconClick -= ChooseUnit;
            
            _unitTypesView.LeftClick -= ChooseUnitByType;
            _unitTypesView.RightClick -= ChooseUnits;
        }

        private void InitializeUnitByTypeDictionary()
        {
            foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
            {
                _lastSelectedUnitByType.Add(unitType, 0);
            }
        }

        private void ChooseUnit(UnitFacade unit)
        {
            _selectedUnits.Set(unit);
        }

        private void ChooseUnitByType(UnitType unitType)
        {
            var units = _unitsRepository.GetObjectsByType(unitType).ToArray();
            
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
            var units = _unitsRepository.GetObjectsByType(unitType).ToList();

            if (units.Count != 0)
            {
                _selectedUnits.Set(units);
            }
        }
    }
}