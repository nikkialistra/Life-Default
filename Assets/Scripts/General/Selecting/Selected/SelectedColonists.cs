using System;
using System.Collections.Generic;
using System.Linq;
using Colonists;
using UI.Game.GameLook.Components.Info;

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
        public int Count => Colonists.Count;

        public void Set(List<Colonist> colonists)
        {
            UnsubscribeFromColonists();

            Colonists = colonists.ToList();
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

        public void Add(List<Colonist> colonists)
        {
            UnsubscribeFromColonists();

            Colonists = Colonists.Union(colonists).ToList();
            UpdateSelectionStatuses();
            
            _infoPanelView.SetColonists(Colonists);

            SubscribeToColonists();
        }

        public void Add(Colonist colonist)
        {
            if (!Colonists.Contains(colonist))
            {
                Colonists.Add(colonist);
                colonist.ColonistDying += RemoveFromSelected;
            }
            else
            {
                colonist.Deselect();
                Colonists.Remove(colonist);
                colonist.ColonistDying -= RemoveFromSelected;
            }

            UpdateSelectionStatuses();
            _infoPanelView.SetColonists(Colonists);
        }

        public void Deselect()
        {
            UnsubscribeFromColonists();

            foreach (var colonist in Colonists)
            {
                colonist.Deselect();
            }

            Colonists.Clear();
            
            SelectionChange?.Invoke();
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
