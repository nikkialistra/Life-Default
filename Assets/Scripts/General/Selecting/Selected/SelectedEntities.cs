using System.Collections.Generic;
using Entities;
using UI.Game.GameLook.Components;

namespace General.Selecting.Selected
{
    public class SelectedEntities
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedEntities(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public List<Entity> Entities { get; private set; } = new();

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
            
            entity.EntityDestroying += RemoveFromSelected;
        }

        public void Deselect()
        {
            UnsubscribeFromEntities();

            foreach (var entity in Entities)
            {
                entity.Deselect();
            }

            Entities.Clear();
        }
        
        public void Destroy()
        {
            UnsubscribeFromEntities();

            foreach (var entity in Entities)
            {
                entity.Destroy();
            }

            Entities.Clear();
        }

        private void SubscribeToColonists()
        {
            foreach (var entity in Entities)
            {
                entity.EntityDestroying += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromEntities()
        {
            foreach (var entity in Entities)
            {
                entity.EntityDestroying -= RemoveFromSelected;
            }
        }

        private void RemoveFromSelected(Entity entity)
        {
            Entities.Remove(entity);
        }

        private void UpdateSelectionStatuses()
        {
            foreach (var entity in Entities)
            {
                entity.Select();
            }
        }
    }
}
