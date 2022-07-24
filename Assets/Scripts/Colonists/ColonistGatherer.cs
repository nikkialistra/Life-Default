using System.Collections;
using Colonists.Activities;
using CompositionRoot.Settings;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units;
using Units.Ancillaries.Fields;
using UnityEngine;
using Zenject;

namespace Colonists
{
    [RequireComponent(typeof(ColonistGathererParameters))]
    [RequireComponent(typeof(ColonistAnimator))]
    [RequireComponent(typeof(ColonistActivities))]
    public class ColonistGatherer : MonoBehaviour
    {
        public bool IsGathering { get; private set; }

        [Required]
        [SerializeField] private UnitEquipment _unitEquipment;

        [Space]
        [Required]
        [SerializeField] private FieldOfView _resourceFieldOfView;

        private float _waitTime;

        private Resource _resource;

        private ColonistGathererParameters _parameters;

        private ColonistAnimator _colonistAnimator;
        private ColonistActivities _colonistActivities;

        private Coroutine _watchForExhaustionCoroutine;

        [Inject]
        public void Construct(AnimationSettings animationSettings)
        {
            _waitTime = animationSettings.TimeToStopInteraction;
        }

        private void Awake()
        {
            _parameters = GetComponent<ColonistGathererParameters>();

            _colonistAnimator = GetComponent<ColonistAnimator>();
            _colonistActivities = GetComponent<ColonistActivities>();
        }

        public bool AtInteractionDistance(Resource resource)
        {
            var distance = Vector3.Distance(transform.position, resource.transform.position);

            return distance <= _parameters.InteractionDistanceFromColonistCenterFor(resource.ResourceType);
        }

        public bool TryGather(Resource resource)
        {
            if (_resource == resource || !AtInteractionDistance(resource))
                return false;

            if (!_unitEquipment.TryEquipToolFor(resource.ResourceType))
                return false;

            _resource = resource;

            _colonistAnimator.Gather(resource);

            _watchForExhaustionCoroutine = StartCoroutine(CWatchForExhaustion());
            IsGathering = true;

            return true;
        }

        public void Hit(float duration)
        {
            if (_resource == null || _resource.Exhausted)
            {
                StopGathering();
                return;
            }

            _colonistActivities.Advance(ActivityType.Gathering, duration);

            var extractedQuantity = _parameters.ResourceDestructionSpeed * duration;

            _resource.Extract(extractedQuantity, _parameters.ResourceExtractionEfficiency);
            _resource.Hit(transform.position);

            if (_resource.Exhausted)
            {
                _resource = null;
                StopGathering();
            }
        }

        private IEnumerator CWatchForExhaustion()
        {
            while (!_resource.Exhausted)
                yield return new WaitForSeconds(_waitTime);

            _resource = null;
            StopGathering();
        }

        // Add wait time for cancelling stop gathering if user clicked same resource,
        // and prolongate animation after gathering a little
        public void FinishGathering()
        {
            if (!IsGathering) return;

            if (_watchForExhaustionCoroutine != null)
            {
                StopCoroutine(_watchForExhaustionCoroutine);
                _watchForExhaustionCoroutine = null;
            }

            _resource = null;
            IsGathering = false;

            StartCoroutine(CStopGatheringLater());
        }

        public void ToggleResourceFieldOfView()
        {
            _resourceFieldOfView.ToggleDebugShow();
        }

        public void HideResourceFieldOfView()
        {
            _resourceFieldOfView.HideDebugShow();
        }

        private IEnumerator CStopGatheringLater()
        {
            yield return new WaitForSeconds(_waitTime);

            StopGathering();
        }

        private void StopGathering()
        {
            if (_resource != null) return;

            if (_watchForExhaustionCoroutine != null)
            {
                StopCoroutine(_watchForExhaustionCoroutine);
                _watchForExhaustionCoroutine = null;
            }

            _colonistAnimator.StopGathering();
        }
    }
}
