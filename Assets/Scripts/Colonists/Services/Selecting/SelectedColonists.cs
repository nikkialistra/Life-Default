using System;
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

        public event Action<List<ColonistFacade>> SelectionChange; 

        public List<ColonistFacade> Colonists { get; private set; } = new();
        private List<ColonistFacade> _lastSelectedColonists = new();

        public void Clear()
        {
            UnsubscribeFromColonists();

            foreach (var colonist in Colonists)
            {
                colonist.Deselect();
            }

            Colonists.Clear();
        }

        public void Set(List<ColonistFacade> colonists)
        {
            UnsubscribeFromColonists();

            Colonists = colonists.ToList();
            UpdateSelectionStatuses();
            _infoPanelView.SetColonists(Colonists);

            SubscribeToColonists();
        }

        public void Set(ColonistFacade colonist)
        {
            UnsubscribeFromColonists();

            Colonists = new List<ColonistFacade>() { colonist };
            UpdateSelectionStatuses();
            _infoPanelView.SetColonist(colonist);

            SubscribeToColonists();
        }

        public void Add(ColonistFacade colonist)
        {
            Colonists.Add(colonist);
            UpdateSelectionStatuses();
            _infoPanelView.SetColonists(Colonists);
            
            colonist.ColonistDie += RemoveFromSelected;
        }

        private void SubscribeToColonists()
        {
            foreach (var colonist in Colonists)
            {
                colonist.ColonistDie += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromColonists()
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

            foreach (var deselected in _lastSelectedColonists.Except(Colonists))
            {
                deselected.Deselect();
            }

            _lastSelectedColonists = new List<ColonistFacade>(Colonists);
            
            SelectionChange?.Invoke(_lastSelectedColonists);
        }
    }
}
