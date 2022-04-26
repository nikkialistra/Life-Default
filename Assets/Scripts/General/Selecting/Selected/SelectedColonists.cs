using System;
using System.Collections.Generic;
using Colonists;
using UI.Game.GameLook.Components;

namespace General.Selecting.Selected
{
    public class SelectedColonists
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedColonists(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public event Action SelectionChange; 

        public List<Colonist> Colonists { get; private set; } = new();

        public void Set(List<Colonist> colonists)
        {
            UnsubscribeFromColonists();

            Colonists = colonists;
            UpdateSelectionStatuses();
            
            _infoPanelView.SetColonists(Colonists);

            SubscribeToColonists();
        }

        public void Set(Colonist colonist)
        {
            UnsubscribeFromColonists();

            Colonists = new List<Colonist> { colonist };
            UpdateSelectionStatuses();
            _infoPanelView.SetColonist(colonist);

            SubscribeToColonists();
        }

        public void Add(Colonist colonist)
        {
            Colonists.Add(colonist);
            UpdateSelectionStatuses();
            _infoPanelView.SetColonists(Colonists);
            
            colonist.ColonistDying += RemoveFromSelected;
        }

        public void Deselect()
        {
            UnsubscribeFromColonists();

            foreach (var colonist in Colonists)
            {
                colonist.Deselect();
            }

            Colonists.Clear();
        }

        public void Destroy()
        {
            UnsubscribeFromColonists();

            foreach (var colonist in Colonists)
            {
                colonist.Die();
            }

            Colonists.Clear();
        }

        public bool Contains(Colonist colonist)
        {
            return Colonists.Contains(colonist);
        }

        private void SubscribeToColonists()
        {
            foreach (var colonist in Colonists)
            {
                colonist.ColonistDying += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromColonists()
        {
            foreach (var colonist in Colonists)
            {
                colonist.ColonistDying -= RemoveFromSelected;
            }
        }

        private void RemoveFromSelected(Colonist colonist)
        {
            Colonists.Remove(colonist);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var colonist in Colonists)
            {
                colonist.Select();
            }

            SelectionChange?.Invoke();
        }
    }
}
