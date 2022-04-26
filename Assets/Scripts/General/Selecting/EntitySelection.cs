using System.Collections.Generic;
using Entities;
using Entities.Types;
using General.Selecting.Selected;
using ResourceManagement;

namespace General.Selecting
{
    public class EntitySelection
    {
        private readonly SelectedEntities _selectedEntities;
        
        private List<Entity> _entities;

        private readonly List<Resource> _resources = new();
        private readonly List<ResourceChunk> _resourceChunks = new();
        
        private readonly Dictionary<ResourceType, int> _resourceCounts = new();
        private readonly Dictionary<ResourceType, int> _resourceChunkCounts = new();

        private KeyValuePair<ResourceType, int> _maxResourceCount;
        private KeyValuePair<ResourceType, int> _maxResourceChunkCount;

        private readonly List<Resource> _resourcesToSelect = new();
        private readonly List<ResourceChunk> _resourceChunksToSelect = new();

        public EntitySelection(SelectedEntities selectedEntities)
        {
            _selectedEntities = selectedEntities;
        }

        public void ChooseToSelect(List<Entity> entities)
        {
            _entities = entities;
            
            SplitEntitiesByType();
            FindEntityFrequenciesByInnerType();
            FindMostFrequentEntitiesByInnerType();
            SelectMostFrequentEntityByInnerType();
        }

        private void SplitEntitiesByType()
        {
            _resources.Clear();
            _resourceChunks.Clear();
            
            foreach (var entity in _entities)
            {
                if (entity.EntityType == EntityType.Resource)
                {
                    _resources.Add(entity.Resource);
                }

                if (entity.EntityType == EntityType.ResourceChunk)
                {
                    _resourceChunks.Add(entity.ResourceChunk);
                }
            }
        }

        private void FindEntityFrequenciesByInnerType()
        {
            _resourceCounts.Clear();
            _resourceChunkCounts.Clear();
            
            foreach (var resource in _resources)
            {
                _resourceCounts.TryGetValue(resource.ResourceType, out var count);
                _resourceCounts[resource.ResourceType] = count + 1;
            }
            
            foreach (var resourceChunk in _resourceChunks)
            {
                _resourceChunkCounts.TryGetValue(resourceChunk.ResourceType, out var count);
                _resourceChunkCounts[resourceChunk.ResourceType] = count + 1;
            }
        }

        private void FindMostFrequentEntitiesByInnerType()
        {
            _maxResourceCount = default;
            
            foreach (var resourceCount in _resourceCounts)
            {
                if (resourceCount.Value > _maxResourceCount.Value)
                {
                    _maxResourceCount = resourceCount;
                }
            }

            _maxResourceChunkCount = default;
            
            foreach (var resourceChunkCount in _resourceChunkCounts)
            {
                if (resourceChunkCount.Value > _maxResourceChunkCount.Value)
                {
                    _maxResourceChunkCount = resourceChunkCount;
                }
            }
        }

        private void SelectMostFrequentEntityByInnerType()
        {
            if (_maxResourceCount.Value == 0 && _maxResourceChunkCount.Value == 0)
            {
                return;
            }

            if (_maxResourceCount.Value > _maxResourceChunkCount.Value)
            {
                SelectResourceWithType(_maxResourceCount.Key);
            }
            else
            {
                SelectResourceChunkWithType(_maxResourceChunkCount.Key);
            }
        }

        private void SelectResourceWithType(ResourceType resourceType)
        {
            _resourcesToSelect.Clear();
            
            foreach (var resource in _resources)
            {
                if (resource.ResourceType == resourceType)
                {
                    _resourcesToSelect.Add(resource);
                }
            }

            _selectedEntities.Set(_resourcesToSelect);
        }

        private void SelectResourceChunkWithType(ResourceType resourceType)
        {
            _resourceChunksToSelect.Clear();
            
            foreach (var resource in _resourceChunks)
            {
                if (resource.ResourceType == resourceType)
                {
                    _resourceChunksToSelect.Add(resource);
                }
            }

            _selectedEntities.Set(_resourceChunksToSelect); 
        }
    }
}
