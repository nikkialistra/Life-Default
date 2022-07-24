using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Types;
using ResourceManagement;
using UI.Game.GameLook.Components.Info;

namespace Selecting.Selected.Entities
{
    public class SelectedResourceEntities
    {
        public int Count => _resources.Count;

        public bool HasFirst => Count > 0;
        public Entity First => _resources[0].Entity;

        private List<Resource> _resources = new();

        private readonly InfoPanelView _infoPanelView;

        public SelectedResourceEntities(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public void Set(List<Resource> resources)
        {
            UnsubscribeFromResources();

            _resources = resources.ToList();
            UpdateResourceSelectionStatuses();

            _infoPanelView.SetResources(resources);

            SubscribeToResources();
        }

        public void Set(Resource resource)
        {
            UnsubscribeFromResources();

            _resources = new List<Resource> { resource };
            UpdateResourceSelectionStatuses();

            _infoPanelView.SetResource(resource);

            SubscribeToResources();
        }

        public void Add(List<Resource> resources)
        {
            UnsubscribeFromResources();

            _resources = _resources.Union(resources).ToList();
            UpdateResourceSelectionStatuses();

            _infoPanelView.SetResources(resources);

            SubscribeToResources();
        }

        public void AddIfSameTypes(Entity entity)
        {
            if (_resources.Count == 0)
                return;

            if (entity.EntityType == EntityType.Resource &&
                entity.Resource.ResourceType == _resources[0].ResourceType)
                Add(entity.Resource);
        }

        public void Deselect()
        {
            UnsubscribeFromResources();

            foreach (var resource in _resources)
                resource.Deselect();

            _resources.Clear();
        }

        public void Destroy()
        {
            UnsubscribeFromResources();

            foreach (var resource in _resources)
                resource.Destroy();

            _resources.Clear();
        }

        private void Add(Resource resource)
        {
            if (!_resources.Contains(resource))
            {
                _resources.Add(resource);
                resource.ResourceDestroying += RemoveFromSelected;
            }
            else
            {
                resource.Deselect();
                _resources.Remove(resource);
                resource.ResourceDestroying -= RemoveFromSelected;
            }

            UpdateResourceSelectionStatuses();
            _infoPanelView.SetResources(_resources);
        }

        private void SubscribeToResources()
        {
            foreach (var resource in _resources)
                resource.ResourceDestroying += RemoveFromSelected;
        }

        private void UnsubscribeFromResources()
        {
            foreach (var resource in _resources)
                resource.ResourceDestroying -= RemoveFromSelected;
        }

        private void UpdateResourceSelectionStatuses()
        {
            foreach (var resource in _resources)
                resource.Select();
        }

        private void RemoveFromSelected(Resource resource)
        {
            _resources.Remove(resource);
        }
    }
}
