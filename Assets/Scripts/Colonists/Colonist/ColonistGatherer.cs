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

        private Action _onInteractionFinish;

        private Coroutine _gatheringCoroutine;

        private ICountable _acquired;
        
        private ColonistAnimator _animator;

        private ColonistStats _colonistStats;

        private ResourceCounts _resourceCounts;

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
            _gatheringCoroutine = StartCoroutine(Gathering(resource, onInteractionFinish));
        }

        public void StopGathering()
        {
            if (_gatheringCoroutine != null)
            {
                StopCoroutine(_gatheringCoroutine);
                _animator.StopGathering();
            }

            ReleaseAcquired();
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
        
                var resourceOutput = resource.Extract(_colonistStats.ResourceExtractionSpeed, _colonistStats.ResourceExtractionEfficiency);
        
                _resourceCounts.ChangeResourceTypeCount(resourceOutput.ResourceType, resourceOutput.Quantity);
            }
        
            ReleaseAcquired();
        
            _animator.StopGathering();

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
