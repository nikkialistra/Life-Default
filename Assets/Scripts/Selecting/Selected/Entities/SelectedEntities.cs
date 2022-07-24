using System;
using System.Collections.Generic;
using Entities;
using ResourceManagement;
using UI.Game.GameLook.Components.Info;

namespace Selecting.Selected.Entities
{
    public class SelectedEntities
    {
        public int Count => _resources.Count + _resourceChunks.Count;

        private readonly SelectedResourceEntities _resources;
        private readonly SelectedResourceChunkEntities _resourceChunks;

        public SelectedEntities(InfoPanelView infoPanelView)
        {
            _resources = new SelectedResourceEntities(infoPanelView);
            _resourceChunks = new SelectedResourceChunkEntities(infoPanelView);
        }

        public Entity First()
        {
            if (_resources.HasFirst)
                return _resources.First;

            if (_resourceChunks.HasFirst)
                return _resourceChunks.First;

            throw new InvalidOperationException("Cannot take first selected entity when 0 entities are selected");
        }

        public void Set(List<Resource> resources)
        {
            _resources.Set(resources);
        }

        public void Set(List<ResourceChunk> resourceChunks)
        {
            _resourceChunks.Set(resourceChunks);
        }

        public void Add(List<Resource> resources)
        {
            _resources.Add(resources);
        }

        public void Add(List<ResourceChunk> resourceChunks)
        {
            _resourceChunks.Add(resourceChunks);
        }

        public void Set(Resource resource)
        {
            _resources.Set(resource);
        }

        public void Set(ResourceChunk resourceChunk)
        {
            _resourceChunks.Set(resourceChunk);
        }

        public void AddIfSameTypes(Entity entity)
        {
            _resources.AddIfSameTypes(entity);
            _resourceChunks.AddIfSameTypes(entity);
        }

        public void Deselect()
        {
            _resources.Deselect();
            _resourceChunks.Deselect();
        }

        public void Destroy()
        {
            _resources.Destroy();
            _resourceChunks.Destroy();
        }
    }
}
