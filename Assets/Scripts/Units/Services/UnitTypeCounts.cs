using System;
using System.Collections.Generic;
using System.Linq;
using UI.Game;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using Zenject;

namespace Units.Services
{
    public class UnitTypeCounts : MonoBehaviour
    {
        private UnitRepository _unitRepository;
        private UnitTypesView _unitTypesView;

        private IEnumerable<UnitFacade> _units;

        [Inject]
        public void Construct(UnitRepository unitRepository, UnitTypesView unitTypesView)
        {
            _unitRepository = unitRepository;
            _unitTypesView = unitTypesView;
        }

        private void Start()
        {
            Show();
        }

        private void OnEnable()
        {
            _unitRepository.Add += IncreaseUnitTypeCount;
            _unitRepository.Remove += DecreaseUnitTypeCount;
        }

        private void OnDisable()
        {
            _unitRepository.Add -= IncreaseUnitTypeCount;
            _unitRepository.Remove -= DecreaseUnitTypeCount;
        }

        private void Show()
        {
            _units = _unitRepository.GetUnits();

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

        private void IncreaseUnitTypeCount(UnitFacade unit)
        {
            _unitTypesView.IncreaseUnitTypeCount(unit.UnitType);
        }

        private void DecreaseUnitTypeCount(UnitFacade unit)
        {
            _unitTypesView.DecreaseFromUnitTypeCount(unit.UnitType);
        }
    }
}
