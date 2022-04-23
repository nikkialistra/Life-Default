using System.Collections.Generic;
using General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Ancillaries.Fields
{
    [RequireComponent(typeof(FieldVisualization))]
    public class FieldOfView : MonoBehaviour
    {
        [Space]
        [SerializeField] private float _viewRadius;
        [Range(0, 360)]
        [SerializeField] private float _viewAngle;
        [Space]
        [SerializeField] private LayerMask _targetMask;

        private Vector3 _targetPositionCorrection;
        private float _recalculationTime;
        private LayerMask _obstacleMask;

        private readonly List<Transform> _visibleTargets = new();

        private bool _showFieldOfView;
        private float _updateTime;
        
        private FieldVisualization _fieldVisualization;

        private void Awake()
        {
            _fieldVisualization = GetComponent<FieldVisualization>();
        }

        private void Start()
        {
            _targetPositionCorrection = GlobalParameters.Instance.TargetPositionCorrection;
            _recalculationTime = GlobalParameters.Instance.VisibilityFieldRecalculationTime;
            _obstacleMask = GlobalParameters.Instance.ObstacleMask;
        }

        private void Update()
        {
            if (_showFieldOfView && Time.time > _updateTime)
            {
                _updateTime += _recalculationTime;
                FindVisibleTargets();
                _fieldVisualization.DrawCircleSegment(_viewAngle, _viewRadius, _obstacleMask, _visibleTargets);
            }
        }

        [Button(ButtonSizes.Large)]
        public void ToggleDebugShow()
        {
            _showFieldOfView = !_showFieldOfView;

            if (_showFieldOfView)
            {
                _fieldVisualization.Show();
                
            }
            else
            {
                _fieldVisualization.Hide();
            }
        }

        public IEnumerable<Transform> FindVisibleTargets()
        {
            _visibleTargets.Clear();
            var targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

            foreach (var target in targetsInViewRadius)
            {
                var targetPosition = target.transform.position + _targetPositionCorrection;
                var directionToTarget = (targetPosition - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < _viewAngle / 2)
                {
                    var distanceToTarget = Vector3.Distance(transform.position, targetPosition);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        _visibleTargets.Add(target.transform);
                    }
                }
            }

            return _visibleTargets;
        }
    }
}
