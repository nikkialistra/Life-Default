using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UI.Game.GameLook.Components;

namespace General.Selection.Selected
{
    public class SelectedEntities
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedEntities(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public event Action SelectionChange; 

        public List<Entity> Entities { get; private set; } = new();
        private List<Entity> _lastSelectedEntities = new();

        public void Clear()
        {
            UnsubscribeFromEntities();

            foreach (var entity in Entities)
            {
                entity.Deselect();
            }

            Entities.Clear();
        }

        public void Set(List<Entity> enemies)
        {
            UnsubscribeFromEntities();

            Entities = enemies;
            UpdateSelectionStatuses();
            
            _infoPanelView.SetEntities(Entities);

            SubscribeToColonists();
        }

        public void Set(Entity entity)
        {
            UnsubscribeFromEntities();

            Entities = new List<Entity> { entity };
            UpdateSelectionStatuses();
            _infoPanelView.SetEntity(entity);

            SubscribeToColonists();
        }

        public void Add(Entity entity)
        {
            Entities.Add(entity);
            UpdateSelectionStatuses();
            _infoPanelView.SetEntities(Entities);
            
            entity.EntityDie += RemoveFromSelected;
        }

        public bool Contains(Entity entity)
        {
            return Entities.Contains(entity);
        }

        private void SubscribeToColonists()
        {
            foreach (var entity in Entities)
            {
                entity.EntityDie += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromEntities()
        {
            foreach (var entity in Entities)
            {
                entity.EntityDie -= RemoveFromSelected;
            }
        }

        private void RemoveFromSelected(Entity entity)
        {
            Entities.Remove(entity);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var forSelection in Entities)
            {
                forSelection.Select();
            }
            foreach (var forDeselection in _lastSelectedEntities.Except(Entities))
            {
                if (!forDeselection.Alive)
                {
                    continue;
                }
                
                forDeselection.Deselect();
            }

            _lastSelectedEntities = new List<Entity>(Entities);
            
            SelectionChange?.Invoke();
        }
    }
}
