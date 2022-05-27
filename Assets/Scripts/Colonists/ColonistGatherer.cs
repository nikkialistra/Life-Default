﻿using System;
using System.Collections;
using Common;
using Infrastructure.Settings;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries.Fields;
using Units.Stats;
using UnityEngine;
using Zenject;

namespace Colonists
{
    [RequireComponent(typeof(ColonistAnimator))]
    public class ColonistGatherer : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;
        
        [ValidateInput(nameof(EveryResourceHasDistanceInteraction))]
        [SerializeField] private ResourceInteractionDistanceDictionary _resourceInteractionDistances;

        [SerializeField] private float _distanceCorrectionFromCenter = 2f;

        [Space]
        [Required]
        [SerializeField] private FieldOfView _resourceFieldOfView;
        
        private float _waitTime;
        
        private float _resourceDestructionSpeed;
        private float _resourceExtractionEfficiency;

        private Resource _resource;

        private ColonistAnimator _animator;

        private Coroutine _watchForExhaustionCoroutine;

        [Inject]
        public void Construct(AnimationSettings animationSettings)
        {
            _waitTime = animationSettings.TimeToStopInteraction;
        }

        private void Awake()
        {
            _animator = GetComponent<ColonistAnimator>();
        }
        
        public bool IsGathering { get; private set; }
        
        public void BindStats(Stat<ColonistStat> resourceDestructionSpeed, Stat<ColonistStat> resourceExtractionEfficiency)
        {
            _resourceDestructionSpeed = resourceDestructionSpeed.Value;
            _resourceExtractionEfficiency = resourceExtractionEfficiency.Value;

            resourceDestructionSpeed.ValueChange += OnResourceDestructionSpeedChange;
            resourceExtractionEfficiency.ValueChange += OnResourceExtractionEfficiencyChange;
        }

        public void UnbindStats(Stat<ColonistStat> resourceDestructionSpeed,
            Stat<ColonistStat> resourceExtractionEfficiency)
        {
            resourceDestructionSpeed.ValueChange -= OnResourceDestructionSpeedChange;
            resourceExtractionEfficiency.ValueChange -= OnResourceExtractionEfficiencyChange;
        }

        public float InteractionDistanceFor(ResourceType resourceType)
        {
            return _resourceInteractionDistances[resourceType];
        }

        public bool AtInteractionDistance(Resource resource)
        {
            var distance = Vector3.Distance(transform.position, resource.transform.position);

            return distance <= _resourceInteractionDistances[resource.ResourceType] + _distanceCorrectionFromCenter;
        }

        public bool TryGather(Resource resource)
        {
            if (_resource == resource || !AtInteractionDistance(resource))
            {
                return false;
            }
            
            if (!_unitEquipment.TryEquipInstrumentFor(resource.ResourceType))
            {
                return false;
            }

            _resource = resource;

            _animator.Gather(resource);

            _watchForExhaustionCoroutine = StartCoroutine(WatchForExhaustion());
            IsGathering = true;

            return true;
        }

        public void Hit(float passedTime)
        {
            if (_resource == null || _resource.Exhausted)
            {
                StopGathering();
                return;
            }

            var extractedQuantity = _resourceDestructionSpeed * passedTime;

            _resource.Extract(extractedQuantity, _resourceExtractionEfficiency);
            _resource.Hit(transform.position);

            if (_resource.Exhausted)
            {
                _resource = null;
                StopGathering();
            }
        }

        private void OnResourceDestructionSpeedChange(float value)
        {
            _resourceDestructionSpeed = value;
        }

        private void OnResourceExtractionEfficiencyChange(float value)
        {
            _resourceExtractionEfficiency = value;
        }

        private IEnumerator WatchForExhaustion()
        {
            while (!_resource.Exhausted)
            {
                yield return new WaitForSeconds(_waitTime);
            }

            _resource = null;
            StopGathering();
        }

        // Add wait time for cancelling stop gathering if user clicked same resource,

        // and prolongate animation after gathering a little

        public void FinishGathering()
        {
            if (!IsGathering)
            {
                return;
            }
            
            if (_watchForExhaustionCoroutine != null)
            {
                StopCoroutine(_watchForExhaustionCoroutine);
                _watchForExhaustionCoroutine = null;
            }
            
            _resource = null;
            IsGathering = false;
            
            StartCoroutine(StopGatheringLater());
        }

        public void ToggleResourceFieldOfView()
        {
            _resourceFieldOfView.ToggleDebugShow();
        }

        public void HideResourceFieldOfView()
        {
            _resourceFieldOfView.HideDebugShow();
        }

        private IEnumerator StopGatheringLater()
        {
            yield return new WaitForSeconds(_waitTime);

            StopGathering();
        }

        private void StopGathering()
        {
            if (_resource != null)
            {
                return;
            }

            if (_watchForExhaustionCoroutine != null)
            {
                StopCoroutine(_watchForExhaustionCoroutine);
                _watchForExhaustionCoroutine = null;
            }
            
            _animator.StopGathering();
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
