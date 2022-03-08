using System.Collections.Generic;
using System.Linq;
using Colonists.Colonist;
using UI.Game.GameLook.Components;

namespace Colonists.Services.Selecting
{
    public class SelectedColonists
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedColonists(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public List<ColonistFacade> Colonists { get; private set; } = new();
        private List<ColonistFacade> _lastSelectedUnits = new();

        public void Clear()
        {
            UnsubscribeFromUnits();

            foreach (var colonist in Colonists)
            {
                colonist.Deselect();
            }

            Colonists.Clear();
        }

        public void Set(List<ColonistFacade> colonists)
        {
            UnsubscribeFromUnits();

            Colonists = colonists.ToList();
            UpdateSelectionStatuses();
            _infoPanelView.SetUnits(Colonists);

            SubscribeToUnits();
        }

        public void Set(ColonistFacade colonist)
        {
            UnsubscribeFromUnits();

            Colonists = new List<ColonistFacade>() { colonist };
            UpdateSelectionStatuses();
            _infoPanelView.SetColonist(colonist);

            SubscribeToUnits();
        }

        private void SubscribeToUnits()
        {
            foreach (var colonist in Colonists)
            {
                colonist.ColonistDie += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromUnits()
        {
            foreach (var colonist in Colonists)
            {
                colonist.ColonistDie -= RemoveFromSelected;
            }
        }

        private void RemoveFromSelected(ColonistFacade colonist)
        {
            Colonists.Remove(colonist);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var selected in Colonists)
            {
                selected.Select();
            }

            foreach (var deselected in _lastSelectedUnits.Except(Colonists))
            {
                deselected.Deselect();
            }

            _lastSelectedUnits = new List<ColonistFacade>(Colonists);
        }
    }
}
