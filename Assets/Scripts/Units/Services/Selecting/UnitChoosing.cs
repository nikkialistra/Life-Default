using System;
using System.Linq;
using UI.Game.GameLook.Components;
using Units.Unit;
using UnityEngine;
using Zenject;

namespace Units.Services.Selecting
{
    public class UnitChoosing : MonoBehaviour
    {
        private UnitsInfoView _unitsInfoView;
        private UnitRepository _unitRepository;
        private SelectedUnits _selectedUnits;
        
        private int _indexToTake;

        [Inject]
        public void Construct(UnitsInfoView unitsInfoView, UnitRepository unitRepository,
            SelectedUnits selectedUnits)
        {
            _unitsInfoView = unitsInfoView;
            _unitRepository = unitRepository;
            _selectedUnits = selectedUnits;
        }

        public Action<UnitFacade> UnitChosen;

        private void OnEnable()
        {
            _unitsInfoView.SelectUnit += ChooseUnit;
        }

        private void OnDisable()
        {
            _unitsInfoView.SelectUnit -= ChooseUnit;
        }
        
        private void ChooseUnit(UnitFacade unit)
        {
            _selectedUnits.Set(unit);
        }

        public void NextUnitTo(UnitFacade unit)
        {
            var units = _unitRepository.GetUnits().ToArray();

            var index = GetUnitIndex(units, unit);
            ChangeToNextUnit(units, index);
        }

        private int GetUnitIndex(UnitFacade[] units, UnitFacade unit)
        {
            for (var i = 0; i < units.Length; i++)
            {
                if (units[i] == unit)
                {
                    return i;
                }
            }

            return -1;
        }

        private void ChangeToNextUnit(UnitFacade[] units, int index)
        {
            index++;
            if (index < units.Length)
            {
                var unit = units[index];
                _selectedUnits.Set(unit);
                UnitChosen?.Invoke(unit);
            }
            else
            {
                var unit = units[0];
                _selectedUnits.Set(units[0]);
                UnitChosen?.Invoke(unit);
            }
        }
    }
}
