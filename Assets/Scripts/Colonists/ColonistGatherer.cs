using System;
using System.Collections;
using Common;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(ColonistStats))]
    public class ColonistGatherer : MonoBehaviour
    {
        [Required]
        [SerializeField] private ColonistHandEquipment _handEquipment;

        [ValidateInput(nameof(EveryResourceHasDistanceInteraction))]
        [SerializeField] private ResourceInteractionDistanceDictionary _resourceInteractionDistances;

        [SerializeField] private float _waitTime = 0.2f;

        private Action _onInteractionFinish;

        private Resource _gatheringResource;
        
        private ColonistAnimator _animator;
        private ColonistStats _colonistStats;
        
        private Coroutine _watchForExhaustionCoroutine;

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
                return;
            }

            _gatheringResource = resource;
            _onInteractionFinish = onInteractionFinish;
            
            _handEquipment.EquipInstrumentFor(resource.ResourceType);
            _animator.Gather(resource);

            _watchForExhaustionCoroutine = StartCoroutine(WatchForExhaustion());
        }

        private IEnumerator WatchForExhaustion()
        {
            while (!_gatheringResource.Exhausted)
            {
                yield return new WaitForSeconds(_waitTime);
            }

            _gatheringResource = null;
            FinishGathering();
        }

        // Add wait time for cancelling stop gathering if user clicked same resource,
        // and prolongate animation after gathering a little
        public void StopGathering()
        {
            if (_watchForExhaustionCoroutine != null)
            {
                StopCoroutine(_watchForExhaustionCoroutine);
            }
            _gatheringResource = null;
            
            StartCoroutine(StopGatheringLater());
        }
        
        public void Hit()
        {
            if (_gatheringResource == null || _gatheringResource.Exhausted)
            {
                FinishGathering();
                return;
            }

            _gatheringResource.Extract(_colonistStats.ResourceDestructionSpeed, _colonistStats.ResourceExtractionEfficiency);
            _gatheringResource.Hit(transform.position);

            if (_gatheringResource.Exhausted)
            {
                _gatheringResource = null;
                FinishGathering();
            }
        }

        private IEnumerator StopGatheringLater()
        {
            yield return new WaitForSeconds(_waitTime);

            FinishGathering();
        }

        private void FinishGathering()
        {
            if (_gatheringResource != null)
            {
                return;
            }

            if (_watchForExhaustionCoroutine != null)
            {
                StopCoroutine(_watchForExhaustionCoroutine);
            }
            
            _animator.StopGathering();

            if (_onInteractionFinish != null)
            {
                _onInteractionFinish();
                _onInteractionFinish = null;
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
