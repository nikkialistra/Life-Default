using System.Collections.Generic;
using System.Linq;
using UI.Game.GameLook.Components;
using Units.Unit;

namespace Units.Services.Selecting
{
    public class SelectedUnits
    {
        private readonly UnitRepository _unitRepository;
        private readonly InfoPanelView _infoPanelView;

        public SelectedUnits(UnitRepository unitRepository, InfoPanelView infoPanelView)
        {
            _unitRepository = unitRepository;
            _infoPanelView = infoPanelView;
        }

        public List<UnitFacade> Units { get; private set; } = new();
        private List<UnitFacade> _lastSelectedUnits = new();

        public void Clear()
        {
            UnsubscribeFromUnits();

            foreach (var unit in Units)
            {
                unit.Deselect();
            }

            Units.Clear();
        }

        public void Set(List<UnitFacade> units)
        {
            UnsubscribeFromUnits();

            Units = units.ToList();
            UpdateSelectionStatuses();
            _infoPanelView.SetUnits(Units);

            SubscribeToUnits();
        }

        public void Set(UnitFacade unit)
        {
            UnsubscribeFromUnits();

            Units = new List<UnitFacade>() { unit };
            UpdateSelectionStatuses();
            _infoPanelView.SetUnit(unit);

            SubscribeToUnits();
        }

        private void SubscribeToUnits()
        {
            foreach (var unit in Units)
            {
                unit.UnitDie += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromUnits()
        {
            foreach (var unit in Units)
            {
                unit.UnitDie -= RemoveFromSelected;
            }
        }

        private void RemoveFromSelected(UnitFacade unit)
        {
            Units.Remove(unit);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var selected in Units)
            {
                selected.Select();
            }

            foreach (var deselected in _lastSelectedUnits.Except(Units))
            {
                deselected.Deselect();
            }

            _lastSelectedUnits = new List<UnitFacade>(Units);
        }
    }
}
