using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ResourceManagement
{
    public class ResourceChunkScattering : MonoBehaviour
    {
        [Required]
        [SerializeField] private Transform _spawnPoint;

        [ValidateInput("@_resourceChunkPrefabs.Count > 0")]
        [SerializeField] private List<ResourceChunk> _resourceChunkPrefabs;

        [SerializeField] private float _maxSideForce;
        [SerializeField] private float _minSideForce;

        [SerializeField] private float _maxUpForce;
        [SerializeField] private float _minUpForce;
        
        private Transform _parent;

        [Inject]
        public void Construct([Inject(Id = "ResourceChunksParent")] Transform parent)
        {
            _parent = parent;
        }

        public void Spawn(ResourceType resourceType, int quantity)
        {
            var resourceChunkPrefab = _resourceChunkPrefabs[Random.Range(0, _resourceChunkPrefabs.Count)];

            var resourceChunk = Instantiate(resourceChunkPrefab, _spawnPoint.position, Quaternion.identity, _parent);

            resourceChunk.Initialize(resourceType, quantity);
            
            var randomForce = new Vector3(Random.Range(_minSideForce, _maxSideForce),
                Random.Range(_minUpForce, _maxUpForce),
                Random.Range(_minSideForce, _maxSideForce));

            resourceChunk.BurstOutTo(randomForce);
        }
    }
}
