﻿using System.Collections.Generic;
using System.Linq;
using Game.UI.Game;
using Game.Units.Services;
using Game.Units.Unit;

namespace Game.Units.Selecting
{
    public class SelectedUnits
    {
        private readonly UnitsRepository _unitsRepository;
        private InfoPanelView _infoPanelView;

        public SelectedUnits(UnitsRepository unitsRepository, InfoPanelView infoPanelView)
        {
            _unitsRepository = unitsRepository;
            _infoPanelView = infoPanelView;
        }

        public List<UnitFacade> Units { get; private set; } = new();

        public void Clear()
        {
            Units.Clear();
            
            var allUnits = _unitsRepository.GetObjects();
            foreach (var unit in allUnits)
            {
                unit.Deselect();
            }
        }

        public void Set(List<UnitFacade> units)
        {
            Units = units.ToList();
            UpdateSelectionStatuses();
            _infoPanelView.SetUnits(Units);
        }

        public void Set(UnitFacade unit)
        {
            Units = new List<UnitFacade>() { unit };
            UpdateSelectionStatuses();
            _infoPanelView.SetUnit(unit);
        }

        private void UpdateSelectionStatuses()
        {
            var allUnits = _unitsRepository.GetObjects();

            foreach (var deselected in allUnits.Except(Units))
            {
                deselected.Deselect();
            }

            foreach (var selected in Units)
            {
                selected.Select();
            }
        }
    }
}