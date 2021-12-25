using System;
using System.Collections.Generic;
using System.Linq;
using Kernel.UI.Game;

namespace Game.Units.Services
{
    public class SelectedUnits
    {
        private readonly UnitRepository _unitRepository;
        private InfoPanelView _infoPanelView;

        public SelectedUnits(UnitRepository unitRepository, InfoPanelView infoPanelView)
        {
            _unitRepository = unitRepository;
            _infoPanelView = infoPanelView;
        }
        
        public IEnumerable<UnitFacade> Units { get; private set; } = Array.Empty<UnitFacade>();

        public void Clear()
        {
            Units = Array.Empty<UnitFacade>();
            
            var allUnits = _unitRepository.GetObjects();
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
            var allUnits = _unitRepository.GetObjects();

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