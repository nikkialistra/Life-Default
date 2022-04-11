using System;
using System.Collections;
using Entities.Interfaces;
using ResourceManagement;
using UnityEngine;
using Zenject;

namespace Colonists.Colonist
{
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(ColonistStats))]
    public class ColonistGatherer : MonoBehaviour
    {
        private Action _onInteractionFinish;

        private Coroutine _gatheringCoroutine;

        private ICountable _acquired;
        
        private ColonistAnimator _colonistAnimator;
        private ColonistStats _colonistStats;
        
        private ResourceCounts _resourceCounts;

        [Inject]
        public void Construct(ResourceCounts resourceCounts)
        {
            _resourceCounts = resourceCounts;
        }

        private void Awake()
        {
            _colonistAnimator = GetComponent<ColonistAnimator>();
            _colonistStats = GetComponent<ColonistStats>();
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
                _colonistAnimator.StopGathering();
            }

            ReleaseAcquired();
        }

        private IEnumerator Gathering(Resource resource, Action onInteractionFinish)
        {
            _colonistAnimator.Gather(resource);
        
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
        
            _colonistAnimator.StopGathering();
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
    }
}
