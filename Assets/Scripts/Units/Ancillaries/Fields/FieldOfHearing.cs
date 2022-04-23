using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Ancillaries.Fields
{
    [RequireComponent(typeof(FieldVisualization))]
    public class FieldOfHearing : MonoBehaviour
    {
        [SerializeField] private float _recalculationTime = 0.2f;
        
        [Space]
        [SerializeField] private float _viewRadius;
        
        [Title("Masks")]
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstacleMask;
        
        private static readonly Vector3 TargetPositionCorrection = Vector3.up * 1.5f;
        
        private readonly List<Transform> _visibleTargets = new();

        private bool _showFieldOfView;
        private float _updateTime;
        
        private FieldVisualization _fieldVisualization;

        private void Awake()
        {
            _fieldVisualization = GetComponent<FieldVisualization>();
        }
        
        private void Update()
        {
            if (_showFieldOfView && Time.time > _updateTime)
            {
                _updateTime += _recalculationTime;
                FindVisibleTargets();
                _fieldVisualization.DrawCircle(_viewRadius, _obstacleMask, _visibleTargets);
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
                var targetPosition = target.transform.position + TargetPositionCorrection;
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
