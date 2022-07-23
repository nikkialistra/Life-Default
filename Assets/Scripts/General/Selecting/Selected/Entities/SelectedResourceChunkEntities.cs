using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Types;
using ResourceManagement;
using UI.Game.GameLook.Components.Info;

namespace General.Selecting.Selected.Entities
{
    public class SelectedResourceChunkEntities
    {
        public int Count => _resourceChunks.Count;

        public bool HasFirst => Count > 0;
        public Entity First => _resourceChunks[0].Entity;

        private List<ResourceChunk> _resourceChunks = new();

        private readonly InfoPanelView _infoPanelView;

        public SelectedResourceChunkEntities(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        public void Set(List<ResourceChunk> resourceChunks)
        {
            UnsubscribeFromResourceChunks();

            _resourceChunks = resourceChunks.ToList();
            UpdateResourceChunkSelectionStatuses();

            _infoPanelView.SetResourceChunks(resourceChunks);

            SubscribeToResourceChunks();
        }

        public void Set(ResourceChunk resourceChunk)
        {
            UnsubscribeFromResourceChunks();

            _resourceChunks = new List<ResourceChunk> { resourceChunk };
            UpdateResourceChunkSelectionStatuses();

            _infoPanelView.SetResourceChunk(resourceChunk);

            SubscribeToResourceChunks();
        }

        public void Add(List<ResourceChunk> resourceChunks)
        {
            UnsubscribeFromResourceChunks();

            _resourceChunks = _resourceChunks.Union(resourceChunks).ToList();
            UpdateResourceChunkSelectionStatuses();

            _infoPanelView.SetResourceChunks(resourceChunks);

            SubscribeToResourceChunks();
        }

        public void AddIfSameTypes(Entity entity)
        {
            if (_resourceChunks.Count == 0)
                return;

            if (entity.EntityType == EntityType.ResourceChunk &&
                entity.ResourceChunk.ResourceType == _resourceChunks[0].ResourceType)
                Add(entity.ResourceChunk);
        }

        public void Deselect()
        {
            UnsubscribeFromResourceChunks();

            foreach (var resourceChunk in _resourceChunks)
                resourceChunk.Deselect();

            _resourceChunks.Clear();
        }

        public void Destroy()
        {
            UnsubscribeFromResourceChunks();

            foreach (var resourceChunk in _resourceChunks)
                resourceChunk.Destroy();

            _resourceChunks.Clear();
        }

        private void Add(ResourceChunk resourceChunk)
        {
            if (_resourceChunks.Contains(resourceChunk))
            {
                _resourceChunks.Add(resourceChunk);
                resourceChunk.ResourceChunkDestroying += RemoveFromSelected;
            }
            else
            {
                resourceChunk.Deselect();
                _resourceChunks.Remove(resourceChunk);
                resourceChunk.ResourceChunkDestroying -= RemoveFromSelected;
            }

            UpdateResourceChunkSelectionStatuses();
            _infoPanelView.SetResourceChunks(_resourceChunks);
        }

        private void SubscribeToResourceChunks()
        {
            foreach (var resourceChunk in _resourceChunks)
                resourceChunk.ResourceChunkDestroying += RemoveFromSelected;
        }

        private void UnsubscribeFromResourceChunks()
        {
            foreach (var resourceChunk in _resourceChunks)
                resourceChunk.ResourceChunkDestroying -= RemoveFromSelected;
        }

        private void UpdateResourceChunkSelectionStatuses()
        {
            foreach (var resourceChunk in _resourceChunks)
                resourceChunk.Select();
        }

        private void RemoveFromSelected(ResourceChunk resourceChunk)
        {
            _resourceChunks.Remove(resourceChunk);
        }
    }
}
