using System;
using System.Collections;
using Common;
using General;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries;
using Units.Ancillaries.Fields;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(ColonistStats))]
    [RequireComponent(typeof(ColonistAnimator))]
    public class ColonistGatherer : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;
        
        [ValidateInput(nameof(EveryResourceHasDistanceInteraction))]
        [SerializeField] private ResourceInteractionDistanceDictionary _resourceInteractionDistances;

        [SerializeField] private float _distanceCorrectionFromCenter = 1.85f;

        [Space]
        [Required]
        [SerializeField] private FieldOfView _resourceFieldOfView;
        
        private float _waitTime;
        
        private Resource _resource;

        private ColonistStats _colonistStats;
        
        private ColonistAnimator _animator;

        private Coroutine _watchForExhaustionCoroutine;

        private void Awake()
        {
            _animator = GetComponent<ColonistAnimator>();
            _colonistStats = GetComponent<ColonistStats>();
        }
        
        public bool IsGathering { get; private set; }

        private void Start()
        {
            _waitTime = GlobalParameters.Instance.TimeToStopInteraction;
        }

        public float InteractionDistanceFor(ResourceType resourceType)
        {
            return _resourceInteractionDistances[resourceType];
        }

        public bool CanGather(Resource resource)
        {
            var distance = Vector3.Distance(transform.position, resource.transform.position);

            return distance <= _resourceInteractionDistances[resource.ResourceType] + _distanceCorrectionFromCenter;
        }

        public void Gather(Resource resource)
        {
            if (_resource == resource)
            {
                return;
            }

            _resource = resource;
            
            _unitEquipment.EquipInstrumentFor(resource.ResourceType);
            _animator.Gather(resource);

            _watchForExhaustionCoroutine = StartCoroutine(WatchForExhaustion());

            IsGathering = true;
        }
        
        public void Hit(float passedTime)
        {
            if (_resource == null || _resource.Exhausted)
            {
                StopGathering();
                return;
            }

            var extractedQuantity = _colonistStats.ResourceDestructionSpeed * passedTime;

            _resource.Extract(extractedQuantity, _colonistStats.ResourceExtractionEfficiency);
            _resource.Hit(transform.position);

            if (_resource.Exhausted)
            {
                _resource = null;
                StopGathering();
            }
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
