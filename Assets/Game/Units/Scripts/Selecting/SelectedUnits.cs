using System;
using System.Collections.Generic;
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
        
        public IEnumerable<UnitFacade> Units { get; private set; } = Array.Empty<UnitFacade>();

        public void Clear()
        {
            Units = Array.Empty<UnitFacade>();
            
            var allUnits = _unitsRepository.GetObjects();
            foreach (var unit in allUnits)
            {
                unit.Deselect();
            }
        }

        public void Set(IEnumerable<UnitFacade> units)
        {
            Units = units as UnitFacade[] ?? units.ToArray();
            UpdateSelectionStatuses();
        }

        public void Set(UnitFacade unit)
        {
            Units = new[] { unit };
            _infoPanelView.SetUnit(unit);
            UpdateSelectionStatuses();
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