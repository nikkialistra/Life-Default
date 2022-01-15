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

        private readonly Dictionary<UnitType, int> _unitTypeCounts = new();

        [Inject]
        public void Construct(UnitRepository unitRepository, UnitTypesView unitTypesView)
        {
            _unitRepository = unitRepository;
            _unitTypesView = unitTypesView;
        }

        private void Start()
        {
            FillInCounts();
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

        private void FillInCounts()
        {
            _unitTypeCounts.Add(UnitType.Traveler, 0);
            _unitTypeCounts.Add(UnitType.Lumberjack, 0);
            _unitTypeCounts.Add(UnitType.Mason, 0);
            _unitTypeCounts.Add(UnitType.Melee, 0);
            _unitTypeCounts.Add(UnitType.Archer, 0);
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

            _unitTypesView.UpdateUnitTypeCount(unitType, count);
        }

        private void IncreaseUnitTypeCount(UnitFacade unit)
        {
            CheckUnitTypeExistence(unit.UnitType);

            _unitTypeCounts[unit.UnitType] += 1;
            var value = _unitTypeCounts[unit.UnitType];

            _unitTypesView.UpdateUnitTypeCount(unit.UnitType, _unitTypeCounts[unit.UnitType]);
        }

        private void DecreaseUnitTypeCount(UnitFacade unit)
        {
            CheckUnitTypeExistence(unit.UnitType);

            _unitTypeCounts[unit.UnitType] -= 1;
            var value = _unitTypeCounts[unit.UnitType];

            if (value < 0)
            {
                throw new InvalidOperationException($"{unit.UnitType} cannot be less than zero");
            }

            _unitTypesView.UpdateUnitTypeCount(unit.UnitType, _unitTypeCounts[unit.UnitType]);
        }

        private void CheckUnitTypeExistence(UnitType unitType)
        {
            if (!_unitTypeCounts.ContainsKey(unitType))
            {
                throw new ArgumentException($"Dictionary doesn't contain key {unitType}");
            }
        }
    }
}
