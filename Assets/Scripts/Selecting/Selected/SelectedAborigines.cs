using System.Collections.Generic;
using System.Linq;
using Aborigines;
using UI.Game.GameLook.Components.Info;

namespace Selecting.Selected
{
    public class SelectedAborigines
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedAborigines(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public List<Aborigine> Aborigines { get; private set; } = new();
        public int Count => Aborigines.Count;

        public void Set(List<Aborigine> aborigines)
        {
            UnsubscribeFromAborigines();

            Aborigines = aborigines.ToList();
            UpdateSelectionStatuses();

            _infoPanelView.SetAborigines(Aborigines);

            SubscribeToAborigines();
        }

        public void Add(List<Aborigine> aborigines)
        {
            UnsubscribeFromAborigines();

            Aborigines = Aborigines.Union(aborigines).ToList();
            UpdateSelectionStatuses();

            _infoPanelView.SetAborigines(Aborigines);

            SubscribeToAborigines();
        }

        public void Set(Aborigine aborigine)
        {
            UnsubscribeFromAborigines();

            Aborigines = new List<Aborigine> { aborigine };
            UpdateSelectionStatuses();
            _infoPanelView.SetAborigine(aborigine);

            SubscribeToAborigines();
        }

        public void Add(Aborigine aborigine)
        {
            if (!Aborigines.Contains(aborigine))
            {
                Aborigines.Add(aborigine);
                aborigine.AborigineDying += RemoveFromSelected;
            }
            else
            {
                aborigine.Deselect();
                Aborigines.Remove(aborigine);
                aborigine.AborigineDying -= RemoveFromSelected;
            }

            UpdateSelectionStatuses();
            _infoPanelView.SetAborigines(Aborigines);
        }

        public void Deselect()
        {
            UnsubscribeFromAborigines();

            foreach (var aborigine in Aborigines)
                aborigine.Deselect();

            Aborigines.Clear();
        }

        public void Destroy()
        {
            UnsubscribeFromAborigines();

            foreach (var aborigine in Aborigines)
                aborigine.Die();

            Aborigines.Clear();
        }

        private void SubscribeToAborigines()
        {
            foreach (var aborigine in Aborigines)
                aborigine.AborigineDying += RemoveFromSelected;
        }

        private void UnsubscribeFromAborigines()
        {
            foreach (var aborigine in Aborigines)
                aborigine.AborigineDying -= RemoveFromSelected;
        }

        private void RemoveFromSelected(Aborigine aborigine)
        {
            Aborigines.Remove(aborigine);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var aborigine in Aborigines)
                aborigine.Select();
        }
    }
}
