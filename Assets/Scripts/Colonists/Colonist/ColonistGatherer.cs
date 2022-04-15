using System;
using System.Collections;
using Common;
using Entities.Interfaces;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Colonists.Colonist
{
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(ColonistStats))]
    public class ColonistGatherer : MonoBehaviour
    {
        [Required]
        [SerializeField] private ColonistHandEquipment _handEquipment;

        [ValidateInput(nameof(EveryResourceHasDistanceInteraction))]
        [SerializeField] private ResourceInteractionDistanceDictionary _resourceInteractionDistances;

        private const float WaitTime = 0.5f;

        private Action _onInteractionFinish;

        private Resource _gatheringResource;
        
        private ColonistAnimator _animator;
        private ColonistStats _colonistStats;

        private ICountable _acquired;
        
        private ResourceCounts _resourceCounts;
        
        private Coroutine _gatheringCoroutine;
        private Coroutine _stopGatheringCoroutine;

        [Inject]
        public void Construct(ResourceCounts resourceCounts)
        {
            _resourceCounts = resourceCounts;
        }

        private void Awake()
        {
            _animator = GetComponent<ColonistAnimator>();
            _colonistStats = GetComponent<ColonistStats>();
        }

        public float InteractionDistanceFor(ResourceType resourceType)
        {
            return _resourceInteractionDistances[resourceType];
        }

        public bool CanGather(Resource resource)
        {
            return true;
        }

        public void Gather(Resource resource, Action onInteractionFinish)
        {
            if (_gatheringResource == resource)
            {
                StopCoroutine(_stopGatheringCoroutine);
                _stopGatheringCoroutine = null;
                return;
            }
            
            if (_gatheringCoroutine != null)
            {
                return;
            }

            _gatheringResource = resource;
            
            _gatheringCoroutine = StartCoroutine(Gathering(resource, onInteractionFinish));
        }

        public void StopGathering()
        {
            _stopGatheringCoroutine = StartCoroutine(StopGatheringLater());
        }

        private IEnumerator StopGatheringLater()
        {
            yield return new WaitForSeconds(WaitTime);

            FinishGathering();
        }

        private void FinishGathering()
        {
            if (_gatheringCoroutine != null)
            {
                StopCoroutine(_gatheringCoroutine);
                _gatheringCoroutine = null;
                
                _animator.StopGathering();
            }

            ReleaseAcquired();

            _gatheringResource = null;
        }

        private IEnumerator Gathering(Resource resource, Action onInteractionFinish)
        {
            _handEquipment.EquipInstrumentFor(resource.ResourceType);
            _animator.Gather(resource);
        
            AddToAcquired(resource);
        
            while (!resource.Exhausted)
            {
                yield return new WaitForSeconds(1f);
        
                if (resource.Exhausted)
                {
                    break;
                }
        
                resource.Extract(_colonistStats.ResourceDestructionSpeed, _colonistStats.ResourceExtractionEfficiency);
            }
        
            _animator.StopGathering();
            
            ReleaseAcquired();
            _gatheringCoroutine = null;

            onInteractionFinish();
        }

        private void AddToAcquired(ICountable toAcquaire)
        {
            toAcquaire.Acquire();
            _acquired = toAcquaire;
        }

        private void ReleaseAcquired()
        {
            if (_acquired != null)
            {
                _acquired.Release();
                _acquired = null;
            }
        }
        
        private bool EveryResourceHasDistanceInteraction(ResourceInteractionDistanceDictionary distances, ref string errorMessage)
        {
            foreach (var resourceType in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
            {
                if (!distances.ContainsKey(resourceType))
                {
                    errorMessage = $"{resourceType} don't have distance";
                    return false;
                }
            }

            return true;
        }
        
        [Serializable] public class ResourceInteractionDistanceDictionary : SerializableDictionary<ResourceType, float> { }
    }
}
