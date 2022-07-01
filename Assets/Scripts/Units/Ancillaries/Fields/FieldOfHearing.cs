using System.Collections.Generic;
using Infrastructure.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Units.Ancillaries.Fields
{
    [RequireComponent(typeof(FieldVisualization))]
    public class FieldOfHearing : MonoBehaviour
    {
        [SerializeField] private float _viewRadius;
        [Space]
        [SerializeField] private LayerMask _targetMask;

        private Vector3 _targetPositionCorrection;
        private float _recalculationTime;
        private LayerMask _obstacleMask;

        private readonly List<Transform> _visibleTargets = new();

        private bool _show;
        private float _updateTime;

        private FieldVisualization _fieldVisualization;

        [Inject]
        public void Construct(VisibilityFieldsSettings visibilityFieldsSettings)
        {
            _targetPositionCorrection = visibilityFieldsSettings.TargetPositionCorrection;
            _recalculationTime = visibilityFieldsSettings.VisibilityFieldRecalculationTime;
            _obstacleMask = visibilityFieldsSettings.ObstacleMask;
        }

        private void Awake()
        {
            _fieldVisualization = GetComponent<FieldVisualization>();
        }

        private void Update()
        {
            if (_show && Time.time > _updateTime)
            {
                _updateTime += _recalculationTime;
                FindVisibleTargets();
                _fieldVisualization.DrawCircle(_viewRadius, _obstacleMask, _visibleTargets);
            }
        }

        [Button(ButtonSizes.Large)]
        public void ToggleDebugShow()
        {
            _show = !_show;

            if (_show)
                _fieldVisualization.Show();
            else
                _fieldVisualization.Hide();
        }

        public void HideDebugShow()
        {
            _show = false;

            _fieldVisualization.Hide();
        }

        public IEnumerable<Transform> FindVisibleTargets()
        {
            _visibleTargets.Clear();
            var targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

            foreach (var target in targetsInViewRadius)
            {
                var targetPosition = target.transform.position + _targetPositionCorrection;
                var directionToTarget = (targetPosition - transform.position).normalized;

                var distanceToTarget = Vector3.Distance(transform.position, targetPosition);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    _visibleTargets.Add(target.transform);
                }
            }

            return _visibleTargets;
        }
    }
}
