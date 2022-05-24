using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Info;
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
        
        [Space]
        [SerializeField] private float _minSideForce;
        [SerializeField] private float _maxSideForce;
        [Space]
        [SerializeField] private float _minUpForce;
        [SerializeField] private float _maxUpForce;
        [Space]
        [SerializeField] private float _minTimeToFreeze = 1f;
        [SerializeField] private float _maxTimeToFreeze = 2f;

        private Transform _parent;
        
        private InfoPanelView _infoPanelView;

        [Inject]
        public void Construct([Inject(Id = "ResourceChunksParent")] Transform parent, InfoPanelView infoPanelView)
        {
            _parent = parent;
            
            _infoPanelView = infoPanelView;
        }

        public void Spawn(ResourceType resourceType, int quantity, float sizeMultiplier)
        {
            var resourceChunkPrefab = _resourceChunkPrefabs[Random.Range(0, _resourceChunkPrefabs.Count)];

            var rotation = new Vector3(0, Random.Range(0, 359), 0);
            var resourceChunk = Instantiate(resourceChunkPrefab, _spawnPoint.position, Quaternion.Euler(rotation), _parent);

            resourceChunk.Initialize(resourceType, quantity, sizeMultiplier, _infoPanelView);
            
            BurstOutResource(resourceChunk);
        }

        private void BurstOutResource(ResourceChunk resourceChunk)
        {
            var xSign = Random.Range(0, 2) == 0 ? 1 : -1; 
            var zSign = Random.Range(0, 2) == 0 ? 1 : -1; 
            
            var force = new Vector3(Random.Range(_minSideForce, _maxSideForce) * xSign,
                Random.Range(_minUpForce, _maxUpForce),
                Random.Range(_minSideForce, _maxSideForce) * zSign);

            var timeToFreeze = Random.Range(_minTimeToFreeze, _maxTimeToFreeze);

            resourceChunk.BurstOutTo(force, timeToFreeze);
        }
    }
}
