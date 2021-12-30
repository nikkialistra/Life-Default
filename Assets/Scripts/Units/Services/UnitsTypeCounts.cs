using System;
using System.Collections.Generic;
using System.Linq;
using UI.Game;
using Units.Unit;
using Units.UnitTypes;
using UnityEngine;
using Zenject;

namespace Units.Services
{
    public class UnitsTypeCounts : MonoBehaviour
    {
        private UnitsRepository _unitsRepository;
        private UnitTypesView _unitTypesView;

        private IEnumerable<UnitFacade> _units;

        [Inject]
        public void Construct(UnitsRepository unitsRepository, UnitTypesView unitTypesView)
        {
            _unitsRepository = unitsRepository;
            _unitTypesView = unitTypesView;
        }

        private void Start()
        {
            Show();
        }

        private void OnEnable()
        {
            _unitsRepository.Update += Show;
        }

        private void OnDisable()
        {
            _unitsRepository.Update -= Show;
        }

        private void Show()
        {
            _units = _unitsRepository.GetUnits();

            foreach (UnitType unitType in Enum.GetValues(typeof(UnitType)))
            {
                ShowUnitTypeCount(unitType);
            }
        }

        private void ShowUnitTypeCount(UnitType unitType)
        {
            var count = _units.Count(unit => unit.UnitType == unitType);

            _unitTypesView.ChangeUnitTypeCount(unitType, count);
        }
    }
}