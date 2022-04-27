using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Types;
using ResourceManagement;
using UI.Game.GameLook.Components.Info;

namespace General.Selecting.Selected
{
    public class SelectedEntities
    {
        private readonly InfoPanelView _infoPanelView;

        public SelectedEntities(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public int Count => _resources.Count + _resourceChunks.Count;
        
        private List<Resource> _resources = new();
        private List<ResourceChunk> _resourceChunks = new();

        public void Set(List<Resource> resources)
        {
            UnsubscribeFromResources();

            _resources = resources;
            UpdateResourceSelectionStatuses();

            _infoPanelView.SetResources(resources);

            SubscribeToResources();
        }
        
        public void Set(List<ResourceChunk> resourceChunks)
        {
            UnsubscribeFromResourceChunks();

            _resourceChunks = resourceChunks;
            UpdateResourceChunkSelectionStatuses();

            _infoPanelView.SetResourceChunks(resourceChunks);

            SubscribeToResourceChunks();
        }
        
        public void Add(List<Resource> resources)
        {
            UnsubscribeFromResources();

            _resources = _resources.Concat(resources).ToList();
            UpdateResourceSelectionStatuses();

            _infoPanelView.SetResources(resources);

            SubscribeToResources();
        }
        
        public void Add(List<ResourceChunk> resourceChunks)
        {
            UnsubscribeFromResourceChunks();

            _resourceChunks = _resourceChunks.Concat(resourceChunks).ToList();
            UpdateResourceChunkSelectionStatuses();

            _infoPanelView.SetResourceChunks(resourceChunks);

            SubscribeToResourceChunks();
        }

        public void Set(Resource resource)
        {
            UnsubscribeFromResources();

            _resources = new List<Resource> { resource };
            UpdateResourceSelectionStatuses();

            _infoPanelView.SetResource(resource);

            SubscribeToResources();
        }
        
        public void Set(ResourceChunk resourceChunk)
        {
            UnsubscribeFromResourceChunks();

            _resourceChunks = new List<ResourceChunk> { resourceChunk };
            UpdateResourceChunkSelectionStatuses();

            _infoPanelView.SetResourceChunk(resourceChunk);

            SubscribeToResourceChunks();
        }

        public void AddIfSameTypes(Entity entity)
        {
            if (_resources.Count > 0)
            {
                if (entity.EntityType == EntityType.Resource &&
                    entity.Resource.ResourceType == _resources[0].ResourceType)
                {
                    Add(entity.Resource);
                }
            }
            
            if (_resourceChunks.Count > 0)
            {
                if (entity.EntityType == EntityType.ResourceChunk &&
                    entity.ResourceChunk.ResourceType == _resourceChunks[0].ResourceType)
                {
                    Add(entity.ResourceChunk);
                }
            }
        }

        private void Add(Resource resource)
        {
            _resources.Add(resource);
            UpdateResourceSelectionStatuses();
            
            _infoPanelView.SetResources(_resources);

            resource.ResourceDestroying += RemoveFromSelected;
        }

        private void Add(ResourceChunk resourceChunk)
        {
            _resourceChunks.Add(resourceChunk);
            UpdateResourceChunkSelectionStatuses();
            
            _infoPanelView.SetResourceChunks(_resourceChunks);

            resourceChunk.ResourceChunkDestroying += RemoveFromSelected;
        }

        public void Deselect()
        {
            UnsubscribeFromResources();
            UnsubscribeFromResourceChunks();

            foreach (var resource in _resources)
            {
                resource.Deselect();
            }

            foreach (var resourceChunk in _resourceChunks)
            {
                resourceChunk.Deselect();
            }

            _resources.Clear();
            _resourceChunks.Clear();
        }

        public void Destroy()
        {
            UnsubscribeFromResources();
            UnsubscribeFromResourceChunks();

            foreach (var resource in _resources)
            {
                resource.Destroy();
            }

            foreach (var resourceChunk in _resourceChunks)
            {
                resourceChunk.Destroy();
            }

            _resources.Clear();
            _resourceChunks.Clear();
        }

        private void SubscribeToResources()
        {
            foreach (var resource in _resources)
            {
                resource.ResourceDestroying += RemoveFromSelected;
            }
        }

        private void SubscribeToResourceChunks()
        {
            foreach (var resourceChunk in _resourceChunks)
            {
                resourceChunk.ResourceChunkDestroying += RemoveFromSelected;
            }
        }

        private void UnsubscribeFromResources()
        {
            foreach (var resource in _resources)
            {
                resource.ResourceDestroying -= RemoveFromSelected;
            }
        }

        private void UnsubscribeFromResourceChunks()
        {
            foreach (var resourceChunk in _resourceChunks)
            {
                resourceChunk.ResourceChunkDestroying -= RemoveFromSelected;
            }
        }

        private void UpdateResourceSelectionStatuses()
        {
            foreach (var resource in _resources)
            {
                resource.Select();
            }
        }

        private void UpdateResourceChunkSelectionStatuses()
        {
            foreach (var resourceChunk in _resourceChunks)
            {
                resourceChunk.Select();
            }
        }

        private void RemoveFromSelected(Resource resource)
        {
            _resources.Remove(resource);
        }

        private void RemoveFromSelected(ResourceChunk resourceChunk)
        {
            _resourceChunks.Remove(resourceChunk);
        }
    }
}
